using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types
{
    public class ComponentRotation_X : IComponent
    {
        public Component BitMaskID => Component.ROTATION_X;

        private float _rotX;
        public float Rotation
        {
            get => _rotX;
            set
            {
                if (IsRotaitonAllowed)
                    _rotX = value;
            }
        }

        private bool  _isInterpolating;
        public  bool  IsInterpolating { get => _isInterpolating; }
        private float _interpolationRot;  //total number of radians to traverse over the specified time period;
        private float _interpolationTime; //time over which the interpolation will take place in seconds;

        private bool  _hasConstantRotationRate;
        public  bool  HasConstantRotationRate { get => _hasConstantRotationRate; }
        private float _rotationRate;  //radians to traverse in one second

        private bool _isRotationAllowed;
        public bool IsRotaitonAllowed    //freezes all updates to the angle of rotation, for purposes of effects or whatever
        {
            get => _isRotationAllowed;
            set
            {
                _isRotationAllowed = value;
                if (value == false)
                {
                    _isInterpolating = false;
                    _interpolationRot = 0.0f;
                    _interpolationTime = 1.0f;

                    _hasConstantRotationRate = false;
                    _rotationRate = 0.0f;
                }
            }
        }


        public ComponentRotation_X()
        {
            _rotX = 0.0f;

            _isInterpolating = false;
            _interpolationRot = 0.0f;
            _interpolationTime = 1.0f;

            _hasConstantRotationRate = false;
            _rotationRate = 0.0f;

            _isRotationAllowed = true;
        }

        public ComponentRotation_X(float rotX)
        {
            _rotX = rotX;

            _isInterpolating = false;
            _interpolationRot = 0.0f;
            _interpolationTime = 1.0f;

            _hasConstantRotationRate = false;
            _rotationRate = 0.0f;

            _isRotationAllowed = true;
        }

        /// <summary>
        /// Used to tell an object to move a specified amount of radians from the current rotation angle over a specified time in seconds.
        /// </summary>
        /// <param name="interpolationRot">Total change in radians for the interpolation.</param>
        /// <param name="interpolationTime">Time over which the interpolation occurs in seconds.</param>
        public void InterpolateAngle(float interpolationRot, float interpolationTime)
        {
            if (_isRotationAllowed)
            {
                if (interpolationRot != 0 && interpolationTime != 0)
                {
                    _interpolationRot = interpolationRot;
                    _interpolationTime = interpolationTime;
                    _isInterpolating = true;
                }
                else
                {
                    _interpolationRot = 0.0f;
                    _interpolationTime = 1.0f;
                    _isInterpolating = false;
                }
            }
        }

        /// <summary>
        /// Stops the interpolation process.
        /// </summary>
        /// <param name="skipToEnd">Determines whether or not to skip the rotation to the end of the interpolation.</param>
        public void StopInterpolation(bool skipToEnd)
        {
            if (skipToEnd)
                _rotX += _interpolationRot;

            _interpolationRot = 0.0f;
            _interpolationTime = 1.0f;
            _isInterpolating = false;
        }

        /// <summary>
        /// Used to set the rotation rate of the object.
        /// </summary>
        /// <param name="rotationRate">Rotation rate in radians per second.</param>
        public void SetConstantRotation(float rotationRate)
        {
            if (_isRotationAllowed)
            {
                if (rotationRate != 0)
                {
                    _rotationRate = rotationRate;
                    _hasConstantRotationRate = true;
                }
                else
                {
                    _rotationRate = 0.0f;
                    _hasConstantRotationRate = false;
                }
            }
        }
    }
}
