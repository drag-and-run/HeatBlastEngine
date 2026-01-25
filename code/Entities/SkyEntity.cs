using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core;
using HeatBlastEngine.code.Core.Entities.Lights;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;
using System.Numerics;
using HeatBlastEngine.code.Entities;
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
        Renderer.Render(this,World.ActiveMap.camera, Renderer._window, World.ActiveMap.PointLight, true);
    }

    public override void Dispose()
    {
        BaseMaterial.Texture.Dispose();
        BaseMaterial.Shader.Dispose();
        Model.Dispose();
        Instance = null;
    }
}