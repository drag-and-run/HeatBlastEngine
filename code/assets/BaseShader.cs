using System.Numerics;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace HeatBlastEngine
{

    public class BaseShader : IDisposable
    {
        private uint _handle;
        private GL _gl = null!;

        public string vertexShaderPath { get;  set; }
        public string fragmentShaderPath { get;  set; }

        public BaseShader() { }

        public BaseShader(string vertexPath, string fragmentPath)
        {
            vertexShaderPath = vertexPath;
            fragmentShaderPath = fragmentPath;
            _gl = RenderManager.GL;
            
            uint vertex = LoadShader(ShaderType.VertexShader, vertexPath);
            uint fragment = LoadShader(ShaderType.FragmentShader, fragmentPath);
            _handle = RenderManager.GL.CreateProgram();

            _gl.AttachShader(_handle, vertex);
            _gl.AttachShader(_handle, fragment);
            _gl.LinkProgram(_handle);

            _gl.GetProgram(_handle, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                throw new Exception($"Program failed to link with error: {_gl.GetProgramInfoLog(_handle)}");
            }
            _gl.DetachShader(_handle, vertex);
            _gl.DetachShader(_handle, fragment);
            _gl.DeleteShader(vertex);
            _gl.DeleteShader(fragment);
        }

        public void Use()
        {
            //Using the program
            _gl.UseProgram(_handle);
        }

        public void SetUniform(string name, int value)
        {
            //Setting a uniform on a shader using a name.
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1) //If GetUniformLocation returns -1 the uniform is not found.
            {
                //Console.WriteLine($"Warning: uniform '{name}' not found");
                return;
            }
            _gl.Uniform1(location, value);
        }

        public unsafe void SetUniform(string name, Matrix4x4 value)
        {
            //A new overload has been created for setting a uniform so we can use the transform in our shader.
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
               // Console.WriteLine($"Warning: matrix uniform '{name}' not found on shader (skipping).");
                return;
            }
            _gl.UniformMatrix4(location, 1, false, (float*)&value);
        }

        public void SetUniform(string name, float value)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
                //Console.WriteLine($"Warning: float uniform '{name}' not found on shader (skipping).");
                return;
            }
            _gl.Uniform1(location, value);
        }

        public void SetUniform(string name, Vector3 value)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
               // Console.WriteLine($"Warning: vec3 uniform '{name}' not found on shader (skipping).");
                return;
            }
            _gl.Uniform3(location, value);

        }

        public unsafe void SetUniform(string name, Matrix4X4<float> value)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
                //Console.WriteLine($"Warning: matrix4 uniform '{name}' not found on shader (skipping).");
                return;
            }
            _gl.UniformMatrix4(location, 1, false, (float*)&value);
        }

        private uint LoadShader(ShaderType type, string path)
        {
            string src = System.IO.File.ReadAllText(path);
            uint shader = _gl.CreateShader(type);
            _gl.ShaderSource(shader, src);
            _gl.CompileShader(shader);

            string infoLog = _gl.GetShaderInfoLog(shader);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
            }
            return shader;
        }

        public void Dispose()
        {
            _gl.DeleteProgram(_handle);
        }


    }
}
