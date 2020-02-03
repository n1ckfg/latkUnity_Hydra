using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d Gyro Cam Script revised 12.30.12
 * Gyroscope-controlled camera for iPhone & Android revised 5.12.12
 * Usage: Attach this script to main camera.
 * Note: Unity Remote does not currently support gyroscope. 
 * This script uses three techniques to get the correct orientation out of the gyroscope attitude:
   1. creates a parent transform (camParent) and rotates it with eulerAngles
   2. for Android (Samsung Galaxy Nexus) only: remaps gyro.Attitude quaternion values from xyzw to wxyz (quatMap)
   3. multiplies attitude quaternion by quaternion quatMult
 * Also creates a grandparent (camGrandparent) which can be rotated to change heading
 * This node allows an arbitrary heading to be added to the gyroscope reading
   so that the virtual camera can be facing any direction in the scene, no matter which way the phone is actually facing
 * Option for direct touch input - horizontal swipe controls heading (leave OFF if using s3dRotateHeading.js)
 * Note: As of Unity 3.5.2: for correct operation, in Player Settings, Default Rotation has to be set to Auto Rotation
 * So this script checks (once per second) if screen orientation has changed while running
 */
// camera grandparent node to rotate heading
// mouse/touch input
[UnityEngine.AddComponentMenu("Stereoskopix/s3d Gyro Cam")]
public partial class s3dGyroCam : MonoBehaviour
{
    public static bool gyroBool;
    private Gyroscope gyro;
    private Compass compass;
    private Quaternion quatMult;
    private Quaternion quatMap;
    private ScreenOrientation prevScreenOrientation;
    private GameObject camParent;
    private GameObject camGrandparent;
    public float heading;
    public float Pitch;
    public bool setZeroToNorth;
    public bool checkForAutoRotation;
    public bool touchRotatesHeading;
    private float headingAtTouchStart;
    private float pitchAtTouchStart;
    private Vector2 mouseStartPoint;
    private Vector2 screenSize;
    public virtual void Awake()
    {
        // find the current parent of the camera's transform
        Transform currentParent = this.transform.parent;
        // instantiate a new transform
        this.camParent = new GameObject("camParent");
        // match the transform to the camera position
        this.camParent.transform.position = this.transform.position;
        // make the new transform the parent of the camera transform
        this.transform.parent = this.camParent.transform;
        // instantiate a new transform
        this.camGrandparent = new GameObject("camGrandParent");
        // match the transform to the camera position
        this.camGrandparent.transform.position = this.transform.position;
        // make the new transform the grandparent of the camera transform
        this.camParent.transform.parent = this.camGrandparent.transform;
        // make the original parent the great grandparent of the camera transform
        this.camGrandparent.transform.parent = currentParent;
        // check whether device supports gyroscope
        s3dGyroCam.gyroBool = SystemInfo.supportsGyroscope;
        if (s3dGyroCam.gyroBool)
        {
            this.prevScreenOrientation = Screen.orientation;
            this.gyro = Input.gyro;
            this.gyro.enabled = true;
            if (this.setZeroToNorth)
            {
                this.compass = Input.compass;
                this.compass.enabled = true;
            }
            this.fixScreenOrientation();
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public virtual void Start()
    {
        if (s3dGyroCam.gyroBool)
        {
            if (this.setZeroToNorth)
            {
                this.StartCoroutine(this.turnToFaceNorth());
            }
        }
        this.screenSize.x = Screen.width;
        this.screenSize.y = Screen.height;
    }

    public virtual IEnumerator turnToFaceNorth()
    {
        yield return new WaitForSeconds(1);
        this.heading = Input.compass.magneticHeading;
    }

    public virtual void Update()
    {
        if (s3dGyroCam.gyroBool)
        {
            this.transform.localRotation = this.quatMap * this.quatMult;
        }
        if (this.touchRotatesHeading)
        {
            this.GetTouchMouseInput();
        }

        {
            float _49 = this.heading;
            Vector3 _50 = this.camGrandparent.transform.localEulerAngles;
            _50.y = _49;
            this.camGrandparent.transform.localEulerAngles = _50;
        }

        {
            float _51 = // only update pitch if in Unity Editor (on device, pitch is handled by gyroscope)
            this.Pitch;
            Vector3 _52 = this.transform.localEulerAngles;
            _52.x = _51;
            this.transform.localEulerAngles = _52;
        }
    }

    public virtual void checkAutoRotation()
    {
        // check if Screen.orientation has changed
        if (this.prevScreenOrientation != Screen.orientation)
        {
            // fix gyroscope orientation settings
            this.fixScreenOrientation();
            // also need to fix camera aspect
            this.StartCoroutine(this.fixCameraAspect());
        }
        this.prevScreenOrientation = Screen.orientation;
    }

    public virtual IEnumerator fixCameraAspect()
    {
        s3dCamera theCamera = null;
        yield return new WaitForSeconds(1);
        theCamera = (s3dCamera) this.GetComponent(typeof(s3dCamera));
        if (!theCamera.useStereoShader)
        {
            if (theCamera)
            {
                theCamera.fixCameraAspect();
            }
        }
    }

    public virtual void fixScreenOrientation()
    {
    }

    public virtual void GetTouchMouseInput()
    {
        Vector2 delta = default(Vector2);
        if (Input.GetMouseButtonDown(0))
        {
            this.mouseStartPoint = Input.mousePosition;
            this.headingAtTouchStart = this.heading;
            this.pitchAtTouchStart = this.Pitch;
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Input.mousePosition;
                delta.x = (mousePos.x - this.mouseStartPoint.x) / this.screenSize.x;
                this.heading = this.headingAtTouchStart + (delta.x * 100);
                this.heading = this.heading % 360;
                delta.y = (mousePos.y - this.mouseStartPoint.y) / this.screenSize.y;
                this.Pitch = this.pitchAtTouchStart + (delta.y * -100);
                this.Pitch = Mathf.Clamp(this.Pitch % 360, -60, 60);
            }
        }
    }

    public s3dGyroCam()
    {
        this.setZeroToNorth = true;
    }

}