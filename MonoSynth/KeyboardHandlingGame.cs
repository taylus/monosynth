using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoSynth
{
    public class KeyboardHandlingGame : Game
    {
        protected virtual KeyboardState PreviousKeyboardState { get; set; }
        protected virtual KeyboardState CurrentKeyboardState { get; set; }

        protected override void Initialize()
        {
            CurrentKeyboardState = PreviousKeyboardState = Keyboard.GetState();
            base.Initialize();
        }

        protected bool IsKeyDown(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key);
        }

        protected bool WasJustPressed(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key) && !PreviousKeyboardState.IsKeyDown(key);
        }
    }
}