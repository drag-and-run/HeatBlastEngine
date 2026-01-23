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
    public class Material
    {
        public string Name { get; set; }
        public BaseShader Shader { get; set; }
        public BaseTexture Texture { get; set; }



        public Material(BaseShader _shader, BaseTexture _texture, string _name = "default_material")
        {
            Texture = _texture;
            Shader = _shader;
            Name = _name;
        }
        [JsonConstructor]
        public Material()
        {

        }

        public static Material LoadFromFile(string filepath)
        {
            string jsonString = File.ReadAllText(filepath);
            if (jsonString is null)
            {
                Console.WriteLine($"faile to load material{filepath}");
                return null;
            }
            Material material = JsonSerializer.Deserialize<Material>(jsonString);
                return new Material(new BaseShader(material.Shader.vertexShaderPath, material.Shader.fragmentShaderPath),
                    new BaseTexture(Renderer.OpenGl, material.Texture.Path, material.Texture.Type), material.Name);

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
