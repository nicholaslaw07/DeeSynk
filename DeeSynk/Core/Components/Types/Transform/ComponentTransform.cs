using DeeSynk.Core.Components.Types.Render;
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

    [Flags]
    public enum TransformComponents: byte
    {
        NONE =        0,
        TRANSLATION = 1,
        ROTATION_X =  1 << 1,
        ROTATION_Y =  1 << 2,
        ROTATION_Z =  1 << 3,
        SCALE =       1 << 4,

        ROTATION_XYZ = ROTATION_X | ROTATION_Y | ROTATION_Z,

    }

    public class ComponentTransform : IComponent
    {
        public Component BitMaskID => Component.TRANSFORM;

        private TransformComponents _transformComponents;
        public  TransformComponents TransformComponents { get => _transformComponents; }

        private ComponentLocation _location;
        private ComponentRotation _rotationX,
                                  _rotationY,
                                  _rotationZ;
        private ComponentScale    _scale;

        public ref ComponentLocation LocationComp
        {
            get
            {
                if (_location == null)
                {
                    _location = new ComponentLocation(false);
                    _transformComponents |= TransformComponents.TRANSLATION;
                }
                return ref _location;
            }
        }
        public ref ComponentRotation RotationXComp
        {
            get
            {
                if (_rotationX == null)
                {
                    _rotationX = new ComponentRotation(RotationAxes.X);
                    _transformComponents |= TransformComponents.ROTATION_X;
                }
                return ref _rotationX;
            }
        }
        public ref ComponentRotation RotationYComp
        {
            get
            {
                if (_rotationY == null)
                {
                    _rotationY = new ComponentRotation(RotationAxes.Y);
                    _transformComponents |= TransformComponents.ROTATION_Y;
                }
                return ref _rotationY;
            }
        }
        public ref ComponentRotation RotationZComp
        {
            get
            {
                if (_rotationZ == null)
                {
                    _rotationZ = new ComponentRotation(RotationAxes.Z);
                    _transformComponents |= TransformComponents.ROTATION_Z;
                }
                return ref _rotationZ;
            }
        }
        public ref ComponentScale    ScaleComp
        {
            get
            {
                if (_scale == null)
                {
                    _scale = new ComponentScale();
                    _transformComponents |= TransformComponents.SCALE;
                }
                return ref _scale;
            }
        }

        private Matrix4 _model;

        public ref Matrix4 GetModelMatrix { get => ref _model; }

        public ComponentTransform(TransformComponents transformComponents, bool includeNormalsMatrix,
                                  float locX = 0f, float locY = 0f, float locZ = 0f, 
                                  float rotX = 0f, float rotY = 0f, float rotZ = 0f, 
                                  float sclX = 1f, float sclY = 1f, float sclZ = 1f)
        {
            _transformComponents = transformComponents;

            if (_transformComponents.HasFlag(TransformComponents.TRANSLATION))
            {
                bool doOrigin = false;
                if (_transformComponents.HasFlag(TransformComponents.ROTATION_X) ||
                    _transformComponents.HasFlag(TransformComponents.ROTATION_Y) ||
                    _transformComponents.HasFlag(TransformComponents.ROTATION_Z))

                    doOrigin = true;

                _location = new ComponentLocation(doOrigin, locX, locY, locZ);
            }

            if (_transformComponents.HasFlag(TransformComponents.ROTATION_X))
                _rotationX = new ComponentRotation(RotationAxes.X, rotX);

            if (_transformComponents.HasFlag(TransformComponents.ROTATION_Y))
                _rotationX = new ComponentRotation(RotationAxes.Y, rotY);

            if (_transformComponents.HasFlag(TransformComponents.ROTATION_Z))
                _rotationX = new ComponentRotation(RotationAxes.Z, rotZ);

            if (_transformComponents.HasFlag(TransformComponents.SCALE))
                _scale = new ComponentScale(sclX, sclY, sclZ);

            _model = Matrix4.Identity;

            ComputeModelProduct();
        }

        public ComponentTransform(ref ComponentModelStatic modelComp)
        {
            var flags = modelComp.ConstructionFlags;
            _transformComponents = TransformComponents.NONE;
            if (flags.HasFlag(ConstructionFlags.VECTOR3_OFFSET))
            {
                _transformComponents |= TransformComponents.TRANSLATION;
                bool doOrigin = false;
                
                if (flags.HasFlag(ConstructionFlags.FLOAT_ROTATION_X) || flags.HasFlag(ConstructionFlags.FLOAT_ROTATION_Y) || flags.HasFlag(ConstructionFlags.FLOAT_ROTATION_Z))
                    doOrigin = true;

                modelComp.GetConstructionParameter(ConstructionFlags.VECTOR3_OFFSET, out float[] data);
                    
                _location = new ComponentLocation(doOrigin, data[0], data[1], data[2]);
            }

            if (flags.HasFlag(ConstructionFlags.FLOAT_ROTATION_X))
            {
                _transformComponents |= TransformComponents.ROTATION_X;
                modelComp.GetConstructionParameter(ConstructionFlags.FLOAT_ROTATION_X, out float[] data);
                _rotationX = new ComponentRotation(RotationAxes.X, data[0]);
            }

            if (flags.HasFlag(ConstructionFlags.FLOAT_ROTATION_Y))
            {
                _transformComponents |= TransformComponents.ROTATION_Y;
                modelComp.GetConstructionParameter(ConstructionFlags.FLOAT_ROTATION_Y, out float[] data);
                _rotationY = new ComponentRotation(RotationAxes.Y, data[0]);
            }

            if (flags.HasFlag(ConstructionFlags.FLOAT_ROTATION_Z))
            {
                _transformComponents |= TransformComponents.ROTATION_Z;
                modelComp.GetConstructionParameter(ConstructionFlags.FLOAT_ROTATION_Z, out float[] data);
                _rotationZ = new ComponentRotation(RotationAxes.Z, data[0]);
            }

            if (flags.HasFlag(ConstructionFlags.VECTOR3_SCALE))
            {
                _transformComponents |= TransformComponents.SCALE;
                modelComp.GetConstructionParameter(ConstructionFlags.VECTOR3_SCALE, out float[] data);
                _scale = new ComponentScale(data[0], data[1], data[2]);
            }

            _model = Matrix4.Identity;

            ComputeModelProduct();
        }

        /// <summary>
        /// Constructs the model transformation matrix only with those matrices that exist.
        /// </summary>
        public void ComputeModelProduct()
        {
            if(_transformComponents.HasFlag(TransformComponents.ROTATION_X) || _transformComponents.HasFlag(TransformComponents.ROTATION_Y) || _transformComponents.HasFlag(TransformComponents.ROTATION_Z))
            {
                Matrix4 rotMats = Matrix4.Identity;
                if (_transformComponents.HasFlag(TransformComponents.ROTATION_X))
                    Matrix4.Mult(ref rotMats, ref _rotationX.Matrix, out rotMats);
                if (_transformComponents.HasFlag(TransformComponents.ROTATION_Y))
                    Matrix4.Mult(ref rotMats, ref _rotationY.Matrix, out rotMats);
                if (_transformComponents.HasFlag(TransformComponents.ROTATION_Z))
                    Matrix4.Mult(ref rotMats, ref _rotationZ.Matrix, out rotMats);

                if (_transformComponents.HasFlag(TransformComponents.TRANSLATION) && _transformComponents.HasFlag(TransformComponents.SCALE))
                {
                    Matrix4.Mult(ref rotMats, ref _location.Matrix, out _model);
                    Matrix4.Mult(ref _scale.Matrix, ref _model, out _model);
                }
                else if (_transformComponents.HasFlag(TransformComponents.TRANSLATION))
                {
                    Matrix4.Mult(ref rotMats, ref _location.Matrix, out _model);
                }
                else if (_transformComponents.HasFlag(TransformComponents.SCALE))
                {
                    Matrix4.Mult(ref _scale.Matrix, ref rotMats, out _model);
                }
                else
                    _model = rotMats;
            }
            else
            {
                if (_transformComponents.HasFlag(TransformComponents.TRANSLATION) && _transformComponents.HasFlag(TransformComponents.SCALE))
                    Matrix4.Mult(ref _scale.Matrix, ref _location.Matrix, out _model);

                else if (_transformComponents.HasFlag(TransformComponents.TRANSLATION))
                    _model = _location.Matrix;

                else if (_transformComponents.HasFlag(TransformComponents.SCALE))
                    _model = _scale.Matrix;

                else
                    _model = Matrix4.Identity;
            }
        }

        public bool Update(float time)
        {
            clearUpdates();

            bool recomputeProduct = false;
            if (_transformComponents.HasFlag(TransformComponents.TRANSLATION))
                recomputeProduct |= _location.Update(time);

            if (_transformComponents.HasFlag(TransformComponents.ROTATION_X))
                recomputeProduct |= _rotationX.Update(time);

            if (_transformComponents.HasFlag(TransformComponents.ROTATION_Y))
                recomputeProduct |= _rotationY.Update(time);

            if (_transformComponents.HasFlag(TransformComponents.ROTATION_Z))
                recomputeProduct |= _rotationZ.Update(time);

            if (_transformComponents.HasFlag(TransformComponents.SCALE))
                recomputeProduct |= _scale.Update(time);

            if (recomputeProduct)
            {
                ComputeModelProduct();
                return true;
            }
            return false;
        }

        private void clearUpdates()
        {
            if (_location != null) { _location.PreviouslyUpdated = false; }
            if (_rotationX != null) { _rotationX.PreviouslyUpdated = false; }
            if (_rotationY != null) { _rotationY.PreviouslyUpdated = false; }
            if (_rotationZ != null) { _rotationZ.PreviouslyUpdated = false; }
            if (_scale != null) { _scale.PreviouslyUpdated = false; }
        }

        public class ComponentLocation
        {
            private bool _isUpdateAllowed;
            public bool IsUpdateAllowed { get => _isUpdateAllowed; set => _isUpdateAllowed = value; }
            private bool _valueUpdated;
            public bool ValueUpdated { get => _valueUpdated; }
            private bool _previouslyUpdated;
            public bool PreviouslyUpdated { get => _previouslyUpdated; set => _previouslyUpdated = value; }

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
                _isUpdateAllowed = true;
                _valueUpdated = true;
                _previouslyUpdated = false;

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
                _isUpdateAllowed = true;
                _valueUpdated = true;
                _previouslyUpdated = false;

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
                _translateMat4 = Matrix4.CreateTranslation(_location.X, _location.Y, _location.Z);
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

            public bool Update(float time)
            {
                if (_isInterpolating)  //future add switch statement for interp types
                {
                    if (time < _dT)
                    {
                        Vector3 locStep = _dX * (time / _dT);
                        //_translateMat4.Row3 += new Vector4(locStep);
                        _location += locStep;
                        _dT -= time;
                        _dX -= locStep;
                        _valueUpdated = true;
                    }
                    else if (time >= _dT)
                    {
                        _location += _dX;
                        _dX = Vector3.Zero;
                        _dT = 0f;
                        _isInterpolating = false;
                        _valueUpdated = true;
                    }
                }

                if (_valueUpdated)
                {
                    BuildMatrix();
                    _valueUpdated = false;
                    _previouslyUpdated = true;
                    return true;
                }

                return false;
            }
        }

        public class ComponentRotation
        {
            private const float TAO = 6.283185f;

            private bool _isUpdateAllowed;
            public bool IsUpdateAllowed { get => _isUpdateAllowed; set => _isUpdateAllowed = value; }
            private bool _valueUpdated;
            public bool ValueUpdated { get => _valueUpdated; }
            private bool _previouslyUpdated;
            public bool PreviouslyUpdated { get => _previouslyUpdated; set => _previouslyUpdated = value; }

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
                _previouslyUpdated = false;

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
                _previouslyUpdated = false;

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

            public bool Update(float time)
            {
                if (_isInterpolating)  //future add switch statement for interp types
                {
                    if(time < _dT)
                    {
                        float rotStep = _dR * (time / _dT);
                        _angle += rotStep;
                        _dT -= time;
                        _dR -= rotStep;
                        _valueUpdated = true;
                    }
                    else if(time >= _dT)
                    {
                        _angle += _dR;
                        _dR = 0f;
                        _dT = 0f;
                        _isInterpolating = false;
                        _valueUpdated = true;
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

                    _valueUpdated = false;
                    _previouslyUpdated = true;
                    return true;
                }

                return false;
            }
        }

        public class ComponentScale
        {
            private bool _isUpdateAllowed;
            public bool IsUpdateAllowed { get => _isUpdateAllowed; set => _isUpdateAllowed = value; }
            private bool _valueUpdated;
            public bool ValueUpdated { get => _valueUpdated; }
            private bool _previouslyUpdated;
            public bool PreviouslyUpdated { get => _previouslyUpdated; set => _previouslyUpdated = value; }

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
                _previouslyUpdated = false;

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
                _previouslyUpdated = false;

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

            public bool Update(float time)
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
                {
                    BuildMatrix();

                    _valueUpdated = false;
                    _previouslyUpdated = true;
                    return true;
                }

                return false;
            }
        }
    }
}
