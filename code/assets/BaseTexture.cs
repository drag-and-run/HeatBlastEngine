
using Silk.NET.OpenGL;
using StbImageSharp;
using System.Text.Json;
using System.Text.Json.Serialization;

public enum TextureType
{
    Color,
    Normal,
    Roughness,
    AO,
    Cubemap
}

public class BaseTexture : IDisposable
{
    private uint _texture;
    private GL _gl = null!;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TextureType Type { get; set; }

    public string[] Path { get;  set; }

    public BaseTexture() { }

    public unsafe BaseTexture(GL gl, string[] filepath, TextureType _type) 
    { 
        _gl = gl;
        _texture = _gl.GenTexture();
        Type = _type;

        Bind();

        switch (_type)
        {
            case TextureType.Cubemap:
                for (int i = 0; i < 6; i++)
                {
                    Path = new string[i];
                    Path = filepath;
                    string path = filepath[i];
                    ImageResult img = ImageResult.FromMemory(System.IO.File.ReadAllBytes(path), ColorComponents.RedGreenBlue);

                    fixed (byte* ptr = img.Data)
                    {
                        _gl.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, InternalFormat.Rgb, (uint)img.Width, (uint)img.Height, 0, 
                            PixelFormat.Rgb, PixelType.UnsignedByte, ptr
                        );
                    }
                }
            break;

            case TextureType.Color:
                Path = filepath;
                ImageResult result = ImageResult.FromMemory(System.IO.File.ReadAllBytes(filepath[0]), ColorComponents.RedGreenBlueAlpha);
                fixed (byte* ptr = result.Data)
                {
                    _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, (uint)result.Width,
                        (uint)result.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
                }
                break;
        }


        SetParameters();
    }

    public void Bind(TextureUnit textureSlot = TextureUnit.Texture0)
    {
        //When we bind a texture we can choose which textureslot we can bind it to.
        _gl.ActiveTexture(textureSlot);
        switch (Type)
        {
            case TextureType.Color:
                _gl.BindTexture(TextureTarget.Texture2D, _texture);
                break;

            case TextureType.Cubemap:
                _gl.BindTexture(TextureTarget.TextureCubeMap, _texture);
                break;
        }
       
    }

    private void SetParameters()
    {
        switch (Type)
        {
            case TextureType.Color:
                _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
                _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
                _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
                _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);
                break;

            case TextureType.Cubemap:
                _gl.TexParameter(GLEnum.TextureCubeMap, GLEnum.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                _gl.TexParameter(GLEnum.TextureCubeMap, GLEnum.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                _gl.TexParameter(GLEnum.TextureCubeMap, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
                _gl.TexParameter(GLEnum.TextureCubeMap, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);
                break;
        }

    }

    public void Dispose()
    {
        _gl.DeleteTexture(_texture);
    }
}

