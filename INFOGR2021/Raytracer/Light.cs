using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
    // This class holds the location, intensity and color of a light source
    class Light
    {
        public Vector3 Location;
        public Vector3 Intensity;
        public Vector3 Color;

        public Light(Vector3 _location, Vector3 _intensity, Vector3 _color)
        {
            this.Location = _location;
            this.Intensity = _intensity;
            this.Color = _color;
        }
    }
}
