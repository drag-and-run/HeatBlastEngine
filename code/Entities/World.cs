using System.Numerics;
using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core;
using HeatBlastEngine.code.Core.Entities.Lights;
using HeatBlastEngine.Code.Editor;
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
        camera.Transform.Position = new Vector3(0, 0, 4);
        PointLight = new PointLight();

        
        var error1 = new RenderEntity(BaseMaterial.LoadFromFile("textures/plane.matfile"),null, "Texture only");
        var error2 = new RenderEntity(null,new Model("models/test.obj"), "Model only");
        error2.Transform.Position = new Vector3(4, 0, 0);
        var error3 = new RenderEntity(null,null, "NULL model");
        error3.Transform.Position = new Vector3(-4, 0, 0);

        
        var axis = new MapAxis(BaseMaterial.LoadFromFile("textures/editor/axis.matfile"),new Model("models/editor/axis.obj"));
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