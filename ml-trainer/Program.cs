using Microsoft.ML;
using Microsoft.ML.Vision;
using poe.lib.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Microsoft.ML.DataOperationsCatalog;

namespace ml_trainer
{
    class Program
    {
        static void Main(string[] args)
        {
            Training();

            Console.WriteLine("press ENTER to continue..");
            Console.ReadLine();
        }

        private static void Training()
        {
            var solutionDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
            var assetsRelativePath = Path.Combine(solutionDirectory, "assets");

            var workspaceRelativePath = Path.Combine(solutionDirectory, "workspace");
            var modelRelativePath = Path.Combine(workspaceRelativePath, "model.zip");

            MLContext mlContext = new MLContext();

            IEnumerable<ImageData> images = LoadImagesFromDirectory(folder: assetsRelativePath, useFolderNameAsLabel: true);
            IDataView imageData = mlContext.Data.LoadFromEnumerable(images);
            IDataView shuffledData = mlContext.Data.ShuffleRows(imageData);

            var preprocessingPipeline = mlContext.Transforms.Conversion.MapValueToKey(
                    outputColumnName: "LabelAsKey",
                    inputColumnName: "Label")
                .Append(mlContext.Transforms.LoadRawImageBytes(
                    outputColumnName: "Image",
                    inputColumnName: "ImagePath",
                    imageFolder: assetsRelativePath));
            IDataView preProcessedData = preprocessingPipeline
                                .Fit(shuffledData)
                                .Transform(shuffledData);

            TrainTestData trainSplit = mlContext.Data.TrainTestSplit(data: preProcessedData, testFraction: 0.2);

            IDataView trainSet = trainSplit.TrainSet;
            IDataView validationSet = trainSplit.TestSet;

            var trainingPipeline = mlContext.MulticlassClassification.Trainers.ImageClassification(
                    featureColumnName: "Image",
                    labelColumnName: "LabelAsKey",
                    validationSet: validationSet)
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            ITransformer trainedModel = trainingPipeline.Fit(trainSet);
            Console.WriteLine("save model");
            mlContext.Model.Save(trainedModel, imageData.Schema, modelRelativePath);

            var testSet = validationSet;
            ClassifySingleImage(mlContext, testSet, trainedModel);
            ClassifyImages(mlContext, testSet, trainedModel);
        }

        public static void ClassifySingleImage(MLContext mlContext, IDataView data, ITransformer trainedModel)
        {
            PredictionEngine<ModelInput, ModelOutput> predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(trainedModel);

            ModelInput image = mlContext.Data.CreateEnumerable<ModelInput>(data, reuseRowObject: true).FirstOrDefault();
            if (image == null)
            {
                Console.WriteLine("no data");
                return;
            }

            ModelOutput prediction = predictionEngine.Predict(image);

            Console.WriteLine("Classifying single image");
            OutputPrediction(prediction);
        }

        public static void ClassifyImages(MLContext mlContext, IDataView data, ITransformer trainedModel)
        {
            IDataView predictionData = trainedModel.Transform(data);

            IEnumerable<ModelOutput> predictions = mlContext.Data.CreateEnumerable<ModelOutput>(predictionData, reuseRowObject: true).Take(10);

            Console.WriteLine("Classifying multiple images");
            foreach (var prediction in predictions)
            {
                OutputPrediction(prediction);
            }
        }

        private static void OutputPrediction(ModelOutput prediction)
        {
            string imageName = Path.GetFileName(prediction.ImagePath);
            Console.WriteLine($"Image: {imageName} | Actual Value: {prediction.Label} | Predicted Value: {prediction.PredictedLabel} | Score: [{string.Join(",", prediction.Score)}]");
        }

        public static IEnumerable<ImageData> LoadImagesFromDirectory(string folder, bool useFolderNameAsLabel = true)
        {
            var files = Directory.GetFiles(folder, "*",
                searchOption: SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if ((Path.GetExtension(file) != ".jpg") && (Path.GetExtension(file) != ".png"))
                    continue;

                var label = Path.GetFileName(file);

                if (useFolderNameAsLabel)
                    label = Directory.GetParent(file).Name;
                else
                {
                    for (int index = 0; index < label.Length; index++)
                    {
                        if (!char.IsLetter(label[index]))
                        {
                            label = label.Substring(0, index);
                            break;
                        }
                    }
                }

                yield return new ImageData()
                {
                    ImagePath = file,
                    Label = label
                };
            }
        }
    }
}
