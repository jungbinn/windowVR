using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Camera cam;
    public Vector3 pa, pb, pc;
    
	// Use this for initialization
    void Start()
    {
        Calculator calculator = GameObject.Find("Cameras").GetComponent<Calculator>();

        /* Set event handler */
        calculator.CameraControl += CameraControl;

        Cursor.visible = false;  // hide mouse cursor
    }

    void CameraControl(Vector3 pe)
    {
        CalculateFOV(pe);
        OffAxisProjection(pe);
    }

    void CalculateFOV(Vector3 pe)
    {
        float screenHeight = Vector3.Distance(pa, pc);
        float rad = Mathf.Atan(0.5f * screenHeight / (-pe.z));

        cam.fieldOfView = 2.0f * rad * Mathf.Rad2Deg;
    }

    void OffAxisProjection(Vector3 pe)
    {
        // Orthonormal basis vectors representing screen
        Vector3 vr, vu, vn;

        // right vector
        vr = pb - pa;
        vr.Normalize();

        // up vector
        vu = pc - pa;
        vu.Normalize();

        // normal vector
        vn = Vector3.Cross(vu, vr);
        vn.Normalize();

        // Screen extent vectors
        Vector3 va, vb, vc;

        // from eye positions (pe) to screen extents (pa, pb, pc)
        va = pa - pe;
        vb = pb - pe;
        vc = pc - pe;

        // Distance from eye position (pe) to the screen-space origin of screen plane
        float d = -Vector3.Dot(vn, va);

        // Frustrum extents on near plane
        float n, f, l, r, b, t;

        n = cam.nearClipPlane;
        f = cam.farClipPlane;
        l = Vector3.Dot(vr, va) * n / d;
        r = Vector3.Dot(vr, vb) * n / d;
        b = Vector3.Dot(vu, va) * n / d;
        t = Vector3.Dot(vu, vc) * n / d;

        // Perspective projection matrix
        Matrix4x4 P = new Matrix4x4();

        P[0, 0] = 2.0f * n / (r - l);
        P[0, 1] = 0.0f;
        P[0, 2] = (r + l) / (r - l);
        P[0, 3] = 0.0f;

        P[1, 0] = 0.0f;
        P[1, 1] = 2.0f * n / (t - b);
        P[1, 2] = (t + b) / (t - b);
        P[1, 3] = 0.0f;

        P[2, 0] = 0.0f;
        P[2, 1] = 0.0f;
        P[2, 2] = -(f + n) / (f - n);
        P[2, 3] = -2.0f * f * n / (f - n);

        P[3, 0] = 0.0f;
        P[3, 1] = 0.0f;
        P[3, 2] = -1.0f;
        P[3, 3] = 0.0f;

        // Align view with XY plane
        // Mt is transpose of M
        Matrix4x4 Mt = new Matrix4x4();

        Mt[0, 0] = vr.x;
        Mt[0, 1] = vr.y;
        Mt[0, 2] = vr.z;
        Mt[0, 3] = 0.0f;

        Mt[1, 0] = vu.x;
        Mt[1, 1] = vu.y;
        Mt[1, 2] = vu.z;
        Mt[1, 3] = 0.0f;

        Mt[2, 0] = vn.x;
        Mt[2, 1] = vn.y;
        Mt[2, 2] = vn.z;
        Mt[2, 3] = 0.0f;

        Mt[3, 0] = 0.0f;
        Mt[3, 1] = 0.0f;
        Mt[3, 2] = 0.0f;
        Mt[3, 3] = 1.0f;

        // Translate view to origin
        Matrix4x4 T = new Matrix4x4();

        T[0, 0] = 1.0f;
        T[0, 1] = 0.0f;
        T[0, 2] = 0.0f;
        T[0, 3] = -pe.x;

        T[1, 0] = 0.0f;
        T[1, 1] = 1.0f;
        T[1, 2] = 0.0f;
        T[1, 3] = -pe.y;

        T[2, 0] = 0.0f;
        T[2, 1] = 0.0f;
        T[2, 2] = 1.0f;
        T[2, 3] = -pe.z;

        T[3, 0] = 0.0f;
        T[3, 1] = 0.0f;
        T[3, 2] = 0.0f;
        T[3, 3] = 1.0f;

        cam.projectionMatrix = P;
        cam.worldToCameraMatrix = Mt * T;
    }
}
