using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSynth
{
    public class SampleBufferRenderer
    {
        private readonly Texture2D pixel;
        public byte[] Samples { get; set; }
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

        public void Draw(SpriteBatch spriteBatch, float x, float y)
        {
            FillRectangle(spriteBatch, x, y, Width, Height, Color.Black);

            float horizontalStep = (float)Width / Samples.Length;
            float heightScale = (float)Height / byte.MaxValue;
            for (int i = 0; i < Samples.Length - 1; i++)
            {
                DrawLine(spriteBatch,
                    x1: x + (i * horizontalStep),
                    y1: y + (Samples[i] * heightScale),
                    x2: x + ((i + 1) * horizontalStep),
                    y2: y + (Samples[i + 1] * heightScale),
                    Color,
                    thickness: 2);
            }
        }

        private void DrawLine(SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color, int thickness)
        {
            Vector2 direction = new Vector2(x2 - x1, y2 - y1);
            float rotation = (float)Math.Atan2(y2 - y1, x2 - x1);
            spriteBatch.Draw(pixel, new Vector2(x1, y1), new Rectangle(1, 1, 1, thickness), color, rotation, new Vector2(0f, (float)thickness / 2), new Vector2(direction.Length(), 1f), SpriteEffects.None, 0f);
        }

        private void FillRectangle(SpriteBatch spriteBatch, float x1, float y1, float width, float height, Color color)
        {
            spriteBatch.Draw(pixel, new Rectangle((int)x1, (int)y1, (int)width, (int)height), color);
        }
    }
}
