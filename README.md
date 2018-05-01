# Window VR
## What's Window VR?
Window VR is a kind of virtual reality, but you don't need any head mount devices. By some method, it tracks the user's position and renders the monitor display refers to it. It will looks like a virtual world inside the monitor, just like a window.

I refered to [Johnny Chung Lee's project](https://www.youtube.com/watch?v=Jd3-eiid-Uw). He tracked the user's position by Wii remote.
  
  
## Environment
- Intel RealSense camera (SR300)
- Intel RealSense SDK
- Unity 5.3.5
  
  
## Implementation
### Face Tracking
`SenseInput.cs` script tracks the user's face using Intel RealSense SDK. Enable face detection and get the data by `PXCMFaceModule.PXCMFaceData.LandmarksData.QueryLandmarks()`. Extract two eyes' position and use their average value as a representative value of the face. `LandmarksData` only has XY position values, so we additionally need to extract the depth data. `PXCMFaceModule.PXCMFaceData.DetectionData.QueryFaceAverageDepth()` gives the average depth value of the face.
  
### Adjust Camera View
`Calculator.cs` script calculates the position that the camera should locate, using face position data and the information about the monitors. `OffAxisProjection()` does matrix calculations to adjust the camera's frustum.

We need three matrices.

![Alt text](https://github.com/jungbinn/windowVR/blob/master/images/p.png)  
![Alt text](https://github.com/jungbinn/windowVR/blob/master/images/m.png)  
![Alt text](https://github.com/jungbinn/windowVR/blob/master/images/t.png)  

By setting camera matrices like this, we can implement off-axis projection:
```
Camera.projectionMatrix = P;
Camera.worldToCameraMatrix = Mt * T;
```
while Mt is a transpose of M.  
  
## Result
These are the demo videos of this project:

Single monitor: <https://youtu.be/0jk94qyTluo>  
Multi monitor: <https://youtu.be/swNLzO3btKM>
