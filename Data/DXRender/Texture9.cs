using Vortice.Direct3D9;
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Data.DXRender
{
    // Texture9 wrapper for IDirect3DTexture9 providing SharpDX-style API
    public class Texture9 : IDisposable
    {
        private IDirect3DTexture9? _texture;
        private int _width;
        private int _height;

        public Texture9(IDirect3DTexture9? texture)
        {
            _texture = texture;
            if (_texture != null)
            {
                var desc = _texture.GetLevelDesc(0);
                _width = (int)desc.Width;
                _height = (int)desc.Height;
            }
            else
            {
                _width = 0;
                _height = 0;
            }
        }

        public static Texture9 FromStream(IDirect3DDevice9 device, Stream stream, int width, int height, int levels, Usage usage, Format format, Pool pool, int colorKey)
        {
            return new Texture9(null);
        }

        public static Texture9 FromFile(IDirect3DDevice9 device, string fileName, Usage usage, Pool pool)
        {
            if (!System.IO.File.Exists(fileName))
                return new Texture9(null);
            try
            {
                using var bitmap = new Bitmap(fileName);
                return CreateTextureFromBitmap(device, bitmap);
            }
            catch
            {
                return new Texture9(null);
            }
        }

        public static Texture9 CreateTextureFromBitmap(IDirect3DDevice9 device, Bitmap bitmap)
        {
            if (bitmap == null)
                return new Texture9(null);

            try
            {
                // Convert to 32-bit ARGB format
                using var bitmap32 = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppArgb);

                int width = NextPowerOf2(bitmap32.Width);
                int height = NextPowerOf2(bitmap32.Height);

                var texture = device.CreateTexture((uint)width, (uint)height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
                if (texture == null)
                    return new Texture9(null);

                var rect = new Rectangle(0, 0, bitmap32.Width, bitmap32.Height);
                var bitmapData = bitmap32.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                try
                {
                    var levelDesc = texture.GetLevelDesc(0);
                    var destRect = texture.LockRect(0, LockFlags.None);
                    try
                    {
                        unsafe
                        {
                            byte* srcPtr = (byte*)bitmapData.Scan0;
                            byte* dstPtr = (byte*)destRect.DataPointer;

                            int srcStride = bitmapData.Stride;
                            int dstStride = destRect.Pitch;

                            // Copy row by row
                            for (int y = 0; y < bitmap32.Height && y < (int)levelDesc.Height; y++)
                            {
                                byte* srcRow = srcPtr + y * srcStride;
                                byte* dstRow = dstPtr + y * dstStride;

                                // Copy and swap R and B channels (BGRA -> RGBA)
                                for (int x = 0; x < bitmap32.Width && x < (int)levelDesc.Width; x++)
                                {
                                    int srcIdx = x * 4;
                                    int dstIdx = x * 4;
                                    dstRow[dstIdx + 0] = srcRow[srcIdx + 2]; // R
                                    dstRow[dstIdx + 1] = srcRow[srcIdx + 1]; // G
                                    dstRow[dstIdx + 2] = srcRow[srcIdx + 0]; // B
                                    dstRow[dstIdx + 3] = srcRow[srcIdx + 3]; // A
                                }
                            }
                        }
                    }
                    finally
                    {
                        texture.UnlockRect(0);
                    }
                }
                finally
                {
                    bitmap32.UnlockBits(bitmapData);
                }

                return new Texture9(texture);
            }
            catch
            {
                return new Texture9(null);
            }
        }

        private static int NextPowerOf2(int value)
        {
            int power = 1;
            while (power < value) power *= 2;
            return power;
        }

        public int GetWidth() => _width;
        public int GetHeight() => _height;
        public IDirect3DTexture9? NativeTexture => _texture;

        public void Dispose()
        {
            _texture?.Dispose();
            _texture = null;
        }
    }
}