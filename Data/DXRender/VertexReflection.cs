using Vortice.Direct3D9;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Data.DXRender
{
    public class VertexFieldAttribute : Attribute
    {
        private DeclarationUsage _usage;

        public VertexFieldAttribute(DeclarationUsage usage)
        {
            _usage = usage;
        }

        public DeclarationUsage Usage => _usage;
    }

    public static class VertexReflection
    {
        private static DeclarationType MapType(Type type)
        {
            if (type == typeof(Vector2))
                return DeclarationType.Float2;
            if (type == typeof(Vector3))
                return DeclarationType.Float3;
            if (type == typeof(Vector4))
                return DeclarationType.Float4;
            return DeclarationType.Unused;
        }

        public static IDirect3DVertexDeclaration9 CreateVertexDeclaration<T>(IDirect3DDevice9 device) where T : struct
        {
            var ret = new List<VertexElement>();
            var type = typeof(T);

            foreach (var field in type.GetFields())
            {
                var attr = field.GetCustomAttribute<VertexFieldAttribute>();
                if (attr != null)
                {
                    short offset = (short)Marshal.OffsetOf(type, field.Name).ToInt32();
                    var dt = MapType(field.FieldType);
                    ret.Add(new VertexElement(0, offset, dt, DeclarationMethod.Default, attr.Usage, 0));
                }
            }

            ret.Sort((a, b) => a.Offset.CompareTo(b.Offset));
            ret.Add(VertexElement.VertexDeclarationEnd);

            return device.CreateVertexDeclaration(ret.ToArray());
        }
    }
}