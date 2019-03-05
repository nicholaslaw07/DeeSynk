using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.Transform
{
    public class ComponentRotation_Y : IComponent
    {
        public Component BitMaskID => Component.ROTATION_Y;

        private bool _valueUpdated;
        public bool ValueUpdated { get => _valueUpdated; }

        private float _rotY;
        public float Rotation
        {
            get => _rotY;
            set
            {
                if (IsRotationAllowed)
                {
                    _rotY = value;
                    if (!_valueUpdated)
                        _valueUpdated = true;
                }
            }
        }

        private bool _isInterpolating;
        public bool IsInterpolating { get => _isInterpolating; }  //instead, maybe check for certain values in the fields (return if values are not zero)
        private float _interpolationRot;  //total number of radians to traverse over the specified time period;
        private float _interpolationTime; //time over which the interpolation will take place in seconds;

        private bool _hasConstantRotationRate;
        public bool HasConstantRotationRate { get => _hasConstantRotationRate; }  //instead, maybe update these to simply check for certain values in the fields
        private float _rotationRate;  //radians to traverse in one second

        private bool _isRotationAllowed;
        public bool IsRotationAllowed    //freezes all updates to the angle of rotation, for purposes of effects or whatever
        {
            get => _isRotationAllowed;
            set
            {
                _isRotationAllowed = value;
                if (value == false)
                {
                    _isInterpolating = false;
                    _interpolationRot = 0.0f;
                    _interpolationTime = 0.0f;

                    _hasConstantRotationRate = false;
                    _rotationRate = 0.0f;
                }
            }
        }


        public ComponentRotation_Y()
        {
            _rotY = 0.0f;
            _valueUpdated = true;

            _isInterpolating = false;
            _interpolationRot = 0.0f;
            _interpolationTime = 0.0f;

            _hasConstantRotationRate = false;
            _rotationRate = 0.0f;

            _isRotationAllowed = true;
        }

        public ComponentRotation_Y(float rotY)
        {
            _rotY = rotY;
            _valueUpdated = true;

            _isInterpolating = false;
            _interpolationRot = 0.0f;
            _interpolationTime = 0.0f;

            _hasConstantRotationRate = false;
            _rotationRate = 0.0f;

            _isRotationAllowed = true;
        }

        /// <summary>
        /// Used to tell an object to move a specified amount of radians from the current rotation angle on the Y axis over a specified time in seconds.  Cancels ConstantRotationRate if non-zero values are provided.
        /// </summary>
        /// <param name="interpolationRot">Total change in radians for the interpolation.</param>
        /// <param name="interpolationTime">Time over which the interpolation occurs in seconds.</param>
        public void InterpolateAngle(float deltaAngle, float interpolationTime)
        {
            if (_isRotationAllowed)
            {
                if (deltaAngle != 0 && interpolationTime != 0)
                {
                    if (_hasConstantRotationRate)
                    {
                        _hasConstantRotationRate = false;
                        _rotationRate = 0.0f;
                    }
                    _interpolationRot = deltaAngle;
                    _interpolationTime = interpolationTime;
                    _isInterpolating = true;
                }
                else
                {
                    _interpolationRot = 0.0f;
                    _interpolationTime = 0.0f;
                    _isInterpolating = false;
                }
            }
        }

        /// <summary>
        /// Stops the interpolation process for rotation on the Y axis.
        /// </summary>
        /// <param name="skipToEnd">Determines whether or not to skip the rotation to the end of the interpolation.</param>
        public void StopInterpolation(bool skipToEnd)
        {
            if (skipToEnd)
            {
                Rotation += _interpolationRot;
            }

            _interpolationRot = 0.0f;
            _interpolationTime = 0.0f;
            _isInterpolating = false;
        }

        /// <summary>
        /// Used to set the rotation rate of the object along the Y axis. If the object is interpolating and an appropriate value is sent in, then the object will stop interpolating with no skip to the end.
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
                    if (IsInterpolating)
                        StopInterpolation(false);
                }
                else
                {
                    _rotationRate = 0.0f;
                    _hasConstantRotationRate = false;
                }
            }
        }

        /// <summary>
        /// Updates the status and angle along the Y axis for either interpolation or a constant rotation rate.
        /// </summary>
        /// <param name="time">Time that the last frame took to complete.</param>
        public void Update(float time)  //if time is too small (which should never happen) then the interpolation will never update as long as the time value stays equally small (<10E-7)
        {
            if (_isInterpolating)
            {
                if (time < _interpolationTime)
                {
                    float deltaRot = _interpolationRot * (time / _interpolationTime); //fraction of rotation to progress
                    Rotation += deltaRot;
                    _interpolationTime -= time;
                    _interpolationRot -= deltaRot;
                }
                else if (time >= _interpolationTime)
                {
                    Rotation += _interpolationRot;
                    _interpolationRot = 0.0f;
                    _interpolationTime = 0.0f;
                    _isInterpolating = false;
                }
            }
            else if (_hasConstantRotationRate)
            {
                Rotation += _rotationRate * time;
            }

            if (_valueUpdated)  //prevents loss of significant digits by keep Rotation within +-2pi
                Rotation %= 6.283185f;
        }
    }
}
