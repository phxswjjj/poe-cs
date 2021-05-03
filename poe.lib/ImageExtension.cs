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
    }
}
