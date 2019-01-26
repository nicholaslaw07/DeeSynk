﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace DeeSynk
{
    /// <summary>
    /// The MainWindow class contains all the highest level operations going on in the game. All updates,
    /// drawing, and initialization should occur somewhere in this class.
    /// </summary>
    public sealed class MainWindow : GameWindow
    {

        private KeyboardState keyState;    // holds current keyboard state, updated every frame
        private Color4 clearColor = Color4.White;     // the color that OpenGL uses to clear the color buffer on each frame

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
                                    0,                          // OpenGL minor version
                                    GraphicsContextFlags.ForwardCompatible)
        {
            Title += " | The WIP Student Video Game | OpenGL Version: " + GL.GetString(StringName.Version);
        }
        
        /// <summary>
        /// OnResize is called if the user decides to resize the window. It will reset the viewport
        /// so that OpenGL knows the new size of the window.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
        }

        /// <summary>
        /// The OnLoad method gets called as soon as the window is created. Anything that needs to be
        /// created or initialized at the start of the game should go here.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            CursorVisible = true;
        }

        /// <summary>
        /// The OnUpdateFrame method gets called on every frame. Anything that needs to be updated or
        /// changed on every frame should go here. Drawing does NOT go here.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            HandleKeyboard();
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

            SwapBuffers();
        }
        
        /// <summary>
        /// The OnUnload method gets called after the Exit() function is called, but before the OpenGL instance is destroyed.
        /// If something needs to happen after the game has been closed, it should go here.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUnload(EventArgs e)
        {
            Console.WriteLine('Goodbye!');
        }

        /// <summary>
        /// The HandleKeyboard method listens for any keyboard inputs from the user. Anything dealing
        /// with keybindings should go here.
        /// </summary>
        private void HandleKeyboard()
        {
            keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Key.Escape))
                Exit();
        }
    }
}
