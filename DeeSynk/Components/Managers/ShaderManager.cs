﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Components.Managers
{
    /// <summary>
    /// Pulls the shaders from the resources folder, compiles them, and links them with a set of programs corresponding
    /// to each set of shaders. These are stored in a dictionary, whose keys are the filenames of the shaders, and whose 
    /// values are the GL generated integers for each program.
    /// </summary>
    public class ShaderManager
    {
        private static ShaderManager _shaderManager;            //--DIF--//

        private string _vertPath = @"..\..\Resources\Shaders\Vertex";
        private string _fragPath = @"..\..\Resources\Shaders\Fragment";

        private Dictionary<string, int> _programs;

        /// <summary>
        /// Instantiates the program dictionary and begins the chain of events to create the programs and store them in said dictionary.
        /// </summary>
        private ShaderManager()     //--DIF--//
        {
            _programs = new Dictionary<string, int>();
        }

        public static ref ShaderManager GetInstance()
        {
            if(_shaderManager == null)
                _shaderManager = new ShaderManager();

            return ref _shaderManager;
        }

        public void Load()  //This should be a generic method in the interface
        {
            CreatePrograms();
        }

        /// <summary>
        /// Takes the shader source code, and GL compiles into a shader corresponding to that shader's type. The shader is bound to the
        /// GL context, and is referenced in the integer return value generated by GL.
        /// </summary>
        private int CompileShader(ShaderType type, string source)
        {
            var shader = GL.CreateShader(type);

            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            var info = GL.GetShaderInfoLog(shader);

            if (!string.IsNullOrWhiteSpace(info))
                Console.WriteLine($"GL.CompileShader [{type}] had info log: {info}");

            return shader;
        }

        /// <summary>
        /// Retrieves source code for all shaders and iteratively creates a new program corresponding to each shader type,
        /// which is then thrown in a dictionary _shaders.
        /// </summary>
        private void CreatePrograms()
        {
            string[] vertexShaders = Directory.GetFiles(_vertPath);
            string[] fragmentShaders = Directory.GetFiles(_fragPath);

            string[] fileNames = Directory.GetFiles(_vertPath)
                                     .Select(Path.GetFileNameWithoutExtension)
                                     .ToArray();

            for(int i=0; i < vertexShaders.Length; i++)
            {
                var Program = GL.CreateProgram();                                               // creates a new program id in the GL context
                var Shaders = new List<int>();
                Shaders.Add(CompileShader(ShaderType.VertexShader, vertexShaders[i]));
                Shaders.Add(CompileShader(ShaderType.FragmentShader, fragmentShaders[i]));

                foreach (var shader in Shaders)
                    GL.AttachShader(Program, shader);                                           // attaches each type of shader to the generated program

                GL.LinkProgram(Program);                                                        // links the created program to the GL context, does not give this program focus

                foreach(var shader in Shaders)
                {
                    GL.DetachShader(Program, shader);                                           // after linking the program, you can detach and delete the shaders you used to
                    GL.DeleteShader(shader);                                                    // create the program that you just linked
                }

                _programs.Add(fileNames[i], Program);                                            // adds the program created to the shaders dictionary
            }
        }
        
        /// <summary>
        /// Retrieves the program corresponding to the filename of the associated shaders.
        /// </summary>
        /// <param name="name">shader/filename</param>
        /// <returns>program ID</returns>
        public int GetProgram(string name)  //Add ability to get multiple shaders
        {
            int programOut = -1;  //if -1 is returned, querying method will handle error
            _programs.TryGetValue(name, out programOut);  //Add error console output?

            return programOut;
        }


        /// <summary>
        /// Used during the unloading phase of the program.  Runs through the list of programID's and deletes each one from the GL context.
        /// </summary>
        public void Unload()  //This should be a generic method in the interface
        {
            foreach(string key in _programs.Keys)
            {
                int programID = -1;
                if(_programs.TryGetValue(key, out programID))
                {
                    GL.DeleteProgram(programID);
                    //Add a verification statement that waits until the shader is infact deleted.  Double check to see if shaders need to be unlinked before deletion and then return a bool.  Add a deconstructor/finalizer?
                }
            }
        }
    }
}
