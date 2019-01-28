using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Components.Managers
{
    public class ShaderManager
    {

        //  "SHADERNAME"  "TXT"

        //OR

        //  "SHADERNAME" + "VERT" OR "FRAG"

        private string _vertPath = @"..\..\Resources\Shaders\Vertex";
        private string _fragPath = @"..\..\Resources\Shaders\Fragment";

        private Dictionary<string, int> _shaders;
        
        public ShaderManager()
        {
            _shaders = new Dictionary<string, int>();

            CreatePrograms();
        }
        
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

        private void CreatePrograms()
        {
            string[] vertexShaders = Directory.GetFiles(_vertPath);
            string[] fragmentShaders = Directory.GetFiles(_fragPath);

            string[] fileNames = Directory.GetFiles(_vertPath)
                                     .Select(Path.GetFileNameWithoutExtension)
                                     .ToArray();

            for(int i=0; i < vertexShaders.Length; i++)
            {
                var Program = GL.CreateProgram();
                var Shaders = new List<int>();
                Shaders.Add(CompileShader(ShaderType.VertexShader, vertexShaders[i]));
                Shaders.Add(CompileShader(ShaderType.FragmentShader, fragmentShaders[i]));

                foreach (var shader in Shaders)
                    GL.AttachShader(Program, shader);

                GL.LinkProgram(Program);

                foreach(var shader in Shaders)
                {
                    GL.DetachShader(Program, shader);
                    GL.DeleteShader(shader);
                }

                _shaders.Add(fileNames[i], Program);
            }
        }

        public int GetProgram(string name)
        {
            int programOut = -1;  //if -1 is returned, querying method will handle error
            _shaders.TryGetValue(name, out programOut);

            return programOut;
        }

        /*CREATE OBJECT

        LOAD SHADERS TO STRING

        LOOP (CREATE PROGRAM)
             COMPILE VERT, FRAG
             ADD TO PROGRAM
             RETURN INTEGER ID OF PROGRAM
        */

    }
}
