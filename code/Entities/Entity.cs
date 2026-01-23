namespace HeatBlastEngine.code.Entities;

public class Entity
{
    public string Name {get; set;}
    public Guid uid =  Guid.NewGuid();
    public Transform Transform { get; set; }
    
    public Entity() 
    {
        Transform = new Transform();
    }

    public virtual void OnUpdate(double deltaTime)
    {

    }

    public virtual void OnRender(double deltaTime)
    {
        
    }

}