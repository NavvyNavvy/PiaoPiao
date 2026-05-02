using Vortice.Direct3D9;
using System.Numerics;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Data.DXRender
{
    // Stub sprite rendering - Vortice doesn't have ID3DXSprite
    public class RenderSprite : IDisposable
    {
        private bool _Dirty;
        private Texture9 _Texture = null!;
        public Texture9 Texture
        {
            get { return _Texture; }
            set { _Dirty = true; _Texture = value; }
        }

        private float _Left;
        public float Left
        {
            get { return _Left; }
            set { _Dirty = true; _Left = value; }
        }

        private float _Top;
        public float Top
        {
            get { return _Top; }
            set { _Dirty = true; _Top = value; }
        }

        public float OriginX { get; set; }
        public float OriginY { get; set; }

        private float _TextureLeft;
        public float TextureLeft
        {
            get { return _TextureLeft; }
            set { _Dirty = true; _TextureLeft = value; }
        }

        private float _TextureTop;
        public float TextureTop
        {
            get { return _TextureTop; }
            set { _Dirty = true; _TextureTop = value; }
        }

        private float _TextureRight = 1.0f;
        public float TextureRight
        {
            get { return _TextureRight; }
            set { _Dirty = true; _TextureRight = value; }
        }

        private float _TextureBottom = 1.0f;
        public float TextureBottom
        {
            get { return _TextureBottom; }
            set { _Dirty = true; _TextureBottom = value; }
        }

        private float _ScaleX = 1.0f;
        public float ScaleX
        {
            get { return _ScaleX; }
            set { _Dirty = true; _ScaleX = value; _UseSize = false; }
        }

        private float _ScaleY = 1.0f;
        public float ScaleY
        {
            get { return _ScaleY; }
            set { _Dirty = true; _ScaleY = value; _UseSize = false; }
        }

        private float _SizeX;
        public float SizeX
        {
            get { return _SizeX; }
            set { _Dirty = true; _SizeX = value; _UseSize = true; }
        }

        private float _SizeY;
        public float SizeY
        {
            get { return _SizeY; }
            set { _Dirty = true; _SizeY = value; _UseSize = true; }
        }

        private float _Rotation;
        public float Rotation
        {
            get { return _Rotation; }
            set { _Dirty = true; _Rotation = value; }
        }

        private float _Rotation0;
        public float Rotation0
        {
            get { return _Rotation0; }
            set { _Dirty = true; _Rotation0 = value; }
        }

        private Vector4 _Color = new Vector4(1, 1, 1, 1);

        public float Alpha
        {
            get { return _Color.W; }
            set { _Color.W = value; }
        }

        public float Red
        {
            get { return _Color.X; }
            set { _Color.X = value; }
        }

        public float Green
        {
            get { return _Color.Y; }
            set { _Color.Y = value; }
        }

        public float Blue
        {
            get { return _Color.Z; }
            set { _Color.Z = value; }
        }

        public float RotationOffset { get; set; }

        private readonly RenderEngine _RenderEngine;
        public RenderEngine RenderEngine => _RenderEngine;

        private bool _UseSize;

        public RenderSprite(RenderEngine re)
        {
            _RenderEngine = re;
            _ScaleX = 1.0f;
            _ScaleY = 1.0f;
        }

        public void Render()
        {
            // Stub - rendering not implemented in Vortice migration
        }

        public void Dispose()
        {
        }
    }
}