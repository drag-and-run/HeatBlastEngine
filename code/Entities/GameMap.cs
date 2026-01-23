using System.Numerics;
using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core;
using HeatBlastEngine.code.Core.Entities.Lights;
using HeatBlastEngine.code.Entities;

namespace HeatBlastEngine.code.maps;


public class GameMap
{
    
    public List<Entity> Entities = new List<Entity>();

    public LightObject _Light;
    public Camera camera { get; private set; }
    public GameMap()
    {
        camera = new Camera();
        camera.Transform.Position = new Vector3(0, 1, 2);
        Entities.Add(camera);
        
        var mat = Material.LoadFromFile("textures/default_material.matfile");
        

        var cubebox_mdl = new Model("models/test.obj");
        Entities.Add(new RenderEntity(mat, cubebox_mdl));

        var skymat = Material.LoadFromFile("textures/skybox.matfile");
        var skymdl = new Model("models/editor/cube.obj");
        
        Entities.Add(new SkyEntity(skymat, skymdl));
        _Light = new LightObject(new Transform(new Vector3(0,0,0)));
    }
    
}