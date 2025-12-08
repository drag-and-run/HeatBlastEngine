using HeatBlastEngine.code.assets.models;
using HeatBlastEngine.code.Core;
using HeatBlastEngine.code.Core.Entities.Lights;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;
using System.Numerics;


public class BaseEntity : IDisposable
{
    public string Name;
    public Guid uid =  Guid.NewGuid();

    public Transform Transform = new Transform();

    public BaseMaterial Material;
    public Model Model;

    public BaseEntity(BaseMaterial _material, Model _model, Transform _transform, string _name = "default", GL _gl = null)
    {

        Console.WriteLine(uid);
        Material = _material;
        Model = _model;
        Name = _name;
        Transform = _transform;

        Material.Use();

 
    }

    public virtual unsafe void Render(HeatBlastEngine.code.Core.Camera camera, IWindow _window, GL _gl, LightObject _light)
    {
        var size = _window.FramebufferSize;
        var model = Matrix4x4.CreateFromQuaternion(Transform.Rotation) * Matrix4x4.CreateTranslation(Transform.Position);
        var view = Matrix4x4.CreateLookAt(camera.Transform.Position, camera.Transform.Position + camera.Front, camera.Transform.Up);
        var projection = Matrix4X4.CreatePerspectiveFieldOfView(float.DegreesToRadians(camera.Fov), (float)size.X / size.Y, 0.01f, 100f);
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
            _gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Indices.Length, DrawElementsType.UnsignedInt, null);

        }
    }

    public void Dispose()
    {
        Material.Texture.Dispose();
        Material.Shader.Dispose();
        Model.Dispose();
    }

}