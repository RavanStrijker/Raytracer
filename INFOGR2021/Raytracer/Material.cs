using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
    // This class stores information about the material of a primitive
    class Material
    {
        public bool isMirror; 
        public Vector3 color; // Diffuse coefficient
        public float glossy; // Specular coefficient
        public bool texture; // If true, checkerboard pattern


        public Material(bool _isMirror, Vector3 _color, float _glossy, bool _texture)
        {
            this.isMirror = _isMirror;
            this.color = _color;
            this.glossy = _glossy;
            this.texture = _texture;
        }
    }
}
