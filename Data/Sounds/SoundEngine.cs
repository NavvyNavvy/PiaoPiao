using Vortice.DirectSound;
using System;

namespace Data.Sounds
{
    public class SoundEngine : IDisposable
    {
        private IDirectSound _device;

        public SoundEngine(IntPtr hwnd)
        {
            _device = DSound.DirectSoundCreate();
            _device.SetCooperativeLevel(hwnd, CooperativeLevel.Priority);
        }

        public IDirectSound Device => _device;

        public void Dispose()
        {
            _device?.Dispose();
            _device = null;
        }
    }
}