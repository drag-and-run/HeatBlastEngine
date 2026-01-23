using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using HeatBlastEngine.code.Core.Input;
using HeatBlastEngine.code.Entities;
using HeatBlastEngine.code.Entities.Interfaces;
using Silk.NET.Input;

namespace HeatBlastEngine.code.Core
{
    public class Camera : Entity, IMouseMove
    {

        public float Yaw = 270f;
        public float Pitch = 0f;
        public float Fov = 60f;
        
        private static Vector2 LastMousePosition;
        private static float lookSensitivity = 0.1f;

        public Vector3 Direction = Vector3.Zero;

        public Vector3 Front = new Vector3(0, 0, -1);

        public override void OnUpdate(double deltaTime)
        {
            var speed = 5f * (float)deltaTime;
            if (InputManager.primaryKeyboard.IsKeyPressed(Key.W))
            {
                GameState.ActiveMap.camera.Transform.Position += speed * GameState.ActiveMap.camera.Front;
            }
            if (InputManager.primaryKeyboard.IsKeyPressed(Key.S))
            {
                GameState.ActiveMap.camera.Transform.Position -= speed * GameState.ActiveMap.camera.Front;
            }
            if (InputManager.primaryKeyboard.IsKeyPressed(Key.D))
            {
                GameState.ActiveMap.camera.Transform.Position += Vector3.Normalize(Vector3.Cross(GameState.ActiveMap.camera.Front, GameState.ActiveMap.camera.Transform.Up)) * speed;
            }
            if (InputManager.primaryKeyboard.IsKeyPressed(Key.A))
            {
                GameState.ActiveMap.camera.Transform.Position -= Vector3.Normalize(Vector3.Cross(GameState.ActiveMap.camera.Front, GameState.ActiveMap.camera.Transform.Up)) * speed;
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

                GameState.ActiveMap.camera.Yaw += xOffset;
                GameState.ActiveMap.camera.Pitch -= yOffset;

                GameState.ActiveMap.camera.Pitch = Math.Clamp(GameState.ActiveMap.camera.Pitch, -89f, 89f);

                GameState.ActiveMap.camera.Direction.X = MathF.Cos(float.DegreesToRadians( GameState.ActiveMap.camera.Yaw)) * MathF.Cos(float.DegreesToRadians(GameState.ActiveMap.camera.Pitch));
                GameState.ActiveMap.camera.Direction.Y = MathF.Sin(float.DegreesToRadians(GameState.ActiveMap.camera.Pitch));
                GameState.ActiveMap.camera.Direction.Z = MathF.Sin(float.DegreesToRadians( GameState.ActiveMap.camera.Yaw)) * MathF.Cos(float.DegreesToRadians(GameState.ActiveMap.camera.Pitch));

                GameState.ActiveMap.camera.Front = Vector3.Normalize(GameState.ActiveMap.camera.Direction);
            }
        }
    }
}
