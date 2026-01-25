using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core.Entities.Lights;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;
using HeatBlastEngine.code.Core;
using HeatBlastEngine.code.Entities;
using HeatBlastEngine.code.maps;


public class RenderEntity : Entity, IDisposable
{

    public BaseMaterial BaseMaterial;
    public Model Model;

    public RenderEntity(BaseMaterial baseMaterial, Model _model,string name ="defaulRenderEntity") : base(name)
    {
        BaseMaterial = baseMaterial;
        Model = _model;
    }

    public override void OnRender(double deltaTime)
    {
        Renderer.Render(this,World.ActiveMap.camera, Renderer._window, World.ActiveMap.PointLight, false);
    }



    public virtual void Dispose()
    {
        BaseMaterial.Texture.Dispose();
        BaseMaterial.Shader.Dispose();
        Model.Dispose();
    }

}

