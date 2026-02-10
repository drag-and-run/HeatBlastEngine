using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core;
using HeatBlastEngine.code.maps;


public class SkyEntity : RenderEntity
{
    public static SkyEntity? Instance;
    
    public SkyEntity(BaseMaterial baseMaterial, Model _model, string name = "defaultSky") : base(baseMaterial, _model, name)
    {
        Instance = this;
    }


    public override void OnRender(double deltaTime)
    {
        Renderer.Render(this,World.ActiveMap.camera, Renderer._window, RenderType.Sky);
    }

    public override void Dispose()
    {
        BaseMaterial.Texture.Dispose();
        BaseMaterial.Shader.Dispose();
        Model.Dispose();
        Instance = null;
    }
}