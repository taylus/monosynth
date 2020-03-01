using Microsoft.Xna.Framework;
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
        private SoundGenerator soundGenerator;
        private SampleBufferRenderer visualization;
        private const float amplitudeStepOnKeypress = 0.005f;
        private const float frequencyStepOnKeypress = 1.0f;

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
            soundGenerator = new SoundGenerator();
            soundGenerator.PrintCurrentWaveFunctionName();
            soundGenerator.Play();
        }

        protected override void Update(GameTime gameTime)
        {
            CurrentKeyboardState = Keyboard.GetState();

            if (IsKeyDown(Keys.Escape)) Exit();

            if (IsKeyDown(Keys.Up)) soundGenerator.Amplitude += amplitudeStepOnKeypress;
            else if (IsKeyDown(Keys.Down)) soundGenerator.Amplitude -= amplitudeStepOnKeypress;

            if (IsKeyDown(Keys.Right)) soundGenerator.Frequency += frequencyStepOnKeypress;
            else if (IsKeyDown(Keys.Left)) soundGenerator.Frequency -= frequencyStepOnKeypress;

            if (WasJustPressed(Keys.P)) soundGenerator.Pause();
            if (WasJustPressed(Keys.R)) soundGenerator.Resume();
            if (WasJustPressed(Keys.Space))
            {
                soundGenerator.SelectNextWaveFunction();
                soundGenerator.PrintCurrentWaveFunctionName();
            }

            soundGenerator.Update();
            visualization.Samples = soundGenerator.XnaAudioBuffer;

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
    }
}