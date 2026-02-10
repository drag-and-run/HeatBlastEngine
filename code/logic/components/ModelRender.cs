using System.Diagnostics;
using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core;
using HeatBlastEngine.code.maps;

namespace HeatBlastEngine.code.logic.components;

public class ModelRender : Component, IDisposable
{
    public BaseMaterial? BaseMaterial;
    public Model? Model;
    
    public ModelRender(BaseMaterial baseMaterial, Model _model)
    {
        BaseMaterial = baseMaterial;
        Model = _model;
    }
    
    public void Render(double deltaTime)
    {
        if (World.ActiveMap is null) return;
        Renderer.Render(this,World.ActiveMap.camera, Renderer._window, BaseMaterial);
    }



    public virtual void Dispose()
    {
        BaseMaterial.Texture.Dispose();
        BaseMaterial.Shader.Dispose();
        Model.Dispose();
    }
}