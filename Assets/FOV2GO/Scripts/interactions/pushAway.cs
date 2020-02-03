using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* This is an s3d Interaction script.
 * Requires a s3dInteractor component
 * tap pushes object away from camera
 * requires rigidbody
 */
 // 1 = short tap 2 = long tap 3 = any tap
[UnityEngine.RequireComponent(typeof(Rigidbody))]
[UnityEngine.RequireComponent(typeof(s3dInteractor))]
public partial class pushAway : MonoBehaviour
{
    public float pushForce;
    public float spinForce;
    public int tapType;
    public virtual void NewTap(TapParams @params)
    {
        if ((@params.tap == tapType) || (tapType == 3))
        {
            Vector3 vec = transform.position - @params.hit.point; // the direction from clickPoint to the rigidbody
            vec = vec.normalized;
            GetComponent<Rigidbody>().AddForce((vec * pushForce) * 100);
            Vector3 vec2 = new Vector3(Random.Range(-spinForce, spinForce), Random.Range(-spinForce, spinForce), Random.Range(-spinForce, spinForce));
            GetComponent<Rigidbody>().AddRelativeTorque(vec2 * 100);
        }
    }

    public pushAway()
    {
        pushForce = 5;
        spinForce = 20;
        tapType = 3;
    }

}