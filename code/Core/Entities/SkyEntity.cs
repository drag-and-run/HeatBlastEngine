using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core;
using HeatBlastEngine.code.Core.Entities.Lights;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;
using System.Numerics;


public class SkyEntity : BaseEntity, IDisposable
{
    public SkyEntity(BaseMaterial _material, Model _model, Transform _transform, string _name = "default", GL _gl = null) : base(_material, _model, _transform, _name, _gl)
    {
        
    }

    public override unsafe void Render(HeatBlastEngine.code.Core.Camera camera, IWindow _window, GL _gl, LightObject _light)
    {
       
        var size = _window.FramebufferSize;
        var view = Matrix4x4.CreateLookAt(camera.Transform.Position, camera.Transform.Position + camera.Front, camera.Transform.Up);

        var projection = Matrix4X4.CreatePerspectiveFieldOfView(float.DegreesToRadians(camera.Fov), (float)size.X / size.Y, 0.01f, 500f);
        Material.Shader.Use();
        foreach (var mesh in Model.Meshes)
        {
            mesh.Bind();
            Material.Texture.Bind();

            Material.Shader.SetUniform("uView", view);
            Material.Shader.SetUniform("uProjection", projection);

            _gl.DepthFunc(GLEnum.Lequal);
            _gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Indices.Length, DrawElementsType.UnsignedInt, null);
            _gl.DepthFunc(GLEnum.Less);
        }
        
    }
}