using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
	// This class owns the scene, camera and display surface(s) and does the raytracing with the Render method
    class RayTracer
    {
        public Scene scene;
        public Camera camera;
        public Surface screen, debug, rt;
        public Vector3[] pixels;

        public RayTracer()
        {
			scene = new Scene();
			camera = new Camera();
            debug = new Surface(512, 512);
            debug.Clear(0x000000);
            rt = new Surface(512, 512);
            rt.Clear(0x000000);
            pixels = new Vector3[512 * 512];
            pixels[0].X = 1e37f;
			
		}

		
		public void Render() // Loops through the pixels of the screen plane, creating a Rays for all of them and intersecting them with the scene to get the right color. Also draws primitves, the screen plane and primary, secondary and shadow rays on the debug screen
		{
			debug.CopyTo(screen, 512, 0);
			rt.CopyTo(screen, 0, 0);
			debug.Clear(0x000000);

			// Prepare debug screen with primitives and light sources
			foreach(Light L in scene.lightSources)
            {
				debug.Plot(debugX(L.Location.X), debugY(L.Location.Z), 0xffff00);
            }
			foreach(Primitive P in scene.Primitives)
            {
				if(P.Type == 1) // Draw the spheres in debug
                {
					float tempX = 0, tempY = 0;
					int color = convertColor(P.material.color);
                    for(double t = 0; t < 100; t++)
                    {
						double angle = (2 * Math.PI * t) / 100;
						float x = (((Sphere)P).Radius * (float)Math.Cos(angle)) + ((Sphere)P).Position.X;
						float y = (((Sphere)P).Radius * (float)Math.Sin(angle)) + ((Sphere)P).Position.Z;
						
						if(t > 0)
                        {
							debug.Line(debugX(tempX), debugY(tempY), debugX(x), debugY(y), color);
                        }
						tempX = x;
						tempY = y;
                    }
					debug.Line(debugX(tempX), debugY(tempY), debugX(((Sphere)P).Radius + ((Sphere)P).Position.X), debugY(((Sphere)P).Position.Z), color);
                }
				if(P.Type == 2) // Draw the triangles in debug
                {
					Triangle T = (Triangle)P;
					int color = convertColor(T.material.color);
					debug.Line(debugX(T.Corners[0].X), debugY(T.Corners[0].Z), debugX(T.Corners[1].X), debugY(T.Corners[1].Z), color);
					debug.Line(debugX(T.Corners[0].X), debugY(T.Corners[0].Z), debugX(T.Corners[2].X), debugY(T.Corners[2].Z), color);
					debug.Line(debugX(T.Corners[2].X), debugY(T.Corners[2].Z), debugX(T.Corners[1].X), debugY(T.Corners[1].Z), color);
				}
            }

			Vector3 origin = camera.EyePos;
			for (int y = 0; y < 512; y++) // Create rays for every screen pixel
			{
				bool debugRay = false;
				if (y == 256)
				{
					debugRay = true;
				}

				for (int x = 0; x < 512; x++)
				{
					// Conversion to screen plane coordinates
					float u = (float)x / 512;
					float v = (float)y / 512;

					// Returns world space location of a point on the screen plane 
					Vector3 P = camera.planePoint(u, v);

					// Create ray from the camera through every point/pixel on the screen plane
					Ray ray = new Ray();
					ray.Origin = camera.EyePos;
					ray.Direction = Vector3.Normalize(P - ray.Origin);
					ray.t = 1e37f;

					// Intersect the ray with the world
					pixels[y*512 + x] = scene.Intersect(ref ray, 0);

					// Rays in the debug screen (primary in yellow, secondary in light blue and shadow in purple)
					if (debugRay && (x % 15 == 0))
					{
						Vector3 end;
						if (ray.t == 1e37f)
                        {
							end = ray.Origin + 20 * ray.Direction;
                        }
                        else
                        {
							end = ray.intersection.intersectPoint;
                        }
						debug.Line(debugX(end.X), debugY(end.Z), debugX(origin.X), debugY(origin.Z), 0xffff00);
						if (ray.intersection.Closest != null)
						{
							if (ray.intersection.Closest.Type != 0)
								foreach (Ray S in ray.shadowRays)
								{
									Vector3 shadowEnd = S.Origin + S.t * S.Direction;
									debug.Line(debugX(shadowEnd.X), debugY(shadowEnd.Z), debugX(S.Origin.X), debugY(S.Origin.Z), 0xff00ff);

								}
							if (ray.secondary != null)
							{
								debug.Line(debugX(ray.secondary.Origin.X), debugY(ray.secondary.Origin.Z), debugX(ray.secondary.intersection.intersectPoint.X), debugY(ray.secondary.intersection.intersectPoint.Z), 0x00ffff);
							}
						}
					}
				}
			}
			// Draw the screen plane as a red line on the debug screen
			debug.Line(debugX(camera.planeCorners[0].X), debugY(camera.planeCorners[0].Z), debugX(camera.planeCorners[1].X), debugY(camera.planeCorners[1].Z), 0xff0000);
			
			// Convert the float colors to int 
			for (int t = 0; t < (512 * 512); t++)
			{
				rt.pixels[t] = convertColor(pixels[t]);
			}
		}

		int convertColor(Vector3 floats) // Convert float value color to int value color
		{
			int red = Math.Min((int)(floats.X * 256), 255);
			int green = Math.Min((int)(floats.Y * 256), 255);
			int blue = Math.Min((int)(floats.Z * 256), 255);
			return (red << 16) + (green << 8) + blue;
		}

		
		// Convert world coordinates to debug screen coordinates
		int debugY(float y)
        {
			return (int)(((y * -1) + 10.5f) * (512 / 11));
        }

		int debugX(float x)
        {
			return (int)((x + 5.5f) * (512 / 11));
        }
    }
}
