using UnityEngine;
using System.Collections;

public class SenseInput : MonoBehaviour {

    public delegate void SetFaceDataDelegate(PXCMFaceData.LandmarksData data, float depth);

    public event SetFaceDataDelegate SetFaceData;

    private PXCMSenseManager sm = null;

	// Use this for initialization
	void Start ()
    {
        /* Initialize a PXCMSenseManager instance */
        sm = PXCMSenseManager.CreateInstance();
        if (sm == null)
        {
            Debug.Log("sm Failed");
            return;
        }

        /* Enable face detection and configure the face module */
        pxcmStatus sts = sm.EnableFace();
        PXCMFaceModule face = sm.QueryFace();
        if (face != null)
        {
            PXCMFaceConfiguration face_cfg = face.CreateActiveConfiguration();
            if (face_cfg != null)
            {
                face_cfg.landmarks.isEnabled = true;
                face_cfg.ApplyChanges();
                face_cfg.Dispose();
            }
            else { Debug.Log("face_cfg Failed"); }
        }
        else { Debug.Log("face Failed"); }

        /* Initialize the execution pipeline */
        sts = sm.Init();
        if (sts < pxcmStatus.PXCM_STATUS_NO_ERROR)
        {
            OnDisable();
            return;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        float depth;

        if (sm == null) 
            return;

        /* Wait until any frame data is available */
        if (sm.AcquireFrame(false, 0) < pxcmStatus.PXCM_STATUS_NO_ERROR)
            return;

        /* Retrieve face data if ready */
        PXCMFaceModule face = sm.QueryFace();
        if (face != null)
        {
            PXCMFaceData face_data = face.CreateOutput();
            face_data.Update();

            PXCMFaceData.Face face0 = face_data.QueryFaceByIndex(0);
            if (face0 != null)
            {
                /* XY position data */  
                PXCMFaceData.LandmarksData data = face0.QueryLandmarks();

                /* Depth data */
                PXCMFaceData.DetectionData faceDetectionData = face0.QueryDetection();

                if (data != null && faceDetectionData != null)
                {
                    faceDetectionData.QueryFaceAverageDepth(out depth);
                    SetFaceData(data, depth);
                }
            }
            face_data.Dispose();
        }

        /* Now, process the next frame */
        sm.ReleaseFrame();
	}

    void OnDisable()
    {
        if (sm == null) 
            return;
        sm.Dispose();
        sm = null;
    }
}
