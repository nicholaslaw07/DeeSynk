using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using DeeSynk.Core.Components;
using DeeSynk.Core.Components.Input;
using DeeSynk.Core.Managers;
using OpenTK;
using OpenTK.Graphics;
using System.Windows.Media;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.IO;

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

        Vector2i center;
        Vector2i mousePos;
        private bool _centerMouse;

        private long fpsOld = 0;
        private long frameCount = 0;
        private double timeCount = 0;
        private Stopwatch sw, sw2;

        public static int width =  1920;
        public static int height = 1080;

        private Stopwatch loadTimer, timeTracker;

        private MouseCursor _cursor;

        /// <summary>
        /// Basic constructor for the game window. The base keyword allows parameters to be
        /// passed to the parent class constructor. The title of the window is then set with
        /// additional information, including the current OpenGL version.
        /// </summary>
        public MainWindow() : base(new GameWindowSettings(), new NativeWindowSettings())
        {
            loadTimer = new Stopwatch();
            timeTracker = new Stopwatch();
            loadTimer.Start();
            Title += " | The WIP Student Video Game | OpenGL Version: " + GL.GetString(StringName.Version);

            this.Size = new Vector2i(width, height);

            VSync = VSyncMode.Off;
            center = new Vector2i(width / 2, height/ 2);
            mousePos = PointToScreen(center);
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
        protected override void OnResize(ResizeEventArgs e)
        {
            width = e.Width;
            height = e.Height;

            GL.Viewport(0, 0, width, height);
            _camera.Width = width;
            _camera.Height = height;

        }

        /// <summary>
        /// The OnLoad method gets called as soon as the window is created. Anything that needs to be
        /// created or initialized at the start of the game should go here.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad()
        {
            DebugLoadStart("InputManager");
            Managers.InputManager.GetInstance().Load();
            DebugLoadFinish();

            DebugLoadStart("FontManager");
            Managers.FontManager.GetInstance().Load();
            DebugLoadFinish();

            DebugLoadStart("ModelManager");
            Managers.ModelManager.GetInstance().Load();
            DebugLoadFinish();

            DebugLoadStart("ShaderManager");
            Managers.ShaderManager.GetInstance().Load();
            DebugLoadFinish();

            DebugLoadStart("TextureManager");
            Managers.TextureManager.GetInstance().Load();
            DebugLoadFinish();

            SetGLState();

            //_camera = new Camera(1.0f, (float)Width, (float)Height, 0.001f, 300f);
            _camera = new Camera(1.0f, (float)width, (float)height, 0.001f, 100f);
            _camera.OverrideLookAtVector = true;
            _camera.Location = new Vector3(0.0f, 0.25f, 1.0f);
            _camera.UpdateMatrices();

            _game = new Game(ref _camera);

            //CursorVisible = true;
            //_centerMouse = true;
            this.CursorGrabbed = true;
            this.Cursor = MouseCursor.Empty;
            Action<float, MouseArgs> c = CenterMouse;
            var im = InputManager.GetInstance();

            {
                var ic = new List<InputAssignment>();
                ic.Add(new InputAssignment(InputType.Keyboard, Keys.C));
                var dl = new List<Action<float, MouseArgs>>();
                dl.Add(c);
                {
                    im.Configurations.TryGetValue("primary move", out InputConfiguration config);
                    var ia = new Components.Input.InputAction(Qualifiers.IGNORE_AFTER | Qualifiers.IGNORE_BEFORE, ic);
                    ia.UpActions = dl;
                    config.InputActions.AddLast(ia);
                    im.SetConfig("primary move");
                }
                {
                    im.Configurations.TryGetValue("unlocked mouse", out InputConfiguration config1);
                    var ia = new Components.Input.InputAction(Qualifiers.IGNORE_AFTER | Qualifiers.IGNORE_BEFORE, ic);
                    ia.UpActions = dl;
                    config1.InputActions.AddLast(ia);
                }
            }
            im.StartThreads(1);

            Debug.WriteLine(TimeText() + GL.GetString(StringName.Renderer));
            _game.LoadGameData();

            _game.PushCameraRef(ref _camera);

            GCCollectDebug();

            loadTimer.Stop();
            Debug.WriteLine(TimeText() + "Initial program data loaded");

            /*
            {
                GlyphTypeface g = new GlyphTypeface(new Uri(@"C:\Users\Nicholas\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Fonts\OfficeCodePro-Light\OfficeCodePro-Light.otf"));
                if (g.CharacterToGlyphMap.TryGetValue(54, out ushort v))
                {
                    var geo = g.GetGlyphOutline(v, 1.0d, 1.0d);
                    var pGeo = geo.GetFlattenedPathGeometry();
                    //Debug.WriteLine(pGeo.Figures);
                    Debug.WriteLine(geo.GetArea());
                    Debug.WriteLine(geo.ToString());
                }
            }
            */

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
                    OnUnload();

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
                    var t = InputManager.GetInstance().AverageTime / ((float)Stopwatch.Frequency) * 1000;
                    Title = $"DeeSynk | Vsync: {VSync} | FPS: {fpsOld} | {width}x{height} | {RoundVector(_camera.Location, 2)} | {MousePosition.X}, {MousePosition.Y} | {t}ms";
                }

                if (sw.ElapsedMilliseconds > 1000 / 120)
                {
                    var t = InputManager.GetInstance().AverageTime / ((float)Stopwatch.Frequency) * 1000;
                    sw.Stop();
                    //Title = $"DeeSynk | The WIP Student Video Game | OpenGL Version: {GL.GetString(StringName.Version)} | Vsync: {VSync} | FPS: {1f/timeCount * ((float)frameCount):0} | {_camera.Location.ToString()}"; // adds miscellaneous information to the title bar of the window
                    fpsOld = (long)(1f / timeCount * ((float)frameCount));
                    Title = $"DeeSynk | Vsync: {VSync} | FPS: {fpsOld} | {width}x{height} | {RoundVector(_camera.Location, 2)} | {MousePosition.X}, {MousePosition.Y} | {t}ms"; // adds miscellaneous information to the title bar of the window
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
        protected override void OnUnload()
        {
            Debug.WriteLine("I listen to you sleep...");
            InputManager.GetInstance().IsInputThreadRunning = false;
            DestroyWindow();
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (_centerMouse)
                this.CursorGrabbed = true;
            else
                this.CursorGrabbed = false;
        }

        private void CenterMouse(float time, MouseArgs args)
        {
            if (_centerMouse)
            {
                CursorVisible = true;
                Cursor = _cursor;
                var im = InputManager.GetInstance();
                im.SetConfig("unlocked mouse");
                MousePosition = new Vector2(mousePos.X, mousePos.Y); //cursor only appears after an update
                _game.SystemUI.ScreenCenter = new Vector2(Cursor.X, Cursor.Y);
            }
            else
            {
                CursorVisible = false;
                Cursor = MouseCursor.Empty;
                var im = InputManager.GetInstance();
                im.SetConfig("primary move");
                MousePosition = new Vector2(mousePos.X, mousePos.Y); //cursor only disappears after an update
            }
            _centerMouse = !_centerMouse;
            MousePosition = new Vector2(mousePos.X, mousePos.Y);
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

        private void StartTimeTracking(Stopwatch sw)
        {
            sw.Start();
        }

        private void StartTime()
        {
            StartTimeTracking(timeTracker);
        }

        private float GetTimeDelta(Stopwatch sw, bool pause, bool reset)
        {
            float time = sw.ElapsedTicks / ((float)Stopwatch.Frequency);
            if (pause)
                sw.Stop();
            if (reset)
                sw.Reset();

            return time;
        }

        private float TimeDelta()
        {
            return GetTimeDelta(timeTracker, true, true);
        }

        private void DebugLoadStart(string name)
        {
            Debug.Write($"[{GetTimeDelta(loadTimer, false, false)}]: Loading {name}... "); StartTime();
        }

        private void DebugLoadFinish()
        {
            Debug.Write($"Loaded in {TimeDelta()} seconds\n");
        }

        private string TimeText()
        {
            return $"[{ GetTimeDelta(loadTimer, false, false)}]: ";
        }
    }
}
