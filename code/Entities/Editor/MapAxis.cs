using System.Numerics;
using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core;
using HeatBlastEngine.code.Core.Entities.Lights;
using HeatBlastEngine.code.maps;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace HeatBlastEngine.Code.Editor;

public class MapAxis : RenderEntity
{
    public MapAxis(BaseMaterial baseMaterial, Model _model, string name = "defaulAxisEntity") : base(baseMaterial, _model, name)
    {
    }
    
    public override void OnRender(double deltaTime)
    {
        Renderer.Render(this,World.ActiveMap.camera, Renderer._window, World.ActiveMap.PointLight, RenderType.Gizmo);
    }
    

}
