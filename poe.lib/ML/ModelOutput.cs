using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace poe.lib.ML
{
    public class ModelOutput : ImageData
    {
        public float[] Score { get; set; }

        public string PredictedLabel { get; set; }

        public float MaxScore => this.Score.Max();
    }
}
