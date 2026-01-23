using HeatBlastEngine.code.assets.models;
using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using System.Numerics;
using HeatBlastEngine.code.Core;

public class Model : IDisposable
{
    private readonly GL _gl;
    private Assimp _assimp;
    public string Directory { get; protected set; } = string.Empty;
    public List<BaseMesh> Meshes { get; protected set; } = new List<BaseMesh>();
    public Model( string filepath, bool gamma = false)
    {
        var assimp = Silk.NET.Assimp.Assimp.GetApi();
        _assimp = assimp;
        _gl = Renderer._gl;
        LoadModel(filepath);
    }

    private unsafe void LoadModel(string filepath)
    {

        var scene = _assimp.ImportFile(filepath, (uint)PostProcessSteps.Triangulate);

        if (scene == null)
        {
            var error = _assimp.GetErrorStringS();
            Console.WriteLine("Assimp import returned null. Error: " + error);
            throw new Exception(error);
        }


        if (scene->MRootNode == null || scene->MFlags == Silk.NET.Assimp.Assimp.SceneFlagsIncomplete)
        {
            var error = _assimp.GetErrorStringS();
            throw new Exception(error);
        }



        ProcessNode(scene->MRootNode, scene);
    }

    private unsafe void ProcessNode(Node* node, Scene* scene)
    {
        for (var i = 0; i < node->MNumMeshes; i++)
        {
            var mesh = scene->MMeshes[node->MMeshes[i]];
            Meshes.Add(ProcessMesh(mesh, scene));

        }

        for (var i = 0; i < node->MNumChildren; i++)
        {
            ProcessNode(node->MChildren[i], scene);
        }
    }

    private unsafe BaseMesh ProcessMesh(Silk.NET.Assimp.Mesh* mesh, Scene* scene)
    {
        // data to fill
        List<VertexData> vertices = new List<VertexData>();
        List<uint> indices = new List<uint>();


        // walk through each of the mesh's vertices
        for (uint i = 0; i < mesh->MNumVertices; i++)
        {
            VertexData vertex = new VertexData();

            vertex.Position = mesh->MVertices[i];

            
            
            if (mesh->MNormals != null)
                vertex.Normal = mesh->MNormals[i];


            // texture coordinates
            if (mesh->MTextureCoords[0] != null) // does the mesh contain texture coordinates?
            {
                // a vertex can contain up to 8 different texture coordinates. We thus make the assumption that we won't 
                // use models where a vertex can have multiple texture coordinates so we always take the first set (0).
                Vector3 texcoord3 = mesh->MTextureCoords[0][i];
                vertex.uvCoords = new Vector2(texcoord3.X, texcoord3.Y);
            }

            if (mesh->MColors[0] != null)
            {
                Vector4 vertexColors = mesh->MColors[0][i];
                vertex.Color = new Vector3(vertexColors.X, vertexColors.Y, vertexColors.Z);
            }
            else
            {
                vertex.Color = Vector3.One;
            }

            vertices.Add(vertex);
        }

        // now wak through each of the mesh's faces (a face is a mesh its triangle) and retrieve the corresponding vertex indices.
        for (uint i = 0; i < mesh->MNumFaces; i++)
        {
            Face face = mesh->MFaces[i];
            // retrieve all indices of the face and store them in the indices vector
            for (uint j = 0; j < face.MNumIndices; j++)
                indices.Add(face.MIndices[j]);
        }

        // process materials
        Material* material = scene->MMaterials[mesh->MMaterialIndex];
        // we assume a convention for sampler names in the shaders. Each diffuse texture should be named
        // as 'texture_diffuseN' where N is a sequential number ranging from 1 to MAX_SAMPLER_NUMBER. 
        // Same applies to other texture as the following list summarizes:
        // diffuse: texture_diffuseN

        // return a mesh object created from the extracted mesh data
        var result = new BaseMesh(_gl, BuildVertices(vertices), BuildIndices(indices), null);
        return result;
    }

    private float[] BuildVertices(List<VertexData> vertexCollection)
    {
        var vertices = new List<float>();

        foreach (var vertex in vertexCollection)
        {
            vertices.Add(vertex.Position.X);
            vertices.Add(vertex.Position.Y);
            vertices.Add(vertex.Position.Z);

            vertices.Add(vertex.Normal.X);
            vertices.Add(vertex.Normal.Y);
            vertices.Add(vertex.Normal.Z);

            vertices.Add(vertex.Color.X);
            vertices.Add(vertex.Color.Y);
            vertices.Add(vertex.Color.Z);

            vertices.Add(vertex.uvCoords.X);
            vertices.Add(vertex.uvCoords.Y);
        }

        return vertices.ToArray();
    }

    private uint[] BuildIndices(List<uint> indices)
    {
        return indices.ToArray();
    }
    public void Dispose()
    {
        foreach (var mesh in Meshes)
        {
            mesh.Dispose();
        }
    }
}