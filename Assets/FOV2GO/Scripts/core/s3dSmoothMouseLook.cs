using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public partial class s3dSmoothMouseLook : MonoBehaviour
{
    /* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d Smooth Mouse Look revised 12.30.12
 * javascript version for Stereoskopix package
 * Usage: Replace standard Unity MouseLook character controller script with this script.
 * Provides smoother mouse movement, by default only active when mouse button is down 
 * Uncheck MouseDownRequired to make always active.
 * Only active on desktop, automatically disabled on iOS and Android.
 * Note: conflicts to some extent with s3dTouchpad system, because this script uses entire screen for input, so touchpad movement triggers it
 */
    //enum Axes {MouseXandY, MouseX, MouseY} - declared in s3denums.js
    public Axes Axis;
    public bool MouseDownRequired;
    public float frameCounter;
    private Quaternion originalRotation;
    public float sensitivityX;
    public float sensitivityY;
    public float minimumX;
    public float maximumX;
    public float minimumY;
    public float maximumY;
    private float rotationX;
    private float rotationY;
    private List<float> rotArrayX;
    private float rotAverageX;
    private List<float> rotArrayY;
    private float rotAverageY;
    private Quaternion xQuaternion;
    private Quaternion yQuaternion;
    public virtual void Update()
    {
        float tempFloat = 0.0f;
        if (Axis == Axes.MouseXandY)
        {
            rotAverageY = 0;
            rotAverageX = 0;
            if (Input.GetMouseButton(0) || !MouseDownRequired)
            {
                rotationX = rotationX + (Input.GetAxis("Mouse X") * sensitivityX);
                rotationY = rotationY + (Input.GetAxis("Mouse Y") * sensitivityY);
            }
            rotArrayY.Add(rotationY);
            rotArrayX.Add(rotationX);
            if (rotArrayY.Count >= frameCounter)
            {
                rotArrayY.RemoveAt(0);
            }
            if (rotArrayX.Count >= frameCounter)
            {
                rotArrayX.RemoveAt(0);
            }
            int j = 0;
            while (j < rotArrayY.Count)
            {
                tempFloat = (float) rotArrayY[j];
                rotAverageY = rotAverageY + tempFloat;
                j++;
            }
            int i = 0;
            while (i < rotArrayX.Count)
            {
                tempFloat = (float) rotArrayX[i];
                rotAverageX = rotAverageX + tempFloat;
                i++;
            }
            rotAverageY = rotAverageY / rotArrayY.Count;
            rotAverageX = rotAverageX / rotArrayX.Count;
            rotAverageY = Mathf.Clamp(rotAverageY, minimumY, maximumY);
            rotAverageX = Mathf.Clamp(rotAverageX % 360, minimumX, maximumX);
            yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
            xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);
            transform.localRotation = (originalRotation * xQuaternion) * yQuaternion;
        }
        else
        {
            if (Axis == Axes.MouseX)
            {
                rotAverageX = 0;
                if (Input.GetMouseButton(0) || !MouseDownRequired)
                {
                    rotationX = rotationX + (Input.GetAxis("Mouse X") * sensitivityX);
                }
                rotArrayX.Add(rotationX);
                if (rotArrayX.Count >= frameCounter)
                {
                    rotArrayX.RemoveAt(0);
                }
                int i = 0;
                while (i < rotArrayX.Count)
                {
                    tempFloat = (float) rotArrayX[i];
                    rotAverageX = rotAverageX + tempFloat;
                    i++;
                }
                rotAverageX = rotAverageX / rotArrayX.Count;
                rotAverageX = Mathf.Clamp(rotAverageX % 360, minimumX, maximumX);
                xQuaternion = Quaternion.AngleAxis(rotAverageX, Vector3.up);
                transform.localRotation = originalRotation * xQuaternion;
            }
            else
            {
                rotAverageY = 0;
                if (Input.GetMouseButton(0) || !MouseDownRequired)
                {
                    rotationY = rotationY + (Input.GetAxis("Mouse Y") * sensitivityY);
                }
                rotArrayY.Add(rotationY);
                if (rotArrayY.Count >= frameCounter)
                {
                    rotArrayY.RemoveAt(0);
                }
                int j = 0;
                while (j < rotArrayY.Count)
                {
                    tempFloat = (float) rotArrayY[j];
                    rotAverageY = rotAverageY + tempFloat;
                    j++;
                }
                rotAverageY = rotAverageY / rotArrayY.Count;
                rotAverageY = Mathf.Clamp(rotAverageY % 360, minimumY, maximumY);
                yQuaternion = Quaternion.AngleAxis(rotAverageY, Vector3.left);
                transform.localRotation = originalRotation * yQuaternion;
            }
        }
    }

    public virtual void Start()
    {
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
        originalRotation = transform.localRotation;
    }

    public s3dSmoothMouseLook()
    {
        Axis = Axes.MouseXandY;
        MouseDownRequired = true;
        frameCounter = 20;
        sensitivityX = 1f;
        sensitivityY = 1f;
        minimumX = -360f;
        maximumX = 360f;
        minimumY = -60f;
        maximumY = 60f;
        rotArrayX = new List<float>();
        rotArrayY = new List<float>();
    }

}