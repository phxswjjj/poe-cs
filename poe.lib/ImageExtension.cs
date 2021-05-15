using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace poe.lib.ImageExtension
{
    public static class ImageExtension
    {
        public static Image CropImage(this Image src, int x, int y, int width, int height)
        {
            var srcCropRect = new Rectangle(x, y, width, height);
            var target = new Bitmap(width, height);
            var destRect = new Rectangle(new Point(0, 0), target.Size);

            using (var bitSrc = new Bitmap(src))
            {
                using (var g = Graphics.FromImage(target))
                {
                    g.DrawImage(bitSrc, destRect, srcCropRect, GraphicsUnit.Pixel);
                }
            }

            if (src.RawFormat != target.RawFormat)
            {
                using (var ms = new MemoryStream())
                {
                    target.Save(ms, src.RawFormat);
                    var png = new Bitmap(ms);
                    target.Dispose();
                    return png;
                }
            }
            return target;
        }

        public static void TagImage(this Image src, Rectangle area, ML.ModelOutput prediction)
        {
            var label = prediction.PredictedLabel;
            var labelScore = prediction.MaxScore.ToString("0.00");
            src.TagImage(area, label, labelScore);
        }
        public static void TagImage(this Image src, Rectangle area, string label, string label2 = "")
        {
            Pen pen = new Pen(Color.Red, 2);
            var fnt = new Font("Arial", 10);

            using (var g = Graphics.FromImage(src))
            {
                g.DrawRectangle(pen, area);

                var sizeOfLabel = g.MeasureString(label, fnt);
                var rectOfLabel = new Rectangle(area.Location, sizeOfLabel.ToSize());
                g.FillRectangle(Brushes.White, rectOfLabel);
                g.DrawString(label, fnt, Brushes.Blue, rectOfLabel.Location);

                if (label2.Length > 0)
                {
                    sizeOfLabel = g.MeasureString(label2, fnt);
                    var loc = area.Location;
                    loc.Offset(0, (int)sizeOfLabel.Height);
                    rectOfLabel = new Rectangle(loc, sizeOfLabel.ToSize());
                    g.FillRectangle(Brushes.White, rectOfLabel);
                    g.DrawString(label2, fnt, Brushes.Blue, rectOfLabel.Location);
                }
            }
        }

        public static void ShowFPS(this Image src, int fps)
        {
            var label = fps.ToString();
            var fnt = new Font("Arial", 40, FontStyle.Bold);

            using (var g = Graphics.FromImage(src))
            {
                var sizeOfLabel = g.MeasureString(label, fnt);
                g.DrawString(label, fnt, Brushes.Lime, Point.Empty);
            }
        }
    }
}
