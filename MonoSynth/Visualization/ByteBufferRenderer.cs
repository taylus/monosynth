using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSynth
{
    public class ByteBufferRenderer : SampleBufferRenderer
    {
        public byte[] Samples { get; set; }

        public ByteBufferRenderer(GraphicsDevice graphicsDevice, int width, int height, Color? color = null)
            : base(graphicsDevice, width, height, color)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, float x, float y)
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
    }
}
