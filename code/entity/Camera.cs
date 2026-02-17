using System.Numerics;
using Silk.NET.Input;

namespace HeatBlastEngine
{
    public class Camera : Entity, IMouseMove
    {
        public Camera(string name = "Camera") : base(name)
        {

        }

        public float Yaw = 270f;
        public float Pitch = 0f;
        public float Fov = 60f;
        
        private static Vector2 LastMousePosition;
        public static float lookSensitivity = 0.1f;

        public Vector3 Direction = Vector3.Zero;
        public Vector3 Front = new Vector3(0, 0, 0);


        


        public override void OnUpdate(double deltaTime)
        {
            var speed = 5f * (float)deltaTime;
            if (InputManager.PrimaryKeyboard.IsKeyPressed(Key.W))
            {
                GetComponent<Transform>().Position += speed * Front;
            }
            if (InputManager.PrimaryKeyboard.IsKeyPressed(Key.S))
            {
                GetComponent<Transform>().Position -= speed * Front;
            }
            if (InputManager.PrimaryKeyboard.IsKeyPressed(Key.D))
            {
                GetComponent<Transform>().Position += Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY)) * speed;
            }
            if (InputManager.PrimaryKeyboard.IsKeyPressed(Key.A))
            {
                GetComponent<Transform>().Position -= Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY)) * speed;
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

                Yaw += xOffset;
                Pitch -= yOffset;

                Pitch = Math.Clamp(Pitch, -89f, 89f);

                Direction.X = MathF.Cos(float.DegreesToRadians( Yaw)) * MathF.Cos(float.DegreesToRadians(Pitch));
                Direction.Y = MathF.Sin(float.DegreesToRadians(Pitch));
                Direction.Z = MathF.Sin(float.DegreesToRadians( Yaw)) * MathF.Cos(float.DegreesToRadians(Pitch));

                Front = Vector3.Normalize(Direction);
            }
        }
    }
}
