using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.Transform
{
    public enum InterpolationMode : byte
    {
        NONE = 0,
        LINEAR = 1
    }

    public enum RotationAxes : byte
    {
        X = 1,
        Y = 2,
        Z = 3,
    }

    public class ComponentTransform : IComponent
    {
        public int BitMaskID => (int)Component.TRANSFORM;

        private int _transformComponentsMask;
        public int TransformComponentsMask { get => _transformComponentsMask; }

        private ComponentLocation _location;
        private ComponentRotation _rotationX,
                                  _rotationY,
                                  _rotationZ;
        private ComponentScale    _scale;

        public ref ComponentLocation LocationComp  { get => ref _location; }
        public ref ComponentRotation RotationXComp { get => ref _rotationX; }
        public ref ComponentRotation RotationYComp { get => ref _rotationY; }
        public ref ComponentRotation RotationZComp { get => ref _rotationZ; }
        public ref ComponentScale    ScaleComp     { get => ref _scale; }

        private Matrix4 _modelView;

        public ref Matrix4 GetModelView { get => ref _modelView; }

        public ComponentTransform(int transformComponentsMask, 
                                  float locX = 0f, float locY = 0f, float locZ = 0f, 
                                  float rotX = 0f, float rotY = 0f, float rotZ = 0f, 
                                  float sclX = 1f, float sclY = 1f, float sclZ = 1f)
        {
            _transformComponentsMask = transformComponentsMask;

            if ((_transformComponentsMask & (int)Component.LOCATION) != 0)
            {
                bool doOrigin = false;
                if ((_transformComponentsMask & ((int)Component.ROTATION_XYZ)) != 0)
                    doOrigin = true;

                _location = new ComponentLocation(doOrigin, locX, locY, locZ);
            }

            if ((_transformComponentsMask & (int)Component.ROTATION_X) != 0)
                _rotationX = new ComponentRotation(RotationAxes.X, rotX);

            if ((_transformComponentsMask & (int)Component.ROTATION_Y) != 0)
                _rotationX = new ComponentRotation(RotationAxes.Y, rotY);

            if ((_transformComponentsMask & (int)Component.ROTATION_Z) != 0)
                _rotationX = new ComponentRotation(RotationAxes.Z, rotZ);

            if ((_transformComponentsMask & (int)Component.SCALE) != 0)
                _scale = new ComponentScale(sclX, sclY, sclZ);

            _modelView = Matrix4.Identity;
        }

        public void ComputeModelViewProduct()
        {
            if((_transformComponentsMask & ((int)Component.ROTATION_XYZ)) != 0)
            {
                Matrix4 rotMats = Matrix4.Identity;
                if ((_transformComponentsMask & (int)Component.ROTATION_X) != 0)
                    Matrix4.Mult(ref rotMats, ref _rotationX.Matrix, out rotMats);
                if ((_transformComponentsMask & (int)Component.ROTATION_Y) != 0)
                    Matrix4.Mult(ref rotMats, ref _rotationY.Matrix, out rotMats);
                if ((_transformComponentsMask & (int)Component.ROTATION_Z) != 0)
                    Matrix4.Mult(ref rotMats, ref _rotationZ.Matrix, out rotMats);

                if (((_transformComponentsMask & (int)Component.LOCATION) != 0) && ((_transformComponentsMask & (int)Component.SCALE) != 0))
                {
                    Matrix4.Mult(ref rotMats, ref _location.Matrix, out _modelView);
                    Matrix4.Mult(ref _scale.Matrix, ref _modelView, out _modelView);
                    Matrix4.Mult(ref _location.Matrix_Origin, ref _modelView, out _modelView);

                    //TEST START 
                    Matrix4 l = _location.Matrix;
                    Matrix4 s = _scale.Matrix;
                    Matrix4 lo = _location.Matrix_Origin;

                    var m = lo * s * rotMats * l;
                    //TEST END
                }
                else if ((_transformComponentsMask & (int)Component.LOCATION) != 0)
                {
                    Matrix4.Mult(ref rotMats, ref _location.Matrix, out _modelView);
                    Matrix4.Mult(ref _location.Matrix_Origin, ref _modelView, out _modelView);
                }
                else if ((_transformComponentsMask & (int)Component.SCALE) != 0)
                {
                    Matrix4.Mult(ref rotMats, ref _location.Matrix, out _modelView);
                    Matrix4.Mult(ref _scale.Matrix, ref _modelView, out _modelView);
                }
                else
                    _modelView = rotMats;
            }
            else
            {
                if (((_transformComponentsMask & (int)Component.LOCATION) != 0) && ((_transformComponentsMask & (int)Component.SCALE) != 0))
                    Matrix4.Mult(ref _scale.Matrix, ref _location.Matrix, out _modelView);
                else if ((_transformComponentsMask & (int)Component.LOCATION) != 0)
                    _modelView = _location.Matrix;
                else if ((_transformComponentsMask & (int)Component.SCALE) != 0)
                    _modelView = _scale.Matrix;
                else
                    _modelView = Matrix4.Identity;
            }
        }

        public void Update(float time)
        {
            bool recomputeProduct = false;
            if ((_transformComponentsMask & (int)Component.LOCATION) != 0)
            {
                _location.Update(time);
                recomputeProduct |= _location.ValueUpdated;
            }

            if ((_transformComponentsMask & (int)Component.ROTATION_X) != 0)
            {
                _rotationX.Update(time);
                recomputeProduct |= _rotationX.ValueUpdated;
            }

            if ((_transformComponentsMask & (int)Component.ROTATION_Y) != 0)
            {
                _rotationY.Update(time);
                recomputeProduct |= _rotationY.ValueUpdated;
            }

            if ((_transformComponentsMask & (int)Component.ROTATION_Z) != 0)
            {
                _rotationZ.Update(time);
                recomputeProduct |= _rotationZ.ValueUpdated;
            }

            if ((_transformComponentsMask & (int)Component.SCALE) != 0)
            {
                _scale.Update(time);
                recomputeProduct |= _scale.ValueUpdated;
            }

            if (recomputeProduct)
                ComputeModelViewProduct();
        }

        public class ComponentLocation
        {
            private bool _isUpdateAllowed;
            public bool IsUpdateAllowed { get => _isUpdateAllowed; set => _isUpdateAllowed = value; }
            private bool _valueUpdated;
            public bool ValueUpdated { get => _valueUpdated; }

            private Vector3 _location,
                            _velocity,
                            _acceleration;

            public Vector3 Location
            {
                get => _location;
                set
                {
                    if (_isUpdateAllowed && value != Vector3.Zero)
                    {
                        if (_isInterpolating)
                        {
                            _dX = Vector3.Zero;
                            _dT = 0f;
                            _isInterpolating = false;
                        }
                        _location = new Vector3(value);
                        _valueUpdated = true;
                    }
                }
            }
            public Vector3 Velocity
            {
                get => _velocity;
                set
                {
                    if (_isUpdateAllowed && value != Vector3.Zero)
                    {
                        if (_isInterpolating)
                        {
                            _dX = Vector3.Zero;
                            _dT = 0f;
                            _isInterpolating = false;
                        }
                        _velocity = new Vector3(value);
                        _valueUpdated = true;
                    }
                }
            }
            public Vector3 Acceleration
            {
                get => _acceleration;
                set
                {
                    if (_isUpdateAllowed && value != Vector3.Zero)
                    {
                        if (_isInterpolating)
                        {
                            _dX = Vector3.Zero;
                            _dT = 0f;
                            _isInterpolating = false;
                        }
                        _acceleration = new Vector3(value);
                        _valueUpdated = true;
                    }
                }
            }

            private InterpolationMode _mode;

            private bool _isInterpolating;
            public bool IsInterpolating { get => _isInterpolating; }
            private Vector3 _dX;
            private float   _dT;

            private bool _constructOriginTranslation;

            public bool IsMatrixIdentity
            {
                get
                {
                    if (_location != Vector3.Zero)
                        return true;
                    return false;
                }
            }

            private Matrix4 _translateMat4;
            private Matrix4 _translateMat4_Origin;

            public ref Matrix4 Matrix { get => ref _translateMat4; }
            public ref Matrix4 Matrix_Origin { get => ref _translateMat4_Origin; }

            public ComponentLocation(bool constructOriginTranslation)
            {
                _isUpdateAllowed = false;
                _valueUpdated = false;

                _location = Vector3.Zero;
                _velocity = Vector3.Zero;
                _acceleration = Vector3.Zero;

                _mode = InterpolationMode.NONE;

                _isInterpolating = false;
                _dX = Vector3.Zero;
                _dT = 0f;

                _constructOriginTranslation = constructOriginTranslation;

                _translateMat4 = Matrix4.Identity;
                _translateMat4_Origin = Matrix4.Identity;
            }
            public ComponentLocation(bool constructOriginTranslation, float x, float y, float z)
            {
                _isUpdateAllowed = false;
                _valueUpdated = false;

                _location = new Vector3(x, y, z);
                _velocity = Vector3.Zero;
                _acceleration = Vector3.Zero;

                _mode = InterpolationMode.NONE;

                _isInterpolating = false;
                _dX = Vector3.Zero;
                _dT = 0f;

                _constructOriginTranslation = constructOriginTranslation;

                BuildMatrix();
            }

            private void BuildMatrix()
            {
                Matrix4.CreateTranslation(ref _location, out _translateMat4);
                if (_constructOriginTranslation)
                    Matrix4.CreateTranslation(-_location.X, -_location.Y, -_location.Z, out _translateMat4_Origin);
            }

            public void InterpolateTranslation(Vector3 deltaLocation, float interpolationTime, InterpolationMode mode)
            {
                if (_isUpdateAllowed)
                {
                    if (deltaLocation != Vector3.Zero && interpolationTime != 0f)
                    {
                        _mode = mode;

                        _velocity = Vector3.Zero;
                        _acceleration = Vector3.Zero;

                        _dX = deltaLocation;
                        _dT = interpolationTime;
                        //VELOCITY = DX/DT
                        _isInterpolating = true;
                    }
                    else
                    {
                        _dX = Vector3.Zero;
                        _dT = 0f;
                        _isInterpolating = false;
                        _mode = InterpolationMode.NONE;
                    }
                }
            }

            public void Update(float time)
            {
                if (_isInterpolating)  //future add switch statement for interp types
                {
                    if (time < _dT)
                    {
                        Vector3 locStep = _dX * (time / _dT);
                        _location += locStep;
                        _dT -= time;
                        _dX -= locStep;
                    }
                    else if (time >= _dT)
                    {
                        _location += _dX;
                        _dX = Vector3.Zero;
                        _dT = 0f;
                        _isInterpolating = false;
                    }
                }

                if (_valueUpdated)
                    BuildMatrix();
            }
        }

        public class ComponentRotation
        {
            private const float TAO = 6.283185f;

            private bool _isUpdateAllowed;
            public bool IsUpdateAllowed { get => _isUpdateAllowed; set => _isUpdateAllowed = value; }
            private bool _valueUpdated;
            public bool ValueUpdated { get => _valueUpdated; }

            private float _angle,
                          _velocity,
                          _acceleration;

            private RotationAxes _rotationAxis;
            public  RotationAxes RotationAxis { get => _rotationAxis; }

            public float Angle
            {
                get => _angle;
                set
                {
                    if (_isUpdateAllowed && value != 0f)
                    {
                        if (_isInterpolating)
                        {
                            _dR = 0f;
                            _dT = 0f;
                            _isInterpolating = false;
                        }
                        _angle = value;
                        _valueUpdated = true;
                    }
                }
            }
            public float Velocity
            {
                get => _velocity;
                set
                {
                    if (_isUpdateAllowed && value != 0f)
                    {
                        if (_isInterpolating)
                        {
                            _dR = 0f;
                            _dT = 0f;
                            _isInterpolating = false;
                        }
                        _velocity = value;
                        _valueUpdated = true;
                    }
                }
            }
            public float Acceleration
            {
                get => _acceleration;
                set
                {
                    if (_isUpdateAllowed && value != 0f)
                    {
                        if (_isInterpolating)
                        {
                            _dR = 0f;
                            _dT = 0f;
                            _isInterpolating = false;
                        }
                        _acceleration = value;
                        _valueUpdated = true;
                    }
                }
            }
                
            private InterpolationMode _mode;

            private bool _isInterpolating;
            public bool IsInterpolating { get => _isInterpolating; }
            private float _dR,
                          _dT;

            public bool IsMatrixIdentity
            {
                get
                {
                    if (_angle != 0f)
                        return true;
                    return false;
                }
            }

            private Matrix4 _rotMat4;
            public ref Matrix4 Matrix { get => ref _rotMat4; }

            public ComponentRotation(RotationAxes axis)
            {
                _isUpdateAllowed = true;
                _valueUpdated = false;

                _angle = 0f;
                _velocity = 0f;
                _acceleration = 0f;

                _rotationAxis = axis;

                _mode = InterpolationMode.NONE;
                _isInterpolating = false;
                _dR = 0f;
                _dT = 0f;

                _rotMat4 = Matrix4.Identity;
            }
            public ComponentRotation(RotationAxes axis, float angle)
            {
                _isUpdateAllowed = true;
                _valueUpdated = true;

                _angle = angle;
                _velocity = 0f;
                _acceleration = 0f;

                _rotationAxis = axis;

                _mode = InterpolationMode.NONE;
                _isInterpolating = false;
                _dR = 0f;
                _dT = 0f;

                BuildMatrix();
            }

            private void BuildMatrix()
            {
                switch (_rotationAxis)
                {
                    case RotationAxes.X:
                        Matrix4.CreateRotationX(_angle, out _rotMat4);
                        break;
                    case RotationAxes.Y:
                        Matrix4.CreateRotationY(_angle, out _rotMat4);
                        break;
                    case RotationAxes.Z:
                        Matrix4.CreateRotationZ(_angle, out _rotMat4);
                        break;
                    default:
                        _rotMat4 = Matrix4.Identity;
                        break;
                }
            }

            public void InterpolateRotation(float deltaAngle, float interpolationTime, InterpolationMode mode)
            {
                if (_isUpdateAllowed)
                {
                    if (deltaAngle != 0f && interpolationTime != 0f)
                    {
                        _mode = mode;

                        _velocity = 0f;
                        _acceleration = 0f;

                        _dR = deltaAngle;
                        _dT = interpolationTime;
                        //VELOCITY = DR/DT
                        _isInterpolating = true;
                    }
                    else
                    {
                        _dR = 0f;
                        _dT = 0f;
                        _isInterpolating = false;
                        _mode = InterpolationMode.NONE;
                    }
                }
            }

            public void Update(float time)
            {
                if (_isInterpolating)  //future add switch statement for interp types
                {
                    if(time < _dT)
                    {
                        float rotStep = _dR * (time / _dT);
                        _angle += rotStep;
                        _dT -= time;
                        _dR -= rotStep;
                    }
                    else if(time >= _dT)
                    {
                        _angle += _dR;
                        _dR = 0f;
                        _dT = 0f;
                        _isInterpolating = false;
                    }
                }

                if (_valueUpdated)
                {
                    if (_angle <= -TAO || _angle >= TAO)
                        _angle %= TAO;

                    if (_velocity <= -TAO || _velocity >= TAO)
                        _velocity %= TAO;

                    if (_acceleration <= -TAO || _acceleration >= TAO)
                        _acceleration %= TAO;

                    BuildMatrix();
                }
            }
        }

        public class ComponentScale
        {
            private bool _isUpdateAllowed;
            public bool IsUpdateAllowed { get => _isUpdateAllowed; set => _isUpdateAllowed = value; }
            private bool _valueUpdated;
            public bool ValueUpdated { get => _valueUpdated; }

            private Vector3 _scale;

            public Vector3 Scale
            {
                get => _scale;
                set
                {
                    if (_isUpdateAllowed && value != Vector3.Zero)
                    {
                        if (_isInterpolating)
                        {
                            _dS = Vector3.Zero;
                            _dT = 0f;
                            _isInterpolating = false;
                        }
                        _scale = new Vector3(value);
                        _valueUpdated = true;
                    }
                }
            }

            private InterpolationMode _mode;

            private bool _isInterpolating;
            public bool IsInterpolating { get => _isInterpolating; }
            private Vector3 _dS;
            private float   _dT;

            public bool IsMatrixIdentity
            {
                get
                {
                    if (_scale != Vector3.Zero)
                        return true;
                    return false;
                }
            }

            private Matrix4 _scaleMat4;
            public ref Matrix4 Matrix { get => ref _scaleMat4; }

            public ComponentScale()
            {
                _isUpdateAllowed = true;
                _valueUpdated = false;

                _scale = Vector3.One;

                _mode = InterpolationMode.NONE;

                _isInterpolating = false;
                _dS = Vector3.Zero;
                _dT = 0f;

                _scaleMat4 = Matrix4.Identity;
            }
            public ComponentScale(float sX, float sY, float sZ)
            {
                _isUpdateAllowed = true;
                _valueUpdated = false;

                _scale = new Vector3(sZ, sY, sZ);

                _mode = InterpolationMode.NONE;

                _isInterpolating = false;
                _dS = Vector3.Zero;
                _dT = 0f;

                BuildMatrix();
            }

            private void BuildMatrix()
            {
                Matrix4.CreateScale(ref _scale, out _scaleMat4);
            }

            public void InterpolateTranslation(Vector3 deltaLocation, float interpolationTime, InterpolationMode mode)
            {
                if (_isUpdateAllowed)
                {
                    if (deltaLocation != Vector3.Zero && interpolationTime != 0f)
                    {
                        _mode = mode;

                        _dS = deltaLocation;
                        _dT = interpolationTime;
                        //VELOCITY = DX/DT
                        _isInterpolating = true;
                    }
                    else
                    {
                        _dS = Vector3.Zero;
                        _dT = 0f;
                        _isInterpolating = false;
                        _mode = InterpolationMode.NONE;
                    }
                }
            }

            public void Update(float time)
            {
                if (_isInterpolating)  //future add switch statement for interp types
                {
                    if (time < _dT)
                    {
                        Vector3 sclStep = _dS * (time / _dT);
                        _scale += sclStep;
                        _dT -= time;
                        _dS -= sclStep;
                    }
                    else if (time >= _dT)
                    {
                        _scale += _dS;
                        _dS = Vector3.Zero;
                        _dT = 0f;
                        _isInterpolating = false;
                    }
                }

                if (_valueUpdated)
                    BuildMatrix();
            }
        }
    }
}
