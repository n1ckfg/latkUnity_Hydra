using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* This is an s3d Interaction script.
 * Requires a s3dInteractor component
 * tap makes object jump in the air
 * requires rigidbody
 */
 // 1 = short tap 2 = long tap 3 = any tap
[UnityEngine.RequireComponent(typeof(Rigidbody))]
[UnityEngine.RequireComponent(typeof(s3dInteractor))]
public partial class jumpInTheAir : MonoBehaviour
{
    public float upForce;
    public float randomForce;
    public float torqueForce;
    public int tapType;
    public virtual void Start()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void NewTap(TapParams @params)
    {
        if ((@params.tap == this.tapType) || (this.tapType == 3))
        {
            Vector3 pushForce = new Vector3(Random.Range(-this.randomForce, this.randomForce), this.upForce, Random.Range(-this.randomForce, this.randomForce));
            this.GetComponent<Rigidbody>().AddForce(pushForce);
            Vector3 spinForce = new Vector3(Random.Range(-this.torqueForce, this.torqueForce), Random.Range(-this.torqueForce, this.torqueForce), Random.Range(-this.torqueForce, this.torqueForce));
            this.GetComponent<Rigidbody>().AddRelativeTorque(spinForce);
        }
    }

    public jumpInTheAir()
    {
        this.upForce = 100;
        this.randomForce = 20;
        this.torqueForce = 20;
        this.tapType = 3;
    }

}