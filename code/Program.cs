using HeatBlastEngine.code.Core;
using ImGuiNET;
using Sandbox;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using HeatBlastEngine.code.Core.Input;
using HeatBlastEngine.code.Entities;
using HeatBlastEngine.code.Entities.Interfaces;
using HeatBlastEngine.code.maps;
using Steamworks;


public class Program
{
    public static string MAIN_TITLE = "HeastBlastEngine";
    public static int ENGINE_FPS = 140;

    private static IWindow _window;
    
    private static ImGuiController _controller;



    


    public static void Main(string[] args)
    {
        #region steam
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
        #endregion
        
        #region window
        WindowOptions options = WindowOptions.Default with
        {
            Size = new Vector2D<int>(1920, 1080),
            Title = MAIN_TITLE,
            FramesPerSecond = ENGINE_FPS,
            VSync = false,
            Samples = 8
        };
        _window = Window.Create(options);
        
        _window.Load += OnLoad;
        _window.Render += OnRender;
        _window.Update += OnUpdate;
        _window.FramebufferResize += OnFramebufferResize;
        _window.Closing += OnClose;

        _window.Run();
        #endregion
    }
    private static void OnFramebufferResize(Vector2D<int> newsize)
    {
        Renderer.OpenGl.Viewport(newsize);
    }

    
    private static unsafe void OnLoad() 
    {

        #region input 
        IInputContext input = _window.CreateInput();
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
        Renderer.OpenGl = _window.CreateOpenGL();
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

        //Initialized a map and adds all the entities ther
        //TODO: parser/editor for that
        GameState.ActiveMap = new GameMap();
        
        //Imgui
        _controller = new ImGuiController(Renderer.OpenGl, _window, input);
    }

    
    private static void OnMouseMove(IMouse mouse, Vector2 position)
    {
        foreach (IMouseMove entity in GameState.ActiveMap.Entities.OfType<IMouseMove>())
            entity.OnMouseMove(mouse, position);
    }

    private static void OnUpdate(double deltaTime) 
    {
        foreach (Entity entity in GameState.ActiveMap.Entities) 
            entity.OnUpdate(deltaTime);
    }

    static TimeSince updatestats = 0;
    private static unsafe void OnRender(double deltaTime) 
    {
        EngineTime.Elapsed = (float)_window.Time;
        EngineTime.FPS = 1 / deltaTime;

        Renderer.OpenGl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        Renderer.OpenGl.ClearDepth(1f);

        _controller.Update((float)deltaTime);
        ImGui.Begin("DEBUG");
        foreach (RenderEntity entities in GameState.ActiveMap.Entities.OfType<RenderEntity>())
        {
            if (entities is null) return;
            entities.Render(GameState.ActiveMap.camera, _window, GameState.ActiveMap._Light);
            ImGui.Text($"{entities.ToString()} Name: {entities.Name}");
        }

        ImGui.SliderInt("FPS", ref ENGINE_FPS, 5, 1000);
        _window.FramesPerSecond = ENGINE_FPS;


        if (updatestats >= 0.25f)
        {
            _window.Title = $"{MAIN_TITLE} {Math.Ceiling(EngineTime.FPS)} FPS";
            updatestats = 0;
        }

        
     
        ImGui.StyleColorsLight();
        ImGui.End();

        
        _controller.Render();
    }
    static bool isCursorVisible = false;
    static bool drawWireframe = false;
    private static void KeyDown(IKeyboard keyboard, Key keyarg, int keyCode) 
    {
        if (keyarg == Key.Escape) _window.Close();

        if (keyarg == Key.C)
        {
            if (isCursorVisible)
            {
                InputManager.primaryMouse.Cursor.CursorMode = CursorMode.Raw;
                isCursorVisible = false;
            }
            else
            {
                InputManager.primaryMouse.Cursor.CursorMode = CursorMode.Normal;
                isCursorVisible = true;
            }
        }

        if (keyarg == Key.V)
        {
            drawWireframe = !drawWireframe;
            Renderer.OpenGl.PolygonMode(GLEnum.FrontAndBack, drawWireframe? GLEnum.Line : GLEnum.Fill);
        }
    }

    private static void OnClose()
    {
        foreach (IDisposable entity in GameState.ActiveMap.Entities.OfType<IDisposable>())
            entity.Dispose();
    }
}