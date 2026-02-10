using System.Numerics;
using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.logic.components;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using PrimitiveType = Silk.NET.OpenGL.PrimitiveType;

namespace HeatBlastEngine.code.Core;



public static class Renderer
{
    public static GL GL;
    public static IWindow _window;


    
    public static unsafe void Render(ModelRender rendmdl,Camera camera, IWindow _window, BaseMaterial? material)
    {
        if (rendmdl.BaseMaterial is null && rendmdl.Model is not null)
        {

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"No Material Set on {rendmdl}, using error material");
            Console.ResetColor();
            rendmdl.BaseMaterial = BaseMaterial.LoadFromFile("textures/dev/notexture.matfile");

        }
        if (rendmdl.Model is null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"No Model Set on {rendmdl}");
            Console.ResetColor();
            rendmdl.Model = new Model("models/editor/error.obj");
            rendmdl.BaseMaterial = BaseMaterial.LoadFromFile("textures/dev/error.matfile");
        }

        var size = _window.FramebufferSize;
        var model = Matrix4x4.CreateFromQuaternion(rendmdl.entity.GetComponent<Transform>().Rotation) * Matrix4x4.CreateTranslation(rendmdl.entity.GetComponent<Transform>().Position);
        var view = Matrix4x4.CreateLookAt(camera.GetComponent<Transform>().Position, camera.GetComponent<Transform>().Position + camera.Front, Vector3.UnitY);
        var projection = Matrix4X4.CreatePerspectiveFieldOfView(float.DegreesToRadians(camera.Fov), (float)size.X / size.Y, 0.01f, 1000f);
        
        rendmdl.BaseMaterial.Shader.Use();
        rendmdl.BaseMaterial.Shader.SetUniform("uViewPos", camera.GetComponent<Transform>().Position);
        rendmdl.BaseMaterial.Shader.SetUniform("uView", view);
        rendmdl.BaseMaterial.Shader.SetUniform("uProjection", projection);
        rendmdl.BaseMaterial.Shader.SetUniform("uTime", Time.Elapsed);
        foreach (var mesh in rendmdl.Model.Meshes)
        {
            
            mesh.Bind();
            rendmdl.BaseMaterial.Texture.Bind();
            if (material is null)
            {
                rendmdl.BaseMaterial.Shader.SetUniform("uModel", model);
                GL.DrawArrays(PrimitiveType.Triangles, 0, (uint)mesh.Indices.Length);
                return;
            }
            if (material.flags == RenderFlags.Skybox)
            {
                GL.DepthFunc(GLEnum.Lequal);
                GL.DrawArrays(PrimitiveType.Triangles, 0, (uint)mesh.Indices.Length);
                GL.DepthFunc(GLEnum.Less);
            }

            if (material.flags == RenderFlags.Default)
            {
                rendmdl.BaseMaterial.Shader.SetUniform("uModel", model);
                GL.DrawArrays(PrimitiveType.Triangles, 0, (uint)mesh.Indices.Length);
            }


            
        }
    }
}


        
