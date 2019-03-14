using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace DeeSynk.Core
{
    /// <summary>
    /// The MainWindow class contains all the highest level operations going on in the game. All updates,
    /// drawing, and initialization should occur somewhere in this class.
    /// </summary>
    public sealed class MainWindow : GameWindow
    {
        private Game _game;

        private KeyboardState keyState;    // holds current keyboard state, updated every frame
        private Color4 clearColor = Color4.Black;     // the color that OpenGL uses to clear the color buffer on each frame

        private Camera _camera = new Camera();

        private const float v = 10f;

        private Vector3 V_W = new Vector3(0.0f, 0.0f, -v);
        private Vector3 V_S = new Vector3(0.0f, 0.0f, v);
        private Vector3 V_A = new Vector3(-v, 0.0f, 0.0f);
        private Vector3 V_D = new Vector3(v, 0.0f, 0.0f);
        private Vector3 V_Up = new Vector3(0.0f, v, 0.0f);
        private Vector3 V_Dn = new Vector3(0.0f, -v, 0.0f);

        private int dx = 0;
        private int dy = 0;

        Point center;
        Point mousePos;

        private MouseState msPrevious;

        /// <summary>
        /// Basic constructor for the game window. The base keyword allows parameters to be
        /// passed to the parent class constructor. The title of the window is then set with
        /// additional information, including the current OpenGL version.
        /// </summary>
        public MainWindow() : base( 700,                        // initial width
                                    500,                        // initial height
                                    GraphicsMode.Default,
                                    "DeeSynk",                  // initial window title
                                    GameWindowFlags.Default,
                                    DisplayDevice.Default,
                                    4,                          // OpenGL major version
                                    6,                          // OpenGL minor version
                                    GraphicsContextFlags.ForwardCompatible)
        {
            Title += " | The WIP Student Video Game | OpenGL Version: " + GL.GetString(StringName.Version);
            VSync = VSyncMode.Off;
            center = new Point(Width / 2, Height / 2);
            mousePos = PointToScreen(center);
            msPrevious = Mouse.GetState();
        }
        
        /// <summary>
        /// OnResize is called if the user decides to resize the window. It will reset the viewport
        /// so that OpenGL knows the new size of the window.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _camera.Width = Width;
            _camera.Height = Height;

            center = new Point(Width / 2, Height / 2);
            mousePos = PointToScreen(center);
        }

        /// <summary>
        /// The OnLoad method gets called as soon as the window is created. Anything that needs to be
        /// created or initialized at the start of the game should go here.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.DepthTest);

            _game = new Game();
            //CursorVisible = true;

            this.Cursor = MouseCursor.Empty;
            this.WindowState = this.WindowState | WindowState.Fullscreen;

            _camera = new Camera(1.0f, (float)Width, (float)Height, 0.01f, 2000f);

            Console.WriteLine(GL.GetString(StringName.Renderer));
            _game.LoadGameData();

            _game.PushCameraRef(ref _camera);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        /// <summary>
        /// The OnUpdateFrame method gets called on every frame. Anything that needs to be updated or
        /// changed on every frame should go here. Drawing does NOT go here.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            HandleKeyboard((float)e.Time);

            _camera.AddRotation();
            _camera.UpdateMatrices();

            _game.Update((float)(e.Time));

            
        }

        /// <summary>
        /// The OnRenderFrame method gets called on every frame. Anything drawn and rendered on each frame
        /// should go here. Data changes and updates do NOT go here.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Title = $"DeeSynk | OpenGL Version: {GL.GetString(StringName.Version)} | Vsync: {VSync} | FPS: {1f / e.Time:0}"; // adds miscellaneous information to the title bar of the window
            GL.ClearColor(clearColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _game.Render();

            SwapBuffers();
        }
        
        /// <summary>
        /// The OnUnload method gets called after the Exit() function is called, but before the OpenGL instance is destroyed.
        /// If something needs to happen after the game has been closed, it should go here.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUnload(EventArgs e)
        {
            Console.WriteLine("I listen to you sleep...");
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            MouseState ms = Mouse.GetState();
            _camera.AddRotation((msPrevious.Y - ms.Y) * 0.001f, (msPrevious.X - ms.X) * 0.001f);
            OpenTK.Input.Mouse.SetPosition(mousePos.X, mousePos.Y);
            msPrevious = ms;
        }

        /// <summary>
        /// The HandleKeyboard method listens for any keyboard inputs from the user. Anything dealing
        /// with keybindings should go here.
        /// </summary>
        private void HandleKeyboard(float time)
        {
            keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Key.Escape))
                Exit();
            if (keyState.IsKeyDown(Key.W))
                _camera.AddLocation(ref V_W, time);
            if (keyState.IsKeyDown(Key.S))
                _camera.AddLocation(ref V_S, time);
            if (keyState.IsKeyDown(Key.A))
                _camera.AddLocation(ref V_A, time);
            if (keyState.IsKeyDown(Key.D))
                _camera.AddLocation(ref V_D, time);
            if (keyState.IsKeyDown(Key.Space))
                _camera.AddLocation(ref V_Up, time);
            if (keyState.IsKeyDown(Key.ShiftLeft))
                _camera.AddLocation(ref V_Dn, time);
        }
    }
}
