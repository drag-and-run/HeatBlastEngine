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
        public BaseMaterial(BaseShader _shader, BaseTexture _texture)
        {
            Texture = _texture;
            Shader = _shader;
          
        }

        public void Use()
        {
            try
            {
                Console.WriteLine("[BaseMaterial] Using shader and binding texture...");
                Shader.Use();
                Console.WriteLine("[BaseMaterial] Shader.Use() OK");

                if (Texture == null)
                {
                    Console.WriteLine("[BaseMaterial] Warning: Texture is null");
                }

                Texture.Bind(TextureUnit.Texture0);
                Console.WriteLine($"[BaseMaterial] Texture bound (id): {Texture?.GetHashCode()}");

                if (Texture.Type == TextureType.Cubemap)
                {
                    try
                    {
                        Shader.SetUniform("uCubemap", 0);
                        Console.WriteLine("CUBEMAP TEXTURE BOUND");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[BaseMaterial] Error setting cubemap uniform: {ex.Message}\n{ex.StackTrace}");
                    }
                }
                else
                {
                    try
                    {
                        Shader.SetUniform("uTexture", 0);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[BaseMaterial] Error setting texture uniform: {ex.Message}\n{ex.StackTrace}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BaseMaterial] Exception in Use(): {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }
    }
}
