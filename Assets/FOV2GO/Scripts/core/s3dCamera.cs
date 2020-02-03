using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* s3dCamera.js revised 12/30/12. Usage:
 * Attach to camera. Creates, manages and renders stereoscopic view.
 * NOTE: interaxial is measured in millimeters; zeroPrlxDist is measured in meters 
 * Has companion Editor script to create custom inspector 
 */
// 1. Camera
 // left view camera
 // right view camera
 // black mask for mobile formats
 // mask for gui overlay for mobile formats
// Stereo Parameters
 // Distance (in millimeters) between cameras
 // Distance (in meters) at which left and right images overlap exactly
// 3D Camera Configuration // 
 // Angle cameras inward to converge. Bad idea!
// Camera Selection // 
//enum cams3D {Left_Right, Left_Only, Right_Only, Right_Left} // declared in s3dEnums.js
 // View order - swap cameras for cross-eyed free-viewing
//private var screenSize : Vector2;
// Options
 // Set a custom layer to use multiple stereo cameras
 // Camera will render this layer only
 // Enable layers seen by only one camera
 // Layer seen by left camera only
 // Layer seen by right camera only
 // Layer seen by gui camera only
 // For multiple stereo cameras - higher layers are rendered on top of lower layers
// 2. Render
// enable useStereoShader to use RenderTextures & stereo shader (Unity Pro only) for desktop applications - allows anaglyph format
// turn off for Unity Free, Android and iOS (allows side by side mode only)
 // track changes to useStereoShader
// Stereo Material + Stereo Shader (uses FOV2GO/Shaders/stereo3DViewMethods)
 // Assign FOV2GO/Materials/stereoMat material in inspector
// Render Textures
// 3D Display Mode // 
//enum mode3D {SideBySide, Anaglyph, OverUnder, Interlace, Checkerboard}; // declared in s3dEnums.js
// Anaglyph Mode
//enum anaType {Monochrome, HalfColor, FullColor, Optimized, Purple};  // declared in s3dEnums.js
// Side by Side Mode
// Over Under Mode
// Interlace Variables
[UnityEngine.AddComponentMenu("Stereoskopix/s3d Camera")]
[UnityEngine.ExecuteInEditMode]
public partial class s3dCamera : MonoBehaviour
{
    public GameObject leftCam;
    public GameObject rightCam;
    public GameObject maskCam;
    public GameObject guiCam;
    public float interaxial;
    public float zeroPrlxDist;
    public bool toedIn;
    public cams3D cameraSelect;
    public float H_I_T;
    public float offAxisFrustum;
    public GameObject depthPlane;
    public GameObject planeNear;
    public GameObject planeZero;
    public GameObject planeFar;
    public float horizontalFOV;
    public float verticalFOV;
    public bool useCustomStereoLayer;
    public int stereoLayer;
    public bool useLeftRightOnlyLayers;
    public int leftOnlyLayer;
    public int rightOnlyLayer;
    public int guiOnlyLayer;
    public int renderOrderDepth;
    public bool useStereoShader;
    private bool useStereoShaderPrev;
    public Material stereoMaterial;
    private RenderTexture leftCamRT;
    private RenderTexture rightCamRT;
    public mode3D format3D;
    public anaType anaglyphOptions;
    public bool sideBySideSqueezed;
    public bool overUnderStretched;
    public bool usePhoneMask;
    public Vector4 leftViewRect;
    public Vector4 rightViewRect;
    public int interlaceRows;
    public int checkerboardColumns;
    public int checkerboardRows;
    public Plane[] planes;

    private bool initialized;

    public virtual void Awake()
    {
        initStereoCamera();
    }

    public virtual void initStereoCamera()
    {
        SetupCameras();
        SetupShader();
        SetStereoFormat();
        s3dDepthInfo infoScript = null;
        infoScript = (s3dDepthInfo) GetComponent(typeof(s3dDepthInfo));
        if (infoScript)
        {
            SetupScreenPlanes(); // only create screen planes if necessary
        }
    }

    public virtual void SetupCameras()
    {
        Transform lcam = transform.Find("leftCam"); // check if we've already created a leftCam
        if (lcam)
        {
            leftCam = lcam.gameObject;
            leftCam.GetComponent<Camera>().CopyFrom(GetComponent<Camera>());
        }
        else
        {
            leftCam = new GameObject("leftCam", new System.Type[] {typeof(Camera)});
            leftCam.AddComponent(typeof(GUILayer));
            leftCam.GetComponent<Camera>().CopyFrom(GetComponent<Camera>());
            leftCam.transform.parent = transform;
        }
        Transform rcam = transform.Find("rightCam"); // check if we've already created a rightCam
        if (rcam)
        {
            rightCam = rcam.gameObject;
            rightCam.GetComponent<Camera>().CopyFrom(GetComponent<Camera>());
        }
        else
        {
            rightCam = new GameObject("rightCam", new System.Type[] {typeof(Camera)});
            rightCam.AddComponent(typeof(GUILayer));
            rightCam.GetComponent<Camera>().CopyFrom(GetComponent<Camera>());
            rightCam.transform.parent = transform;
        }
        Transform mcam = transform.Find("maskCam"); // check if we've already created a maskCam
        if (mcam)
        {
            maskCam = mcam.gameObject;
        }
        else
        {
            maskCam = new GameObject("maskCam", new System.Type[] {typeof(Camera)});
            maskCam.AddComponent(typeof(GUILayer));
            maskCam.GetComponent<Camera>().CopyFrom(GetComponent<Camera>());
            maskCam.transform.parent = transform;
        }
        Transform gcam = transform.Find("guiCam"); // check if we've already created a maskCam
        if (gcam)
        {
            guiCam = gcam.gameObject;
        }
        else
        {
            guiCam = new GameObject("guiCam", new System.Type[] {typeof(Camera)});
            guiCam.AddComponent(typeof(GUILayer));
            guiCam.GetComponent<Camera>().CopyFrom(GetComponent<Camera>());
            guiCam.transform.parent = transform;
        }
        GUILayer guiC = (GUILayer) GetComponent(typeof(GUILayer));
        guiC.enabled = false;
        GetComponent<Camera>().depth = -2; // rendering order (back to front): centerCam/maskCam/leftCam1/rightCam1/leftCam2/rightCam2/ etc
        horizontalFOV = (2 * Mathf.Atan(Mathf.Tan((GetComponent<Camera>().fieldOfView * Mathf.Deg2Rad) / 2) * GetComponent<Camera>().aspect)) * Mathf.Rad2Deg;
        verticalFOV = GetComponent<Camera>().fieldOfView;
        leftCam.GetComponent<Camera>().depth = (GetComponent<Camera>().depth + (renderOrderDepth * 2)) + 2;
        rightCam.GetComponent<Camera>().depth = (GetComponent<Camera>().depth + ((renderOrderDepth * 2) + 1)) + 3;
        if (useLeftRightOnlyLayers)
        {
            if (useCustomStereoLayer)
            {
                leftCam.GetComponent<Camera>().cullingMask = (1 << stereoLayer) | (1 << leftOnlyLayer); // show stereo + left only
                rightCam.GetComponent<Camera>().cullingMask = (1 << stereoLayer) | (1 << rightOnlyLayer); // show stereo + right only
            }
            else
            {
                leftCam.GetComponent<Camera>().cullingMask = ~((1 << rightOnlyLayer) | (1 << guiOnlyLayer)); // show everything but right only layer & mask only layer
                rightCam.GetComponent<Camera>().cullingMask = ~((1 << leftOnlyLayer) | (1 << guiOnlyLayer)); // show everything but left only layer & mask only layer
            }
        }
        else
        {
            if (useCustomStereoLayer)
            {
                leftCam.GetComponent<Camera>().cullingMask = 1 << stereoLayer; // show stereo layer only
                rightCam.GetComponent<Camera>().cullingMask = 1 << stereoLayer; // show stereo layer only
            }
        }
        maskCam.GetComponent<Camera>().depth = leftCam.GetComponent<Camera>().depth - 1;
        guiCam.GetComponent<Camera>().depth = rightCam.GetComponent<Camera>().depth + 1;
        maskCam.GetComponent<Camera>().cullingMask = 0;
        guiCam.GetComponent<Camera>().cullingMask = 1 << guiOnlyLayer; // only show what's in the guiOnly layer
        maskCam.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        guiCam.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
        maskCam.GetComponent<Camera>().backgroundColor = Color.black;
    }

    public virtual void SetupShader()
    {
        if (useStereoShader)
        {
            if (!leftCamRT || !rightCamRT)
            {
                leftCamRT = new RenderTexture(Screen.width, Screen.height, 24);
                rightCamRT = new RenderTexture(Screen.width, Screen.height, 24);
            }
            stereoMaterial.SetTexture("_LeftTex", leftCamRT);
            stereoMaterial.SetTexture("_RightTex", rightCamRT);
            leftCam.GetComponent<Camera>().targetTexture = leftCamRT;
            rightCam.GetComponent<Camera>().targetTexture = rightCamRT;
        }
        else
        {
            if (format3D == mode3D.SideBySide)
            {
                if (!usePhoneMask)
                {
                    leftCam.GetComponent<Camera>().rect = new Rect(0, 0, 0.5f, 1);
                    rightCam.GetComponent<Camera>().rect = new Rect(0.5f, 0, 0.5f, 1);
                }
                else
                {
                    leftCam.GetComponent<Camera>().rect = Vector4toRect(leftViewRect);
                    rightCam.GetComponent<Camera>().rect = Vector4toRect(rightViewRect);
                }
                leftViewRect = RectToVector4(leftCam.GetComponent<Camera>().rect);
                rightViewRect = RectToVector4(rightCam.GetComponent<Camera>().rect);
            }
            else
            {
                if (format3D == mode3D.OverUnder)
                {
                    if (!usePhoneMask)
                    {
                        leftCam.GetComponent<Camera>().rect = new Rect(0, 0.5f, 1, 0.5f);
                        rightCam.GetComponent<Camera>().rect = new Rect(0, 0, 1, 0.5f);
                    }
                    else
                    {
                        leftCam.GetComponent<Camera>().rect = Vector4toRect(leftViewRect);
                        rightCam.GetComponent<Camera>().rect = Vector4toRect(rightViewRect);
                    }
                    leftViewRect = RectToVector4(leftCam.GetComponent<Camera>().rect);
                    rightViewRect = RectToVector4(rightCam.GetComponent<Camera>().rect);
                }
                else
                {
                    MonoBehaviour.print("Unity Free only supports Side-by-Side and Over-Under modes!");
                }
            }
            fixCameraAspect();
        }
    }

    public virtual void SetStereoFormat()
    {
        switch (format3D)
        {
            case mode3D.SideBySide:
                if (useStereoShader)
                {
                    maskCam.GetComponent<Camera>().enabled = false;
                }
                else
                {
                    maskCam.GetComponent<Camera>().enabled = usePhoneMask;
                }
                break;
            case mode3D.Anaglyph:
                maskCam.GetComponent<Camera>().enabled = false;
                SetAnaType();
                break;
            case mode3D.OverUnder:
                if (useStereoShader)
                {
                    maskCam.GetComponent<Camera>().enabled = false;
                }
                else
                {
                    maskCam.GetComponent<Camera>().enabled = usePhoneMask;
                }
                break;
            case mode3D.Interlace:
                maskCam.GetComponent<Camera>().enabled = false;
                SetWeave(false);
                break;
            case mode3D.Checkerboard:
                maskCam.GetComponent<Camera>().enabled = false;
                SetWeave(true);
                break;
        }
    }

    public virtual void SetupScreenPlanes()
    {
        Transform screenTest = transform.Find("depthPlanes");
        if (depthPlane) // first make sure that user has assigned a depthPlane prefab
        {
            if (!screenTest)
            {
                planeZero = UnityEngine.Object.Instantiate(depthPlane, transform.position, transform.rotation);
                GameObject depthPlanes = new GameObject("depthPlanes");
                depthPlanes.transform.parent = transform;
                depthPlanes.transform.localPosition = Vector3.zero;
                planeZero.transform.parent = depthPlanes.transform;
                planeZero.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                planeZero.name = "screenDistPlane";
                planeNear = UnityEngine.Object.Instantiate(depthPlane, transform.position, transform.rotation);
                planeNear.transform.parent = depthPlanes.transform;
                planeNear.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                planeNear.name = "nearDistPlane";
                Shader shader1 = Shader.Find("Particles/Additive");
                planeNear.GetComponent<Renderer>().sharedMaterial = new Material(shader1);
                planeNear.GetComponent<Renderer>().sharedMaterial.mainTexture = depthPlane.GetComponent<Renderer>().sharedMaterial.mainTexture;
                planeNear.GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", Color.yellow);
                planeFar = UnityEngine.Object.Instantiate(depthPlane, transform.position, transform.rotation);
                planeFar.transform.parent = depthPlanes.transform;
                planeFar.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                planeFar.name = "farDistPlane";
                Shader shader2 = Shader.Find("Particles/Additive");
                planeFar.GetComponent<Renderer>().sharedMaterial = new Material(shader2);
                planeFar.GetComponent<Renderer>().sharedMaterial.mainTexture = depthPlane.GetComponent<Renderer>().sharedMaterial.mainTexture;
                planeFar.GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", Color.green);
            }
            else
            {
                planeZero = GameObject.Find("screenDistPlane");
                planeNear = GameObject.Find("nearDistPlane");
                planeFar = GameObject.Find("farDistPlane");
            }
            planeZero.GetComponent<Renderer>().enabled = false;
            planeNear.GetComponent<Renderer>().enabled = false;
            planeFar.GetComponent<Renderer>().enabled = false;
        }
    }

    // called from initStereoCamera (above), and from s3dGyroCam.js (when phone orientation is changed due to AutoRotation)
    public virtual void fixCameraAspect()
    {
        GetComponent<Camera>().ResetAspect();
        //yield WaitForSeconds(0.25);
        GetComponent<Camera>().aspect = GetComponent<Camera>().aspect * ((leftCam.GetComponent<Camera>().rect.width * 2) / leftCam.GetComponent<Camera>().rect.height);
        leftCam.GetComponent<Camera>().aspect = GetComponent<Camera>().aspect;
        rightCam.GetComponent<Camera>().aspect = GetComponent<Camera>().aspect;
    }

    public virtual Rect Vector4toRect(Vector4 v)
    {
        Rect r = new Rect(v.x, v.y, v.z, v.w);
        return r;
    }

    public virtual Vector4 RectToVector4(Rect r)
    {
        Vector4 v = new Vector4(r.x, r.y, r.width, r.height);
        return v;
    }

    public virtual void Update()
    {
        if (!useStereoShader)
        {
            if (UnityEditor.EditorApplication.isPlaying) // speeds up rendering while in play mode, but doesn't work if useStereoShader is true
            {
                GetComponent<Camera>().enabled = false;
            }
            else
            {
                GetComponent<Camera>().enabled = true; // need camera enabled when in edit mode
            }
        }
        if (useStereoShader)
        {
            if (useStereoShaderPrev == false)
            {
                initStereoCamera();
            }
        }
        else
        {
            if (useStereoShaderPrev == true)
            {
                releaseRenderTextures();
                SetupShader();
                SetStereoFormat();
            }
        }
        useStereoShaderPrev = useStereoShader;
        planes = GeometryUtility.CalculateFrustumPlanes(GetComponent<Camera>());
        if (Application.isPlaying)
        {
            if (!initialized)
            {
                initialized = true;
            }
        }
        else
        {
            initialized = false;
            SetupShader();
            SetStereoFormat();
        }
        UpdateView();
    }

    public virtual void UpdateView()
    {
        switch (cameraSelect)
        {
            case cams3D.Left_Right:
                leftCam.transform.position = transform.position + transform.TransformDirection(-interaxial / 2000f, 0, 0);
                rightCam.transform.position = transform.position + transform.TransformDirection(interaxial / 2000f, 0, 0);
                break;
            case cams3D.Left_Only:
                leftCam.transform.position = transform.position + transform.TransformDirection(-interaxial / 2000f, 0, 0);
                rightCam.transform.position = transform.position + transform.TransformDirection(-interaxial / 2000f, 0, 0);
                break;
            case cams3D.Right_Only:
                leftCam.transform.position = transform.position + transform.TransformDirection(interaxial / 2000f, 0, 0);
                rightCam.transform.position = transform.position + transform.TransformDirection(interaxial / 2000f, 0, 0);
                break;
            case cams3D.Right_Left:
                leftCam.transform.position = transform.position + transform.TransformDirection(interaxial / 2000f, 0, 0);
                rightCam.transform.position = transform.position + transform.TransformDirection(-interaxial / 2000f, 0, 0);
                break;
        }
        if (toedIn)
        {
            leftCam.GetComponent<Camera>().projectionMatrix = GetComponent<Camera>().projectionMatrix;
            rightCam.GetComponent<Camera>().projectionMatrix = GetComponent<Camera>().projectionMatrix;
            leftCam.transform.LookAt(transform.position + (transform.TransformDirection(Vector3.forward) * zeroPrlxDist));
            rightCam.transform.LookAt(transform.position + (transform.TransformDirection(Vector3.forward) * zeroPrlxDist));
        }
        else
        {
            leftCam.transform.rotation = transform.rotation;
            rightCam.transform.rotation = transform.rotation;
            switch (cameraSelect)
            {
                case cams3D.Left_Right:
                    leftCam.GetComponent<Camera>().projectionMatrix = setProjectionMatrix(true);
                    rightCam.GetComponent<Camera>().projectionMatrix = setProjectionMatrix(false);
                    break;
                case cams3D.Left_Only:
                    leftCam.GetComponent<Camera>().projectionMatrix = setProjectionMatrix(true);
                    rightCam.GetComponent<Camera>().projectionMatrix = setProjectionMatrix(true);
                    break;
                case cams3D.Right_Only:
                    leftCam.GetComponent<Camera>().projectionMatrix = setProjectionMatrix(false);
                    rightCam.GetComponent<Camera>().projectionMatrix = setProjectionMatrix(false);
                    break;
                case cams3D.Right_Left:
                    leftCam.GetComponent<Camera>().projectionMatrix = setProjectionMatrix(false);
                    rightCam.GetComponent<Camera>().projectionMatrix = setProjectionMatrix(true);
                    break;
            }
        }
    }

    // Calculate Stereo Projection Matrix
    public virtual Matrix4x4 setProjectionMatrix(bool isLeftCam)
    {
        float left = 0.0f;
        float right = 0.0f;
        float a = 0.0f;
        float b = 0.0f;
        float FOVrad = 0.0f;
        float tempAspect = GetComponent<Camera>().aspect;
        FOVrad = (GetComponent<Camera>().fieldOfView / 180f) * Mathf.PI;
        if (format3D == mode3D.SideBySide)
        {
            if (!sideBySideSqueezed)
            {
                tempAspect = tempAspect / 2; // if side by side unsqueezed, double width
            }
        }
        else
        {
            if (format3D == mode3D.OverUnder)
            {
                if (overUnderStretched)
                {
                    tempAspect = tempAspect / 4;
                }
                else
                {
                    tempAspect = tempAspect / 2;
                }
            }
        }
        a = GetComponent<Camera>().nearClipPlane * Mathf.Tan(FOVrad * 0.5f);
        b = GetComponent<Camera>().nearClipPlane / zeroPrlxDist;
        if (isLeftCam)
        {
            left = (((-tempAspect * a) + ((interaxial / 2000f) * b)) + (H_I_T / 100)) + (offAxisFrustum / 100);
            right = (((tempAspect * a) + ((interaxial / 2000f) * b)) + (H_I_T / 100)) + (offAxisFrustum / 100);
        }
        else
        {
            left = (((-tempAspect * a) - ((interaxial / 2000f) * b)) - (H_I_T / 100)) + (offAxisFrustum / 100);
            right = (((tempAspect * a) - ((interaxial / 2000f) * b)) - (H_I_T / 100)) + (offAxisFrustum / 100);
        }
        return PerspectiveOffCenter(left, right, -a, a, GetComponent<Camera>().nearClipPlane, GetComponent<Camera>().farClipPlane);
    }

    public virtual Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
    {
        Matrix4x4 m = default(Matrix4x4);
        float x = (2f * near) / (right - left);
        float y = (2f * near) / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -((2f * far) * near) / (far - near);
        float e = -1f;
        m[0, 0] = x;
        m[0, 1] = 0;
        m[0, 2] = a;
        m[0, 3] = 0;
        m[1, 0] = 0;
        m[1, 1] = y;
        m[1, 2] = b;
        m[1, 3] = 0;
        m[2, 0] = 0;
        m[2, 1] = 0;
        m[2, 2] = c;
        m[2, 3] = d;
        m[3, 0] = 0;
        m[3, 1] = 0;
        m[3, 2] = e;
        m[3, 3] = 0;
        return m;
    }

    public virtual void releaseRenderTextures()
    {
        leftCam.GetComponent<Camera>().targetTexture = null;
        rightCam.GetComponent<Camera>().targetTexture = null;
        leftCamRT.Release();
        rightCamRT.Release();
    }

    // Draw Scene Gizmos
    public virtual void OnDrawGizmos()//Gizmos.DrawWireCube (gizmoTarget, Vector3(screenSize.x,screenSize.y,0.01));
    {
        Vector3 gizmoLeft = transform.position + transform.TransformDirection(-interaxial / 2000f, 0, 0); // interaxial/2/1000mm
        Vector3 gizmoRight = transform.position + transform.TransformDirection(interaxial / 2000f, 0, 0);
        Vector3 gizmoTarget = transform.position + (transform.TransformDirection(Vector3.forward) * zeroPrlxDist);
        Gizmos.color = new Color(1, 1, 1, 1);
        Gizmos.DrawLine(gizmoLeft, gizmoTarget);
        Gizmos.DrawLine(gizmoRight, gizmoTarget);
        Gizmos.DrawLine(gizmoLeft, gizmoRight);
        Gizmos.DrawSphere(gizmoLeft, 0.02f);
        Gizmos.DrawSphere(gizmoRight, 0.02f);
        Gizmos.DrawSphere(gizmoTarget, 0.02f);
    }

    public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (useStereoShader)
        {
            RenderTexture.active = destination;
            GL.PushMatrix();
            GL.LoadOrtho();
            switch (format3D)
            {
                case mode3D.Anaglyph:
                    stereoMaterial.SetPass(0);
                    DrawQuad(0);
                    break;
                case mode3D.SideBySide:
                case mode3D.OverUnder:
                    int i = 1;
                    while (i <= 2)
                    {
                        stereoMaterial.SetPass(i);
                        DrawQuad(i);
                        i++;
                    }
                    break;
                case mode3D.Interlace:
                case mode3D.Checkerboard:
                    stereoMaterial.SetPass(3);
                    DrawQuad(3);
                    break;
                default:
                    break;
            }
            GL.PopMatrix();
        }
    }

    // Interlace & Checkerboard Modes
    public virtual void SetWeave(object xy)
    {
        if (xy != null)
        {
            stereoMaterial.SetFloat("_Weave_X", checkerboardColumns);
            stereoMaterial.SetFloat("_Weave_Y", checkerboardRows);
        }
        else
        {
            stereoMaterial.SetFloat("_Weave_X", 1);
            stereoMaterial.SetFloat("_Weave_Y", interlaceRows);
        }
    }

    // Anaglyph Mode
    public virtual void SetAnaType()
    {
        switch (anaglyphOptions)
        {
            case anaType.Monochrome:
                stereoMaterial.SetVector("_Balance_Left_R", new Vector4(0.299f, 0.587f, 0.114f, 0));
                stereoMaterial.SetVector("_Balance_Left_G", new Vector4(0, 0, 0, 0));
                stereoMaterial.SetVector("_Balance_Left_B", new Vector4(0, 0, 0, 0));
                stereoMaterial.SetVector("_Balance_Right_R", new Vector4(0, 0, 0, 0));
                stereoMaterial.SetVector("_Balance_Right_G", new Vector4(0.299f, 0.587f, 0.114f, 0));
                stereoMaterial.SetVector("_Balance_Right_B", new Vector4(0.299f, 0.587f, 0.114f, 0));
                break;
            case anaType.HalfColor:
                stereoMaterial.SetVector("_Balance_Left_R", new Vector4(0.299f, 0.587f, 0.114f, 0));
                stereoMaterial.SetVector("_Balance_Left_G", new Vector4(0, 0, 0, 0));
                stereoMaterial.SetVector("_Balance_Left_B", new Vector4(0, 0, 0, 0));
                stereoMaterial.SetVector("_Balance_Right_R", new Vector4(0, 0, 0, 0));
                stereoMaterial.SetVector("_Balance_Right_G", new Vector4(0, 1, 0, 0));
                stereoMaterial.SetVector("_Balance_Right_B", new Vector4(0, 0, 1, 0));
                break;
            case anaType.FullColor:
                stereoMaterial.SetVector("_Balance_Left_R", new Vector4(1, 0, 0, 0));
                stereoMaterial.SetVector("_Balance_Left_G", new Vector4(0, 0, 0, 0));
                stereoMaterial.SetVector("_Balance_Left_B", new Vector4(0, 0, 0, 0));
                stereoMaterial.SetVector("_Balance_Right_R", new Vector4(0, 0, 0, 0));
                stereoMaterial.SetVector("_Balance_Right_G", new Vector4(0, 1, 0, 0));
                stereoMaterial.SetVector("_Balance_Right_B", new Vector4(0, 0, 1, 0));
                break;
            case anaType.Optimized:
                stereoMaterial.SetVector("_Balance_Left_R", new Vector4(0, 0.7f, 0.3f, 0));
                stereoMaterial.SetVector("_Balance_Left_G", new Vector4(0, 0, 0, 0));
                stereoMaterial.SetVector("_Balance_Left_B", new Vector4(0, 0, 0, 0));
                stereoMaterial.SetVector("_Balance_Right_R", new Vector4(0, 0, 0, 0));
                stereoMaterial.SetVector("_Balance_Right_G", new Vector4(0, 1, 0, 0));
                stereoMaterial.SetVector("_Balance_Right_B", new Vector4(0, 0, 1, 0));
                break;
            case anaType.Purple:
                stereoMaterial.SetVector("_Balance_Left_R", new Vector4(0.299f, 0.587f, 0.114f, 0));
                stereoMaterial.SetVector("_Balance_Left_G", new Vector4(0, 0, 0, 0));
                stereoMaterial.SetVector("_Balance_Left_B", new Vector4(0, 0, 0, 0));
                stereoMaterial.SetVector("_Balance_Right_R", new Vector4(0, 0, 0, 0));
                stereoMaterial.SetVector("_Balance_Right_G", new Vector4(0, 0, 0, 0));
                stereoMaterial.SetVector("_Balance_Right_B", new Vector4(0.299f, 0.587f, 0.114f, 0));
                break;
        }
    }

    // Draw Render Textures Quads
    public virtual void DrawQuad(int cam)
    {
        if (format3D == mode3D.Anaglyph)
        {
            GL.Begin(GL.QUADS);
            GL.TexCoord3(0f, 0f, 0f);
            GL.Vertex3(0f, 0f, 0f);
            GL.TexCoord3(1f, 0f, 0f);
            GL.Vertex3(1f, 0f, 0f);
            GL.TexCoord3(1f, 1f, 0f);
            GL.Vertex3(1f, 1f, 0f);
            GL.TexCoord3(0f, 1f, 0f);
            GL.Vertex3(0f, 1f, 0f);
            GL.End();
        }
        else
        {
            if (format3D == mode3D.SideBySide)
            {
                if (cam == 1)
                {
                    GL.Begin(GL.QUADS);
                    GL.TexCoord2(0f, 0f);
                    GL.Vertex3(0f, 0f, 0.1f);
                    GL.TexCoord2(1f, 0f);
                    GL.Vertex3(0.5f, 0f, 0.1f);
                    GL.TexCoord2(1f, 1f);
                    GL.Vertex3(0.5f, 1f, 0.1f);
                    GL.TexCoord2(0f, 1f);
                    GL.Vertex3(0f, 1f, 0.1f);
                    GL.End();
                }
                else
                {
                    GL.Begin(GL.QUADS);
                    GL.TexCoord2(0f, 0f);
                    GL.Vertex3(0.5f, 0f, 0.1f);
                    GL.TexCoord2(1f, 0f);
                    GL.Vertex3(1f, 0f, 0.1f);
                    GL.TexCoord2(1f, 1f);
                    GL.Vertex3(1f, 1f, 0.1f);
                    GL.TexCoord2(0f, 1f);
                    GL.Vertex3(0.5f, 1f, 0.1f);
                    GL.End();
                }
            }
            else
            {
                if (format3D == mode3D.OverUnder)
                {
                    if (cam == 1)
                    {
                        GL.Begin(GL.QUADS);
                        GL.TexCoord2(0f, 0f);
                        GL.Vertex3(0f, 0.5f, 0.1f);
                        GL.TexCoord2(1f, 0f);
                        GL.Vertex3(1f, 0.5f, 0.1f);
                        GL.TexCoord2(1f, 1f);
                        GL.Vertex3(1f, 1f, 0.1f);
                        GL.TexCoord2(0f, 1f);
                        GL.Vertex3(0f, 1f, 0.1f);
                        GL.End();
                    }
                    else
                    {
                        GL.Begin(GL.QUADS);
                        GL.TexCoord2(0f, 0f);
                        GL.Vertex3(0f, 0f, 0.1f);
                        GL.TexCoord2(1f, 0f);
                        GL.Vertex3(1f, 0f, 0.1f);
                        GL.TexCoord2(1f, 1f);
                        GL.Vertex3(1f, 0.5f, 0.1f);
                        GL.TexCoord2(0f, 1f);
                        GL.Vertex3(0f, 0.5f, 0.1f);
                        GL.End();
                    }
                }
                else
                {
                    if ((format3D == mode3D.Interlace) || (format3D == mode3D.Checkerboard))
                    {
                        GL.Begin(GL.QUADS);
                        GL.TexCoord2(0f, 0f);
                        GL.Vertex3(0f, 0f, 0.1f);
                        GL.TexCoord2(1f, 0f);
                        GL.Vertex3(1, 0f, 0.1f);
                        GL.TexCoord2(1f, 1f);
                        GL.Vertex3(1, 1f, 0.1f);
                        GL.TexCoord2(0f, 1f);
                        GL.Vertex3(0f, 1f, 0.1f);
                        GL.End();
                    }
                }
            }
        }
    }

    public s3dCamera()
    {
        interaxial = 65;
        zeroPrlxDist = 3f;
        cameraSelect = cams3D.Left_Right;
        useLeftRightOnlyLayers = true;
        leftOnlyLayer = 20;
        rightOnlyLayer = 21;
        guiOnlyLayer = 22;
        format3D = mode3D.SideBySide;
        anaglyphOptions = anaType.HalfColor;
        usePhoneMask = true;
        leftViewRect = new Vector4(0, 0, 0.5f, 1);
        rightViewRect = new Vector4(0.5f, 0, 1, 1);
        interlaceRows = 1080;
        checkerboardColumns = 1920;
        checkerboardRows = 1080;
    }

}