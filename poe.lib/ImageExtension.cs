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

        public static void TagImage(this Image src, Rectangle area, string label)
        {
            Pen pen = new Pen(Color.Red, 2);
            var fnt = new Font("Arial", 10);

            using (var g = Graphics.FromImage(src))
            {
                g.DrawRectangle(pen, area);

                var sizeOfLabel = g.MeasureString(label, fnt);
                var rectOfLabel = new Rectangle(area.Location, sizeOfLabel.ToSize());
                g.FillRectangle(Brushes.White, rectOfLabel);
                g.DrawString(label, fnt, Brushes.Blue, area.Location);
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
