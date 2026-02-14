using System.Numerics;

namespace HeatBlastEngine
{
    public class Transform : Component
    {

        [ShowInEditor]
        public Vector3 Position {  get; set; } = new Vector3(0, 0, 0);

        public float Scale { get; set; } = 1f;
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
    
        public Vector3 Up => Vector3.UnitY;
        public Vector3 Forward => Vector3.UnitZ;
        public Vector3 Right => Vector3.UnitX;
    
        public Matrix4x4 ViewMatrix => Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateTranslation(Position);
    }
}