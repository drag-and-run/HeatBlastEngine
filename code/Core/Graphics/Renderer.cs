using System.Numerics;
using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core.Entities.Lights;
using HeatBlastEngine.code.Entities;
using Silk.NET.Assimp;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using PrimitiveType = Silk.NET.OpenGL.PrimitiveType;

namespace HeatBlastEngine.code.Core;

public enum RenderType
{
    Default,
    Sky,
    Gizmo
}

public static class Renderer
{
    public static GL GL;
    public static IWindow _window;


    
    public static unsafe void Render(RenderEntity entity,Camera camera, IWindow _window, PointLight pointLight, RenderType type)
    {
        if (entity.BaseMaterial is null && entity.Model is not null)
        {

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"No Material Set on {entity.Name}, using error material");
            Console.ResetColor();
            entity.BaseMaterial = BaseMaterial.LoadFromFile("textures/dev/notexture.matfile");

        }
        if (entity.Model is null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"No Model Set on {entity.Name}");
            Console.ResetColor();
            entity.Model = new Model("models/editor/error.obj");
            entity.BaseMaterial = BaseMaterial.LoadFromFile("textures/dev/error.matfile");
        }

        var size = _window.FramebufferSize;
        var model = Matrix4x4.CreateFromQuaternion(entity.Transform.Rotation) * Matrix4x4.CreateTranslation(entity.Transform.Position);
        var view = Matrix4x4.CreateLookAt(camera.Transform.Position, camera.Transform.Position + camera.Front, Vector3.UnitY);
        var projection = Matrix4X4.CreatePerspectiveFieldOfView(float.DegreesToRadians(camera.Fov), (float)size.X / size.Y, 0.01f, 1000f);
        
        entity.BaseMaterial.Shader.Use();
        entity.BaseMaterial.Shader.SetUniform("ulightPos", pointLight.Transform.Position);
        entity.BaseMaterial.Shader.SetUniform("uViewPos", camera.Transform.Position);
        entity.BaseMaterial.Shader.SetUniform("uView", view);
        entity.BaseMaterial.Shader.SetUniform("uProjection", projection);
        entity.BaseMaterial.Shader.SetUniform("uTime", Time.Elapsed);
        foreach (var mesh in entity.Model.Meshes)
        {

            mesh.Bind();
            entity.BaseMaterial.Texture.Bind();
            
            //TODO: implement material flags instead of entities
            switch (type)
            {
                case RenderType.Default:
                    entity.BaseMaterial.Shader.SetUniform("uModel", model);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, (uint)mesh.Indices.Length);
                    break;

                case RenderType.Sky:
                    GL.DepthFunc(GLEnum.Lequal);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, (uint)mesh.Indices.Length);
                    GL.DepthFunc(GLEnum.Less);
                    break;
                
                case RenderType.Gizmo:
                    entity.BaseMaterial.Shader.SetUniform("uModel", model);
                    GL.DrawArrays(PrimitiveType.Lines, 0,(uint)mesh.Indices.Length);
                    entity.Transform.Rotation = Quaternion.Identity;
                    break;
            }
        }
        
        
    }
}


        
