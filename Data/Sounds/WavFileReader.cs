using System;

namespace Data.Sounds
{
    // Stub WAV reader - Vortice.DirectSound API is different from SharpDX
    public static class WavFileReader
    {
        public static EngineSound Read(SoundEngine engine, string filePath)
        {
            return new EngineSound();
        }
    }
}