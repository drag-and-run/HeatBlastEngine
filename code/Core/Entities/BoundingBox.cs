using HeatBlastEngine.code.assets;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HeatBlastEngine.code.Core.Entities
{
    internal class BoundingBox : BaseEntity
    {
        public Vector3 min;
        public Vector3 max;
        public BoundingBox(BaseMaterial _material, Model _model, Transform _transform, string _name = "default", GL _gl = null) : base(_material, _model, _transform, _name, _gl)
        {
        }
    }
}
