using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSynth
{
    public class FloatBufferRenderer : SampleBufferRenderer
    {
        public float[,] Samples { get; set; }

        public FloatBufferRenderer(GraphicsDevice graphicsDevice, int width, int height, Color? color = null)
            : base(graphicsDevice, width, height, color)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, float x, float y)
        {
            FillRectangle(spriteBatch, x, y, Width, Height, Color.Black);
            int channels = Samples.GetLength(0);
            float horizontalStep = (float)Width / Samples.Length * channels;
            float heightScale = Height / 2;
            for (int c = 0; c < /*channels*/1; c++)
            {
                for (int i = 0; i < Samples.GetLength(1) - 1; i++)
                {
                    DrawLine(spriteBatch,
                        x1: x + (i * horizontalStep),
                        y1: y + heightScale + MathHelper.Clamp(Samples[c, i] * heightScale, min: -heightScale, max: heightScale),
                        x2: x + ((i + 1) * horizontalStep),
                        y2: y + heightScale + MathHelper.Clamp(Samples[c, i + 1] * heightScale, min: -heightScale, max: heightScale),
                        Color,
                        thickness: 2);
                }
            }
        }
    }
}
