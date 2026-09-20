using Microsoft.ML;
using vangoplus_server.ML;

namespace vangoplus_server.Application.Chat
{
    public record IntentResult(string Intent, float Confidence, string Source);

    /// <summary>
    /// Wraps the trained ML.NET model. Registered as a SINGLETON: loading the model is
    /// expensive and must happen once, not per request.
    ///
    /// Two deterministic rules run BEFORE the model:
    ///   1. Emergency keywords. In testing, emergency was sometimes classified as
    ///      child_status or van_location. It is the one intent that must never be missed,
    ///      so it does not depend on the model.
    ///   2. Short greetings. One or two words carry too little signal — "good morning"
    ///      was classified as pickup_time because "morning" appears throughout the pickup
    ///      examples. Exact match handles these instead.
    /// </summary>
    public class IntentClassifier
    {
        public const float ConfidenceFloor = 0.60f;

        private static readonly string[] EmergencyKeywords =
        {
            "accident", "emergency", "missing", "gum ho", "kho gaya", "kho gayi",
            "injured", "zakhmi", "chot", "hurt", "khatra", "danger", "urgent",
            "foran", "police", "rescue", "ambulance", "unauthorized", "anjaan",
            "zabardasti", "khoon", "takar", "trouble"
        };

        private static readonly Dictionary<string, string> ShortPhrases = new(StringComparer.OrdinalIgnoreCase)
        {
            ["hi"] = "greeting",   ["hii"] = "greeting",  ["hey"] = "greeting",
            ["heyy"] = "greeting", ["hello"] = "greeting",["helloo"] = "greeting",
            ["salam"] = "greeting",["slam"] = "greeting", ["salaam"] = "greeting",
            ["aoa"] = "greeting",  ["assalam"] = "greeting",
            ["assalamualaikum"] = "greeting", ["assalam o alaikum"] = "greeting",
            ["good morning"] = "greeting", ["good afternoon"] = "greeting",
            ["good evening"] = "greeting", ["good day"] = "greeting",
            ["koi hai"] = "greeting", ["kaise ho"] = "greeting", ["kya haal hai"] = "greeting",

            ["bye"] = "goodbye",   ["bye bye"] = "goodbye", ["gn"] = "goodbye",
            ["good night"] = "goodbye", ["allah hafiz"] = "goodbye",
            ["khuda hafiz"] = "goodbye", ["alvida"] = "goodbye",

            ["thanks"] = "thanks", ["thanx"] = "thanks", ["thnx"] = "thanks",
            ["thank you"] = "thanks", ["thanku"] = "thanks", ["ty"] = "thanks",
            ["shukriya"] = "thanks", ["shukria"] = "thanks", ["jazakallah"] = "thanks",
            ["mehrbani"] = "thanks",
            ["ok"] = "thanks",   ["okay"] = "thanks",  ["haan"] = "thanks",
            ["ji"] = "thanks",   ["acha"] = "thanks",  ["theek hai"] = "thanks",
            ["done"] = "thanks", ["got it"] = "thanks"
        };

        private readonly PredictionEngine<IntentRow, IntentPrediction> _engine;
        private readonly object _lock = new();

        public IntentClassifier(IWebHostEnvironment env)
        {
            var modelPath = Path.Combine(env.ContentRootPath, "ML", "intent_model.zip");
            var ml = new MLContext();
            var model = ml.Model.Load(modelPath, out _);

            // MapKeyToValue in the pipeline means PredictedLabel already comes back as the
            // intent name, so the Score slot names are not needed here.
            _engine = ml.Model.CreatePredictionEngine<IntentRow, IntentPrediction>(model);
        }

        public IntentResult Classify(string message)
        {
            var text = Normalise(message);

            if (EmergencyKeywords.Any(k => text.Contains(k, StringComparison.Ordinal)))
                return new IntentResult("emergency", 1f, "keyword");

            if (ShortPhrases.TryGetValue(text, out var shortIntent))
                return new IntentResult(shortIntent, 1f, "exact");

            IntentPrediction prediction;
            // PredictionEngine is NOT thread-safe and this class is a singleton.
            lock (_lock)
            {
                prediction = _engine.Predict(new IntentRow { Text = text, Intent = string.Empty });
            }

            var confidence = prediction.Score.Length > 0 ? prediction.Score.Max() : 0f;
            var intent = prediction.Intent;

            if (confidence < ConfidenceFloor)
                return new IntentResult("fallback", confidence, "low_confidence");

            return new IntentResult(intent, confidence, "model");
        }

        private static string Normalise(string message)
        {
            var text = (message ?? string.Empty).Trim().ToLowerInvariant();
            text = System.Text.RegularExpressions.Regex.Replace(text, @"[^\w\s]", " ");
            return System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").Trim();
        }
    }
}
