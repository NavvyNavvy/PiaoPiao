using Vortice.DirectSound;
using System;

namespace Data.Sounds
{
    public class EngineSound : IDisposable
    {
        private IDirectSoundBuffer _buffer;
        private int _dataSize;
        private bool _loop;
        private bool _disposed;

        public EngineSound(IDirectSoundBuffer buffer, int dataSize)
        {
            _buffer = buffer;
            _dataSize = dataSize;
            _loop = false;
        }

        public void SetVol(int db)
        {
            if (_buffer != null)
            {
                // DirectSound volume: 0 (max) to -10000 (min)
                // Clamp to valid range
                int volume = Math.Max(-10000, Math.Min(0, db));
                _buffer.SetVolume(volume);
            }
        }

        public void SetBgmLoop(bool loop)
        {
            _loop = loop;
        }

        public void Play()
        {
            if (_buffer != null)
            {
                _buffer.SetCurrentPosition(0);
                var flags = _loop ? PlayFlags.Looping : PlayFlags.None;
                _buffer.Play(0, flags);
            }
        }

        public void Stop()
        {
            if (_buffer != null)
            {
                _buffer.Stop();
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _buffer?.Stop();
                _buffer?.Dispose();
                _buffer = null;
                _disposed = true;
            }
        }
    }
}