﻿using DeeSynk.Core.Components.Types.Render;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeeSynk.Core.Managers
{
    using Model = DeeSynk.Core.Components.Models.Model;

    [Flags]
    public enum ConstructionParameterFlags
    {
        NONE = 0,
        VECTOR3_OFFSET = 1
    }

    public class ModelManager : IManager
    {
        private const string FILE_PATH = @"..\..\Resources\Models\";
        private static ModelManager _modelManager;

        private Dictionary<string, Model> _modelLibrary;

        private ModelManager()
        {
            _modelLibrary = new Dictionary<string, Model>();
        }

        public static ref ModelManager GetInstance()
        {
            if (_modelManager == null)
            {
                _modelManager = new ModelManager();
            }

            return ref _modelManager;    
        }

        public void Load()
        {
            string[] models = Directory.GetFiles(FILE_PATH);
            string[] fileNames = models.Select(Path.GetFileNameWithoutExtension).ToArray();
            string[] fileExtensions = models.Select(Path.GetExtension).ToArray();
            for (int idx = 0; idx < models.Length; idx++)
            {
                switch (fileExtensions[idx])
                {
                    case (".obj"):
                        ParseObj(models[idx]);
                        break;
                    case (".ply"):
                        TryParsePly(models[idx]);
                        break;

                }
            }
        }

        public void ParseObj(string filePath)
        {
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using (var StreamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string file = StreamReader.ReadToEnd();

                Vector3[] vertices;

                string rs = @"[v] -?\d\.\d* -?\d.\d* -?\d.\d*";

                Regex regex = new Regex(rs);
                var matches = regex.Matches(file);

                vertices = new Vector3[matches.Count];

                rs = @"-?\d\.\d+(e-?\d+)?";
                regex = new Regex(rs);

                int idx = 0;
                foreach (Match m in matches)
                {
                    string s = m.Value;
                    var components = regex.Matches(s);
                    float x = float.Parse(components[0].ToString());
                    float y = float.Parse(components[1].ToString());
                    float z = float.Parse(components[2].ToString());

                    vertices[idx] = new Vector3(x, y, z);
                    idx++;
                }

                Vector3[] normals;

                rs = @"vn -?\d\.\d* -?\d.\d* -?\d.\d*";

                regex = new Regex(rs);
                matches = regex.Matches(file);

                normals = new Vector3[matches.Count];

                rs = @"-?\d\.\d+(e-?\d+)?";
                regex = new Regex(rs);
                idx = 0;
                foreach (Match m in matches)
                {
                    string s = m.Value;
                    var components = regex.Matches(s);
                    float x = float.Parse(components[0].Value);
                    float y = float.Parse(components[1].Value);
                    float z = float.Parse(components[2].Value);

                    normals[idx] = new Vector3(x, y, z);
                    idx++;
                }

                uint[] vertexIndices, normalIndices;

                rs = @"f \d*//\d* \d*//\d* \d*//\d*";

                regex = new Regex(rs);
                matches = regex.Matches(file);

                vertexIndices = new uint[matches.Count * 3];
                normalIndices = new uint[matches.Count * 3];

                rs = @"\d+";
                regex = new Regex(rs);
                idx = 0;
                foreach (Match m in matches)
                {
                    string s = m.Value;
                    var components = regex.Matches(s);
                    uint v1 = uint.Parse(components[0].Value);
                    uint n1 = uint.Parse(components[1].Value);
                    uint v2 = uint.Parse(components[2].Value);
                    uint n2 = uint.Parse(components[3].Value);
                    uint v3 = uint.Parse(components[4].Value);
                    uint n3 = uint.Parse(components[5].Value);

                    vertexIndices[idx] = v1;
                    normalIndices[idx] = n1;
                    idx++;

                    vertexIndices[idx] = v2;
                    normalIndices[idx] = n2;
                    idx++;

                    vertexIndices[idx] = v3;
                    normalIndices[idx] = n3;
                    idx++;
                }

                _modelLibrary.Add(Path.GetFileNameWithoutExtension(filePath), new Model(ref vertices, ref normals, ref vertexIndices, ref normalIndices));
            }
        }

        public void TryParsePly(string filePath)
        {
            try
            {
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using (var StreamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string file = StreamReader.ReadToEnd();

                    //getting header info (we are disregarding the type properties and assuming that we are reading floats)
                    //vertexCount
                    int vertexElementCount = 0;
                    {
                        string key = @"element vertex \d+";
                        Regex regex = new Regex(key);
                        var matches = regex.Matches(file); //there should be only one match

                        if (matches.Count == 1)
                        {
                            key = @"\d+";
                            regex = new Regex(key);
                            matches = regex.Matches(matches[0].Value);
                            vertexElementCount = int.Parse(matches[0].Value);
                        }
                        else
                            throw new FileLoadException("Invalid .ply file format: duplicate header property - element vertex");
                    }

                    //faceCount
                    int faceElementCount = 0;
                    {
                        string key = @"element face \d+";
                        Regex regex = new Regex(key);
                        var matches = regex.Matches(file);
                        if (matches.Count == 1)
                        {
                            key = @"\d+";
                            regex = new Regex(key);
                            matches = regex.Matches(matches[0].Value);
                            faceElementCount = int.Parse(matches[0].Value);
                        }
                        else
                            throw new FileLoadException("Invalid .ply file format: duplicate header property - element face");
                    }


                    //moving on to the data section
                    string endHeaderKeyword = "end_header";
                    int index = file.IndexOf(endHeaderKeyword);
                    string data = file.Substring(index + endHeaderKeyword.Length);

                    string lastVertexElement = "";

                    Vector3[] vertices = new Vector3[vertexElementCount];
                    {
                        string key = @"-?\d\.\d+(e-?\d+)?";
                        Regex regex = new Regex(key);
                        var matches = regex.Matches(data);

                        if (matches.Count == vertexElementCount * 3)
                        {
                            for(int idx = 0; idx<vertexElementCount; idx++)
                            {
                                float scale = 100f;
                                float x = float.Parse(matches[idx * 3].Value);
                                float y = float.Parse(matches[idx * 3 + 1].Value);
                                float z = float.Parse(matches[idx * 3 + 2].Value);

                                vertices[idx] = new Vector3(scale * x, scale * y, scale * z);
                            }

                            lastVertexElement = matches[matches.Count - 1].Value;
                        }
                        else
                            throw new FileLoadException("Invalid .ply file format: incomplete or incorrectly formatted vertex data");
                    }

                    int lastElementIndex = data.LastIndexOf(lastVertexElement);
                    data = data.Substring(lastElementIndex + lastVertexElement.Length);

                    uint[] faceIndices = new uint[faceElementCount * 3];
                    {
                        string key = @"\d+";
                        Regex regex = new Regex(key);
                        var matches = regex.Matches(data);

                        if(matches.Count == faceElementCount * 4)
                        {
                            for(int idx = 0; idx < faceElementCount; idx++)
                            {
                                faceIndices[idx * 3] = uint.Parse(matches[idx * 4 + 1].Value);
                                faceIndices[idx * 3 + 1] = uint.Parse(matches[idx * 4 + 2].Value);
                                faceIndices[idx * 3 + 2] = uint.Parse(matches[idx * 4 + 3].Value);
                            }
                        }
                    }

                    _modelLibrary.Add(Path.GetFileNameWithoutExtension(filePath), new Model(vertices, faceIndices));
                    Console.WriteLine("Loaded model {0}: {1} vertices, {2} faces", Path.GetFileNameWithoutExtension(filePath), vertexElementCount, faceElementCount);
                }
            }
            catch(Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }
        }

        public Model GetModel(string name)
        {
            Model value;
            _modelLibrary.TryGetValue(name, out value);
            return value;
        }

        public void UnLoad()
        {
            throw new NotImplementedException();
        }
    }
}
