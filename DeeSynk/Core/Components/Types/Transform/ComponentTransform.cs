using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.Transform
{
    public class ComponentTransform : IComponent
    {
        public int BitMaskID => (int)Component.TRANSFORM;

        private int _transformComponentsMask;
        public int TransformComponentsMask { get => _transformComponentsMask; }

        //NOTES FOR FUTURE ADDITIONS:
        //  -ADD
        //    TRANSLATIONAL & ANGULAR VELOCITY
        //    TRANSLATIONAL & ANGULAR ACCELERATION
        //


        //VALUES
        //TRANSFORM FIELDS
        private Vector3 _loc;  //location XYZ
        private float   _rtX,  //rotation X
                        _rtY,  //          Y
                        _rtZ;  //           Z
        private Vector3 _scl;  //scale    XYZ

        private Vector3 _vel_LOC, _acc_LOC;
        private float   _vel_RTX, _acc_RTX;
        private float   _vel_RTY, _acc_RTY;
        private float   _vel_RTZ, _acc_RTZ;
        private Vector3 _vel_SCL, _acc_SCL;

        //TRANSFORM PROPERTIES
        public Vector3 LOCATION
        {
            get => _loc;
            set {
                if(IsUpdateAllowed_LOC && ((_transformComponentsMask & (int)Component.LOCATION) != 0))
                {
                    _loc = value;
                    _valUpdated_LOC = true;
                }
            }
        }
        public float   ROTATION_X
        {
            get => _rtX;
            set
            {
                if(IsUpdateAllowed_RTX && ((_transformComponentsMask & (int)Component.ROTATION_X) != 0))
                {
                    _rtX = value;
                    _valUpdated_RTX = true;
                }
            }
        }
        public float   ROTATION_Y
        {
            get => _rtY;
            set
            {
                if (IsUpdateAllowed_RTY && (_transformComponentsMask & (int)Component.ROTATION_Y) != 0)
                {
                    _rtY = value;
                    _valUpdated_RTY = true;
                }
            }
        }
        public float   ROTATION_Z
        {
            get => _rtZ;
            set
            {
                if (IsUpdateAllowed_RTZ && (_transformComponentsMask & (int)Component.ROTATION_Z) != 0)
                {
                    _rtZ = value;
                    _valUpdated_RTZ = true;
                }
            }
        }
        public Vector3 SCALE
        {
            get => _scl;
            set
            {
                if (IsUpdateAllowed_SCL && ((_transformComponentsMask & (int)Component.SCALE) != 0))
                {
                    _scl = value;
                    _valUpdated_SCL = true;
                }
            }
        }

        //INTERPOLATION
        //IS TRANSFORM TYPE UNDERGOING INTERPOLATION
        private bool _isInterpolating_LOC;
        private bool _isInterpolating_RTX, 
                     _isInterpolating_RTY, 
                     _isInterpolatingRTZ;
        private bool _isInterpolating_SCL;

        //CHANGE IN VALUE FOR INTERPOLATION (delta value)
        private Vector3 _dX_LOC;
        private float   _dX_RTX, 
                        _dX_RTY, 
                        _dx_RTZ;
        private Vector3 _dx_SCL;

        //TIMESPAN FOR INTERPOLATION TO OCCUR (delta time)
        private float   _dt_LOC, 
                        _dt_RTX, 
                        _dt_RTY, 
                        _dt_RTZ, 
                        _dt_SCL;

        //UPDATE FLAGS
        //WAS THIS TRANSFORM VALUE UPDATED
        private bool _valUpdated_LOC,
                     _valUpdated_RTX, 
                     _valUpdated_RTY, 
                     _valUpdated_RTZ,
                     _valUpdated_SCL;

        public bool ValueUpdated_LOC { get => _valUpdated_LOC; }
        public bool ValueUpdated_RTX { get => _valUpdated_RTX; }
        public bool ValueUpdated_RTY { get => _valUpdated_RTY; }
        public bool ValueUpdated_RTZ { get => _valUpdated_RTZ; }
        public bool ValueUpdated_SCL { get => _valUpdated_SCL; }

        private bool _isUpdateAllowed_LOC,
                     _isUpdateAllowed_RTX, 
                     _isUpdateAllowed_RTY, 
                     _isUpdateAllowed_RTZ,
                     _isUpdateAllowed_SCL;

        public bool IsUpdateAllowed_LOC
        {
            get => _isUpdateAllowed_LOC;
            set
            {
                if ((_transformComponentsMask & (int)Component.LOCATION) != 0)
                    _isUpdateAllowed_LOC = value;
            }
        }
        public bool IsUpdateAllowed_RTX
        {
            get => _isUpdateAllowed_RTX;
            set
            {
                if ((_transformComponentsMask & (int)Component.ROTATION_X) != 0)
                    _isUpdateAllowed_RTX = value;
            }
        }
        public bool IsUpdateAllowed_RTY
        {
            get => _isUpdateAllowed_RTY;
            set
            {
                if ((_transformComponentsMask & (int)Component.ROTATION_Y) != 0)
                    _isUpdateAllowed_RTY = value;
            }
        }
        public bool IsUpdateAllowed_RTZ
        {
            get => _isUpdateAllowed_RTZ;
            set
            {
                if ((_transformComponentsMask & (int)Component.ROTATION_Z) != 0)
                    _isUpdateAllowed_RTZ = value;
            }
        }
        public bool IsUpdateAllowed_SCL
        {
            get => _isUpdateAllowed_SCL;
            set
            {
                if ((_transformComponentsMask & (int)Component.SCALE) != 0)
                    _isUpdateAllowed_SCL = value;
            }
        }

        //TRANSFORM MATRICES
        private Matrix4 _translate, 
                        _translate_origin,
                        _rotX, 
                        _rotY, 
                        _rotZ,
                        _scale,
                        _modelView;

        public ref Matrix4 GetModelView { get => ref _modelView; }

        public ComponentTransform()
        {
            _transformComponentsMask = (int)Component.NONE;

            _loc = Vector3.Zero;
            _rtX = 0.0f;
            _rtY = 0.0f;
            _rtZ = 0.0f;
            _scl = Vector3.One;

            _valUpdated_LOC = false;
            _valUpdated_RTX = false;
            _valUpdated_RTY = false;
            _valUpdated_RTZ = false;
            _valUpdated_SCL = false;

            _isUpdateAllowed_LOC = false;
            _isUpdateAllowed_RTX = false;
            _isUpdateAllowed_RTY = false;
            _isUpdateAllowed_RTZ = false;
            _isUpdateAllowed_SCL = false;
        }
        public ComponentTransform(int transformComponentsMask)
        {
            _transformComponentsMask = transformComponentsMask;

            _loc = Vector3.Zero;
            _rtX = 0.0f;
            _rtY = 0.0f;
            _rtZ = 0.0f;
            _scl = Vector3.One;

            _valUpdated_LOC = false;
            _valUpdated_RTX = false;
            _valUpdated_RTY = false;
            _valUpdated_RTZ = false;
            _valUpdated_SCL = false;

            _isUpdateAllowed_LOC = false;  IsUpdateAllowed_LOC = true;
            _isUpdateAllowed_RTX = false;  IsUpdateAllowed_RTX = true;
            _isUpdateAllowed_RTY = false;  IsUpdateAllowed_RTY = true;
            _isUpdateAllowed_RTZ = false;  IsUpdateAllowed_RTZ = true;
            _isUpdateAllowed_SCL = false;  IsUpdateAllowed_SCL = true;
        }

        public void PushLocation(ref Vector3 location)
        {
            if ((_transformComponentsMask & (int)Component.LOCATION) != 0)
            {
                _loc = new Vector3(location);
                Matrix4.CreateTranslation(ref _loc, out _translate);

                if((_transformComponentsMask & ((int)Component.ROTATION_XYZ)) != 0)
                {
                    var loc_o = _loc * -1f;
                    Matrix4.CreateTranslation(ref loc_o, out _translate_origin);
                }
            }
        }
        public void PushRotationX(float rotX)
        {
            if ((_transformComponentsMask & (int)Component.ROTATION_X) != 0)
            {
                _rtX = rotX;
                Matrix4.CreateRotationX(_rtX, out _rotX);
            }
        }
        public void PushRotationY(float rotY)
        {
            if ((_transformComponentsMask & (int)Component.ROTATION_Y) != 0)
            {
                _rtY = rotY;
                Matrix4.CreateRotationY(rotY, out _rotY);
            }
        }
        public void PushRotationZ(float rotZ)
        {
            if ((_transformComponentsMask & (int)Component.ROTATION_Z) != 0)
            {
                _rtZ = rotZ;
                Matrix4.CreateRotationZ(rotZ, out _rotZ);
            }
        }
        public void PushRotationXYZ(float rotX, float rotY, float rotZ)
        {
            PushRotationX(rotX);
            PushRotationY(rotY);
            PushRotationZ(rotZ);
        }
        public void PushScale(ref Vector3 scale)
        {
            if ((_transformComponentsMask & (int)Component.SCALE) != 0)
            {
                _scl = scale;
                Matrix4.CreateScale(ref scale, out _scale);
            }
        }

        public void ComputeModelViewProduct()
        {
            if((_transformComponentsMask & ((int)Component.ROTATION_XYZ)) != 0)
            {
                Matrix4 rotMats = Matrix4.Identity;
                if ((_transformComponentsMask & (int)Component.ROTATION_X) != 0)
                    rotMats *= _rotX;
                if ((_transformComponentsMask & (int)Component.ROTATION_Y) != 0)
                    rotMats *= _rotY;
                if ((_transformComponentsMask & (int)Component.ROTATION_Z) != 0)
                    rotMats *= _rotZ;

                if (((_transformComponentsMask & (int)Component.LOCATION) != 0) && ((_transformComponentsMask & (int)Component.SCALE) != 0))
                    _modelView = _translate_origin * _scale * rotMats * _translate;
                else if ((_transformComponentsMask & (int)Component.LOCATION) != 0)
                    _modelView = _translate_origin * rotMats * _translate;
                else if ((_transformComponentsMask & (int)Component.SCALE) != 0)
                    _modelView = _scale * rotMats;
                else
                    _modelView = rotMats;
            }
            else
            {
                if (((_transformComponentsMask & (int)Component.LOCATION) != 0) && ((_transformComponentsMask & (int)Component.SCALE) != 0))
                    _modelView = _scale * _translate;
                else if ((_transformComponentsMask & (int)Component.LOCATION) != 0)
                    _modelView = _translate;
                else if ((_transformComponentsMask & (int)Component.SCALE) != 0)
                    _modelView = _scale;
                else
                    _modelView = Matrix4.Identity;
            }
        }



        //interpolation methods

        //location //velocity //acceleration
        //rotation XYZ
        //scale XYZ

        //set constant rate methods
        //set constant velocity and rotation rate

        //public void InterpolateRotationX(float deltaAngleX, float interpolationTime)
        //{
        //    if (_isUpdateAllowed_RTX)
        //    {
        //        if (deltaAngleX != 0 && interpolationTime != 0)
        //        {
        //            if (_hasConstantRotationRate)
        //            {
        //                _hasConstantRotationRate = false;
        //                _rotationRate = 0.0f;
        //            }
        //            _interpolationRot = deltaAngle;
        //            _interpolationTime = interpolationTime;
        //            _isInterpolating = true;
        //        }
        //        else
        //        {
        //            _interpolationRot = 0.0f;
        //            _interpolationTime = 0.0f;
        //            _isInterpolating = false;
        //        }
        //    }
        //}

        public void Update(float time)
        {

        }
    }
}
