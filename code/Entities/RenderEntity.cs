using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core.Entities.Lights;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;
using HeatBlastEngine.code.Core;
using HeatBlastEngine.code.Entities;


public class RenderEntity : Entity, IDisposable
{

    public Material Material;
    public Model Model;

    public RenderEntity(Material _material, Model _model, string _name = "default")
    {
        Material = _material;
        Model = _model;
        Name = _name;
    }

    public override void OnRender(double deltaTime)
    {
        Render(GameState.ActiveMap.camera, Renderer._window, GameState.ActiveMap._Light);
    }

    public virtual unsafe void Render(HeatBlastEngine.code.Core.Camera camera, IWindow _window, LightObject _light)
    {
        var size = _window.FramebufferSize;
        var model = Matrix4x4.CreateFromQuaternion(Transform.Rotation) * Matrix4x4.CreateTranslation(Transform.Position);
        var view = Matrix4x4.CreateLookAt(camera.Transform.Position, camera.Transform.Position + camera.Front, camera.Transform.Up);
        var projection = Matrix4X4.CreatePerspectiveFieldOfView(float.DegreesToRadians(camera.Fov), (float)size.X / size.Y, 0.01f, 1000f);
        Material.Shader.Use();
        Material.Shader.SetUniform("ulightPos", _light.Transform.Position);
        Material.Shader.SetUniform("uViewPos", camera.Transform.Position);
        foreach (var mesh in Model.Meshes)
        {

            mesh.Bind();

            
            Material.Texture.Bind();

            Material.Shader.SetUniform("uModel", model);
            Material.Shader.SetUniform("uView", view);
            Material.Shader.SetUniform("uProjection", projection);


            // We have an EBO (indices). Draw using DrawElements with the index count.
            
            Renderer.OpenGl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Indices.Length, DrawElementsType.UnsignedInt, null);

        }
    }

    public void Dispose()
    {
        Material.Texture.Dispose();
        Material.Shader.Dispose();
        Model.Dispose();
    }

}