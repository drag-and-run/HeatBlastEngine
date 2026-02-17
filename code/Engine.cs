#define IMGUI

using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
namespace HeatBlastEngine
{
    public static class Engine
    {
        public static string Title = "HeastBlastEngine";
        public static int Fps = 140;
        private static ImGuiController? _controller;


        public static Action? LoadEvent { get;  set; }

        public static void Init(string[] args)
        {
            WindowOptions options = WindowOptions.Default with
            {
                Size = new Vector2D<int>(1920, 1080),
                Title = Title,
                FramesPerSecond = Fps,
                VSync = false,
                Samples = 8
            };
            RenderManager._window = Window.Create(options);
        
            RenderManager._window.Load += OnLoad;
            RenderManager._window.Render += OnRender;
            RenderManager._window.Update += OnUpdate;
            RenderManager._window.FramebufferResize += OnFramebufferResize;
            RenderManager._window.Closing += OnClose;

            RenderManager._window.Run();
        }
    
        private static void OnFramebufferResize(Vector2D<int> newsize)
        {
            RenderManager.GL.Viewport(newsize);
        }

    
        private static void OnLoad() 
        {
            #region input 
            IInputContext input = RenderManager._window.CreateInput();
            foreach (var keyboard in input.Keyboards)
            {
                keyboard.KeyDown += KeyDown;
            }
            InputManager.PrimaryKeyboard = input.Keyboards.FirstOrDefault();

            foreach (var mouse in input.Mice)
            {
                mouse.Cursor.CursorMode = CursorMode.Raw;
                mouse.MouseMove += OnMouseMove;
            }
            InputManager.PrimaryMouse = input.Mice.FirstOrDefault();
            #endregion

            #region opengl init flags
            RenderManager.GL = RenderManager._window.CreateOpenGL();
            RenderManager.GL.ClearColor(Color.FromKnownColor(KnownColor.Desktop));

            RenderManager.GL.Enable(EnableCap.Blend);
            RenderManager.GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            RenderManager.GL.Enable(GLEnum.CullFace);
            RenderManager.GL.Enable(EnableCap.Multisample);
            RenderManager.GL.Enable(GLEnum.DepthTest);
            #endregion
            
            //Imgui
            _controller = new ImGuiController(RenderManager.GL, RenderManager._window, input);
            
            LoadEvent?.Invoke();
        }
    

    
        private static void OnMouseMove(IMouse mouse, Vector2 position)
        {
            if (World.ActiveMap is not null)
            {
                World.PlayerCamera?.OnMouseMove(mouse, position);
            }
        }

        private static void OnUpdate(double deltaTime)
        {
            Time.Elapsed = (float)RenderManager._window.Time;
            if (World.ActiveMap is not null)
            {
                foreach (Entity entity in World.ActiveMap.EntityList)
                {
                    entity?.OnUpdate(deltaTime);
                }
            }
        
            if (World.ActiveMap is null) return;



            if (InputManager.PrimaryKeyboard != null && InputManager.PrimaryKeyboard.IsKeyPressed(Key.G))
            {
                var ent = World.CreateEntity(new Entity());


            
                ent.AddComponent(new ModelRender(null!, new Model("models/box.obj")));


                Debug.Assert(World.PlayerCamera != null, "World.PlayerCamera is null");
                ent.GetComponent<Transform>().Position = World.PlayerCamera.GetComponent<Transform>().Position +
                                                         World.PlayerCamera.Front * 5f;
            }

        }

        private static readonly int Entcount = 0;
        private static unsafe void OnRender(double deltaTime) 
        {
           
            RenderManager.GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            RenderManager.GL.ClearDepth(1f);


        
#if IMGUI
            _controller?.Update((float)deltaTime);
            ImGui.Begin("DEBUG");
            ImGui.SliderInt("FPS", ref Fps, 5, 1000);
            ImGui.Text(Time.Elapsed.ToString());
            ImGui.Text(Entcount.ToString());


        
            if (World.ActiveMap is not null)
            {
                foreach (Entity entity in World.ActiveMap.EntityList)
                {
                
                    entity.OnRender(deltaTime);
                
                    var mdl = entity.GetComponent<ModelRender>();
                    if (mdl is not null)
                    {
                        mdl.Render(deltaTime);
                    }


                    foreach (var property in  typeof(Transform).GetProperties())
                    {
                        var feature = property.GetCustomAttribute<ShowInEditorAttribute>();
                        if (feature != null)
                        {
                            var comp = entity.GetComponent<Transform>();
                            if (comp is null) return;
                            var transformPosition = comp.Position;
                            ImGui.SliderFloat3(entity.Id.ToString(),ref transformPosition, 0,5f);
                            entity.GetComponent<Transform>().Position = transformPosition;
                        }
                    }
 
                }
            
            }

            if (World.ActiveMap is not null)
            {
                if (ImGui.Button("ADD SKYBOX"))
                {
                    var sky = World.CreateEntity(new Entity());
                    sky?.AddComponent(new ModelRender(BaseMaterial.LoadFromFile("textures/skybox/skybox.matfile", RenderFlags.Skybox), new Model("models/editor/cube.obj")));
                }
            }

#endif
        

            if (World.ActiveMap is not null)
            {
                ImGui.Text("CURRENT MAP: " + World.ActiveMap.Name);
            }
            else
            {
                ImGui.Text("NO MAP LOADED");
            }
        
            RenderManager._window.FramesPerSecond = Fps;
        
#if IMGUI
            ImGui.StyleColorsLight();
            ImGui.End();
            _controller?.Render();
#endif



        }

        private static bool _isCursorVisible = false;
        private static bool _isFullscreen = false;
        private static bool _drawWireframe = false;


        private static void KeyDown(IKeyboard keyboard, Key keyarg, int keyCode) 
        {
            if (keyarg == Key.Escape) RenderManager._window.Close();


            if (keyarg == Key.Tab)
            {

            }
            
            if (keyarg == Key.C)
            {
                if (_isCursorVisible)
                {
                    InputManager.PrimaryMouse?.Cursor.CursorMode = CursorMode.Raw;
                    _isCursorVisible = false;
                    Camera.lookSensitivity = 0.1f;
                }
                else
                {
                    InputManager.PrimaryMouse?.Cursor.CursorMode = CursorMode.Normal;
                    _isCursorVisible = true;
                    Camera.lookSensitivity = 0;
                }
            }

            if (keyarg == Key.V)
            {
            
                _drawWireframe = !_drawWireframe;
                RenderManager.GL.PolygonMode(GLEnum.FrontAndBack, _drawWireframe? GLEnum.Line : GLEnum.Fill);
            }

            if (keyarg == Key.F)
            {
                _isFullscreen = !_isFullscreen;
                RenderManager._window.WindowState = _isFullscreen? WindowState.Fullscreen: WindowState.Normal;
            }

            switch (keyarg)
            {
                case Key.X:
                    World.Unload();
                    break;
                case Key.R:
                    World.Create(true);
                    break;

            }

        }

        private static void OnClose()
        {

        }
    }
}