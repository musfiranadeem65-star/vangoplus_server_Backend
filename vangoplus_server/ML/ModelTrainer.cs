using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;

namespace vangoplus_server.ML
{
    public class IntentRow
    {
        [LoadColumn(0)] public string Text { get; set; } = null!;
        [LoadColumn(1)] public string Intent { get; set; } = null!;
    }

    public class IntentPrediction
    {
        [ColumnName("PredictedLabel")] public string Intent { get; set; } = null!;
        public float[] Score { get; set; } = null!;
    }

    /// <summary>
    /// Trains the parent-chatbot intent classifier on our own dataset and writes
    /// intent_model.zip. Run it once, commit the model file:
    ///     dotnet run -- train
    /// </summary>
    public static class ModelTrainer
    {
        public static void Train(string csvPath, string modelPath)
        {
            var ml = new MLContext(seed: 42);

            var data = ml.Data.LoadFromTextFile<IntentRow>(
                csvPath,
                hasHeader: true,
                separatorChar: ',',
                allowQuoting: true,
                trimWhitespace: true);

            // NOTE: do NOT pass samplingKeyColumnName: "Intent" here. That option keeps every
            // row sharing a key in the SAME split, so every example of an intent would land on
            // one side and the evaluation would be meaningless. Plain random split is correct.
            var split = ml.Data.TrainTestSplit(data, testFraction: 0.2);

            var pipeline = ml.Transforms.Conversion
                .MapValueToKey("Label", nameof(IntentRow.Intent))
                .Append(ml.Transforms.Text.FeaturizeText(
                    "Features",
                    new TextFeaturizingEstimator.Options
                    {
                        CaseMode = TextNormalizingEstimator.CaseMode.Lower,
                        KeepPunctuations = false,
                        KeepNumbers = true,
                        // Word unigrams + bigrams carry the Roman Urdu phrasing ("van kab aye gi").
                        WordFeatureExtractor = new Microsoft.ML.Transforms.Text.WordBagEstimator.Options
                        {
                            NgramLength = 2,
                            UseAllLengths = true
                        },
                        // Character 3-4 grams absorb the spelling variation: shukriya / shukria /
                        // shukrya all share most of their character n-grams.
                        CharFeatureExtractor = new Microsoft.ML.Transforms.Text.WordBagEstimator.Options
                        {
                            NgramLength = 4,
                            UseAllLengths = true
                        }
                    },
                    nameof(IntentRow.Text)))
                .Append(ml.MulticlassClassification.Trainers.SdcaMaximumEntropy(
                    labelColumnName: "Label",
                    featureColumnName: "Features"))
                .Append(ml.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            Console.WriteLine("Training…");
            var model = pipeline.Fit(split.TrainSet);

            var predictions = model.Transform(split.TestSet);
            var metrics = ml.MulticlassClassification.Evaluate(predictions, labelColumnName: "Label");

            Console.WriteLine();
            Console.WriteLine("================ EVALUATION (held-out 20%) ================");
            Console.WriteLine($"MicroAccuracy  : {metrics.MicroAccuracy:P2}");
            Console.WriteLine($"MacroAccuracy  : {metrics.MacroAccuracy:P2}");
            Console.WriteLine($"LogLoss        : {metrics.LogLoss:F4}");
            Console.WriteLine();
            Console.WriteLine("Per-intent recall:");
            for (var i = 0; i < metrics.PerClassLogLoss.Count; i++)
                Console.WriteLine($"  class {i}: logloss {metrics.PerClassLogLoss[i]:F3}");
            Console.WriteLine();
            Console.WriteLine("Confusion matrix (paste this into the report):");
            Console.WriteLine(metrics.ConfusionMatrix.GetFormattedConfusionTable());

            ml.Model.Save(model, split.TrainSet.Schema, modelPath);
            Console.WriteLine($"Saved model → {modelPath}");
        }
    }
}
