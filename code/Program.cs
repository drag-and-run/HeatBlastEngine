using HeatBlastEngine.code.assets;
using HeatBlastEngine.code.Core;
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


public class Program
{
    public static string MAIN_TITLE = "HeastBlastEngine";
    public static int ENGINE_FPS = 140;

    private static IWindow _window;
    private static GL _gl;
    private static ImGuiController _controller;


    private static IKeyboard primaryKeyboard;
    private static IMouse primaryMouse;

    private static Camera camera;
    private static Vector2 LastMousePosition;
    private static float lookSensitivity = 0.1f;

    private static List<BaseEntity> _entities = new List<BaseEntity>();

    private static LightObject _Light;

    

    public static void Main(string[] args)
    {
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

        
    }




    private static void OnFramebufferResize(Vector2D<int> newsize)
    {
        _gl.Viewport(newsize);
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
        _gl = _window.CreateOpenGL();
        _gl.ClearColor(Color.FromKnownColor(KnownColor.Desktop));

        _gl.Enable(EnableCap.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        _gl.Enable(GLEnum.CullFace);
        _gl.Enable(EnableCap.Multisample);
        _gl.Enable(GLEnum.DepthTest);
        _gl.Enable(GLEnum.DebugOutput);
        _gl.DebugMessageCallback((source, type, id, severity, length, message, userParam) =>
        {
            string msg = Marshal.PtrToStringAnsi(message, length);
            Console.WriteLine($"OpenGL Debug: {msg}, Severity: {severity}");
        }, IntPtr.Zero);
        #endregion



        camera = new Camera();
        camera.Transform.Position = new Vector3(0, 1, 2);

        _controller = new ImGuiController(_gl, _window, input);

        /*
        var mat = new BaseMaterial( new BaseShader(_gl, "shaders/vertshader.glsl", "shaders/frag_light_basic.glsl"), 
            new BaseTexture(_gl, "textures/test.png", TextureType.Color), "test_material");
        
        string json = JsonSerializer.Serialize(mat, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("textures/test.matfile", json);
        
        string[] cubemap = new string[]
        {
            "textures/skybox/px.png", 
            "textures/skybox/nx.png",
            "textures/skybox/py.png", //Top
            "textures/skybox/ny.png", //Bottom
            "textures/skybox/pz.png",
            "textures/skybox/nz.png",
        
        };
        
        var skybox = new BaseMaterial(new BaseShader(_gl, "shaders/vert_cubemap.glsl", "shaders/frag_cubemap.glsl"),
           new BaseTexture(_gl, cubemap), "sky_material");
        
        string jsonsky = JsonSerializer.Serialize(skybox, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("textures/skybox.matfile", jsonsky);
        */

        var mat = BaseMaterial.LoadFromFile("textures/default_material.matfile", _gl);
        //var planemat = BaseMaterial.LoadFromFile("textures/plane.matfile", _gl);
        //var plane_mdl = new Model(_gl, "models/editor/plane.obj");
        //_entities.Add(new BaseEntity(planemat, plane_mdl, new Transform(Vector3.Zero)));


        var cubebox_mdl = new Model(_gl, "models/test.obj");
        _entities.Add(new BaseEntity(mat, cubebox_mdl, new Transform(Vector3.Zero)));

        var skymat = BaseMaterial.LoadFromFile("textures/skybox.matfile", _gl);
        var skymdl = new Model(_gl, "models/editor/cube.obj");
        
        _entities.Add(new SkyEntity(skymat, skymdl,  new Transform(Vector3.Zero)));


        _Light = new LightObject(new Transform(new Vector3(0,0,0)));

    }

    
    private static void OnMouseMove(IMouse mouse, Vector2 position)
    {

        if (LastMousePosition == default) { LastMousePosition = position; }
        else
        {
           
            var xOffset = (position.X - LastMousePosition.X) * lookSensitivity;
            var yOffset = (position.Y - LastMousePosition.Y) * lookSensitivity;
            LastMousePosition = position;

            camera.Yaw += xOffset;
            camera.Pitch -= yOffset;

            camera.Pitch = Math.Clamp(camera.Pitch, -89f, 89f);

            camera.Direction.X = MathF.Cos(float.DegreesToRadians( camera.Yaw)) * MathF.Cos(float.DegreesToRadians(camera.Pitch));
            camera.Direction.Y = MathF.Sin(float.DegreesToRadians(camera.Pitch));
            camera.Direction.Z = MathF.Sin(float.DegreesToRadians( camera.Yaw)) * MathF.Cos(float.DegreesToRadians(camera.Pitch));

            camera.Front = Vector3.Normalize(camera.Direction);
        }
    }

    private static void OnUpdate(double deltaTime) 
    {
        var speed = 5f * (float)deltaTime;
        if (primaryKeyboard.IsKeyPressed(Key.W))
        {
            camera.Transform.Position += speed * camera.Front;
        }
        if (primaryKeyboard.IsKeyPressed(Key.S))
        {

            camera.Transform.Position -= speed * camera.Front;
        }
        if (primaryKeyboard.IsKeyPressed(Key.D))
        {
            camera.Transform.Position += Vector3.Normalize(Vector3.Cross(camera.Front, camera.Transform.Up)) * speed;
        }
        if (primaryKeyboard.IsKeyPressed(Key.A))
        {
            camera.Transform.Position -= Vector3.Normalize(Vector3.Cross(camera.Front, camera.Transform.Up)) * speed;
        }

        _Light.Transform.Position = camera.Transform.Position;
    }

    static TimeSince updatestats = 0;
    private static unsafe void OnRender(double deltaTime) 
    {
        BaseTime.Elapsed = (float)_window.Time;
        BaseTime.FPS = 1 / deltaTime;

        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _gl.ClearDepth(1f);

        _controller.Update((float)deltaTime);
        ImGui.Begin("DEBUG");
        foreach (BaseEntity ent in _entities)
        {
            if (ent is null) return;
            ent.Render(camera, _window, _gl, _Light);
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
            _gl.PolygonMode(GLEnum.FrontAndBack, drawWireframe? GLEnum.Line : GLEnum.Fill);
        }
    }

    private static void OnClose()
    {
        foreach (BaseEntity ent in _entities)
        {
            if (ent is null) return;
               ent.Dispose();
        }
    }
}