using System.Numerics;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using PrimitiveType = Silk.NET.OpenGL.PrimitiveType;

namespace HeatBlastEngine
{
    public static class RenderManager
    {
        public static GL GL;
        public static IWindow _window;


    
        public static unsafe void Render(ModelRender modelRender,Camera camera, IWindow _window, BaseMaterial? material)
        {
            if (modelRender.BaseMaterial is null && modelRender.Model is not null)
            {
                DebugLog.Error($"No Material Set on {modelRender}, using error material");

                modelRender.BaseMaterial = BaseMaterial.LoadFromFile("textures/dev/notexture.matfile");

            }
            if (modelRender.Model is null)
            {
                DebugLog.Error($"No Model Set on {modelRender}");
                modelRender.Model = new Model("models/editor/error.obj");
                modelRender.BaseMaterial = BaseMaterial.LoadFromFile("textures/dev/error.matfile");
            }

            var size = _window.FramebufferSize;
            var x4 = Matrix4x4.CreateFromQuaternion(modelRender.entity.GetComponent<Transform>().Rotation) * Matrix4x4.CreateTranslation(modelRender.entity.GetComponent<Transform>().Position);
            var view = Matrix4x4.CreateLookAt(camera.GetComponent<Transform>().Position, camera.GetComponent<Transform>().Position + camera.Front, Vector3.UnitY);
            var projection = Matrix4X4.CreatePerspectiveFieldOfView(float.DegreesToRadians(camera.Fov), (float)size.X / size.Y, 0.01f, 1000f);

            if (modelRender.BaseMaterial is null) throw new Exception("Failed to load BaseMaterial");
            modelRender.BaseMaterial.Shader.Use();
            modelRender.BaseMaterial.Shader.SetUniform("uViewPos", camera.GetComponent<Transform>().Position);
            modelRender.BaseMaterial.Shader.SetUniform("uView", view);
            modelRender.BaseMaterial.Shader.SetUniform("uProjection", projection);
            modelRender.BaseMaterial.Shader.SetUniform("uTime", Time.Elapsed);
            foreach (var mesh in modelRender.Model.Meshes)
            {
            
                mesh.Bind();
                modelRender.BaseMaterial.Texture.Bind();
                if (material is null)
                {
                    modelRender.BaseMaterial.Shader.SetUniform("uModel", x4);
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
                    modelRender.BaseMaterial.Shader.SetUniform("uModel", x4);
                    GL.DrawArrays(PrimitiveType.Triangles, 0, (uint)mesh.Indices.Length);
                }


            
            }
        }
    }
}


        
