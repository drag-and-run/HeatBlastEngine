using HeatBlastEngine.code;


namespace HeatBlastEngine.code.world;


public class World
{
    public static World? ActiveMap { get; set; }

    public HashSet<Entity> EntityList { get; } = [];

    public Camera? PlayerCamera { get; private set; }
    
    public void LoadMap()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Loading {ActiveMap?.GetType().Name}");
        
        PlayerCamera = (Camera)CreateEntity(new Camera());
    }



    public Entity CreateEntity(Entity ent)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"created {ent}");
        EntityList.Add(ent);
        
        return ent;
    }
    

    
    public void UnloadMap()
    {
        if (ActiveMap == null) return;
        foreach (IDisposable entity in ActiveMap.EntityList.OfType<IDisposable>())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            entity.Dispose();
            Console.WriteLine($"{entity.GetType().Name} disposed");
        }

        Console.WriteLine($"{ActiveMap.GetType().Name} unloaded");
        ActiveMap.EntityList.Clear();
        ActiveMap = null;
    }
}