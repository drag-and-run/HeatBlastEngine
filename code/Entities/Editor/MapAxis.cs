using System.Numerics;
using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core;
using HeatBlastEngine.code.Core.Entities.Lights;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace HeatBlastEngine.Code.Editor;

public class MapAxis : RenderEntity
{
    public MapAxis(Material _material, Model _model, string name = "defaulAxisEntity") : base(_material, _model, name)
    {
    }
    
    public override unsafe void Render(HeatBlastEngine.code.Core.Camera camera, IWindow _window, PointLight pointLight)
    {
        var size = _window.FramebufferSize;
        var model = Matrix4x4.CreateFromQuaternion(Transform.Rotation) * Matrix4x4.CreateTranslation(Transform.Position);
        var view = Matrix4x4.CreateLookAt(camera.Transform.Position, camera.Transform.Position + camera.Front, Vector3.UnitY);
        var projection = Matrix4X4.CreatePerspectiveFieldOfView(float.DegreesToRadians(camera.Fov), (float)size.X / size.Y, 0.01f, 1000f);
        Material.Shader.Use();
        Material.Shader.SetUniform("uViewPos", camera.Transform.Position);
        foreach (var mesh in Model.Meshes)
        {

            mesh.Bind();
            Material.Texture.Bind();

            Material.Shader.SetUniform("uModel", model);
            Material.Shader.SetUniform("uView", view);
            Material.Shader.SetUniform("uProjection", projection);
            
            Renderer.GL.DrawArrays(PrimitiveType.Lines, 0,(uint)mesh.Indices.Length);
        }
        
        Transform.Rotation = Quaternion.Identity;
    }
}
