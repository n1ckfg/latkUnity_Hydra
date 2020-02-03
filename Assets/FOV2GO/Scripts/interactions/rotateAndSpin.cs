using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* This is an s3d Interaction script.
 * Requires a s3dInteractor component
 * tap spins object around random axis
 * when not spinning, object can be set to turn constantly around a random or specified axis
 */
 // 1 = short tap 2 = long tap 3 = any tap
// spin on tap
// rotate constantly
[UnityEngine.RequireComponent(typeof(s3dInteractor))]
public partial class rotateAndSpin : MonoBehaviour
{
    public int tapType;
    public float spinTime;
    public bool rotateOnRest;
    public Vector3 rotateAxis;
    public bool randomAxis;
    public float rotateSpeed;
    public bool randomSpeed;
    public float minSpeed;
    public float maxSpeed;
    private Vector3 spinAmount;
    private bool spinning;
    private float spinTapTime;
    private Quaternion goalRotation;
    public virtual void Start()
    {
        if (this.randomAxis)
        {
            this.rotateAxis = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        }
        if (this.randomSpeed)
        {
            this.rotateSpeed = Random.Range(this.minSpeed, this.maxSpeed);
        }
    }

    public virtual void Update()
    {
        if (!this.spinning)
        {
            this.transform.Rotate(new Vector3((Time.deltaTime * this.rotateSpeed) * this.rotateAxis.x, (Time.deltaTime * this.rotateSpeed) * this.rotateAxis.y, (Time.deltaTime * this.rotateSpeed) * this.rotateAxis.z));
        }
    }

    public virtual void NewTap(TapParams @params)
    {
        if ((@params.tap == this.tapType) || (this.tapType == 3))
        {
            if (!this.spinning)
            {
                this.spinTapTime = Time.time;
                this.spinAmount = new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), Random.Range(-30, 30));
                this.goalRotation = this.transform.rotation * Quaternion.Euler(this.spinAmount);
                this.spinning = true;
                this.StartCoroutine(this.rotate(this.transform.rotation, this.goalRotation));
            }
        }
    }

    public virtual IEnumerator rotate(Quaternion from, Quaternion to)
    {
        float rate = 1f / this.spinTime;
        float t = 1f;
        while (t > 0f)
        {
            t = t - (Time.deltaTime * rate);
            this.transform.Rotate(this.spinAmount * t);
            this.spinning = true;
            yield return null;
        }
        this.spinning = false;
    }

    public rotateAndSpin()
    {
        this.tapType = 3;
        this.spinTime = 1f;
        this.randomAxis = true;
        this.rotateSpeed = 1;
        this.randomSpeed = true;
        this.minSpeed = 1f;
        this.maxSpeed = 5f;
    }

}