using Vortice.DirectSound;
using System;

namespace Data.Sounds
{
    // Stub sound engine - Vortice.DirectSound API is different from SharpDX
    public class SoundEngine : IDisposable
    {
        public SoundEngine(IntPtr hwnd)
        {
            // Stub - DirectSound initialization not implemented
        }

        public void Dispose()
        {
        }
    }
}