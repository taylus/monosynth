using System;

namespace MonoSynth
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using (var game = new MonoSynthGame())
                game.Run();
        }
    }
}
