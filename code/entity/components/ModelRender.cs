using System.Diagnostics;

namespace HeatBlastEngine
{

    public class ModelRender(BaseMaterial? baseMaterial, Model model) : Component
    {
        public BaseMaterial? BaseMaterial = baseMaterial;
        public Model? Model = model;

        public void Render(double deltaTime)
        {
            if (World.ActiveMap is null) return;
            if (World.PlayerCamera is null) return;
                RenderManager.Render(this, World.PlayerCamera, RenderManager._window, BaseMaterial);
        }
    }
}