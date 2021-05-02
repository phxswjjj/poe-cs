using System;
using System.Collections.Generic;
using System.Text;

namespace poe.lib.ML
{
    public class ModelInput : ImageData
    {
        public byte[] Image { get; set; }

        public UInt32 LabelAsKey { get; set; }
    }
}
