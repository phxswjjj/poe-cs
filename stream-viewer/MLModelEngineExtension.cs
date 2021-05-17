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
        public static void PredictionImage(this MLModelEngine<ModelInput, ModelOutput> mlEngine,
            Image img)
        {
            BlockingCollection<dynamic> predictions = new BlockingCollection<dynamic>();
            var imgRawFormat = img.RawFormat;

            var imgRepo = new ScreenRepository(img);
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
