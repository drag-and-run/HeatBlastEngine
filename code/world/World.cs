namespace HeatBlastEngine
{
    public class World(string name)
    {
        public string Name { get; set; } = name;

        public static World? ActiveMap { get; set; }

        public HashSet<Entity> EntityList { get; } = [];

        public static Camera? PlayerCamera { get; private set; }

        public static void Create(bool autoload = false, string name = "untitled")
        {
            if (ActiveMap is not null) return;
            ActiveMap = new World(name);

            if (!autoload) return;
            Load();
        }
    
        public static void Load()
        {
            DebugLog.Msg($"Loading {ActiveMap?.GetType().Name}");
            PlayerCamera = (Camera)CreateEntity(new Camera());
        }



        public static Entity? CreateEntity(Entity ent)
        {
            if (ActiveMap is null) return null;
            DebugLog.Msg($"created {ent}");
            ActiveMap.EntityList.Add(ent);
        
            return ent;
        }
    

    
        public static void Unload()
        {
            if (ActiveMap == null) return;

            DebugLog.Warning($"{ActiveMap.GetType().Name} unloaded");
            ActiveMap.EntityList.Clear();
            ActiveMap = null;
        }
    }
}