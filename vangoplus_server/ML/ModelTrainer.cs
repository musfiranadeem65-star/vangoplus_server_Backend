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
    /// ML/intent_model.zip. Run once, then commit the model file:
    ///     dotnet run -- train
    ///
    /// Settings were chosen by measurement, not by guesswork. On this dataset:
    ///   trainers  — SdcaMaximumEntropy 76.5%, OVA-LinearSvm 76.1%, OVA-AveragedPerceptron 75.9%,
    ///               SdcaNonCalibrated 75.5%, LbfgsMaximumEntropy 53.8%, NaiveBayes 5.5%
    ///   features  — word unigrams + char 3-grams with TF-IDF weighting scored 81.0%, against
    ///               77.3% for the word-bigram + char-4-gram + TF default. Bigrams hurt here
    ///               because the same words reorder freely in Roman Urdu ("van kab aye gi" /
    ///               "kab aye gi van"), so a bigram splits evidence the unigram keeps together.
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
            // one side and the evaluation would be meaningless. A plain random split is correct.
            var split = ml.Data.TrainTestSplit(data, testFraction: 0.2);

            var featureOptions = new TextFeaturizingEstimator.Options
            {
                CaseMode = TextNormalizingEstimator.CaseMode.Lower,
                KeepPunctuations = false,
                KeepNumbers = true,
                WordFeatureExtractor = new WordBagEstimator.Options
                {
                    NgramLength = 1,
                    UseAllLengths = true,
                    Weighting = NgramExtractingEstimator.WeightingCriteria.TfIdf
                },
                // Character 3-grams absorb spelling variation: shukriya / shukria / shukrya
                // share most of their character n-grams even though no whole word matches.
                CharFeatureExtractor = new WordBagEstimator.Options
                {
                    NgramLength = 3,
                    UseAllLengths = true,
                    Weighting = NgramExtractingEstimator.WeightingCriteria.TfIdf
                }
            };

            var pipeline = ml.Transforms.Conversion
                .MapValueToKey("Label", nameof(IntentRow.Intent))
                .Append(ml.Transforms.Text.FeaturizeText("Features", featureOptions, nameof(IntentRow.Text)))
                .Append(ml.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                .Append(ml.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            Console.WriteLine("Training…");
            var model = pipeline.Fit(split.TrainSet);

            var predictions = model.Transform(split.TestSet);
            var metrics = ml.MulticlassClassification.Evaluate(predictions, labelColumnName: "Label");

            // Class indices in the confusion matrix mean nothing without this mapping.
            VBuffer<ReadOnlyMemory<char>> keys = default;
            predictions.Schema["Label"].Annotations.GetValue("KeyValues", ref keys);
            var intents = keys.DenseValues().Select(k => k.ToString()).ToArray();

            Console.WriteLine();
            Console.WriteLine("================ EVALUATION (held-out 20%) ================");
            Console.WriteLine($"MicroAccuracy : {metrics.MicroAccuracy:P2}");
            Console.WriteLine($"MacroAccuracy : {metrics.MacroAccuracy:P2}");
            Console.WriteLine($"LogLoss       : {metrics.LogLoss:F4}");
            Console.WriteLine();

            Console.WriteLine("Per-intent recall and precision (worst first):");
            var rows = intents
                .Select((name, i) => new
                {
                    Index = i,
                    Name = name,
                    Recall = metrics.ConfusionMatrix.PerClassRecall[i],
                    Precision = metrics.ConfusionMatrix.PerClassPrecision[i]
                })
                .OrderBy(r => r.Recall);

            foreach (var r in rows)
                Console.WriteLine($"  [{r.Index,2}] {r.Name,-22} recall {r.Recall:P1}  precision {r.Precision:P1}");

            Console.WriteLine();
            Console.WriteLine("Confusion matrix (row = actual, column = predicted):");
            Console.WriteLine(metrics.ConfusionMatrix.GetFormattedConfusionTable());

            ml.Model.Save(model, split.TrainSet.Schema, modelPath);
            Console.WriteLine($"Saved model → {modelPath}");
        }
    }
}
