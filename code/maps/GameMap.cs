using System.Numerics;
using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core;
using HeatBlastEngine.code.Core.Entities.Lights;

namespace HeatBlastEngine.code.maps;


public class GameMap
{
    public List<Entity> _entities = new List<Entity>();

    public LightObject _Light;
    public Camera camera { get; private set; }
    public GameMap()
    {
        camera = new Camera();
        camera.Transform.Position = new Vector3(0, 1, 2);
        
        var mat = BaseMaterial.LoadFromFile("textures/default_material.matfile");
        

        var cubebox_mdl = new Model("models/test.obj");
        _entities.Add(new Entity(mat, cubebox_mdl, new Transform(new Vector3(2,2,0))));

        var skymat = BaseMaterial.LoadFromFile("textures/skybox.matfile");
        var skymdl = new Model("models/editor/cube.obj");
        
        _entities.Add(new SkyEntity(skymat, skymdl,  new Transform(Vector3.Zero )));
        _Light = new LightObject(new Transform(new Vector3(0,0,0)));
    }
}