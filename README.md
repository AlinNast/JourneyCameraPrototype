# JourneyCameraPrototype

## Camera Controller
In this project I experiment with a Camera controller that is independent both from the player and the camera. it works by accepting a Node as a look at target and it can also take different camera positions and interpolate between them
##### Todo
To improve this logic the camera positions can be Generalized in an Enum as well as the transitions between the different states. the Interface function can call these changes with the minimum of parameters.

## Camera 
The camera is independent from the controller. the instance is referenced and the controller takes the job of orienting and moving the camera. 
The current project is set up to work with a joystick. The left analogue controlls this orientation with a simple acceleration/deceleration pattern
##### Todo
Add option to invert axis

## Camera feel
Besides the player-controller-camera chain of comunication the controller does one more very important thing. It sends traces towards the camera and at different angles to create feel wiskers. These take control from the player and do slight adjustments in camera positioning. The exact intensity of these adjustment may vary depending of the genre or other subjective design choises 

In term of feel I found out these adjustments comming from the wiskers need to be invisible to the player. They do add significant improvement to gameography, and level design can keep this in consideration when building the arhitectural elements. However if the wiskers correct the camera postitioning too much the player might feel restrained of his power to look where he wants, or in the case of games that use aiming as a mechanic this might not be possible at all.
