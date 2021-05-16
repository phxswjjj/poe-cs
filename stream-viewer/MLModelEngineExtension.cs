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
    static class MLModelEngineExtension
    {
        public static void PredictionImage(this MLModelEngine<ModelInput, ModelOutput> mlEngine,
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
                ModelOutput prediction = mlEngine.Predict(inputData);
                img.TagImage(part.Location, prediction);
            }
        }
    }
}
