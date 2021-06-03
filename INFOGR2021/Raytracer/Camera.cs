using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Template
{
    // This class stores the location and direction of the camera, and calculates the corners of the screen plane
    // It also holds the function planePoint, which gives the location of a point on the screen plane given 2D coordinates
    // The FOV is stored as an angle in degrees, and used to calculate distance d from EyePos to the center of the screen plane
    // Two angles theta and phi are stored, to help calculate the EyeDir with Mouse control in the Application
    class Camera
    {
        // member variables
        public Vector3 EyePos, EyeDir;
        public Vector3[] planeCorners;
        public float FOV;
        public float d;
        public float theta; // The angle between the z-axis and EyeDir
        public float phi; // The angle between the x-axis and EyeDir's projection on the xy plane

        public Camera()
        {
            this.EyePos = new Vector3(0, 0, 0.1f);
            this.EyeDir = new Vector3(0, 0, 1);
            this.theta = 0;
            this.phi = 0;
            this.FOV = 70;
            changeCamera(EyePos, EyeDir, 70);
        }

        void calcutateCorners() // Calculates the worldspace coordinates of 3 of the screen plane corners
        {
            this.d = 0.5f / (float)Math.Tan(FOV * 0.5f * (Math.PI/180));
            Vector3 center = EyePos + d * EyeDir;
            planeCorners = new Vector3[3];

            Vector3 u;
            if (EyeDir == new Vector3(1, 0, 0))
                u = Vector3.Cross(EyeDir, new Vector3(0, 0, -1));
            else
                u = Vector3.Cross(EyeDir, new Vector3(1, 0, 0));

            Vector3 v = Vector3.Cross(EyeDir, u);

            u = Vector3.Normalize(u);
            v = Vector3.Normalize(v);
            planeCorners[0] = center + 0.5f * u + 0.5f * v;
            planeCorners[1] = center + 0.5f * u - 0.5f * v;
            planeCorners[2] = center - 0.5f * u + 0.5f * v;
        }

        public Vector3 planePoint(float a, float b) // Finds a 3D worldspace point on the screenplane given the 2D screenspave coordinates, used to create the rays from the camera into the world
        {
            Vector3 u = Vector3.Normalize(planeCorners[1] - planeCorners[0]);
            Vector3 v = Vector3.Normalize(planeCorners[2] - planeCorners[0]);
            return planeCorners[0] + a * u + b * v;
        }

        public void changeCamera(Vector3 newPos, Vector3 newDir, float newFOV) // Can change the position, direction and FOV of the camera and then calculates the new corners of the screen plane. Called from the contructor and from the Application after a key or mousebutton was pressed
        {
            this.EyePos = newPos;
            this.EyeDir = Vector3.Normalize(newDir);
            this.FOV = newFOV % 180;

            if(FOV == 0)
            {
                FOV = 1;
            }
            EyePos.X = Math.Min(5, EyePos.X);
            EyePos.Y = Math.Min(5, EyePos.Y);
            EyePos.Z = Math.Min(10, EyePos.Z);
            EyePos.X = Math.Max(-5, EyePos.X);
            EyePos.Y = Math.Max(-5, EyePos.Y);
            EyePos.Z = Math.Max(0, EyePos.Z);

            calcutateCorners();
        }

    }
}
