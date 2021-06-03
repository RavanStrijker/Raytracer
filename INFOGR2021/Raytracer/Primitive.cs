using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
    // The superclass for all Primitives, holds the material, type and ambient color and makes sure every Primitive inherits an Intersect method
    abstract class Primitive
    {
        public float ambient;
        public int Type; // Plane = 0, Sphere = 1, Triangle = 2
        public Material material;

        public Primitive() { }
        public abstract Intersection Intersect(Ray R);
    }
}
