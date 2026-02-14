#define IMGUI

using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
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
        public static string MAIN_TITLE = "HeastBlastEngine";
        public static int ENGINE_FPS = 140;
        private static ImGuiController _controller;
    

        public static Action OnLoadEvent;

        public static void Init(string[] args)
        {
            WindowOptions options = WindowOptions.Default with
            {
                Size = new Vector2D<int>(1920, 1080),
                Title = MAIN_TITLE,
                FramesPerSecond = ENGINE_FPS,
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

    
        private static unsafe void OnLoad() 
        {
            #region input 
            IInputContext input = RenderManager._window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += KeyDown;
           
            }
            InputManager.primaryKeyboard = input.Keyboards.FirstOrDefault();

            for (int i = 0; i < input.Mice.Count; i++)
            {
                input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
                input.Mice[i].MouseMove += OnMouseMove;
            }
            InputManager.primaryMouse = input.Mice.FirstOrDefault();
            #endregion

            #region opengl init flags
            RenderManager.GL = RenderManager._window.CreateOpenGL();
            RenderManager.GL.ClearColor(Color.FromKnownColor(KnownColor.Desktop));

            RenderManager.GL.Enable(EnableCap.Blend);
            RenderManager.GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            RenderManager.GL.Enable(GLEnum.CullFace);
            RenderManager.GL.Enable(EnableCap.Multisample);
            RenderManager.GL.Enable(GLEnum.DepthTest);
            RenderManager.GL.Enable(GLEnum.DebugOutput);
            RenderManager.GL.DebugMessageCallback((source, type, id, severity, length, message, userParam) =>
            {
                string msg = Marshal.PtrToStringAnsi(message, length);
            }, IntPtr.Zero);
            #endregion

            //Initializes a map and adds all the entities there
            //TODO: parser/editor for that
            World.ActiveMap = new World();
            World.ActiveMap.LoadMap();
        

        
            //Imgui
            _controller = new ImGuiController(RenderManager.GL, RenderManager._window, input);
            OnLoadEvent?.Invoke();
        }
    

    
        private static void OnMouseMove(IMouse mouse, Vector2 position)
        {
            if (World.ActiveMap is not null)
            {
                World.ActiveMap.PlayerCamera.OnMouseMove(mouse, position);
            }
        }
        static TimeSince sinceAdded = 0;
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


        
            if (InputManager.primaryKeyboard.IsKeyPressed(Key.G) && sinceAdded > 0.1f)
            {
                var ent = World.ActiveMap.CreateEntity(new Entity());


            
                ent.AddComponent(new ModelRender(null, new Model("models/box.obj")));
                ent.GetComponent<Transform>().Position = World.ActiveMap.PlayerCamera.GetComponent<Transform>().Position +
                                                         World.ActiveMap.PlayerCamera.Front * 5f;

                sinceAdded = 0;

            }

        }

        public static int entcount = 0;
        private static unsafe void OnRender(double deltaTime) 
        {
            RenderManager.GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            RenderManager.GL.ClearDepth(1f);


        
#if IMGUI
            _controller.Update((float)deltaTime);
            ImGui.Begin("DEBUG");
            ImGui.SliderInt("FPS", ref ENGINE_FPS, 5, 1000);
            ImGui.Text(Time.Elapsed.ToString());
            ImGui.Text(entcount.ToString());


        
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
                            var transformPosition = entity.GetComponent<Transform>().Position;
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
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("PRESSED");
                    var sky = World.ActiveMap.CreateEntity(new Entity());
                    sky.AddComponent(new ModelRender(BaseMaterial.LoadFromFile("textures/skybox.matfile", RenderFlags.Skybox), new Model("models/editor/cube.obj")));
                }
            }

#endif
        

            if (World.ActiveMap is not null)
            {
                ImGui.Text("CURRENT MAP: " + World.ActiveMap.ToString());
            }
            else
            {
                ImGui.Text("NO MAP LOADED");
            }
        
            RenderManager._window.FramesPerSecond = ENGINE_FPS;
        
#if IMGUI
            ImGui.StyleColorsLight();
            ImGui.End();
            _controller.Render();
#endif



        }
        static bool isCursorVisible = false;
        static bool isFullscreen = false;
        static bool drawWireframe = false;


        private static void KeyDown(IKeyboard keyboard, Key keyarg, int keyCode) 
        {
            if (keyarg == Key.Escape) RenderManager._window.Close();

            if (keyarg == Key.C)
            {
                if (isCursorVisible)
                {
                    InputManager.primaryMouse.Cursor.CursorMode = CursorMode.Raw;
                    isCursorVisible = false;
                    Camera.lookSensitivity = 0.1f;
                }
                else
                {
                    InputManager.primaryMouse.Cursor.CursorMode = CursorMode.Normal;
                    isCursorVisible = true;
                    Camera.lookSensitivity = 0;
                }
            }

            if (keyarg == Key.V)
            {
            
                drawWireframe = !drawWireframe;
                RenderManager.GL.PolygonMode(GLEnum.FrontAndBack, drawWireframe? GLEnum.Line : GLEnum.Fill);
            }

            if (keyarg == Key.F)
            {
                isFullscreen = !isFullscreen;
                RenderManager._window.WindowState = isFullscreen? WindowState.Fullscreen: WindowState.Normal;
            }

            switch (keyarg)
            {
                case Key.X:
                    if (World.ActiveMap is null) return;
                    World.ActiveMap.UnloadMap();
                    break;
                case Key.R:
                    if (World.ActiveMap is not null)
                    {
                        World.ActiveMap.UnloadMap();
                    }
                
                    World.ActiveMap = new World();
                    World.ActiveMap.LoadMap();
                    break;

            }

        }

        private static void OnClose()
        {

        }
    }
}