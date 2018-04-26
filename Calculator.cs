using UnityEngine;
using System.Collections;

public class Calculator : MonoBehaviour {

    public delegate void CameraControlDeligate(Vector3 pe);
    public event CameraControlDeligate CameraControl;
    
    private Vector3 origin;
    private Vector3 pa, pb, pc;
    private float aspectRatio;
    private float screenWidth, screenHeight;
    private float maxDepth;

	// Use this for initialization
    void Start()
    {
        SenseInput input = GameObject.Find("Sense Input").GetComponent<SenseInput>();
        CameraController camController = GameObject.Find("Main Camera").GetComponent<CameraController>();

        /* Set event handler */
        input.SetFaceData += SetFaceData;

        /*
         *   pc |----------------|
         *      |                |
         *      |     origin     |
         *      |                |
         *   pa |----------------| pb
         */
        pa = camController.pa;
        pb = camController.pb;
        pc = camController.pc;

        aspectRatio = (float) Screen.width / Screen.height;
        screenWidth = Vector3.Distance(pa, pb);
        screenHeight = screenWidth / aspectRatio;
        maxDepth = -1.5f;
        
        origin.x = (pa.x + pb.x) / 2.0f;
        origin.y = (pa.y + pc.y) / 2.0f;
        origin.z = 0.0f;
    }
    	
    void SetFaceData(PXCMFaceData.LandmarksData data, float depth)
    {
        Vector3 normalizedPos;

        PXCMFaceData.LandmarkPoint[] points;
        if (!data.QueryPoints(out points))
            return;

        /* Use center of two eyes as the control point */
        PXCMPointF32 rightEye = points[data.QueryPointIndex(PXCMFaceData.LandmarkType.LANDMARK_EYE_RIGHT_CENTER)].image;
        PXCMPointF32 leftEye = points[data.QueryPointIndex(PXCMFaceData.LandmarkType.LANDMARK_EYE_LEFT_CENTER)].image;

        float eye_x = Mathf.Abs(rightEye.x + leftEye.x) / 2.0f;
        float eye_y = Mathf.Abs(rightEye.y + leftEye.y) / 2.0f;

        normalizedPos.x = 1.0f - (eye_x / 640.0f);
        normalizedPos.y = 1.0f - (eye_y / 480.0f);
        normalizedPos.z = depth / 1500.0f;

        CalculateVariables(normalizedPos);
    }

    void CalculateVariables(Vector3 normalizedPos)
    {
        Vector3 cameraPos;
        float smoothTime = 0.03f;
        Vector3 velocity = Vector3.zero;

        cameraPos.x = origin.x + ((normalizedPos.x - 0.5f) * screenWidth);
        cameraPos.y = origin.y + ((normalizedPos.y - 0.5f) * screenHeight);
        cameraPos.z = maxDepth * normalizedPos.z;

        Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, cameraPos, ref velocity, smoothTime);
        transform.position = smoothedPos;

        CameraControl(smoothedPos);
    }
     
    void Update()
    {
        if (Input.GetKeyDown("escape"))  // Exit
            Application.Quit();
    }
}
