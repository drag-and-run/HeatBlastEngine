using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core;
using HeatBlastEngine.code.Core.Entities;
using HeatBlastEngine.code.Core.Entities.Lights;
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
using System.Text.Json;
using System.Text.Json.Serialization;
using HeatBlastEngine.code.maps;
using Steamworks;


public class Program
{
    public static string MAIN_TITLE = "HeastBlastEngine";
    public static int ENGINE_FPS = 140;

    private static IWindow _window;
    
    private static ImGuiController _controller;


    private static IKeyboard primaryKeyboard;
    private static IMouse primaryMouse;

    
    private static Vector2 LastMousePosition;
    private static float lookSensitivity = 0.1f;

    public static GameMap ActiveMap;

    

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
        Renderer._gl.Viewport(newsize);
    }

    
    private static unsafe void OnLoad() 
    {

        #region input 
        IInputContext input = _window.CreateInput();
        for (int i = 0; i < input.Keyboards.Count; i++)
        {
            input.Keyboards[i].KeyDown += KeyDown;
           
        }
        primaryKeyboard = input.Keyboards.FirstOrDefault();

        for (int i = 0; i < input.Mice.Count; i++)
        {
            input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
            input.Mice[i].MouseMove += OnMouseMove;
        }
        primaryMouse = input.Mice.FirstOrDefault();
        #endregion

        #region opengl init flags
        Renderer._gl = _window.CreateOpenGL();
        Renderer._gl.ClearColor(Color.FromKnownColor(KnownColor.Desktop));

        Renderer._gl.Enable(EnableCap.Blend);
        Renderer._gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        Renderer._gl.Enable(GLEnum.CullFace);
        Renderer._gl.Enable(EnableCap.Multisample);
        Renderer._gl.Enable(GLEnum.DepthTest);
        Renderer._gl.Enable(GLEnum.DebugOutput);
        Renderer._gl.DebugMessageCallback((source, type, id, severity, length, message, userParam) =>
        {
            string msg = Marshal.PtrToStringAnsi(message, length);
        }, IntPtr.Zero);
        #endregion

        //Initialized a map and adds all the entities ther
        //TODO: parser/editor for that
        ActiveMap = new GameMap();
        
        //Imgui
        _controller = new ImGuiController(Renderer._gl, _window, input);
    }

    
    private static void OnMouseMove(IMouse mouse, Vector2 position)
    {

        if (LastMousePosition == default) { LastMousePosition = position; }
        else
        {
           
            var xOffset = (position.X - LastMousePosition.X) * lookSensitivity;
            var yOffset = (position.Y - LastMousePosition.Y) * lookSensitivity;
            LastMousePosition = position;

            ActiveMap.camera.Yaw += xOffset;
            ActiveMap.camera.Pitch -= yOffset;

            ActiveMap.camera.Pitch = Math.Clamp(ActiveMap.camera.Pitch, -89f, 89f);

            ActiveMap.camera.Direction.X = MathF.Cos(float.DegreesToRadians( ActiveMap.camera.Yaw)) * MathF.Cos(float.DegreesToRadians(ActiveMap.camera.Pitch));
            ActiveMap.camera.Direction.Y = MathF.Sin(float.DegreesToRadians(ActiveMap.camera.Pitch));
            ActiveMap.camera.Direction.Z = MathF.Sin(float.DegreesToRadians( ActiveMap.camera.Yaw)) * MathF.Cos(float.DegreesToRadians(ActiveMap.camera.Pitch));

            ActiveMap.camera.Front = Vector3.Normalize(ActiveMap.camera.Direction);
        }
    }

    private static void OnUpdate(double deltaTime) 
    {
        var speed = 5f * (float)deltaTime;
        if (primaryKeyboard.IsKeyPressed(Key.W))
        {
            ActiveMap.camera.Transform.Position += speed * ActiveMap.camera.Front;
        }
        if (primaryKeyboard.IsKeyPressed(Key.S))
        {

            ActiveMap.camera.Transform.Position -= speed * ActiveMap.camera.Front;
        }
        if (primaryKeyboard.IsKeyPressed(Key.D))
        {
            ActiveMap.camera.Transform.Position += Vector3.Normalize(Vector3.Cross(ActiveMap.camera.Front, ActiveMap.camera.Transform.Up)) * speed;
        }
        if (primaryKeyboard.IsKeyPressed(Key.A))
        {
            ActiveMap.camera.Transform.Position -= Vector3.Normalize(Vector3.Cross(ActiveMap.camera.Front, ActiveMap.camera.Transform.Up)) * speed;
        }

        ActiveMap._Light.Transform.Position = ActiveMap.camera.Transform.Position;
    }

    static TimeSince updatestats = 0;
    private static unsafe void OnRender(double deltaTime) 
    {
        BaseTime.Elapsed = (float)_window.Time;
        BaseTime.FPS = 1 / deltaTime;

        Renderer._gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        Renderer._gl.ClearDepth(1f);

        _controller.Update((float)deltaTime);
        ImGui.Begin("DEBUG");
        foreach (Entity ent in ActiveMap._entities)
        {
            if (ent is null) return;
            ent.Render(ActiveMap.camera, _window, Renderer._gl, ActiveMap._Light);
            ImGui.Text($"{ent.ToString()} Name: {ent.Name}");
        }

        ImGui.SliderInt("FPS", ref ENGINE_FPS, 5, 1000);
        _window.FramesPerSecond = ENGINE_FPS;


        if (updatestats >= 0.25f)
        {
            _window.Title = $"{MAIN_TITLE} {Math.Ceiling(BaseTime.FPS)} FPS";
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
        Console.WriteLine("\u001b[36m" + "KEY PRESSED: " +keyarg.ToString() + " " + keyCode.ToString() + "\u001b[0m");
        if (keyarg == Key.Escape) _window.Close();

        if (keyarg == Key.C)
        {
            if (isCursorVisible)
            {
                primaryMouse.Cursor.CursorMode = CursorMode.Raw;
                isCursorVisible = false;
                lookSensitivity = 0.1f;
            }
            else
            {
                primaryMouse.Cursor.CursorMode = CursorMode.Normal;
                isCursorVisible = true;
                lookSensitivity = 0;
            }
        }

        if (keyarg == Key.V)
        {
            drawWireframe = !drawWireframe;
            Renderer._gl.PolygonMode(GLEnum.FrontAndBack, drawWireframe? GLEnum.Line : GLEnum.Fill);
        }
    }

    private static void OnClose()
    {
        foreach (Entity ent in ActiveMap._entities)
        {
            if (ent is null) return;
               ent.Dispose();
        }
    }
}