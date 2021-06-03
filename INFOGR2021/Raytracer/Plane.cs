using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
    // Subclass of Primitive that enables the use of Planes, also inherits the Intersect method that checks if a ray intersects with the plane
    class Plane : Primitive
    {
        public Vector3 Normal;
        public float Distance;

        public Plane(Vector3 _normal, float _distance, Material _material)
        {
            this.Normal = _normal;
            this.Distance = _distance;
            this.material = _material;
            this.Type = 0;
            this.ambient = 0.1f;
        }

        public override Intersection Intersect(Ray R) // Checks whether a ray intersects with the plane
        {
            Intersection I = new Intersection();
            float t = -((Vector3.Dot(R.Origin, Normal) + Distance) / (Vector3.Dot(R.Direction, Normal)));
            
            if(t > 0)
            {
                I.intersectPoint = R.Origin + t * R.Direction;
                I.Closest = this;
                I.Normal = this.Normal;
                I.Distance = t;
            }
            else
            {
                I.Distance = 0;
            }
            
            
            return I;
        }
    }
}
