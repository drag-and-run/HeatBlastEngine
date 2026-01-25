using HeatBlastEngine.code.Core;
using HeatBlastEngine.code.maps;
using Silk.NET.OpenGL;

namespace HeatBlastEngine.code.Entities;

public class Entity
{
    public string Name {get; set;}
    public Guid uid =  Guid.NewGuid();
    public Transform Transform { get; set; }

    public float test = 0;
    
    public Entity(string name = "Default") 
    {
        Name = name;
        Transform = new Transform();
        World.ActiveMap.Entities.Add(this);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Entity created: {Name} {this.GetType()}");
    }

    public virtual void OnUpdate(double deltaTime)
    {

    }

    public virtual void OnRender(double deltaTime)
    {
        
    }

}