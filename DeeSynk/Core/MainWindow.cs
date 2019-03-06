using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private Color4 clearColor = Color4.White;     // the color that OpenGL uses to clear the color buffer on each frame
        private Matrix4 viewmat4;
        private Matrix4 viewmat4_o;
        private Vector3 offsetLocation;
        private Vector3 offsetLocationN;
        private Matrix4 lam4;

        private float rotY;
        private Matrix4 rotYMat4;

        public static Matrix4 ORTHO_MATRIX = Matrix4.Identity;

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
        }
        
        /// <summary>
        /// OnResize is called if the user decides to resize the window. It will reset the viewport
        /// so that OpenGL knows the new size of the window.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            Matrix4.CreateOrthographic((float)Width, (float)Height, -1f, 1f, out ORTHO_MATRIX);
        }

        /// <summary>
        /// The OnLoad method gets called as soon as the window is created. Anything that needs to be
        /// created or initialized at the start of the game should go here.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            _game = new Game();
            Matrix4.CreateOrthographic((float)Width, (float)Height, -1f, 1f, out ORTHO_MATRIX);
            CursorVisible = true;

            Console.WriteLine(GL.GetString(StringName.Renderer));
            _game.LoadGameData();

            lam4 = Matrix4.LookAt(offsetLocation, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, 1.0f, 0.0f));

            rotYMat4 = Matrix4.Identity;

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
            _game.Update((float)(e.Time));
            HandleKeyboard((float)e.Time);
            offsetLocationN = offsetLocation * -1f;
            Vector4 val = new Vector4(offsetLocation);
            val = rotYMat4 * val;
            var val2 = val.Xyz;
            var val3 = val2 * -1f;
            
            //Matrix4.LookAt()
            Matrix4.CreateTranslation(ref val2, out viewmat4);
            Matrix4.CreateTranslation(ref val3, out viewmat4_o);
            Matrix4.CreateRotationY(rotY, out rotYMat4);

            var la = new Vector4(0.0f, 0.0f, -1.0f, 1.0f);
            la = rotYMat4 * la;
            var la2 = la.Xyz;

            lam4 = Matrix4.LookAt(offsetLocation, la2 + offsetLocation, new Vector3(0.0f, 1.0f, 0.0f));
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
            _game.Render(ref lam4);

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
                offsetLocation += (rotYMat4 * new Vector4(0.0f, 0.0f, -time * 100f, 1.0f)).Xyz;
            if (keyState.IsKeyDown(Key.S))
                offsetLocation += (rotYMat4 * new Vector4(0.0f, 0.0f, time * 100f, 1.0f)).Xyz;
            if (keyState.IsKeyDown(Key.A))
                offsetLocation += (rotYMat4 * new Vector4(-time * 100f, 0.0f, 0.0f, 1.0f)).Xyz;
            if (keyState.IsKeyDown(Key.D))
                offsetLocation += (rotYMat4 * new Vector4(time * 100f, 0.0f, 0.0f, 1.0f)).Xyz;
            if (keyState.IsKeyDown(Key.Space))
                offsetLocation += new Vector3(0.0f, -time * 100f, 0.0f);
            if (keyState.IsKeyDown(Key.ShiftLeft))
                offsetLocation += new Vector3(0.0f, time * 100f, 0.0f);
            if (keyState.IsKeyDown(Key.Q))
                rotY -= time * 0.5f;
            if (keyState.IsKeyDown(Key.E))
                rotY += time * 0.5f;


        }
    }
}
