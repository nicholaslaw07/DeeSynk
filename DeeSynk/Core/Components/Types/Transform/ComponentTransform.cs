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

        private Matrix4 _translate, _translate_origin;
        private Matrix4 _rotX, _rotY, _rotZ;
        private Matrix4 _scale;

        private Matrix4 _modelView;
        public ref Matrix4 GetModelView { get => ref _modelView; }

        public ComponentTransform()
        {
            _transformComponentsMask = (int)Component.NONE;

            _translate          = Matrix4.Identity;
            _translate_origin   = Matrix4.Identity;
            _rotX               = Matrix4.Identity;
            _rotY               = Matrix4.Identity;
            _rotZ               = Matrix4.Identity;
            _scale              = Matrix4.Identity;
        }

        public ComponentTransform(int transformComponentsMask)
        {
            _transformComponentsMask = transformComponentsMask;

            _translate          = Matrix4.Identity;
            _translate_origin   = Matrix4.Identity;
            _rotX               = Matrix4.Identity;
            _rotY               = Matrix4.Identity;
            _rotZ               = Matrix4.Identity;
            _scale              = Matrix4.Identity;
        }

        public void PushLocation(ref Vector3 location)
        {
            var l = location;
            if ((_transformComponentsMask & (int)Component.LOCATION) != 0)
            {
                Matrix4.CreateTranslation(ref l, out _translate);

                if((_transformComponentsMask & (int)Component.ROTATION_X & (int)Component.ROTATION_Y & (int)Component.ROTATION_Z) != 0)
                {
                    var lo = location * -1f;
                    Matrix4.CreateTranslation(ref lo, out _translate_origin);
                }
            }
        }

        public void PushRotationX(float rotX)
        {
            if ((_transformComponentsMask & (int)Component.ROTATION_X) != 0)
                Matrix4.CreateRotationX(rotX, out _rotX);
        }
        public void PushRotationY(float rotY)
        {
            if ((_transformComponentsMask & (int)Component.ROTATION_Y) != 0)
                Matrix4.CreateRotationY(rotY, out _rotY);
        }
        public void PushRotationZ(float rotZ)
        {
            if ((_transformComponentsMask & (int)Component.ROTATION_Z) != 0)
                Matrix4.CreateRotationZ(rotZ, out _rotZ);
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
                Matrix4.CreateScale(ref scale, out _scale);
        }

        public void ComputeModeViewProduct()
        {
            if((_transformComponentsMask & ((int)Component.ROTATION_X | (int)Component.ROTATION_Y | (int)Component.ROTATION_Z)) != 0)
            {
                Matrix4 rotMats = Matrix4.Identity;
                if ((_transformComponentsMask & (int)Component.ROTATION_X) != 0)
                    rotMats *= _rotX;
                if ((_transformComponentsMask & (int)Component.ROTATION_Y) != 0)
                    rotMats *= _rotY;
                if ((_transformComponentsMask & (int)Component.ROTATION_Z) != 0)
                    rotMats *= _rotZ;

                if (((_transformComponentsMask & (int)Component.LOCATION) != 0) && ((_transformComponentsMask & (int)Component.SCALE) != 0))
                    _modelView = _translate_origin * rotMats * _scale * _translate;
                else if ((_transformComponentsMask & (int)Component.LOCATION) != 0)
                    _modelView = _translate_origin * rotMats * _translate;
                else if ((_transformComponentsMask & (int)Component.SCALE) != 0)
                    _modelView = rotMats * _scale;
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

        public void Update(float time)
        {

        }
    }
}
