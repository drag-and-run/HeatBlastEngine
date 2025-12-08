using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HeatBlastEngine.code.Core
{
    public class Camera
    {
        public Transform Transform { get; set; }

        public float Yaw = 270f;
        public float Pitch = 0f;
        public float Fov = 60f;

        public Vector3 Direction = Vector3.Zero;

        public Vector3 Front = new Vector3(0, 0, -1);

        public Camera() 
        {
            Transform = new Transform();

        }
    }
}
