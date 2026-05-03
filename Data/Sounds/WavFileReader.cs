using Vortice.DirectSound;
using Vortice.Multimedia;
using System;
using System.IO;
using System.Text;

namespace Data.Sounds
{
    public static class WavFileReader
    {
        public static EngineSound Read(SoundEngine engine, string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return ReadFromStream(engine, fs);
            }
        }

        private static EngineSound ReadFromStream(SoundEngine engine, Stream stream)
        {
            var reader = new BinaryReader(stream);

            // RIFF header
            var riff = reader.ReadBytes(4);
            if (riff[0] != 'R' || riff[1] != 'I' || riff[2] != 'F' || riff[3] != 'F')
                throw new InvalidOperationException("Not a valid WAV file: missing RIFF header");

            // RIFF chunk size (4 bytes) - skip it
            reader.ReadBytes(4);

            // WAVE header at offset 8
            var wave = reader.ReadBytes(4);
            if (wave[0] != 'W' || wave[1] != 'A' || wave[2] != 'V' || wave[3] != 'E')
                throw new InvalidOperationException("Not a valid WAV file: missing WAVE header");

            // Find fmt chunk
            int channels = 0;
            int sampleRate = 0;
            int bitsPerSample = 0;
            int audioDataSize = 0;

            while (stream.Position < stream.Length)
            {
                var chunkId = reader.ReadBytes(4);
                var chunkSize = reader.ReadInt32();

                string chunkStr = Encoding.ASCII.GetString(chunkId);

                if (chunkStr == "fmt ")
                {
                    var fmtData = reader.ReadBytes(chunkSize);
                    channels = BitConverter.ToInt16(fmtData, 2);
                    sampleRate = BitConverter.ToInt32(fmtData, 4);
                    bitsPerSample = BitConverter.ToInt16(fmtData, 14);
                }
                else if (chunkStr == "data")
                {
                    audioDataSize = chunkSize;
                }
                else
                {
                    stream.Position += chunkSize;
                }

                if (audioDataSize > 0 && channels > 0)
                    break;
            }

            if (audioDataSize == 0 || channels == 0)
                throw new InvalidOperationException("Invalid WAV format");

            // Read audio data
            var audioData = reader.ReadBytes(audioDataSize);

            // Create DirectSound buffer
            var device = engine.Device;
            var waveFormat = new WaveFormat(sampleRate, bitsPerSample, channels);

            var bufferDesc = new SoundBufferDescription()
            {
                BufferBytes = audioDataSize,
                Format = waveFormat,
                Flags = BufferFlags.GlobalFocus | BufferFlags.ControlVolume
            };

            var buffer = device.CreateSoundBuffer(bufferDesc, null);

            // Write audio data to buffer using the Write method
            buffer.Write(audioData, 0, LockFlags.None);

            return new EngineSound(buffer, audioDataSize);
        }
    }
}