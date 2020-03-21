using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace MonoSynth
{
    public class SoundGenerator
    {
        private readonly DynamicSoundEffectInstance sound;
        private const int audioChannels = 2;
        private const int audioSampleRate = 44100;  //Hz => samples per sec
        private const int bytesPerSample = 2;
        private const int samplesPerBuffer = 3000;
        private double audioTime = 0;

        private readonly Func<float, float, float, float>[] waveFunctions =
        {
            WaveForms.Sine,
            WaveForms.Pulse125,
            WaveForms.Pulse25,
            WaveForms.Square,
            WaveForms.Pulse75,
            WaveForms.Sawtooth,
            WaveForms.Triangle,
            WaveForms.Noise
        };
        private readonly string[] waveFunctionNames =
        {
            "Sine Wave",
            "Pulse Wave (12.5% duty cycle)",
            "Pulse Wave (25% duty cycle)",
            "Square Wave (pulse wave w/ 50% duty cycle)",
            "Pulse Wave (75% duty cycle)",
            "Sawtooth Wave",
            "Triangle Wave",
            "Noise"
        };
        private int currentWaveFunc = 0;
        public Func<float, float, float, float> CurrentWaveFunction => waveFunctions[currentWaveFunc];

        public float[,] WorkingAudioBuffer { get; private set; } = new float[audioChannels, samplesPerBuffer];
        public byte[] XnaAudioBuffer { get; private set; } = new byte[audioChannels * samplesPerBuffer * bytesPerSample];

        public float Frequency { get; set; } = 220f;
        public float Amplitude { get; set; } = 1.0f;

        public float Volume { get => sound.Volume; set => sound.Volume = value; }

        public SoundGenerator()
        {
            sound = new DynamicSoundEffectInstance(audioSampleRate, audioChannels == 2 ? AudioChannels.Stereo : AudioChannels.Mono);
        }

        public void Pause() => sound.Pause();
        public void Play() => sound.Play();
        public void Stop() => sound.Stop();
        public void Resume() => sound.Resume();
        public void Mute() => sound.Volume = 0;

        public void SelectNextWaveFunction()
        {
            currentWaveFunc++;
            if (currentWaveFunc >= waveFunctions.Length) currentWaveFunc = 0;
        }

        public void SelectPreviousWaveFunction()
        {
            currentWaveFunc--;
            if (currentWaveFunc < 0) currentWaveFunc = waveFunctions.Length - 1;
        }

        public string CurrentWaveFunctionName => waveFunctionNames[currentWaveFunc];

        public void Update()
        {
            while (sound.PendingBufferCount < 3)
            {
                SubmitAudioBuffer();
            }
        }

        private void SubmitAudioBuffer()
        {
            FillWorkingAudioBuffer();
            ConvertAudioBuffer(WorkingAudioBuffer, XnaAudioBuffer);
            sound.SubmitBuffer(XnaAudioBuffer);
        }

        private void FillWorkingAudioBuffer()
        {
            for (int i = 0; i < samplesPerBuffer; i++)
            {
                // Here is where you sample your wave function
                WorkingAudioBuffer[0, i] = waveFunctions[currentWaveFunc](Frequency, Amplitude, (float)audioTime); // Left Channel
                WorkingAudioBuffer[1, i] = waveFunctions[currentWaveFunc](Frequency, Amplitude, (float)audioTime); // Right Channel

                // Advance time passed since beginning
                // Since the amount of samples in a second equals the chosen SampleRate
                // Then each sample should advance the time by 1 / SampleRate
                audioTime += 1.0 / audioSampleRate;
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
