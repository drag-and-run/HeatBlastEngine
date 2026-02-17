using Silk.NET.OpenGL;
using System.Numerics;

namespace HeatBlastEngine
{
    public struct VertexData
    {
       public Vector3 Position;
       public Vector3 Normal;
       public Vector3 Color;
       public Vector2 UvCoords;
    }




    public class BaseMesh : IDisposable
    {
        private VertexArrayObject<float, uint> Vao { get; set; }
        private BufferObject<float> Vbo { get; set; }
        private BufferObject<uint> Ebo { get; set; }
        private float[] Vertices { get; }
        public uint[] Indices { get; }
        public BaseMesh( float[] verticies, uint[] indicies, List<Texture> textures)
        {
 
            Vertices = verticies;
            Indices = indicies;
            SetupMesh();
        }

        private unsafe void SetupMesh()
        {

            Ebo = new BufferObject<uint>(RenderManager.GL, BufferTargetARB.ElementArrayBuffer,Indices );
            Vbo = new BufferObject<float>(RenderManager.GL, BufferTargetARB.ArrayBuffer, Vertices);    
            Vao = new VertexArrayObject<float, uint>(RenderManager.GL, Vbo, Ebo);
            // Match attribute locations with the vertex shader:
            // layout(location = 0) aPos (vec3)
            // layout(location = 1) aTexCoord (vec2)
            // layout(location = 2) aVertexColor (vec3)
            // Normal is present in the vertex data but the shader doesn't use it; place it at location 3.
            Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 11, 0); // position
            Vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 11, 9); // texcoord (uv)
            Vao.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, 11, 6); // color
            Vao.VertexAttributePointer(3, 3, VertexAttribPointerType.Float, 11, 3); // normal (unused by shader)
            
            RenderManager.GL.BindVertexArray(0);
        }

        public void Bind()
        {
            Vao.Bind();
        }

        public void Dispose()
        {
            Console.WriteLine("Mesh disposed");
            Vao.Dispose();
            Vbo.Dispose();
            Ebo.Dispose();
        }

    }
}
