namespace HeatBlastEngine
{

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
            RenderManager.Render(this, World.ActiveMap.PlayerCamera, RenderManager._window, BaseMaterial);
        }



        public virtual void Dispose()
        {
            BaseMaterial.Texture.Dispose();
            BaseMaterial.Shader.Dispose();
            Model.Dispose();
        }
    }
}