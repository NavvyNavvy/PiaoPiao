using Vortice.Direct3D9;
using System.Numerics;
using System;

namespace Data.DXRender
{
    static class RenderExt
    {
        public static Matrix4x4 GetDefault2DViewMatrix()
        {
            return Matrix4x4.CreateLookAt(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY);
        }

        public static Matrix4x4 GetDefault2DProjectionMatrix(IDirect3DDevice9 device)
        {
            // Stub - return default projection matrix without querying device viewport
            return Matrix4x4.CreateOrthographicOffCenter(0, 1280, 0, 720, -100, 100) * Matrix4x4.CreateScale(1, -1, 1);
        }

        public static int GetWidth(this Texture9 texture)
        {
            return texture.GetWidth();
        }

        public static int GetHeight(this Texture9 texture)
        {
            return texture.GetHeight();
        }
    }
}