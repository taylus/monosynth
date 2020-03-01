using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoSynth
{
    /// <summary>
    /// Audio synthesis demo: https://www.david-gouveia.com/creating-a-basic-synth-in-xna-part-ii
    /// </summary>
    public class MonoSynthGame : KeyboardHandlingGame
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private DynamicSoundEffectInstance sound;
        private SampleBufferRenderer visualization;
        private const int audioChannels = 2;
        private const int audioSampleRate = 44100;  //Hz => samples per sec
        private const int bytesPerSample = 2;
        private const int samplesPerBuffer = 3000;
        private readonly float[,] workingAudioBuffer = new float[audioChannels, samplesPerBuffer];
        private readonly byte[] xnaAudioBuffer = new byte[audioChannels * samplesPerBuffer * bytesPerSample];
        private float audioTime = 0.0f;
        private float frequency = 220f;
        private float amplitude = 1.0f;
        private Func<float, float, float, float>[] waveFunctions = { Synth.Sine, Synth.Square, Synth.Sawtooth, Synth.Triangle, Synth.Noise };
        private readonly string[] waveFunctionNames = { "Sine", "Square", "Sawtooth", "Triangle", "Noise" };
        private int currentWaveFunc = 0;
        private const float amplitudeStep = 0.005f;
        private const float frequencyStep = 1.0f;

        public MonoSynthGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            SetWindowSize(640, 480);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            visualization = new SampleBufferRenderer(GraphicsDevice, GraphicsDevice.Viewport.Width - 100, GraphicsDevice.Viewport.Height - 100);
            sound = new DynamicSoundEffectInstance(audioSampleRate, audioChannels == 2 ? AudioChannels.Stereo : AudioChannels.Mono);
            sound.Play();
            Console.WriteLine(waveFunctionNames[currentWaveFunc]);
        }

        protected override void Update(GameTime gameTime)
        {
            CurrentKeyboardState = Keyboard.GetState();

            if (IsKeyDown(Keys.Escape)) Exit();

            if (IsKeyDown(Keys.Up)) amplitude += amplitudeStep;
            else if (IsKeyDown(Keys.Down)) amplitude -= amplitudeStep;

            if (IsKeyDown(Keys.Right)) frequency += frequencyStep;
            else if (IsKeyDown(Keys.Left)) frequency -= frequencyStep;

            if (WasJustPressed(Keys.Space))
            {
                currentWaveFunc++;
                if (currentWaveFunc >= waveFunctions.Length) currentWaveFunc = 0;
                Console.WriteLine(waveFunctionNames[currentWaveFunc]);
            }

            while (sound.PendingBufferCount < 3)
            {
                SubmitAudioBuffer();
            }

            PreviousKeyboardState = CurrentKeyboardState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var screenCenter = new { X = GraphicsDevice.Viewport.Width / 2.0f, Y = GraphicsDevice.Viewport.Height / 2.0f };
            GraphicsDevice.Clear(new Color(32, 32, 32));
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            visualization.Draw(spriteBatch, screenCenter.X - visualization.Width / 2.0f, screenCenter.Y - visualization.Height / 2.0f);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void SetWindowSize(int width, int height)
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.ApplyChanges();
        }

        private void SubmitAudioBuffer()
        {
            FillWorkingAudioBuffer();
            ConvertAudioBuffer(workingAudioBuffer, xnaAudioBuffer);
            visualization.Samples = xnaAudioBuffer;
            sound.SubmitBuffer(xnaAudioBuffer);
        }

        private void FillWorkingAudioBuffer()
        {
            for (int i = 0; i < samplesPerBuffer; i++)
            {
                // Here is where you sample your wave function
                workingAudioBuffer[0, i] = waveFunctions[currentWaveFunc](frequency * 2, amplitude, audioTime); // Left Channel
                workingAudioBuffer[1, i] = waveFunctions[currentWaveFunc](frequency, amplitude, audioTime); // Right Channel

                // Advance time passed since beginning
                // Since the amount of samples in a second equals the chosen SampleRate
                // Then each sample should advance the time by 1 / SampleRate
                audioTime += 1.0f / audioSampleRate;
            }
        }

        /// <summary>
        /// Converts the given 2D floating-point array of sound samples into the
        /// 1D byte array that XNA expects. Since the example uses stereo audio,
        /// the samples are interleaved (LRLRLRLR).
        /// </summary>
        private static void ConvertAudioBuffer(float[,] from, byte[] to)
        {
            int channels = from.GetLength(0);
            int samplesPerBuffer = from.GetLength(1);

            // Make sure the buffer sizes are correct
            System.Diagnostics.Debug.Assert(to.Length == samplesPerBuffer * channels * bytesPerSample, "Buffer sizes are mismatched.");

            for (int i = 0; i < samplesPerBuffer; i++)
            {
                for (int c = 0; c < channels; c++)
                {
                    // First clamp the value to the [-1.0..1.0] range
                    float floatSample = MathHelper.Clamp(from[c, i], -1.0f, 1.0f);

                    // Convert it to the 16 bit [short.MinValue..short.MaxValue] range
                    short shortSample = (short)(floatSample >= 0.0f ? floatSample * short.MaxValue : floatSample * short.MinValue * -1);

                    // Calculate the right index based on the PCM format of interleaved samples per channel [L-R-L-R]
                    int index = i * channels * bytesPerSample + c * bytesPerSample;

                    // Store the 16 bit sample as two consecutive 8 bit values in the buffer with regard to endian-ness
                    if (!BitConverter.IsLittleEndian)
                    {
                        to[index] = (byte)(shortSample >> 8);
                        to[index + 1] = (byte)shortSample;
                    }
                    else
                    {
                        to[index] = (byte)shortSample;
                        to[index + 1] = (byte)(shortSample >> 8);
                    }
                }
            }
        }
    }
}