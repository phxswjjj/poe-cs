using Microsoft.ML;
using poe.lib;
using poe.lib.ImageExtension;
using poe.lib.ML;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace stream_viewer
{
    static class PredictionEngineExtension
    {
        public static void PredictionImage(this PredictionEngine<ModelInput, ModelOutput> predictionEngine,
            Image img)
        {
            var imgRepo = new ScreenRepository(img);
            foreach (var part in imgRepo.Parts)
            {
                ModelInput inputData = new ModelInput();
                using (var ms = new MemoryStream())
                {
                    part.Source.Save(ms, img.RawFormat);
                    inputData.Image = ms.ToArray();
                }
                ModelOutput prediction = predictionEngine.Predict(inputData);
                img.TagImage(part.Location, prediction);
            }
        }
    }
}
