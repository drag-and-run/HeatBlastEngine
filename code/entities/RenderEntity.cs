using HeatBlastEngine.code.assets;
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
        Renderer.Render(this,World.ActiveMap.camera, Renderer._window, RenderType.Default);
    }



    public virtual void Dispose()
    {
        BaseMaterial.Texture.Dispose();
        BaseMaterial.Shader.Dispose();
        Model.Dispose();
    }
}

