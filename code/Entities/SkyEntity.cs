using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core;
using HeatBlastEngine.code.Core.Entities.Lights;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;
using System.Numerics;
using HeatBlastEngine.code.Entities;


public class SkyEntity : RenderEntity
{
    public static SkyEntity? Instance;
    
    public SkyEntity(Material _material, Model _model, string name = "defaultSky") : base(_material, _model, name)
    {
        Instance = this;
    }

    public override unsafe void Render(HeatBlastEngine.code.Core.Camera camera, IWindow _window, PointLight pointLight)
    {
       
        var size = _window.FramebufferSize;
        var view = Matrix4x4.CreateLookAt(camera.Transform.Position, camera.Transform.Position + camera.Front, Vector3.UnitY);

        var projection = Matrix4X4.CreatePerspectiveFieldOfView(float.DegreesToRadians(camera.Fov), (float)size.X / size.Y, 0.01f, 500f);
        Material.Shader.Use();
        foreach (var mesh in Model.Meshes)
        {
            mesh.Bind();
            Material.Texture.Bind();

            Material.Shader.SetUniform("uView", view);
            Material.Shader.SetUniform("uProjection", projection);

            Renderer.GL.DepthFunc(GLEnum.Lequal);
            Renderer.GL.DrawElements(PrimitiveType.Triangles, (uint)mesh.Indices.Length, DrawElementsType.UnsignedInt, null);
            Renderer.GL.DepthFunc(GLEnum.Less);
        }
        
    }
    
    public override void Dispose()
    {
        Material.Texture.Dispose();
        Material.Shader.Dispose();
        Model.Dispose();
        Instance = null;
    }
}