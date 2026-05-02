using System;

namespace Data.Sounds
{
    // Stub sound - Vortice.DirectSound API is different from SharpDX
    public class EngineSound : IDisposable
    {
        public EngineSound()
        {
        }

        public void SetVol(int db)
        {
        }

        public void SetBgmLoop(bool loop)
        {
        }

        public void Play()
        {
        }

        public void Stop()
        {
        }

        public void Dispose()
        {
        }
    }
}