using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
    // This class holds the origin, direction and the distance of a Ray
    // It also saves the first intersection with the world, a list of it's shadow rays and, when the intersected primitive is a mirror, the secondary ray
    class Ray
    {
        public Vector3 Origin;
        public Vector3 Direction;
        public float t;
        public Intersection intersection;
        public List<Ray> shadowRays = new List<Ray>();
        public Ray secondary;
    }
}
