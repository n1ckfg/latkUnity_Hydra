using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class s3dRotateHeading : MonoBehaviour
{
    /* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d Rotate Heading revised 12.30.12
 * Usage: Provides manual heading (rotation) control for mobile devices
 * Assign s3dTouchpad for input on mobile devices 
 * s3dTouchpad mimics touchpad input with mouse on desktop.
 * controlPitchInEditor = true: y movement controls look up/down in editor
 */
    public s3dTouchpad touchpad;
    // touchpad speed
    public Vector2 touchSpeed;
    public bool controlPitchInEditor;
    private s3dGyroCam gyroScript;
    public virtual void Awake()
    {
    }

    public virtual void Start()
    {
        gyroScript = (s3dGyroCam) gameObject.GetComponentInChildren(typeof(s3dGyroCam));
    }

    public virtual void Update()
    {
        gyroScript.heading = gyroScript.heading + (touchpad.position.x * touchSpeed.x);
        gyroScript.heading = gyroScript.heading % 360;
        if (controlPitchInEditor)
        {
            gyroScript.Pitch = gyroScript.Pitch - (touchpad.position.y * touchSpeed.y);
            gyroScript.Pitch = Mathf.Clamp(gyroScript.Pitch % 360, -60, 60);
        }
    }

    public s3dRotateHeading()
    {
        touchSpeed = new Vector2(1, 1);
        controlPitchInEditor = true;
    }

}