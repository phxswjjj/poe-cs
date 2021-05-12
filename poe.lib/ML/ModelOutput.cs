using System;
using System.Collections.Generic;
using System.Text;

namespace poe.lib.ML
{
    public class ModelOutput : ImageData
    {
        public float[] Score { get; set; }

        public string PredictedLabel { get; set; }
    }
}
