﻿using Microsoft.ML;
using poe.lib;
using poe.lib.ImageExtension;
using poe.lib.ML;
using poe.lib.Win;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stream_viewer
{
    public partial class Form1 : Form
    {
        Thread JobRefreshStream = null;
        bool Stopping = false;
        bool IsPause = false;
        PredictionEngine<ModelInput, ModelOutput> _PredictionEngine;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeML();

            var missAndRetryTime = 10 * 1000;
            var findWindowTitle = "Path of Exile";
            var targetHandle = ScreenCapture.User32.FindWindowByCaption(findWindowTitle);
            var capture = new ScreenCapture() { EnableDebug = true };

            JobRefreshStream = new Thread(() =>
            {
                while (!Stopping)
                {
                    if (targetHandle == IntPtr.Zero)
                    {
                        //wait retry
                        Thread.Sleep(missAndRetryTime);
                        targetHandle = ScreenCapture.User32.FindWindowByCaption(findWindowTitle);
                    }
                    if (!IsPause && targetHandle == ScreenCapture.User32.GetForegroundWindow())
                    {
                        try
                        {
                            var sw = new Stopwatch();
                            sw.Start();

                            var img = capture.CaptureWindow(targetHandle);
                            if (pictureBox1.Image != null)
                                pictureBox1.Image.Dispose();

                            PredictionImage(img);
                            pictureBox1.Image = img;

                            sw.Stop();
                            var msec = sw.ElapsedMilliseconds;
                            if (this.InvokeRequired)
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    this.Text = $"{msec:#,##} ms";
                                });
                            }
                        }
                        catch (System.Runtime.InteropServices.ExternalException)
                        {
                            targetHandle = IntPtr.Zero;
                        }
                    }
                }
            });

            JobRefreshStream.Start();
        }

        private void InitializeML()
        {
            var solutionDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
            var workspaceRelativePath = Path.Combine(solutionDirectory, "workspace");
            var modelRelativePath = Path.Combine(workspaceRelativePath, "model.zip");

            MLContext mlContext = new MLContext();
            ITransformer predictionPipeline = mlContext.Model.Load(modelRelativePath, out var predictionPipelineSchema);
            _PredictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(predictionPipeline);
        }

        private void PredictionImage(Image img)
        {
            var predictionEngine = _PredictionEngine;

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
                var label = prediction.PredictedLabel;
                img.TagImage(part.Location, label);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (JobRefreshStream != null)
            {
                Stopping = true;
                if (!JobRefreshStream.Join(1000))
                {
                    try
                    {
                        JobRefreshStream.Abort();
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                }

            }
        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            IsPause = true;
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            IsPause = false;
        }
    }
}
