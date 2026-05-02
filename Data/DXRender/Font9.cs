using Vortice.Direct3D9;
using Vortice.Mathematics;
using System;
using System.IO;
using Rectangle = System.Drawing.Rectangle;
using Color4 = Vortice.Mathematics.Color4;

namespace Data.DXRender
{
    // Stub font class since Vortice doesn't have ID3DXFont
    public class Font9 : IDisposable
    {
        public Font9(IDirect3DDevice9 device, string faceName, int height, int width,
            int weight, int mipLevels, bool italic, int charSet,
            int precision, int quality, int pitch, string facename)
        {
            // Vortice doesn't have ID3DXFont - stub implementation
        }

        public void DrawText(object? sprite, string? text, int count, Rectangle rect, int flags, int color)
        {
            // Stub - no font rendering in Vortice
        }

        public void Dispose()
        {
            // Stub
        }
    }

    public static class FontDrawFlags
    {
        public const int Left = 0;
        public const int Right = 2;
        public const int Center = 1;
        public const int WordBreak = 0x10;
        public const int SingleLine = 0x20;
    }
}