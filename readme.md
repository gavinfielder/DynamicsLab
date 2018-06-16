
# DynamicsLab
DynamicsLab is a dynamical systems numerical solver and VR simulator. It can accept user-defined systems of differential equations and render or animate their behavior.

This is a Unity 5.6 project. It is unverified with the newer editions of Unity.

![img](https://i.imgur.com/rRrcOry.png)
## Features
 - Uses Runge-Kutta method of order 4 with a data resolution of 64 data points per second.
 - Can visualize ODE dynamical systems with moving objects and trails.
 - Up to second order and up to three dimensions.
 - Can handle nonlinear, nonhomogenous, and non-autonomous systems. 
 - Can handle all basic math functions within expressions: PI, e, sqrt, sin, cos, tan, asin, acos, atan, atan2, abs, ln, floor, ceiling, round, min, max, rnd. 
 - Support for user-defined parameters.
 - Has a built-in spring-mass system as a predefined problem.
 - Can load problems from file. 
 - VR Capable. Tested on the HTC Vive. 
 - Can dump solution data to a csv file.
 - For first order problems, can visualize a 3D vector field which updates in real time.
 - Can pause, play, speed up, slow down simulation speed, or control with a video-like slider. 
 - Solution data grows dynamically, so simulations can run indefinitely. 
## Limitations
Mathematical issues with the user-defined systems themselves are unchecked--the software does not know when the solution encounters an unstable equilibrium, singularity, or other feature that damages integrity of the numerical solution. This may sometimes result in objects going to infinity or NaN, or it sometimes might not--at this point, it is up to the user to determine whether the dynamical system may have a problem using the Runge-Kutta method.

Saving problems is not yet implemented. 
## Known issues
 - Occasionally after adding/removing objects at different times during playback and then resetting, some objects will have data out of bounds error.
 - Graphical bugs on object trails on the built executable.
 - Expression validation doesn't catch unhandled functions. Users can also type nonsense functions. This can also result in hard-to-catch typos such as y(y-x+1), which should be y*(y-x+1) to avoid the parser treating y as an undefined function. 
 - Users can list reserved system variables as user-defined parameters, which has undefined behavior.
## Video Library
https://www.youtube.com/watch?v=n4b2EXpBkk0
https://www.youtube.com/watch?v=iGbwT3sJTn8
https://www.youtube.com/watch?v=Jw1-l80Kvmc
## Technical
Design documentation is located in ~/documentation/

## Acknowledgements
The original team of software engineers at CSU Chico includes Alex Elson, Gavin Fielder, Ryan Lynn, Matthew Johnson , and Joshua Thurston.