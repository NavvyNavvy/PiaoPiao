using Vortice.Direct3D9;
using Vortice.Mathematics;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Data.DXRender
{
    public class RenderEngine : IDisposable
    {
        private IDirect3D9? _Direct3d;
        private IDirect3DDevice9? _Device;
        private PresentParameters _Parameters;
        private bool _IsDeviceLost;

        public IDirect3DDevice9? Device => _Device;

        public event Action OnRender;

        public RenderEngine(Control ctrl)
        {
            var present = new PresentParameters();
            present.Windowed = true;
            present.SwapEffect = SwapEffect.Discard;
            present.BackBufferWidth = (uint)ctrl.ClientSize.Width;
            present.BackBufferHeight = (uint)ctrl.ClientSize.Height;
            present.PresentationInterval = PresentInterval.One;
            present.BackBufferFormat = Format.A8R8G8B8;
            _Parameters = present;

            _Direct3d = Vortice.Direct3D9.D3D9.Direct3DCreate9();
            if (_Direct3d == null)
                throw new InvalidOperationException("Failed to create Direct3D9");

            _Device = _Direct3d.CreateDevice(0, DeviceType.Hardware, ctrl.Handle, CreateFlags.HardwareVertexProcessing, present);

            if (_Device == null)
                throw new InvalidOperationException("Failed to create Direct3D device");

            _Device.SetRenderState(RenderState.CullMode, false);
            _Device.SetRenderState(RenderState.ZFunc, Compare.Always);
            _Device.SetTransform(TransformState.View, Matrix4x4.Identity);
            _Device.SetTransform(TransformState.Projection, GetViewProjectionMatrix());
            _Device.SetRenderState(RenderState.AlphaTestEnable, false);
            _Device.SetRenderState(RenderState.AlphaRef, 0.1f);
            _Device.SetRenderState(RenderState.AlphaFunc, Compare.GreaterEqual);
            _Device.SetRenderState(RenderState.AlphaBlendEnable, true);
            _Device.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);
            _Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            _Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
        }

        public void Dispose()
        {
            if (_Device != null)
            {
                _Device.Dispose();
                _Device = null;
            }
            _Direct3d?.Dispose();
            _Direct3d = null;
        }

        private Matrix4x4 GetViewProjectionMatrix()
        {
            return GetDefault2DViewMatrix() * GetDefault2DProjectionMatrix();
        }

        private Matrix4x4 GetDefault2DViewMatrix()
        {
            return Matrix4x4.CreateLookAt(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY);
        }

        private Matrix4x4 GetDefault2DProjectionMatrix()
        {
            return Matrix4x4.CreateOrthographicOffCenter(0, 1280, 0, 720, -100, 100) * Matrix4x4.CreateScale(1, -1, 1);
        }

        public void RenderAll()
        {
            if (_Device == null) return;

            try
            {
                _Device.BeginScene();
                _Device.Clear(ClearFlags.Target, new Color(0, 0, 0, 255), 1.0f, 0);
                OnRender?.Invoke();
                _Device.EndScene();
                _Device.Present();
            }
            catch (Exception)
            {
                try { _Device.EndScene(); } catch { }
            }
        }
    }
}