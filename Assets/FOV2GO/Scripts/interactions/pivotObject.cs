using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* This is an s3d Interaction script.
 * Requires a s3dInteractor component
 * first tap turns object along chosen axis
 * if toggleTurn = true, next tap returns object to original rotation
 * if toggleTurn = false, each tap rotates object further
 * plays audioClip if one is assigned
 */
 // 1 = short tap 2 = long tap 3 = any tap
// audio clip to play when object pivots
[UnityEngine.RequireComponent(typeof(s3dInteractor))]
[UnityEngine.RequireComponent(typeof(AudioSource))]
public partial class pivotObject : MonoBehaviour
{
    public Vector3 turnAmount;
    public float turnTime;
    public bool toggleTurn;
    public bool turned;
    public int tapType;
    private Quaternion startRotation;
    private Quaternion endRotation;
    private Quaternion goalRotation;
    private bool turning;
    public AudioClip pivotSound;
    public virtual void Start()
    {
        this.startRotation = this.transform.rotation;
        this.endRotation = this.startRotation * Quaternion.Euler(this.turnAmount);
    }

    public virtual void NewTap(TapParams @params)
    {
        if ((@params.tap == this.tapType) || (this.tapType == 3))
        {
            if (!this.turning)
            {
                if (!this.turned || !this.toggleTurn)
                {
                    this.goalRotation = this.endRotation;
                    this.StartCoroutine(this.rotate(this.transform.rotation, this.endRotation));
                    if (this.pivotSound)
                    {
                        this.GetComponent<AudioSource>().PlayOneShot(this.pivotSound);
                    }
                    this.turned = true;
                }
                else
                {
                    this.goalRotation = this.startRotation;
                    this.StartCoroutine(this.rotate(this.transform.rotation, this.startRotation));
                    if (this.pivotSound)
                    {
                        this.GetComponent<AudioSource>().PlayOneShot(this.pivotSound);
                    }
                    this.turned = false;
                }
            }
        }
    }

    public virtual IEnumerator rotate(Quaternion from, Quaternion to)
    {
        float rate = 1f / this.turnTime;
        float t = 0f;
        while (t < 1f)
        {
            t = t + (Time.deltaTime * rate);
            this.transform.rotation = Quaternion.Slerp(from, to, t);
            this.turning = true;
            yield return null;
        }
        this.turning = false;
        if (!this.toggleTurn)
        {
            this.startRotation = this.endRotation;
            this.endRotation = this.startRotation * Quaternion.Euler(this.turnAmount);
        }
    }

    public pivotObject()
    {
        this.turnTime = 1;
        this.toggleTurn = true;
        this.tapType = 3;
    }

}