#define IMGUI

using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core.Input;
using HeatBlastEngine.code.Entities;
using HeatBlastEngine.code.Entities.Interfaces;
using HeatBlastEngine.code.maps;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
namespace HeatBlastEngine.code.Core;


public class Engine
{
    public static string MAIN_TITLE = "HeastBlastEngine";
    public static int ENGINE_FPS = 140;
    private static ImGuiController _controller;
    
    
    public Engine(string[] args)
    {

        
        #region window
        WindowOptions options = WindowOptions.Default with
        {
            Size = new Vector2D<int>(1920, 1080),
            Title = MAIN_TITLE,
            FramesPerSecond = ENGINE_FPS,
            VSync = false,
            Samples = 8
        };
        Renderer._window = Window.Create(options);
        
        Renderer._window.Load += OnLoad;
        Renderer._window.Render += OnRender;
        Renderer._window.Update += OnUpdate;
        Renderer._window.FramebufferResize += OnFramebufferResize;
        Renderer._window.Closing += OnClose;

        Renderer._window.Run();

       

        #endregion
    }
    
    private static void OnFramebufferResize(Vector2D<int> newsize)
    {
        Renderer.GL.Viewport(newsize);
    }

    
    private static unsafe void OnLoad() 
    {
        #region input 
        IInputContext input = Renderer._window.CreateInput();
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
        Renderer.GL = Renderer._window.CreateOpenGL();
        Renderer.GL.ClearColor(Color.FromKnownColor(KnownColor.Desktop));

        Renderer.GL.Enable(EnableCap.Blend);
        Renderer.GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        Renderer.GL.Enable(GLEnum.CullFace);
        Renderer.GL.Enable(EnableCap.Multisample);
        Renderer.GL.Enable(GLEnum.DepthTest);
        Renderer.GL.Enable(GLEnum.DebugOutput);
        Renderer.GL.DebugMessageCallback((source, type, id, severity, length, message, userParam) =>
        {
            string msg = Marshal.PtrToStringAnsi(message, length);
        }, IntPtr.Zero);
        #endregion

        //Initializes a map and adds all the entities there
        //TODO: parser/editor for that
        World.ActiveMap = new World();
        World.ActiveMap.LoadMap();

        //var physics = new PhysicsWorld();
        //physics.Initialize();
        
        //Imgui
        _controller = new ImGuiController(Renderer.GL, Renderer._window, input);
        
    }

    
    private static void OnMouseMove(IMouse mouse, Vector2 position)
    {
        if (World.ActiveMap is not null)
        {
            foreach (IMouseMove entity in World.ActiveMap.Entities.OfType<IMouseMove>())
                entity?.OnMouseMove(mouse, position);
        }
    }

    private static void OnUpdate(double deltaTime) 
    {
        if (World.ActiveMap is not null)
        {
            foreach (Entity entity in World.ActiveMap.Entities)
            {
                entity.OnUpdate(deltaTime);
            }
        }
    }

    
    private static unsafe void OnRender(double deltaTime) 
    {
        Time.Elapsed = (float)Renderer._window.Time;
        Time.FPS = 1 / deltaTime;

        Renderer.GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        Renderer.GL.ClearDepth(1f);


        
        #if IMGUI
        _controller.Update((float)deltaTime);
        ImGui.Begin("DEBUG");
        ImGui.SliderInt("FPS", ref ENGINE_FPS, 5, 1000);
        ImGui.Text(Time.Elapsed.ToString());


        
        if (World.ActiveMap is not null)
        {
            foreach (Entity entity in World.ActiveMap.Entities)
            {
                
                entity.OnRender(deltaTime);
               

                foreach (var property in  typeof(Entity).GetProperties())
                {
                    var feature = property.GetCustomAttribute<ShowInEditorAttribute>();
                    if (feature != null)
                    {
                        var transformPosition = entity.Transform.Position;
                        ImGui.SliderFloat3(entity.id.ToString(),ref transformPosition, 0,5f);
                        entity.Transform.Position = transformPosition;
                    }
                }
 
            }
            
        }

        if (World.ActiveMap is not null && SkyEntity.Instance is null)
        {
            if (ImGui.Button("ADD SKYBOX"))
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("PRESSED");
                World.ActiveMap.CreateEntity(new SkyEntity(BaseMaterial.LoadFromFile("textures/skybox.matfile"), new Model("models/editor/cube.obj")));
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
        
        Renderer._window.FramesPerSecond = ENGINE_FPS;
        
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
        if (keyarg == Key.Escape) Renderer._window.Close();

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
            Renderer.GL.PolygonMode(GLEnum.FrontAndBack, drawWireframe? GLEnum.Line : GLEnum.Fill);
        }

        if (keyarg == Key.F)
        {
            isFullscreen = !isFullscreen;
            Renderer._window.WindowState = isFullscreen? WindowState.Fullscreen: WindowState.Normal;
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
            case Key.G:
                if (World.ActiveMap is null) return;
                var ent = new RenderEntity(BaseMaterial.LoadFromFile("textures/plane.matfile"),
                    new Model("models/monkey.obj"), "CustomEntity");
                ent.Transform.Position = World.ActiveMap.camera.Transform.Position + World.ActiveMap.camera.Front * 5f;
                break;
        }
    }

    private static void OnClose()
    {

    }
}