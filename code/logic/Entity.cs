using HeatBlastEngine.code.logic.components;
using HeatBlastEngine.code.maps;

namespace HeatBlastEngine.code.Entities;


public class Entity
{
    HashSet<Component> components = new();
    
    public Guid id = Guid.NewGuid();
    
    public Entity(string name = "Default")
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Entity created: {this.GetType()}"); 
        
    
        AddComponent(new Transform());
    }

    public void AddComponent(Component component)
    {
        components.Add(component);
        component.entity = this;
    }

    
    public T GetComponent<T>() where T : Component
    {
        foreach (Component component in components)
        {
            if (component.GetType().Equals(typeof(T)))
            {
                return (T)component;
            }
        }
        return null;
    }

    public virtual void OnUpdate(double deltaTime)
    {
        
    }


    public virtual void OnRender(double deltaTime)
    {
        
    }

}