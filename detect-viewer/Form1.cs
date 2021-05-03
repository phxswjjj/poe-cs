using Microsoft.ML;
using poe.lib;
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
            var img = imgRepo.GetPart(PartScreenType.LifePool).Source;

            var solutionDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));

            var workspaceRelativePath = Path.Combine(solutionDirectory, "workspace");
            var modelRelativePath = Path.Combine(workspaceRelativePath, "model.zip");

            MLContext mlContext = new MLContext();
            DataViewSchema predictionPipelineSchema;
            ITransformer predictionPipeline = mlContext.Model.Load(modelRelativePath, out predictionPipelineSchema);
            PredictionEngine<ModelInput, ModelOutput> predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(predictionPipeline);
            ModelInput inputData = new ModelInput();
            ModelOutput prediction = predictionEngine.Predict(inputData);
            OutputPrediction(prediction);

            pictureBox1.Image = img;
        }
        private void OutputPrediction(ModelOutput prediction)
        {
            var msg = $"Actual Value: {prediction.Label} | Predicted Value: {prediction.PredictedLabel}";
            this.Text = msg;
        }
    }
}
