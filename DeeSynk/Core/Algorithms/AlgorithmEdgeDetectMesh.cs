using DeeSynk.Core.Components.GraphicsObjects.Lights;
using DeeSynk.Core.Components.Models;
using DeeSynk.Core.Components.Types.Render;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Algorithms
{
    public struct Triangle
    {
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;

        public Vector3 U;
        public Vector3 V;

        public Vector3 n;

        public Triangle(Vector3 _p1, Vector3 _p2, Vector3 _p3)
        {
            p1 = _p1;
            p2 = _p2;
            p3 = _p3;

            U = _p2 - _p1;
            V = _p3 = _p1;

            n = new Vector3(U.Y * V.Z - U.Z * V.Y,
                            U.Z * V.X - U.X * V.Z,
                            U.X * V.Y - U.Y * V.X);
        }
    }

    public struct Edge
    {
        public Vector4 p1;
        public Vector4 p2;

        public Edge(Vector4 _p1, Vector4 _p2)
        {
            p1 = _p1;
            p2 = _p2;
        }
    }

    public class AlgorithmEdgeDetectMesh
    {
        private Model _model;
        private ComponentLight _componentLight;

        private Triangle[] triangles;
        private bool[] trianglesRemoved;

        private Edge[] edges;

        public AlgorithmEdgeDetectMesh(Model model, ComponentLight componentLight)
        {
            _model = model;
            _componentLight = componentLight;
        }

        public Edge[] Start()
        {
            edges = new Edge[_model.ElementCount];
            int count = 0;
            Vector3 lookAtNorm = Vector3.Zero;
            if(_componentLight.LightType == LightType.SPOTLIGHT)
            {
                lookAtNorm = ((SpotLight)_componentLight.LightObject).View.Location - ((SpotLight)_componentLight.LightObject).View.LookAt;
            }else if(_componentLight.LightType == LightType.SUN)
            {
                lookAtNorm = ((SunLamp)_componentLight.LightObject).View.Location - ((SunLamp)_componentLight.LightObject).View.LookAt;
            }

            lookAtNorm.Normalize();

            lookAtNorm *= -1.0f;

            for(int i=0; i<_model.ElementCount; i += 3)
            {
                float p1DotL = Vector3.Dot(lookAtNorm, _model.Normals[_model.Elements[i + 0]]);
                float p2DotL = Vector3.Dot(lookAtNorm, _model.Normals[_model.Elements[i + 1]]);
                float p3DotL = Vector3.Dot(lookAtNorm, _model.Normals[_model.Elements[i + 2]]);

                if (p1DotL >= 0 && p2DotL >= 0 && p3DotL >= 0)
                    continue;
                else
                {
                    if(p1DotL >= 0 && p2DotL >= 0 && p3DotL < 0)
                    {
                        edges[count] = new Edge(_model.Vertices[_model.Elements[i]], _model.Vertices[_model.Elements[i + 1]]);
                        count++;
                    }
                    if (p1DotL >= 0 && p3DotL >= 0 && p2DotL < 0)
                    {
                        edges[count] = new Edge(_model.Vertices[_model.Elements[i]], _model.Vertices[_model.Elements[i + 2]]);
                        count++;
                    }
                    if (p2DotL >= 0 && p3DotL >= 0 && p1DotL < 0)
                    {
                        edges[count] = new Edge(_model.Vertices[_model.Elements[i+1]], _model.Vertices[_model.Elements[i + 2]]);
                        count++;
                    }

                }
            }

            int total = 0;
            for(int i=0; i<edges.Length; i++)
            {
                if (edges[i].p1 != Vector4.Zero && edges[i].p2 != Vector4.Zero)
                    total++;
                else
                    break;

            }

            Edge[] edgeReturn = new Edge[total];
            for(int i=0; i<total; i++)
            {
                edgeReturn[i] = edges[i];
            }

            return edgeReturn;
        }
    }
}
