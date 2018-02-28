# Augmented Reality Teapot Shooter in Unity

![Animated](anim.gif)

A small demo application that implements an Augmented Reality teapot shooter
with Unity and ARCore. You can use this application to shoot teapots in a real room.
You must wait a little until it has tracked the room, and then you can start
shooting by simply tapping the screen.

This demo is mostly just a modification of the `HelloAR` sample. The files that contain
the main logic, is in the files `Assets/GoogleARCore/Examples/HelloAR/Scripts/HelloARController.cs`
and `Assets/GoogleARCore/Examples/HelloAR/Scripts/SurfaceManager.cs`

The implementation is pretty simple: the app simply creates collision shapes from
the planes tracked by ARCore, and these are used by the Unity physics engine
for the collision between the teapots and the planes.

A compiled apk is provided as `teapot_shooter.apk`.