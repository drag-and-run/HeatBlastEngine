using HeatBlastEngine.code.assets.models;
using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeatBlastEngine.code.Core.Entities.Lights
{
    public class LightObject 
    {
        public Transform Transform = new Transform();

        public LightObject(Transform _transform) 
        {
            Transform = _transform;
        }
    }
}
