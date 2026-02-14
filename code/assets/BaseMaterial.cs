using HeatBlastEngine.code;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        
        private static Dictionary<string, BaseMaterial> _materialCache = new Dictionary<string, BaseMaterial>();

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
            // First, check if the material is already in the cache
            if (_materialCache.ContainsKey(filepath))
            {
                Stopwatch cachedwatch = Stopwatch.StartNew();
                Console.WriteLine($"[BaseMaterial] Returning cached material: {filepath}");
                Console.ForegroundColor = ConsoleColor.Blue;
                cachedwatch.Stop();
                Console.WriteLine($"texture loaded in: {cachedwatch.ElapsedTicks} ticks ({cachedwatch.ElapsedMilliseconds} ms)");
                Console.ResetColor();
                return _materialCache[filepath];
            }
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            string jsonString = File.ReadAllText(filepath);
            if (jsonString is null)
            {
                Console.WriteLine($"faile to load material{filepath}");
                return null;
            }
            BaseMaterial baseMaterial = JsonSerializer.Deserialize<BaseMaterial>(jsonString);


            //TODO: Handle textureless materials
            Console.ForegroundColor = ConsoleColor.Blue;
            stopwatch.Stop();
            Console.WriteLine($"texture loaded in: {stopwatch.ElapsedTicks} ticks ({stopwatch.ElapsedMilliseconds} ms)");
            Console.ResetColor();

                BaseMaterial newMaterial = new BaseMaterial(
                    new BaseShader(baseMaterial.Shader.vertexShaderPath, baseMaterial.Shader.fragmentShaderPath),
                    new BaseTexture(RenderManager.GL, baseMaterial.Texture.Path, baseMaterial.Texture.Type),
                    baseMaterial.Name,
                    flags
                );

                // Cache the loaded material
                _materialCache[filepath] = newMaterial;

                return newMaterial;

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
