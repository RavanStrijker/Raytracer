using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
    // Subclass of Primitive that enables the use of Spheres, also inherits the Intersect method that checks if a ray intersects with the sphere
    class Sphere : Primitive
    {
        public Vector3 Position;
        public float Radius;

        public Sphere(Vector3 _position, float _radius, Material _material)
        {
            this.Position = _position;
            this.Radius = _radius;
            this.material = _material;
            this.Type = 1;
            this.ambient = 0.1f;
        }

        public override Intersection Intersect(Ray R) // Checks whether a ray intersects with the sphere
        {
            Intersection I = new Intersection();
            float a = Vector3.Dot(R.Direction, R.Direction);
            float b = Vector3.Dot(2 * R.Direction, R.Origin - this.Position);
            float c = Vector3.Dot(R.Origin - this.Position, R.Origin - this.Position) - (float)Math.Pow(Radius, 2);
            float D = (float)Math.Pow(b, 2) - 4 * a * c;

            if (D >= 0)
            {
                float t = (-b - (float)Math.Sqrt(D)) / (2 * a);
                I.intersectPoint = R.Origin + t * R.Direction;
                I.Closest = this;
                I.Normal = Vector3.Normalize(I.intersectPoint - this.Position);
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
