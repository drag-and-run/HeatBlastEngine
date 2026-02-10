using HeatBlastEngine.code.Core;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HeatBlastEngine.code.assets
{
    public class BaseMaterial
    {
        public string Name { get; set; }
        public BaseShader Shader { get; set; }
        public BaseTexture Texture { get; set; }

        public RenderFlags flags { get; set; }

        public BaseMaterial(BaseShader _shader, BaseTexture _texture, string _name = "default_material", RenderFlags _flags = RenderFlags.Default)
        {
            Texture = _texture;
            Shader = _shader;
            Name = _name;
            flags = _flags;
        }
        [JsonConstructor]
        public BaseMaterial()
        {

        }

        public static BaseMaterial LoadFromFile(string filepath, RenderFlags flags = RenderFlags.Default)
        {
            string jsonString = File.ReadAllText(filepath);
            if (jsonString is null)
            {
                Console.WriteLine($"faile to load material{filepath}");
                return null;
            }
            BaseMaterial baseMaterial = JsonSerializer.Deserialize<BaseMaterial>(jsonString);
            //TODO: Handle textureless materials
                return new BaseMaterial(new BaseShader(baseMaterial.Shader.vertexShaderPath, baseMaterial.Shader.fragmentShaderPath),
                    new BaseTexture(Renderer.GL, baseMaterial.Texture.Path, baseMaterial.Texture.Type), baseMaterial.Name, flags);

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
