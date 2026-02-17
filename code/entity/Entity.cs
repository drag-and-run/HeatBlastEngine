namespace HeatBlastEngine
{
    public class Entity
    {
        private readonly HashSet<Component> _components = new();
        public Guid Id = Guid.NewGuid();
        public string Name;
        public Entity? Parent;

        public Entity(string name = "Default")
        {
            Name = name;

            DebugLog.Msg($"Entity created: {GetType()}");

           AddComponent(new Transform());
        }

        public void AddComponent(Component component)
        {
            _components.Add(component);
            component.Entity = this;
        }

        public T? GetComponent<T>() where T : Component
        {

            foreach (var component in _components)
                if (component.GetType().Equals(typeof(T)))
                    return (T)component;

            return null;
        }

        public virtual void OnUpdate(double deltaTime)
        {
        }

        public virtual void OnRender(double deltaTime)
        {
        }
    }
}