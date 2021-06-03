using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
    // This struct holds the intersection point, the Primitive that is intersected, the normal at the intersection point and the distance from the intersection point to the camera for an intersection between a Ray and a Primitive 
    struct Intersection
    {
        public Vector3 intersectPoint;
        public Primitive Closest;
        public Vector3 Normal;
        public float Distance;
    }
}
