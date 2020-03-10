using System;

namespace MonoSynth
{
    public static class WaveForms
    {
        private static readonly Random random = new Random();

        public static float Sine(float frequency, float amplitude, float time)
        {
            return (float)Math.Sin(frequency * time * 2 * Math.PI) * amplitude;
        }

        public static float Square(float frequency, float amplitude, float time)
        {
            return Pulse(frequency, amplitude, time, dutyCycle: 0.5f);
        }

        public static float Pulse125(float frequency, float amplitude, float time)
        {
            return Pulse(frequency, amplitude, time, dutyCycle: 0.125f);
        }

        public static float Pulse25(float frequency, float amplitude, float time)
        {
            return Pulse(frequency, amplitude, time, dutyCycle: 0.25f);
        }

        public static float Pulse75(float frequency, float amplitude, float time)
        {
            return Pulse(frequency, amplitude, time, dutyCycle: 0.75f);
        }

        private static float Pulse(float frequency, float amplitude, float time, float dutyCycle)
        {
            return Sine(frequency, amplitude, time) >= (1 - dutyCycle) - 0.5f ? amplitude : -amplitude;
        }

        public static float Sawtooth(float frequency, float amplitude, float time)
        {
            return (float)(2 * (time * frequency - Math.Floor(time * frequency + 0.5))) * amplitude;
        }

        public static float Triangle(float frequency, float amplitude, float time)
        {
            return Math.Abs(Sawtooth(frequency, amplitude, time)) * 2.0f - 1.0f;
        }

        public static float Noise(float frequency, float amplitude, float time)
        {
            return (float)(random.NextDouble() - random.NextDouble()) * amplitude;
        }
    }
}
