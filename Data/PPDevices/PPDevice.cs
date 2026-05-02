using Vortice.Direct3D9;
using Vortice.Mathematics;
using Data.DXRender;
using Data.Globals;
using System;
using System.IO;
using RectangleF = System.Drawing.RectangleF;
using Rectangle = System.Drawing.Rectangle;
using Color4 = Vortice.Mathematics.Color4;

namespace Data.PPDevices
{
    public class PPDevice
    {
        public IDirect3DVertexDeclaration9? Decl_Tex;
        private IDirect3DVertexBuffer9? _vertices;
        public IDirect3DDevice9? device;

        private RenderEngine? _Engine;
        private RenderSprite? _Sprite;

        public Texture9? green;
        public Texture9? blue1;
        public Texture9? blue2;
        public Texture9? red;
        public Texture9? pink;

        public PPDevice(IntPtr handle)
        {
            var present = new PresentParameters();
            present.Windowed = true;
            present.SwapEffect = SwapEffect.Discard;
            present.BackBufferWidth = 800;
            present.BackBufferHeight = 600;
            present.PresentationInterval = PresentInterval.One;
            present.BackBufferFormat = Format.A8R8G8B8;

            var d3d = Vortice.Direct3D9.D3D9.Direct3DCreate9();
            if (d3d == null) throw new InvalidOperationException("Failed to create Direct3D9");

            device = d3d.CreateDevice(0, DeviceType.Hardware, handle, CreateFlags.HardwareVertexProcessing, present);

            if (device == null) throw new InvalidOperationException("Failed to create device");

            device.SetRenderState(RenderState.AlphaBlendEnable, true);
            device.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);
            device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
            device.SetRenderState(RenderState.AlphaTestEnable, false);
            device.SetSamplerState(0, SamplerState.AddressU, (int)TextureAddress.Clamp);
            device.SetSamplerState(0, SamplerState.AddressV, (int)TextureAddress.Clamp);
            device.SetTextureStageState(0, TextureStage.ColorOperation, (int)TextureOperation.Modulate);
            device.SetTextureStageState(0, TextureStage.ColorArg1, (int)TextureArgument.Texture);
            device.SetTextureStageState(0, TextureStage.ColorArg2, (int)TextureArgument.Diffuse);
            device.SetTextureStageState(0, TextureStage.AlphaOperation, (int)TextureOperation.Modulate);
            device.SetTextureStageState(0, TextureStage.AlphaArg1, (int)TextureArgument.Diffuse);
            device.SetTextureStageState(0, TextureStage.AlphaArg2, (int)TextureArgument.Texture);

            var vertexElems = new[] {
                new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
                new VertexElement(0, 16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                new VertexElement(0, 32, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                VertexElement.VertexDeclarationEnd
            };
            Decl_Tex = device.CreateVertexDeclaration(vertexElems);

            const int stride = 36; // Vertex size
            _vertices = device.CreateVertexBuffer(stride * 6, Usage.WriteOnly | Usage.Dynamic, VertexFormat.Position | VertexFormat.Texture1 | VertexFormat.Diffuse, Pool.Default);

            d3d.Dispose();
        }

        public void beginDraw()
        {
            if (device == null) return;
            try
            {
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new Vortice.Mathematics.Color(0, 128, 0, 255), 1.0f, 0);
                device.BeginScene();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("beginDraw error: " + ex.Message);
            }
        }

        public void endDraw()
        {
            if (device == null) return;
            try
            {
                device.EndScene();
                device.Present();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("endDraw error: " + ex.Message);
            }
        }

        public void BitBlt(Resources.BalloonItemPic_Base pic, RectangleF pos, RectangleF tex, float alpha = 1.0f)
        {
            if (device == null || pic == null || pic.bitmap == null) return;

            var texNative = pic.bitmap.NativeTexture;
            if (texNative == null) return;

            device.SetTexture(0, texNative);

            // Vertex format: Float4(pos.xyzw), Float4(tex.xyzw), Float4(col.xyzw) = 48 bytes
            const int stride = 48;
            var vertices = _vertices;
            if (vertices == null) return;

            var locked = vertices.Lock<byte>(0, stride * 4, LockFlags.None);
            try
            {
                // Triangle strip vertices: pos(x,y,z,w), tex(u,v,0,0), col(r,g,b,a)
                float z = 0.5f;
                float w = 1.0f;
                int a = (int)(alpha * 255) << 24;
                int r = 255 << 16;
                int g = 255 << 8;
                int b = 255;
                int col = a | r | g | b;

                // Normalize texture coordinates to [0,1] range
                float texWidth = pic.bitmap.GetWidth();
                float texHeight = pic.bitmap.GetHeight();
                float u0 = tex.Left / texWidth;
                float v0 = tex.Top / texHeight;
                float u1 = tex.Right / texWidth;
                float v1 = tex.Bottom / texHeight;

                unsafe
                {
                    // Get pointer from span
                    fixed (void* ptr = &locked.GetPinnableReference())
                    {
                        IntPtr basePtr = new IntPtr(ptr);
                        WriteVertex(basePtr, pos.Left, pos.Top, z, w, u0, v0, col);
                        WriteVertex(basePtr + stride, pos.Right, pos.Top, z, w, u1, v0, col);
                        WriteVertex(basePtr + stride * 2, pos.Left, pos.Bottom, z, w, u0, v1, col);
                        WriteVertex(basePtr + stride * 3, pos.Right, pos.Bottom, z, w, u1, v1, col);
                    }
                }
            }
            finally
            {
                vertices.Unlock();
            }

            device.SetStreamSource(0, vertices, 0, stride);
            device.VertexDeclaration = Decl_Tex;
            device.DrawPrimitive(PrimitiveType.TriangleStrip, 0, 2);
        }

        public void BitBlt(Resources.BalloonItemPic_Base pic, float x, float y, RectangleF tex)
        {
            BitBlt(pic, new RectangleF(x, y, tex.Width, tex.Height), tex, 1.0f);
        }

        public void BitBlt(Resources.BalloonItemPic_Base pic, float x, float y, RectangleF tex, float alpha)
        {
            BitBlt(pic, new RectangleF(x, y, tex.Width, tex.Height), tex, alpha);
        }

        private static unsafe void WriteVertex(IntPtr ptr, float x, float y, float z, float w, float u, float v, int color)
        {
            float* p = (float*)ptr;
            p[0] = x; p[1] = y; p[2] = z; p[3] = w;  // pos
            p[4] = u; p[5] = v; p[6] = 0; p[7] = 0;  // tex
            byte* c = (byte*)&color;
            p[8] = c[0] / 255f; p[9] = c[1] / 255f; p[10] = c[2] / 255f; p[11] = c[3] / 255f; // col
        }

        public void BitBlt(string str, Rectangle rect, Color4 color, int width, int height, string ZiTi)
        {
            // Text rendering not implemented - stub
        }

        private MemoryStream _BitmapStream = new MemoryStream(1024 * 1024);

        public Texture9? CreateTextureFromBitmap(System.Drawing.Bitmap bitmap)
        {
            if (device == null) return null;
            return Texture9.CreateTextureFromBitmap(device, bitmap);
        }

        public Texture9? LoadBitmapFromFile(string file)
        {
            if (!System.IO.File.Exists(file)) return null;
            try
            {
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(file);
                int w = GlobalB.Get2Min(bitmap.Size.Width);
                int h = GlobalB.Get2Min(bitmap.Size.Height);
                var bitmap2 = new System.Drawing.Bitmap(w, h);
                var g = System.Drawing.Graphics.FromImage(bitmap2);
                g.DrawImage(bitmap, new System.Drawing.Rectangle(0, 0, bitmap.Size.Width, bitmap.Size.Height));
                g.Save();
                g.Dispose();
                var tex = CreateTextureFromBitmap(bitmap2);
                bitmap.Dispose();
                bitmap2.Dispose();
                return tex;
            }
            catch
            {
                return null;
            }
        }

        public PPDevice(System.Windows.Forms.Control handle, Action draw)
        {
            string path = GlobalB.GetRootPath() + @"\ResPoi\";

            _Engine = new RenderEngine(handle);
            _Sprite = new RenderSprite(_Engine);
            _Engine.OnRender += draw;
            device = _Engine.Device;

            if (device == null) throw new InvalidOperationException("Failed to get device from RenderEngine");

            // Set up render states
            device.SetRenderState(RenderState.AlphaBlendEnable, true);
            device.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);
            device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
            device.SetRenderState(RenderState.AlphaTestEnable, false);
            device.SetSamplerState(0, SamplerState.AddressU, (int)TextureAddress.Clamp);
            device.SetSamplerState(0, SamplerState.AddressV, (int)TextureAddress.Clamp);
            device.SetTextureStageState(0, TextureStage.ColorOperation, (int)TextureOperation.Modulate);
            device.SetTextureStageState(0, TextureStage.ColorArg1, (int)TextureArgument.Texture);
            device.SetTextureStageState(0, TextureStage.ColorArg2, (int)TextureArgument.Diffuse);
            device.SetTextureStageState(0, TextureStage.AlphaOperation, (int)TextureOperation.Modulate);
            device.SetTextureStageState(0, TextureStage.AlphaArg1, (int)TextureArgument.Diffuse);
            device.SetTextureStageState(0, TextureStage.AlphaArg2, (int)TextureArgument.Texture);

            // Create vertex declaration
            var vertexElems = new[] {
                new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
                new VertexElement(0, 16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
                new VertexElement(0, 32, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                VertexElement.VertexDeclarationEnd
            };
            Decl_Tex = device.CreateVertexDeclaration(vertexElems);

            // Create vertex buffer
            const int stride = 48;
            _vertices = device.CreateVertexBuffer(stride * 6, Usage.WriteOnly | Usage.Dynamic, VertexFormat.Position | VertexFormat.Texture1 | VertexFormat.Diffuse, Pool.Default);

            green = LoadBitmapFromFile(path + @"green.png");
            blue1 = LoadBitmapFromFile(path + @"blue1.png");
            blue2 = LoadBitmapFromFile(path + @"blue2.png");
            red = LoadBitmapFromFile(path + @"red.png");
            pink = LoadBitmapFromFile(path + @"pink.png");
        }

        public void RenderAll()
        {
            _Engine?.RenderAll();
        }

        public void Clear()
        {
            // Cleanup
        }

        // Debug rectangle drawing methods - stubs for now
        public void BitBlt_Rect_Green(RectangleF pos, float a, float b) { }
        public void BitBlt_Rect_Red(RectangleF pos, float a, float b) { }
        public void BitBlt_Rect_Blue1(RectangleF pos, float a, float b) { }
        public void BitBlt_Rect_Blue2(RectangleF pos, float a, float b) { }
        public void BitBlt_Rect_Pink(RectangleF pos, float a, float b) { }
    }
}