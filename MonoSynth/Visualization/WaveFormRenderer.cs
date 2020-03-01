using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoSynth
{
    /// <see cref="https://github.com/davidluzgouveia/blog-math-function/blob/762243c6fba339974abfae035ba4bf19f7e921cb/MathFunction/MathFunction/MathFunctionRenderer.cs">
    public class WaveFormRenderer : AudioRenderer
    {
        private const int bufferSize = 500;
        private readonly float[] samples = new float[bufferSize];
        public Func<float, float, float, float> WaveFunction { get; set; } = Synth.Sine;
        public float RangeX { get; set; } = 1.0f;
        public float RangeY { get; set; } = 1.0f;

        public WaveFormRenderer(GraphicsDevice graphicsDevice, int width, int height, Color? color = null)
            : base(graphicsDevice, width, height, color)
        {

        }

        public void Update(float frequency, float amplitude)
        {
            SampleWaveFunction(frequency, amplitude);
        }

        /// <summary>
        /// Recalculates height samples from function
        /// </summary>
        private void SampleWaveFunction(float frequency, float amplitude)
        {
            // Calculate how much to advance each step
            float timeStep = 2.0f * RangeX / bufferSize;

            // Choose initial time so that the function is centered on the graph
            float time = -RangeX;

            for (int i = 0; i < bufferSize; i++)
            {
                // Get value at that point
                var value = WaveFunction(frequency, amplitude, time);

                // Scale it by the Y range in order to bring it down to [-1..1]
                value /= RangeY;

                // Transform from [-1..1] range to [0..1] range
                value = (value + 1.0f) / 2.0f;

                // Invert because the Y-axis points down when rendering
                value = 1.0f - value;

                // Translate from [0..1] range to height in pixels
                samples[i] = value * Height;

                // Advance to next value
                time += timeStep;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float x, float y)
        {
            FillRectangle(spriteBatch, x, y, Width, Height, Color.Black);
            float horizontalStep = (float)Width / bufferSize;
            for (int i = 0; i < bufferSize - 1; i++)
            {
                DrawLine(spriteBatch,
                    x1: x + (i * horizontalStep),
                    y1: MathHelper.Clamp(y + samples[i], min: y, max: y + Height),
                    x2: x + ((i + 1) * horizontalStep),
                    y2: MathHelper.Clamp(y + samples[i + 1], min: y, max: y + Height),
                    Color,
                    thickness: 2);
            }
        }
    }
}
