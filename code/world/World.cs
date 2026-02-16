namespace HeatBlastEngine
{
    public class World
    {
        public static World? ActiveMap { get; set; }

        public HashSet<Entity> EntityList { get; } = [];

        public Camera? PlayerCamera { get; private set; }
    
        public void LoadMap()
        {

            DebugLog.Msg($"Loading {ActiveMap?.GetType().Name}");
        
            PlayerCamera = (Camera)CreateEntity(new Camera());
        }



        public Entity CreateEntity(Entity ent)
        {

            DebugLog.Msg($"created {ent}");
            EntityList.Add(ent);
        
            return ent;
        }
    

    
        public void UnloadMap()
        {
            if (ActiveMap == null) return;

            DebugLog.Warning($"{ActiveMap.GetType().Name} unloaded");
            ActiveMap.EntityList.Clear();
            ActiveMap = null;
        }
    }
}