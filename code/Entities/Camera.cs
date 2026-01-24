using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using HeatBlastEngine.code.Core.Input;
using HeatBlastEngine.code.Entities;
using HeatBlastEngine.code.Entities.Interfaces;
using HeatBlastEngine.code.maps;
using Silk.NET.Input;

namespace HeatBlastEngine.code.Core
{
    public class Camera : Entity, IMouseMove
    {
        public Camera(string name = "Camera") : base(name) {}

        public float Yaw = 270f;
        public float Pitch = 0f;
        public float Fov = 60f;
        
        private static Vector2 LastMousePosition;
        public static float lookSensitivity = 0.1f;

        public Vector3 Direction = Vector3.Zero;

        public Vector3 Front = new Vector3(0, 0, -1);

        public override void OnUpdate(double deltaTime)
        {
            var speed = 5f * (float)deltaTime;
            if (InputManager.primaryKeyboard.IsKeyPressed(Key.W))
            {
                World.ActiveMap.camera.Transform.Position += speed * World.ActiveMap.camera.Front;
            }
            if (InputManager.primaryKeyboard.IsKeyPressed(Key.S))
            {
                World.ActiveMap.camera.Transform.Position -= speed * World.ActiveMap.camera.Front;
            }
            if (InputManager.primaryKeyboard.IsKeyPressed(Key.D))
            {
                World.ActiveMap.camera.Transform.Position += Vector3.Normalize(Vector3.Cross(World.ActiveMap.camera.Front, World.ActiveMap.camera.Transform.Up)) * speed;
            }
            if (InputManager.primaryKeyboard.IsKeyPressed(Key.A))
            {
                World.ActiveMap.camera.Transform.Position -= Vector3.Normalize(Vector3.Cross(World.ActiveMap.camera.Front, World.ActiveMap.camera.Transform.Up)) * speed;
            }
        }

        public void OnMouseMove(IMouse mouse, Vector2 position)
        {
            if (LastMousePosition == default) { LastMousePosition = position; }
            else
            {
           
                var xOffset = (position.X - LastMousePosition.X) * lookSensitivity;
                var yOffset = (position.Y - LastMousePosition.Y) * lookSensitivity;
                LastMousePosition = position;

                World.ActiveMap.camera.Yaw += xOffset;
                World.ActiveMap.camera.Pitch -= yOffset;

                World.ActiveMap.camera.Pitch = Math.Clamp(World.ActiveMap.camera.Pitch, -89f, 89f);

                World.ActiveMap.camera.Direction.X = MathF.Cos(float.DegreesToRadians( World.ActiveMap.camera.Yaw)) * MathF.Cos(float.DegreesToRadians(World.ActiveMap.camera.Pitch));
                World.ActiveMap.camera.Direction.Y = MathF.Sin(float.DegreesToRadians(World.ActiveMap.camera.Pitch));
                World.ActiveMap.camera.Direction.Z = MathF.Sin(float.DegreesToRadians( World.ActiveMap.camera.Yaw)) * MathF.Cos(float.DegreesToRadians(World.ActiveMap.camera.Pitch));

                World.ActiveMap.camera.Front = Vector3.Normalize(World.ActiveMap.camera.Direction);
            }
        }
    }
}
