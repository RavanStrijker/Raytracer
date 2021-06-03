using OpenTK;
using System;
using System.Collections.Generic;

namespace Template
{
	// This class owns the raytracer and enables interactivity
	class Application
	{
		public RayTracer raytracer;
		public OpenTKApp parent;

		public void Init() // Initialize application by adding the raytracer and event listeners
		{
			raytracer = new RayTracer();
            parent.KeyPress += KeyPress;
            parent.MouseDown += MouseDown;
            parent.MouseWheel += MouseWheel;
			
		}

		public void Tick() // tick: renders one frame
		{
			raytracer.Render();
		}


		void MouseWheel(object sender, OpenTK.Input.MouseWheelEventArgs e) // Enables mouse handling of the camera view direction for looking up or down
		{
			Camera c = raytracer.camera;
			float change = 0;
			if (c.theta > 0)
            {
				change = -e.DeltaPrecise * 5 * (float)(Math.PI / 180);

			}
			else
			{
				change = e.DeltaPrecise * 5 * (float)(Math.PI / 180);
			}

			Vector3 newDir = new Vector3((float)(Math.Sin(c.theta) * Math.Cos(c.phi + change)), (float)(Math.Sin(c.theta) * Math.Sin(c.phi + change)), (float)(Math.Cos(c.theta)));
			c.phi += change;
			changeCamera(new Vector3(0, 0, 0), newDir, 0);
        }

        void MouseDown(object sender, OpenTK.Input.MouseButtonEventArgs e) // Enables mouse handling of the camera view direction for looking left or right 
		{
			Camera c = raytracer.camera;
			float change = 0;
			if (e.Button == OpenTK.Input.MouseButton.Left)
			{
				change = -2.5f * (float)(Math.PI / 180);
				
			}
			else if (e.Button == OpenTK.Input.MouseButton.Right)
            {
				change = 2.5f * (float)(Math.PI / 180);
			}
			Vector3 newDir = new Vector3((float)(Math.Sin(c.theta + change) * Math.Cos(c.phi)), (float)(Math.Sin(c.theta + change) * Math.Sin(c.phi)), (float)(Math.Cos(c.theta + change)));
			c.theta += change;
			changeCamera(new Vector3(0, 0, 0), newDir, 0);
		}

        void KeyPress(object sender, KeyPressEventArgs e) // Enables keyboard handling of the camera position
        {
            switch (e.KeyChar)
            {
				case 'w': // When 'w' is pressed, the camera moves forward in the Z direction
					changeCamera(new Vector3(0, 0, 1), new Vector3(0, 0, 0), 0);
					break;
				case 's': // When 's' is pressed, backward in the Z direction
					changeCamera(new Vector3(0, 0, -1), new Vector3(0, 0, 0), 0);
					break;
				case 'a': // For 'a' and 'd' it is for- or backward in the X direction
					changeCamera(new Vector3(-1, 0, 0), new Vector3(0, 0, 0), 0);
					break;
				case 'd':
					changeCamera(new Vector3(1, 0, 0), new Vector3(0, 0, 0), 0);
					break;
				case 'q': // And for 'q' and 'e' in the Y direction
					changeCamera(new Vector3(0, 1, 0), new Vector3(0, 0, 0), 0);
					break;
				case 'e':
					changeCamera(new Vector3(0, -1, 0), new Vector3(0, 0, 0), 0);
					break;
				case 'r': // When 'r' is pressed, the FOV increases with 2.5f degrees
					changeCamera(new Vector3(0, 0, 0), new Vector3(0, 0, 0), 2.5f);
					break; // When 'f' is pressed, the FOV decreases
				case 'f':
					changeCamera(new Vector3(0, 0, 0), new Vector3(0, 0, 0), -2.5f);
					break;
				case 't': // When 't' is pressed, the maximum recursion depth for mirror reflections increases
					raytracer.scene.recursionDepth += 1;
					break;
				case 'g': // When 'g' is pressed the maximum recursion depth decreases
					raytracer.scene.recursionDepth -= 1;
					break;
			}
        }

		void changeCamera(Vector3 pos, Vector3 dir, float angle) // Changes the position, view direction or FOV of the camera depending on the key- or mousepress
        {
			Camera c = raytracer.camera;
			if(dir == new Vector3(0, 0, 0))
            {
				dir = c.EyeDir;
            }
			c.changeCamera(c.EyePos + pos * 0.25f, dir, c.FOV + angle);
        }
	}
}