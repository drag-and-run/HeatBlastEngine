
using Silk.NET.OpenGL;

public class BufferObject<TDataType> : IDisposable where TDataType : unmanaged
{
    private uint _handle;
    private GL _gl;
    private BufferTargetARB _bufferType;

    public unsafe BufferObject(GL gl, BufferTargetARB type, Span<TDataType> data)
    {
;       _gl = gl;
        _bufferType = type;

        _handle = gl.GenBuffer();
        Bind();
        fixed (void* buffer = data)
        {
            _gl.BufferData(type, (nuint)(data.Length * sizeof(TDataType)), buffer, BufferUsageARB.StaticDraw);
        }
    }

    public void Bind()
    {
        _gl.BindBuffer(_bufferType, _handle);
    }

    public void Dispose()
    {
        _gl.DeleteBuffer(_handle);
    }
}

