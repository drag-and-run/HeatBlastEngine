using Silk.NET.OpenGL;
using System.Numerics;

namespace HeatBlastEngine
{
    public struct VertexData
    {
       public Vector3 Position;
       public Vector3 Normal;
       public Vector3 Color;
       public Vector2 uvCoords;
    }

    public struct Texture
    {
        uint id;
        string type;
    }


    public class BaseMesh : IDisposable
    {

        public VertexArrayObject<float, uint> VAO { get; set; }
        public BufferObject<float> VBO { get; set; }
        public BufferObject<uint> EBO { get; set; }
        public IReadOnlyList<Texture> Textures { get; private set; }
        public float[] Vertices { get; private set; }
        public uint[] Indices { get; private set; }
        public BaseMesh( float[] verticies, uint[] indicies, List<Texture> textures)
        {
 
            Vertices = verticies;
            Indices = indicies;
            Textures = textures;
            SetupMesh();
        }

        public unsafe void SetupMesh()
        {

            EBO = new BufferObject<uint>(RenderManager.GL, BufferTargetARB.ElementArrayBuffer,Indices );
            VBO = new BufferObject<float>(RenderManager.GL, BufferTargetARB.ArrayBuffer, Vertices);    
            VAO = new VertexArrayObject<float, uint>(RenderManager.GL, VBO, EBO);
            // Match attribute locations with the vertex shader:
            // layout(location = 0) aPos (vec3)
            // layout(location = 1) aTexCoord (vec2)
            // layout(location = 2) aVertexColor (vec3)
            // Normal is present in the vertex data but the shader doesn't use it; place it at location 3.
            VAO.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 11, 0); // position
            VAO.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 11, 9); // texcoord (uv)
            VAO.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, 11, 6); // color
            VAO.VertexAttributePointer(3, 3, VertexAttribPointerType.Float, 11, 3); // normal (unused by shader)
            
            RenderManager.GL.BindVertexArray(0);
        }

        public void Bind()
        {
            VAO.Bind();
        }

        public void Dispose()
        {
            Console.WriteLine("Mesh disposed");
            Textures = null;
            VAO.Dispose();
            VBO.Dispose();
            EBO.Dispose();
        }

    }
}
