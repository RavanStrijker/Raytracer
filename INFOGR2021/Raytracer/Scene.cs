using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
    // This class holds/creates the 'world', it has a list with all the Primitives and Light sources and has an Intersect method that loops through all the Primitives in the list and finds the closest intersection.
    // The method computeColor finds the floating point color of a screen pixel given the ray through it and that ray's Intersection with the world
    class Scene
    {
        public List<Primitive> Primitives;
        public List<Light> lightSources;
        public float stochasticChange = randomFloat(0.2f, -0.2f); // For stochastic glossy reflections
        public int recursionDepth = 3;

        public Scene() // Prepares the scene
        {
            Primitives = new List<Primitive>();
            lightSources = new List<Light>();

            Primitives.Add(new Plane(new Vector3(0, 1, 0), 5, new Material(false, new Vector3(1, 1, 1), 0, true))); // Floor
            Primitives.Add(new Plane(new Vector3(0, -1, 0), 5, new Material(false, new Vector3(1, 1, 1), 0, true))) ; // Ceiling
            Primitives.Add(new Plane(new Vector3(0, 0, -1), 10, new Material(false, new Vector3(1, 1, 1), 0, true))); // Walls
            Primitives.Add(new Plane(new Vector3(-1, 0, 0), 5, new Material(false, new Vector3(1, 1, 1), 0, true)));
            Primitives.Add(new Plane(new Vector3(1, 0, 0), 5, new Material(false, new Vector3(1, 1, 1), 0, true)));
            Primitives.Add(new Plane(new Vector3(0, 0, 1), 0, new Material(false, new Vector3(1, 1, 1), 0, true)));


            Primitives.Add(new Sphere(new Vector3(0, 0, 5), 1, new Material(true, new Vector3(0.5f, 0.5f, 0.5f), 0.5f, false))); // Spheres
            Primitives.Add(new Sphere(new Vector3(-2, 0, 5), 0.75f, new Material(false, new Vector3(1, 0, 0), 0.5f, false)));
            Primitives.Add(new Sphere(new Vector3(2, 0, 5), 0.75f, new Material(false, new Vector3(0, 1, 0), 0, false)));

            Primitives.Add(new Triangle(new Vector3(-2.5f, -2, 4), new Vector3(2.5f, -2, 4), new Vector3(0, -1, 4.5f), new Material(false, new Vector3(0, 0, 1), 0.75f, false)));

            lightSources.Add(new Light(new Vector3(4, 3, 0.1f), new Vector3(25, 25, 25), new Vector3(1, 1, 1)));
            lightSources.Add(new Light(new Vector3(-4, 3, 0.1f), new Vector3(25, 25, 25), new Vector3(1, 1, 1)));
        }

        public Vector3 Intersect(ref Ray R, int recursion) // Intersects a ray with the world and returns the color of the screen pixel of that ray, by finding the first intersection and calling computeColor
        {
            Intersection First = new Intersection();
            First.Distance = 0;

            foreach(Primitive P in Primitives) // Find first intersection
            {
                Intersection temp = P.Intersect(R);
                if(temp.Distance > 0 && (temp.Distance < First.Distance || First.Distance == 0))
                {
                    First = temp;
                }
            }

            if(First.Distance == 0) // If no intersection is found (which should never happen)
            {
                return new Vector3(0, 0, 0);
            }

            R.t = First.Distance;
            R.intersection = First;

            if (First.Closest.material.isMirror && recursion < recursionDepth) // Find color for mirrors, with a cap on the recursion depth
            {
                Ray secondaryRay = new Ray();
                secondaryRay.Origin = First.intersectPoint;
                secondaryRay.Direction = R.Direction - 2 * (Vector3.Dot(R.Direction, R.intersection.Normal) * R.intersection.Normal);
                secondaryRay.t = 1e37f;
                R.secondary = secondaryRay;
                return computeColor(First, ref R) * Intersect(ref secondaryRay, recursion + 1);
            }
            return computeColor(First, ref R); // return the right color
        }

        
        Vector3 computeColor(Intersection I, ref Ray R) // Computes the color of a pixel by creating shadow rays for every light source and using the intersected primitives material (with diffuse, glossy and ambient)
        {
            Vector3 result = new Vector3(0, 0, 0);
            foreach (Light L in lightSources)
            {
                Ray shadowRay = new Ray();
                shadowRay.Direction = Vector3.Normalize(L.Location - I.intersectPoint);
                shadowRay.Origin = I.intersectPoint + shadowRay.Direction * 0.001f;
                shadowRay.t = (L.Location - I.intersectPoint).Length - 0.002f;
                bool intersection = false;
                foreach (Primitive P in Primitives) // Intersect shadowRays with the world, if there is an intersection, no diffuse reflection
                {
                    Intersection shadowIntersect = P.Intersect(shadowRay);
                    if(shadowIntersect.Distance > 0 && shadowIntersect.Distance < shadowRay.t)
                    {
                        shadowRay.t = shadowIntersect.Distance;
                        intersection = true;
                        break;
                    }
                }

                Vector3 materialColor = I.Closest.material.color;
                if(Vector3.Dot(I.Normal, shadowRay.Direction) <= 0) // Flip normal if it is facing away from camera
                {
                    I.Normal = -I.Normal; 
                }

                if (I.Closest.material.texture) // For the walls, check wheter the point is black or white
                {
                    materialColor = checkerBoard(I);
                }

                if(!intersection) // Diffuse reflection
                {
                    result += (L.Intensity * materialColor * Math.Max(0, Vector3.Dot(I.Normal, shadowRay.Direction))) / ((float)Math.Pow((double)(shadowRay.t + 0.002f), 2)); // Ereflected = Elight * kd * max(0, N * L) * 1/r^2 (Lecture 4)
                }
                if(I.Closest.material.glossy > 0 &! intersection) // Stochastic glossy reflection
                {
                    Vector3 r = shadowRay.Direction - (2 + stochasticChange) * (Vector3.Dot(shadowRay.Direction, I.Normal)) * I.Normal;
                    Vector3 v = Vector3.Normalize(I.intersectPoint - R.Origin);
                    result += (L.Intensity * (I.Closest.material.glossy * L.Color) * (float)Math.Pow((double)Math.Max(0, Vector3.Dot(v, r)), 5)) / ((float)Math.Pow((double)(shadowRay.t + 0.002f), 2)); // Ereflected = Elight * ks * max(0, V * R)^a * 1/r^2 (Lecture 4)
                }
                result += materialColor * I.Closest.ambient; // Ambient shading
                R.shadowRays.Add(shadowRay);
            }
            return result;
            
        }

        
        public Vector3 checkerBoard(Intersection I) // Returns black or white depending on the location of the intersection point, using (int(λ1)+(int)λ2) & 1 (Lecture 5) 
        {
            Plane plane = (Plane)I.Closest;
            Vector3 adjust = new Vector3(0, 0, 0);
            if (I.Normal == new Vector3(1, 0, 0))
                adjust = new Vector3(0, -5, 0);
            else if (I.Normal == new Vector3(-1, 0, 0))
                adjust = new Vector3(0, -5, 10);
            else if (I.Normal == new Vector3(0, 1, 0))
                adjust = new Vector3(-5, 0, 0);
            else if (I.Normal == new Vector3(0, -1, 0))
                adjust = new Vector3(-5, 0, 10);
            else if (I.Normal == new Vector3(0, 0, 1))
                adjust = new Vector3(5, -5, 0);
            else
                adjust = new Vector3(-5, -5, 0);

            Vector3 PZero = -I.Normal * plane.Distance + adjust;

            float t = (I.intersectPoint - PZero).X;
            float s = (I.intersectPoint - PZero).Y;
            if(t == 0)
            {
                t = (I.intersectPoint - PZero).Z;
            }
            else if(s == 0)
            {
                s = (I.intersectPoint - PZero).Z;
            }

            float color = (int)(Math.Round(t) + Math.Round(s)) & 1;
            return new Vector3(color, color, color);
        }

        static float randomFloat(float max, float min) // From https://www.codegrepper.com/code-examples/csharp/c%23+random+float+between+two+numbers to create a random float for stochastic sampling of glossy reflections
        {
            System.Random random = new System.Random();
            return (float)(random.NextDouble() * (max - min) + min);
        }

    }
}
