using HeatBlastEngine.code.maps;

namespace HeatBlastEngine.code.Entities;


public class Entity
{
    [ShowInEditor] public Transform Transform { get; set; } = new();
    
    public Guid id = Guid.NewGuid();
    
    public Entity(string name = "Default")
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Entity created: {this.GetType()}"); 
    }
    

    public virtual void OnUpdate(double deltaTime)
    {

    }

    public virtual void OnRender(double deltaTime)
    {
        
    }

}