using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSynth
{
    /// <summary>
    /// https://www.david-gouveia.com/rendering-mathematical-functions-in-xna
    /// </summary>
    public abstract class SampleBufferRenderer
    {
        protected readonly Texture2D pixel;
        public int Width { get; set; }
        public int Height { get; set; }
        public Color Color { get; set; }

        public SampleBufferRenderer(GraphicsDevice graphicsDevice, int width, int height, Color? color = null)
        {
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            Width = width;
            Height = height;
            Color = color ?? Color.Green;
        }

        public abstract void Draw(SpriteBatch spriteBatch, float x, float y);

        protected void DrawLine(SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color, int thickness)
        {
            Vector2 direction = new Vector2(x2 - x1, y2 - y1);
            float rotation = (float)Math.Atan2(y2 - y1, x2 - x1);
            spriteBatch.Draw(pixel, new Vector2(x1, y1), new Rectangle(1, 1, 1, thickness), color, rotation, new Vector2(0f, (float)thickness / 2), new Vector2(direction.Length(), 1f), SpriteEffects.None, 0f);
        }

        protected void FillRectangle(SpriteBatch spriteBatch, float x1, float y1, float width, float height, Color color)
        {
            spriteBatch.Draw(pixel, new Rectangle((int)x1, (int)y1, (int)width, (int)height), color);
        }
    }
}
