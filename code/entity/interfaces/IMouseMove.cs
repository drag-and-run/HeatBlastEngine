using System.Numerics;
using Silk.NET.Input;

namespace HeatBlastEngine
{
    public interface IMouseMove
    {
        public void OnMouseMove(IMouse mouse, Vector2 position);
    }
}