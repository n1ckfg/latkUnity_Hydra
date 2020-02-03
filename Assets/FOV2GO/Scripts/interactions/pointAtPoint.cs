using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* This is an s3d Interaction script.
 * Requires a s3dInteractor component
// attach this script to an object (example gun or flashlight) to make it point at the current s3d_GUI_Pointer position
*/
[UnityEngine.RequireComponent(typeof(s3dInteractor))]
public partial class pointAtPoint : MonoBehaviour
{
    public bool smooth;
    public float damp;
    public virtual void Start()
    {
    }

    public virtual void Update()
    {
    }

    // aim at active object position OR at hit point
    public virtual void NewPosition(Vector3 pos)
    {
        if (this.GetComponent<Renderer>().enabled)
        {
            if (this.smooth)
            {
                // Look at and dampen the rotation
                Quaternion rot = Quaternion.LookRotation(pos - this.transform.position);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rot, Time.deltaTime * this.damp);
            }
            else
            {
                // Just lookat
                this.transform.LookAt(pos);
            }
        }
    }

    public pointAtPoint()
    {
        this.smooth = true;
        this.damp = 6f;
    }

}