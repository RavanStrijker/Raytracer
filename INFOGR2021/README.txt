Ravan Strijker - 7257642

Architecture:
All source files (except surface, template and application) can be found in the Raytracer file within the root of the file.
All fundamental elements are implemented as described in the assignment description. Additionally there is a Material class that holds information about a Primitive's material (color, texture, glossiness), a Ray class that holds information about a ray (primary, secondary or shadow), and the bonus feature Triangle class. 

Mouse/keyboard controls:
As explained in the comments in the Application class, mouse and keyboard controls are implemented.
'w' and 's' to move along the Z-direction, 'a' and 'd' to move along the X-direction, 'q' and 'e' to move along the Y-direction. 'r' and 'f' to change to FOV of the camera. 't' and 'g' to change to recursion depth cap for mirror reflections. 
The left and right mouse buttons to tilt the camera to the left or right and scrolling to tilt the camera up or down. 

Bonus features:
Triangle support (without normal interpolation) - additional to a Sphere and Plane class, the code has a Triangle class that is created by defining it's three corners, it overrides the intersect method from the Primitive class.
One blue triangle is created in the Scene class and added to the world. Triangles also work with the debug view.

Stochastic sampling of glossy reflections - every Primitive is given a material with a specular coefficient called 'glossy'. This is a float with value 0 <= 'glossy' <= 1.
In the computeColor method in the Scene class, glossy reflection is implemented within the following if-statement: 
// if(I.Closest.material.glossy > 0 &! intersection)
Where I is the intersection between a ray and a primitive, and closest is that primitive. 
Ereflected for glossy reflections is added to the resulting color with this formula:
Ereflected = Elight * ks * max(0, V * R)^a * 1/r^2  (as discussed in lecture 4 of INFOGR 2021)
The reflection vector is randomized to achieve stochastic sampling, by adding a random 'stochasticChange' float value between -0.2f and 0.2f to it's calculation.

Textured skybox (not HDR) - a textured box around the primitives is created by adding six planes to the world. They all have a checkerboard texture. The color for each point is calculated in the checkerBoard method in the Scene class, using (int(λ1)+(int)λ2) & 1 as discussed in lecture 5 of INFOGR 2021.  
The bonus feature in the assignment said skydome, so I am not sure if this skybox should give extra points.

Used materials:
Ideas/formulas in Lecture 3, 4 and 5 as well as the 'Getting started with raytracing' lecture on may 20th of the INFOGR 2021 course were used to implement the plane and sphere intersection methods, the color calculation with regard to diffuse, glossy and mirror reflection and ambient shading, and the checkerboard texture of the skybox.

Apart from that, the code on the following web page was used to implement stochastic sampling of glossy reflections, by getting a randomized float between two numbers:
https://www.codegrepper.com/code-examples/csharp/c%23+random+float+between+two+numbers

And the following pdf (page 3 and 4) was used to calculate the intersection between a ray and a triangle:
https://courses.cs.washington.edu/courses/csep557/10au/lectures/triangle_intersection.pdf