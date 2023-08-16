using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace ChatBot
{
    class Learning
    {
        static void d(string[] args)
        {
            var mlContext = new MLContext();

            // Chargement des données
            var data = mlContext.Data.LoadFromTextFile<IntentData>("intent_data.txt", separatorChar: '\t', hasHeader: false);
        
            // Définition du pipeline de traitement et de modèle
            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(IntentData.Text))
                .Append(mlContext.Transforms.Conversion.MapValueToKey("Label"))
                .Append(mlContext.Transforms.CopyColumns("Label", nameof(IntentData.Label)))
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"))
                .Append(mlContext.Transforms.CopyColumns("PredictedLabel", nameof(IntentPrediction.PredictedLabel)))
                .Append(mlContext.Transforms.CopyColumns("Score", nameof(IntentPrediction.Score)))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("Label"));

            var trainer = mlContext.MulticlassClassification.Trainers.SdcaNonCalibrated();
            var trainingPipeline = pipeline.Append(trainer).Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            // Entraînement du modèle
            var model = trainingPipeline.Fit(data);

            // Évaluation du modèle (optionnel)
            var predictions = model.Transform(data);
            var metrics = mlContext.MulticlassClassification.Evaluate(predictions);

            // Sauvegarde du modèle
            mlContext.Model.Save(model, data.Schema, "intent_model.zip");
        }
    }

    class IntentData
    {
        [LoadColumn(0)]
        public string Label { get; set; }

        [LoadColumn(1)]
        public string Text { get; set; }
    }

    class IntentPrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedLabel;

        [ColumnName("Score")]
        public float[] Score;
    }
}
