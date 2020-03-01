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
        private ByteBufferRenderer byteBufferView;
        private FloatBufferRenderer floatBufferView;
        private const float amplitudeStepOnKeypress = 0.005f;
        private const float frequencyStepOnKeypress = 1.0f;

        public MonoSynthGame()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.HardwareModeSwitch = false;    //fullscreen windowed
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            SetWindowSize(1280, 720);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            byteBufferView = new ByteBufferRenderer(GraphicsDevice, 580, 240);
            floatBufferView = new FloatBufferRenderer(GraphicsDevice, 580, 240);
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
            byteBufferView.Samples = soundGenerator.XnaAudioBuffer;
            floatBufferView.Samples = soundGenerator.WorkingAudioBuffer;

            PreviousKeyboardState = CurrentKeyboardState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            const int margin = 40;
            GraphicsDevice.Clear(new Color(32, 32, 32));
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            byteBufferView.Draw(spriteBatch, GraphicsDevice.Viewport.Width - byteBufferView.Width - margin, margin);
            floatBufferView.Draw(spriteBatch, margin, margin);
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