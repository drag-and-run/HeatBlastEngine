using System.Numerics;
using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core;
using HeatBlastEngine.code.Entities;


namespace HeatBlastEngine.code.maps;


public class World
{
    public static World? ActiveMap;
    
    public HashSet<Entity> Entities = new();
    
    public Camera camera { get; private set; }
    
    public void LoadMap()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Loading {ActiveMap.GetType().Name}");
        
        camera = (Camera)CreateEntity(new Camera(), new Vector3(0,0,4));
        

        CreateEntity(new RenderEntity(BaseMaterial.LoadFromFile("textures/plane.matfile"), null, "Texture only"));
        CreateEntity(new RenderEntity(null,new Model("models/test.obj"), "Model only"));
        CreateEntity(new RenderEntity(null,null, "NULL model"), new Vector3(0,1,0));
        

    }


    public Entity CreateEntity(Entity ent, Vector3 position = default)
    {
        ent.Transform.Position = position;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"created {ent}");
        Entities.Add(ent);
        
        return ent;
    }

    
    public void UnloadMap()
    {
        if (ActiveMap != null)
        {
            foreach (IDisposable entity in ActiveMap.Entities.OfType<IDisposable>())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                entity.Dispose();
                Console.WriteLine($"{entity.GetType().Name} disposed");
            }

            Console.WriteLine($"{ActiveMap.GetType().Name} unloaded");
            ActiveMap.Entities.Clear();
            ActiveMap = null;
        }
    }
}