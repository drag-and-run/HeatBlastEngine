using HeatBlastEngine.code.Core;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeatBlastEngine.code.assets.models
{
    public class BaseMaterial
    {
        public BaseTexture Texture;
        public BaseShader Shader;
        public BaseMaterial(BaseTexture _texture, BaseShader _shader)
        {
            Texture = _texture;
            Shader = _shader;
        }

        public void Use()
        {
            Shader.Use();
            Texture.Bind(TextureUnit.Texture0);

            Shader.SetUniform("uTexture", 0);
        }
    }
}
