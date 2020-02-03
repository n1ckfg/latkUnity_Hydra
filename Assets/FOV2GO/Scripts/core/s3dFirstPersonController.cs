using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d First Person Controller for phones + tablets revised 12.30.12
 * Usage: Replace standard Unity FPS controller script with this script.
 * requires s3dGyroCam.js script attached to camera (child)
 * Assign s3dTouchpad for navigation (translation) input
 * default: x movement sidesteps (rotateHeading = false);
 * option: x movement rotates heading (rotateHeading = true);
 * s3dTouchpad mimics touchpad input with mouse on desktop. 
 */
// touchpad speed
[UnityEngine.RequireComponent(typeof(CharacterMotor))]
public partial class s3dFirstPersonController : MonoBehaviour
{
    private CharacterMotor motor;
    public s3dTouchpad touchpad;
    public Vector2 touchSpeed;
    public bool horizontalControlsHeading;
    private Vector3 directionVector;
    private s3dGyroCam gyroCam;
    public virtual void Awake()
    {
        this.motor = (CharacterMotor) this.GetComponent(typeof(CharacterMotor));
    }

    public virtual void Start()
    {
        this.gyroCam = (s3dGyroCam) this.gameObject.GetComponentInChildren(typeof(s3dGyroCam));
    }

    public virtual void Update()
    {
        if (this.horizontalControlsHeading)
        {
            this.directionVector = new Vector3(0, 0, this.touchpad.position.y * this.touchSpeed.y);
            this.gyroCam.heading = this.gyroCam.heading + (this.touchpad.position.x * this.touchSpeed.x);
            this.gyroCam.heading = this.gyroCam.heading % 360;
        }
        else
        {
            this.directionVector = new Vector3(this.touchpad.position.x * this.touchSpeed.x, 0, this.touchpad.position.y * this.touchSpeed.y);
        }
        this.motor.inputMoveDirection = this.gyroCam.transform.rotation * this.directionVector;
        if (this.touchpad.tap > 0)
        {
            this.motor.inputJump = true;
            this.touchpad.reset();
        }
        else
        {
            this.motor.inputJump = false;
        }
    }

    public s3dFirstPersonController()
    {
        this.touchSpeed = new Vector2(1f, 1f);
    }

}