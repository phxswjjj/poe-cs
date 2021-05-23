using poe.lib;
using poe.lib.ImageExtension;
using poe.lib.ML;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace stream_viewer
{
    static class MLModelEngineExtension
    {
        static DateTime? UndefinedImageAllowSaveDate = null;
        public static void PredictionImage(this MLModelEngine<ModelInput, ModelOutput> mlEngine,
            Image img)
        {
            var solutionDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
            var assetsRelativePath = Path.Combine(solutionDirectory, "assets");
            var undefinedRelativePath = Path.Combine(assetsRelativePath, "undefined");

            BlockingCollection<dynamic> predictions = new BlockingCollection<dynamic>();
            var imgRawFormat = img.RawFormat;

            var imgRepo = new ScreenRepository(img);
            var prefixFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            System.Threading.Tasks.Parallel.ForEach(imgRepo.Parts, part =>
            {
                ModelInput inputData = new ModelInput();
                using (var ms = new MemoryStream())
                {
                    part.Source.Save(ms, imgRawFormat);
                    inputData.Image = ms.ToArray();
                }
                ModelOutput prediction = mlEngine.Predict(inputData);
                predictions.Add(new
                {
                    PartImg = part,
                    Prediction = prediction,
                });
                var isPredictFail = false;
                if (part.PartType == PartScreenType.LifePool && !prediction.PredictedLabel.StartsWith("HP"))
                    isPredictFail = true;
                else if (part.PartType == PartScreenType.ManaPool && !prediction.PredictedLabel.StartsWith("MP"))
                    isPredictFail = true;
                else if (prediction.PredictedLabel.StartsWith("HP") && part.PartType != PartScreenType.LifePool)
                    isPredictFail = true;
                else if (prediction.PredictedLabel.StartsWith("MP") && part.PartType != PartScreenType.ManaPool)
                    isPredictFail = true;
                else if (part.PartType == PartScreenType.FlaskSlot1 && !prediction.PredictedLabel.StartsWith("LF")
                    && !prediction.PredictedLabel.StartsWith("EF"))
                    isPredictFail = true;
                else if (part.PartType == PartScreenType.FlaskSlot2 && !prediction.PredictedLabel.StartsWith("MF")
                    && !prediction.PredictedLabel.StartsWith("EF"))
                    isPredictFail = true;
                else if (part.PartType == PartScreenType.FlaskSlot3 && !prediction.PredictedLabel.StartsWith("MF")
                    && !prediction.PredictedLabel.StartsWith("EF"))
                    isPredictFail = true;
                else if (part.PartType == PartScreenType.FlaskSlot4 && !prediction.PredictedLabel.StartsWith("MF")
                    && !prediction.PredictedLabel.StartsWith("EF"))
                    isPredictFail = true;
                else if (part.PartType == PartScreenType.FlaskSlot5 && !prediction.PredictedLabel.StartsWith("MF")
                    && !prediction.PredictedLabel.StartsWith("EF"))
                    isPredictFail = true;

                if (isPredictFail || prediction.MaxScore < 0.5)
                {
                    if (!UndefinedImageAllowSaveDate.HasValue || DateTime.Now > UndefinedImageAllowSaveDate.Value)
                    {
                        UndefinedImageAllowSaveDate = DateTime.Now.AddMinutes(1);
                        var filePath = Path.Combine(undefinedRelativePath, $"{prefixFileName}_{part.PartType}.png");
                        part.Source.Save(filePath);
                    }
                }
            });

            foreach (var d in predictions)
            {
                PartScreen part = d.PartImg;
                ModelOutput prediction = d.Prediction;
                img.TagImage(part.Location, prediction);
            }
        }
    }
}
