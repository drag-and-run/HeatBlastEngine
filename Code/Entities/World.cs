using System.Numerics;
using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core;
using HeatBlastEngine.code.Core.Entities.Lights;
using HeatBlastEngine.code.Entities;

namespace HeatBlastEngine.code.maps;


public class World
{
    public static World? ActiveMap;
    
    public List<Entity> Entities = new List<Entity>();

    public PointLight PointLight;
    public Camera camera { get; private set; }


    public void LoadMap()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Loading {ActiveMap.GetType().Name}");
        
        camera = new Camera();
        PointLight = new PointLight();
        var sky = new SkyEntity(Material.LoadFromFile("textures/skybox.matfile"), new Model("models/editor/cube.obj"));

        new RenderEntity(Material.LoadFromFile("textures/plane.matfile"),new Model("models/test.obj"));
    }

    public void AddEntity(Entity entity, Transform transform)
    {
        entity.Transform = transform;
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