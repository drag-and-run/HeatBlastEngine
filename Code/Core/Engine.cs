#define IMGUI

using System.Drawing;
using System.Numerics;
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
using Steamworks;
namespace HeatBlastEngine.code.Core;


public class Engine
{
    public static string MAIN_TITLE = "HeastBlastEngine";
    public static int ENGINE_FPS = 140;
    private static ImGuiController _controller;
    
    
    public Engine(string[] args)
    {
        #if STEAMWORKS
        if (args.Contains("-steam"))
        {
            Console.WriteLine("INITIALIZING STEAM");
            try 
            {
                SteamClient.Init( 480 );
            }
            catch ( System.Exception e )
            {
                Console.WriteLine(e.Message);
            }
        }
        #endif
        
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
        Renderer.OpenGl.Viewport(newsize);
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
        Renderer.OpenGl = Renderer._window.CreateOpenGL();
        Renderer.OpenGl.ClearColor(Color.FromKnownColor(KnownColor.Desktop));

        Renderer.OpenGl.Enable(EnableCap.Blend);
        Renderer.OpenGl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        Renderer.OpenGl.Enable(GLEnum.CullFace);
        Renderer.OpenGl.Enable(EnableCap.Multisample);
        Renderer.OpenGl.Enable(GLEnum.DepthTest);
        Renderer.OpenGl.Enable(GLEnum.DebugOutput);
        Renderer.OpenGl.DebugMessageCallback((source, type, id, severity, length, message, userParam) =>
        {
            string msg = Marshal.PtrToStringAnsi(message, length);
        }, IntPtr.Zero);
        #endregion

        //Initializes a map and adds all the entities there
        //TODO: parser/editor for that
        World.ActiveMap = new World();
        World.ActiveMap.LoadMap();
        
        //Imgui
        _controller = new ImGuiController(Renderer.OpenGl, Renderer._window, input);
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
                entity.Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY,float.DegreesToRadians((float)deltaTime * 40f));
            }
                
        }
    }

    
    private static unsafe void OnRender(double deltaTime) 
    {
        EngineTime.Elapsed = (float)Renderer._window.Time;
        EngineTime.FPS = 1 / deltaTime;

        Renderer.OpenGl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        Renderer.OpenGl.ClearDepth(1f);

        #if IMGUI
        _controller.Update((float)deltaTime);
        ImGui.Begin("DEBUG");
        ImGui.SliderInt("FPS", ref ENGINE_FPS, 5, 1000);
        ImGui.SliderFloat3("POS",ref newentpos,0,4);


        
        if (World.ActiveMap is not null)
        {
            foreach (Entity entity in World.ActiveMap.Entities)
            {
                entity.OnRender(deltaTime); 
                ImGui.Text($"{entity} ({entity.Name})");
            }
        }

        if (World.ActiveMap is not null && SkyEntity.Instance is null)
        {
            if (ImGui.Button("ADD SKYBOX"))
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("PRESSED");
                var sky = new SkyEntity(Material.LoadFromFile("textures/skybox.matfile"), new Model("models/editor/cube.obj"));
            }
        }

        #endif
        

        if (World.ActiveMap is not null)
        {
            ImGui.Text(World.ActiveMap.ToString());
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
    static bool drawWireframe = false;

    private static Vector3 newentpos;
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
            Renderer.OpenGl.PolygonMode(GLEnum.FrontAndBack, drawWireframe? GLEnum.Line : GLEnum.Fill);
        }

        switch (keyarg)
        {
            case Key.X:
                if (World.ActiveMap is null) return;
                World.ActiveMap.UnloadMap();
                break;
            case Key.R:
                World.ActiveMap = new World();
                World.ActiveMap.LoadMap();
                break;
            case Key.G:
                if (World.ActiveMap is null) return;
                var ent = new RenderEntity(Material.LoadFromFile("textures/plane.matfile"),
                    new Model("models/test.obj"), "CustomEntity");
                    ent.Transform.Position = newentpos;
                break;
        }
    }

    private static void OnClose()
    {

    }
}