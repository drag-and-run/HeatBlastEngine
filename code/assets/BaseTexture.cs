using Silk.NET.OpenGL;
using StbImageSharp;
public class BaseTexture : IDisposable
{
    private uint _texture;
    private GL _gl = null!;

    public string Path { get;  set; }

    public unsafe BaseTexture(GL gl, string filepath) 
    { 
        _gl = gl;
        _texture = _gl.GenTexture();
        Path = filepath;

        Bind();

        ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(filepath), ColorComponents.RedGreenBlueAlpha);
        fixed (byte* ptr = result.Data)
        {
            _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)result.Width,
                (uint)result.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
        }

        SetParameters();

    }

    public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
    {
        //When we bind a texture we can choose which textureslot we can bind it to.
        _gl.ActiveTexture(textureSlot);
        _gl.BindTexture(TextureTarget.Texture2D, _texture);
    }

    private void SetParameters()
    {
        _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
        _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
        _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
        _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);
    }

    public void Dispose()
    {
        _gl.DeleteTexture(_texture);
    }
}

