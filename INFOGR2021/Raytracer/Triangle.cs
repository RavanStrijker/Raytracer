using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
    // Subclass of Primitive that enables the use of Triangles, also inherits the Intersect method that checks if a ray intersects with the plane
    class Triangle : Primitive
    {
        public Vector3 Normal;
        public Vector3[] Corners;

        public Triangle(Vector3 _c0, Vector3 _c1, Vector3 _c2, Material _material)
        {
            this.Corners = new Vector3[3];
            Corners[0] = _c0;
            Corners[1] = _c1;
            Corners[2] = _c2;
            this.Normal = Vector3.Normalize(Vector3.Cross(_c1 - _c0, _c2 - _c0));
            this.material = _material;
            this.Type = 2;
            this.ambient = 0.1f;
        }

        public override Intersection Intersect(Ray R) // Checks whether a ray intersects with the triangle
        {
            Intersection I = new Intersection();
            float dot = Vector3.Dot(Normal, R.Direction);
            if(dot == 0)
            {
                I.Distance = 0;
                return I;
            }

            float t = (Vector3.Dot(Corners[0], Normal) - Vector3.Dot(R.Origin, Normal)) / dot;
            if(t > 0)
            {
                Vector3 Point = R.Origin + t * R.Direction;

                Vector3 AB = Corners[1] - Corners[0];
                Vector3 AC = Corners[2] - Corners[0];
                Vector3 BC = Corners[2] - Corners[1];

                float checkAB = Vector3.Dot(Normal, Vector3.Cross(AB, Point - Corners[0]));
                float checkAC = Vector3.Dot(Normal, Vector3.Cross(AC, Point - Corners[2]));
                float checkBC = Vector3.Dot(Normal, Vector3.Cross(BC, Point - Corners[1]));

                if(checkAB >= 0 && checkAC <= 0 && checkBC >= 0)
                {
                    I.intersectPoint = Point;
                    I.Closest = this;
                    I.Normal = this.Normal;
                    I.Distance = t;
                }
                else
                {
                    I.Distance = 0;
                }

            }
            else
            {
                I.Distance = 0;
            }

            return I;
        }
    }
}
