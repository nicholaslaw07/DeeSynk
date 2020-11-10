using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Input;
using DeeSynk.Core.Managers;
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

        private Color4 clearColor = Color4.Black;     // the color that OpenGL uses to clear the color buffer on each frame

        public Camera _camera = new Camera();

        Point center;
        Point mousePos;
        private bool _centerMouse;

        private long fpsOld = 0;
        private long frameCount = 0;
        private double timeCount = 0;
        private Stopwatch sw, sw2;

        private MouseState msPrevious;

        public static int width =  1920;
        public static int height = 1080;

        private Stopwatch loadTimer;

        private MouseCursor _cursor;

        /// <summary>
        /// Basic constructor for the game window. The base keyword allows parameters to be
        /// passed to the parent class constructor. The title of the window is then set with
        /// additional information, including the current OpenGL version.
        /// </summary>
        public MainWindow() : base( width,                        // initial width
                                    height,                        // initial height
                                    GraphicsMode.Default,
                                    "DeeSynk",                  // initial window title
                                    GameWindowFlags.Default,
                                    DisplayDevice.Default,
                                    4,                          // OpenGL major version
                                    6,                          // OpenGL minor version
                                    GraphicsContextFlags.ForwardCompatible)
        {
            loadTimer = new Stopwatch();
            loadTimer.Start();

            Title += " | The WIP Student Video Game | OpenGL Version: " + GL.GetString(StringName.Version);

            Width = width;
            Height = height;

            VSync = VSyncMode.Off;
            center = new Point(Width / 2, Height / 2);
            mousePos = PointToScreen(center);
            msPrevious = Mouse.GetState();
            _centerMouse = true;
            sw = new Stopwatch();
            sw2 = new Stopwatch();
            int w = 21;
            int h = 21;
            byte[] data = new byte[w * h * 4];
            int count = 0;
            for(int y = 0; y < h; y++)
            {
                for(int x = 0; x < w; x++)
                {
                    count++;
                    double xd = x - w / 2.0d;
                    double yd = y - h / 2.0d;
                    double d = Math.Sqrt(xd * xd + yd * yd);
                    double r = 6.0d;
                    byte alpha = (d <= r) ? (byte)255 : (byte)((1 - (d - r)/2) * 255);
                    if (d <= r + 2)
                    {
                        data[(y * w + x) * 4 + 0] = 255;
                        data[(y * w + x) * 4 + 1] = 255;
                        data[(y * w + x) * 4 + 2] = 255;
                        data[(y * w + x) * 4 + 3] = alpha;
                    }
                    else
                    {
                        data[(y * w + x) * 4 + 0] = 0;
                        data[(y * w + x) * 4 + 1] = 0;
                        data[(y * w + x) * 4 + 2] = 0;
                        data[(y * w + x) * 4 + 3] = 0;
                    }
                }
            }
            _cursor = new MouseCursor((w+1) / 2, (h+1) / 2, w, h, data);
            //WindowState = WindowState.Fullscreen;
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

            width = Width;
            height = Height;
        }

        /// <summary>
        /// The OnLoad method gets called as soon as the window is created. Anything that needs to be
        /// created or initialized at the start of the game should go here.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            Managers.ModelManager.GetInstance().Load();
            Managers.ShaderManager.GetInstance().Load();
            Managers.TextureManager.GetInstance().Load();
            Managers.InputManager.GetInstance().Load();

            SetGLState();

            //_camera = new Camera(1.0f, (float)Width, (float)Height, 0.001f, 300f);
            _camera = new Camera(1.0f, (float)Width, (float)Height, 0.1f, 10f);
            _camera.OverrideLookAtVector = true;
            _camera.Location = new Vector3(0.0f, 0.25f, 1.0f);
            _camera.UpdateMatrices();

            _game = new Game(ref _camera);

            CursorVisible = true;
            _centerMouse = true;
            this.Cursor = MouseCursor.Empty;
            Action<float, MouseArgs> c = CenterMouse;
            var im = InputManager.GetInstance();

            {
                var ic = new List<InputAssignment>();
                ic.Add(new InputAssignment(InputType.Keyboard, Key.C));
                var dl = new List<Action<float, MouseArgs>>();
                dl.Add(c);
                {
                    im.Configurations.TryGetValue("primary move", out InputConfiguration config);
                    var ia = new InputAction(Qualifiers.IGNORE_AFTER | Qualifiers.IGNORE_BEFORE, ic);
                    ia.UpActions = dl;
                    config.InputActions.AddLast(ia);
                    im.SetConfig("primary move");
                }
                {
                    im.Configurations.TryGetValue("unlocked mouse", out InputConfiguration config1);
                    var ia = new InputAction(Qualifiers.IGNORE_AFTER | Qualifiers.IGNORE_BEFORE, ic);
                    ia.UpActions = dl;
                    config1.InputActions.AddLast(ia);
                }
            }
            im.StartThreads(1);

            Console.WriteLine(GL.GetString(StringName.Renderer));
            _game.LoadGameData();

            _game.PushCameraRef(ref _camera);

            GCCollectDebug();

            loadTimer.Stop();
            Console.WriteLine("Window loaded in {0} seconds", ((float)loadTimer.ElapsedMilliseconds) / 1000.0f);

            sw.Start();
            sw2.Start();
        }

        private void SetGLState()
        {
            GL.ClipControl(ClipOrigin.LowerLeft, ClipDepthMode.ZeroToOne);
            GL.Enable(EnableCap.DepthTest);
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
            if (_game.Init)
            {
                if (_game.SystemInput.ShutDownProgram)
                    Exit();

                _camera.UpdateRotation();
                _camera.UpdateMatrices();

                _game.Update((float)(e.Time));
            }
        }

        /// <summary>
        /// The OnRenderFrame method gets called on every frame. Anything drawn and rendered on each frame
        /// should go here. Data changes and updates do NOT go here.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (_game.Init)
            {
                timeCount += e.Time;
                frameCount++;

                //| The WIP Student Video Game

                if (sw.ElapsedMilliseconds % 20 == 0)
                {
                    var state = Mouse.GetCursorState();
                    var p = new Point(state.X, state.Y);
                    var t = InputManager.GetInstance().AverageTime / 10000.0f;
                    Title = $"DeeSynk | Vsync: {VSync} | FPS: {fpsOld} | {Width}x{Height} | {RoundVector(_camera.Location, 2)} | {p.X}, {p.Y} | {t}ms";
                }

                if (sw.ElapsedMilliseconds > 1000 / 120)
                {
                    var state = Mouse.GetCursorState();
                    var p = new Point(state.X, state.Y);
                    var t = InputManager.GetInstance().AverageTime / 10000.0f;
                    sw.Stop();
                    //Title = $"DeeSynk | The WIP Student Video Game | OpenGL Version: {GL.GetString(StringName.Version)} | Vsync: {VSync} | FPS: {1f/timeCount * ((float)frameCount):0} | {_camera.Location.ToString()}"; // adds miscellaneous information to the title bar of the window
                    fpsOld = (long)(1f / timeCount * ((float)frameCount));
                    Title = $"DeeSynk | Vsync: {VSync} | FPS: {fpsOld} | {Width}x{Height} | {RoundVector(_camera.Location, 2)} | {p.X}, {p.Y} | {t}ms"; // adds miscellaneous information to the title bar of the window
                    timeCount = 0d;
                    frameCount = 0;
                    sw.Reset();
                    sw.Start();
                }
                GL.ClearColor(clearColor);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                _game.Render();

                SwapBuffers();
            }
        }
        
        /// <summary>
        /// The OnUnload method gets called after the Exit() function is called, but before the OpenGL instance is destroyed.
        /// If something needs to happen after the game has been closed, it should go here.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUnload(EventArgs e)
        {
            Console.WriteLine("I listen to you sleep...");
            InputManager.GetInstance().IsInputThreadRunning = false;
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if(_centerMouse)
                OpenTK.Input.Mouse.SetPosition(mousePos.X, mousePos.Y);
        }

        private void CenterMouse(float time, MouseArgs args)
        {
            if (_centerMouse)
            {
                this.Cursor = _cursor;
                var im = InputManager.GetInstance();
                im.SetConfig("unlocked mouse");
                Mouse.SetPosition(mousePos.X, mousePos.Y); //cursor only appears after an update
                var state = Mouse.GetCursorState();
                _game.SystemUI.ScreenCenter = new Vector2(state.X, state.Y);
                im.MouseStateScreen = state;
                im.RawMouseInput = false;
            }
            else
            {
                this.Cursor = MouseCursor.Empty;
                var im = InputManager.GetInstance();
                im.SetConfig("primary move");
                Mouse.SetPosition(mousePos.X, mousePos.Y); //cursor only disappears after an update
                im.MouseStateRaw = Mouse.GetState();
                im.RawMouseInput = true;
            }
            _centerMouse = !_centerMouse;
        }

        /// <summary>
        /// Performs a manual garbage collection with debug output.
        /// </summary>
        private void GCCollectDebug()
        {
            long collectionBefore = System.GC.GetTotalMemory(false);
            long collectionAfter = collectionBefore - System.GC.GetTotalMemory(true);
            long magnitude = (long)(Math.Log((double)collectionAfter, 1000d));
            string ending = "";
            switch (magnitude)
            {
                case (0): ending = "B"; break;
                case (1): ending = "KB"; break;
                case (2): ending = "MB"; break;
                case (3): ending = "GB"; break;
            }
            collectionAfter = collectionAfter / (long)Math.Pow(1000, magnitude);

            Console.WriteLine("Collected garbage: {0} {1}", collectionAfter, ending);
        }

        //Move these (maybe also GCCollect) to a helper class that has single use helper functions
        private Vector3 RoundVector(Vector3 v, int decimals)
        {
            return new Vector3((float)Math.Round(v.X, decimals),
                               (float)Math.Round(v.Y, decimals),
                               (float)Math.Round(v.Z, decimals));
        }

        private Vector4 RoundVector(Vector4 v, int decimals)
        {
            return new Vector4((float)Math.Round(v.X, decimals),
                               (float)Math.Round(v.Y, decimals),
                               (float)Math.Round(v.Z, decimals),
                               (float)Math.Round(v.W, decimals));
        }
    }
}
