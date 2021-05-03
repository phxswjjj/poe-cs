using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace poe.lib
{
    using ImageExtension;

    public class ScreenRepository : IDisposable
    {
        public Image Source;
        public List<PartScreen> Parts;

        public ScreenRepository(Image fullScreen)
        {
            if (fullScreen.Width != 1920 || fullScreen.Height != 1080)
                throw new Exception("only support 1920x1080");

            this.Source = fullScreen;

            var parts = new List<PartScreen>();
            parts.Add(new PartScreen(PartScreenType.LifePool, fullScreen, 0, 860, 230, 220));
            parts.Add(new PartScreen(PartScreenType.FlaskSlot1, fullScreen, 300, 970, 50, 110));
            parts.Add(new PartScreen(PartScreenType.FlaskSlot2, fullScreen, 348, 970, 50, 110));
            parts.Add(new PartScreen(PartScreenType.FlaskSlot3, fullScreen, 396, 970, 50, 110));
            parts.Add(new PartScreen(PartScreenType.FlaskSlot4, fullScreen, 444, 970, 50, 110));
            parts.Add(new PartScreen(PartScreenType.FlaskSlot5, fullScreen, 492, 970, 50, 110));
            parts.Add(new PartScreen(PartScreenType.ManaPool, fullScreen, 1690, 860, 230, 220));

            this.Parts = parts;
        }

        public PartScreen GetPart(PartScreenType type)
        {
            return this.Parts.Find(p => p.PartType == type);
        }

        public void Dispose()
        {
            this.Source.Dispose();
            foreach (var part in this.Parts)
                part.Dispose();
        }
    }

    public class PartScreen : IDisposable
    {
        public Image Source;
        public PartScreenType PartType;

        public PartScreen(PartScreenType type, Image fullScreen, int x, int y, int width, int height)
        {
            this.PartType = type;
            var img = fullScreen.CropImage(x, y, width, height);
            this.Source = img;
        }

        public void Dispose()
        {
            this.Source.Dispose();
        }
    }

    public enum PartScreenType
    {
        LifePool,
        FlaskSlot1,
        FlaskSlot2,
        FlaskSlot3,
        FlaskSlot4,
        FlaskSlot5,
        ManaPool,
    }
}
