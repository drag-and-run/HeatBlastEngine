using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HeatBlastEngine.code.assets.models
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
        public GL _gl { get; }
        public VertexArrayObject<float, uint> VAO { get; set; }
        public BufferObject<float> VBO { get; set; }
        public BufferObject<uint> EBO { get; set; }
        public IReadOnlyList<Texture> Textures { get; private set; }
        public float[] Vertices { get; private set; }
        public uint[] Indices { get; private set; }
        public BaseMesh(GL gl, float[] verticies, uint[] indicies, List<Texture> textures)
        {
            _gl = gl;   
            Vertices = verticies;
            Indices = indicies;
            Textures = textures;
            SetupMesh();
        }

        public unsafe void SetupMesh()
        {
            EBO = new BufferObject<uint>(_gl, BufferTargetARB.ElementArrayBuffer,Indices );
            VBO = new BufferObject<float>(_gl, BufferTargetARB.ArrayBuffer, Vertices);    
            VAO = new VertexArrayObject<float, uint>(_gl, VBO, EBO);
            // Match attribute locations with the vertex shader:
            // layout(location = 0) aPos (vec3)
            // layout(location = 1) aTexCoord (vec2)
            // layout(location = 2) aVertexColor (vec3)
            // Normal is present in the vertex data but the shader doesn't use it; place it at location 3.
            VAO.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 11, 0); // position
            VAO.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 11, 9); // texcoord (uv)
            VAO.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, 11, 6); // color
            VAO.VertexAttributePointer(3, 3, VertexAttribPointerType.Float, 11, 3); // normal (unused by shader)
            
            // Unbind VAO to prevent state pollution when setting up other meshes
            _gl.BindVertexArray(0);
        }

        public void Bind()
        {
            VAO.Bind();
        }

        public void Dispose()
        {
            Textures = null;
            VAO.Dispose();
            VBO.Dispose();
            EBO.Dispose();
        }

    }
}
