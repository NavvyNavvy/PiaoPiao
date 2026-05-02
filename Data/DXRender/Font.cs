using Vortice.Direct3D9;
using System;
using Rectangle = System.Drawing.Rectangle;
using Color4 = Vortice.Mathematics.Color4;

namespace Data.DXRender
{
    class RenderFont : IDisposable
    {
        private RenderEngine _Engine;
        private Font9 _Font;

        public RenderFont(RenderEngine re, string fontFace, int width, int height)
        {
            _Engine = re;
            _Font = new Font9(re.Device, fontFace, height, width, 400, 1, false,
                0, 0, 0, 0, fontFace);
        }

        public void RenderText(string text, Rectangle rect, Color4 color)
        {
            // Font rendering not supported in Vortice - stub
        }

        public void Dispose()
        {
            _Font?.Dispose();
        }
    }
}