using Microsoft.ML;
using Microsoft.ML.Data;
using poe.lib;
using poe.lib.ImageExtension;
using poe.lib.ML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace detect_viewer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var imgRepo = new ScreenRepository(Image.FromFile("screenshot.png"));
            //var img = imgRepo.GetPart(PartScreenType.ManaPool).Source;
            var img = imgRepo.Source;

            var solutionDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));

            var workspaceRelativePath = Path.Combine(solutionDirectory, "workspace");
            var modelRelativePath = Path.Combine(workspaceRelativePath, "model.zip");

            var mlEngine = new MLModelEngine<ModelInput, ModelOutput>(modelRelativePath);

            //取得 label list, 對應 score 順序
            //var labels = mlEngine.RetriveLabels();
            foreach (var part in imgRepo.Parts)
            {
                ModelInput inputData = new ModelInput();
                using (var ms = new MemoryStream())
                {
                    part.Source.Save(ms, img.RawFormat);
                    inputData.Image = ms.ToArray();
                }
                ModelOutput prediction = mlEngine.Predict(inputData);
                //var index = Array.IndexOf(labels, prediction.PredictedLabel);
                //var score = prediction.Score[index];
                img.TagImage(part.Location, prediction);
            }

            pictureBox1.Image = img;
        }
    }
}
