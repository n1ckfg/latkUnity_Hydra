using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* This is an s3d Interaction script.
 * Requires a s3dInteractor component
 * first tap centers object to camera
 * next tap releases and returns object to original position
 * works with or without rigidbody
 */
// distance from camera when floating
// speed
 // 1 = short tap 2 = long tap 3 = any tap
[UnityEngine.RequireComponent(typeof(s3dInteractor))]
public partial class centerToCamera : MonoBehaviour
{
    public float floatDistance;
    public float objectSpeed;
    public bool returnToOriginalRotation;
    public Vector3 floatPosOffset;
    public Vector3 floatRotOffset;
    public bool returnToOrigin;
    public int tapType;
    private GameObject mainCamObj;
    private bool floating;
    private bool foundStartPos;
    private Vector3 startPos;
    private Quaternion startRot;
    private bool ready;
    private GameObject cursorObj;
    private s3dGuiCursor cursorScript;
    private Transform originalParent;
    private GameObject tempParent;
    public bool followWhenActive;
    public virtual void Start()
    {
        this.mainCamObj = GameObject.FindWithTag("MainCamera"); // Main Camera
        this.cursorObj = GameObject.FindWithTag("cursor");
        this.cursorScript = (s3dGuiCursor) this.cursorObj.GetComponent(typeof(s3dGuiCursor));
        if (!this.GetComponent<Rigidbody>())
        {
            this.startPos = this.transform.position;
            this.startRot = this.transform.rotation;
            this.foundStartPos = true;
        }
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (!this.foundStartPos)
        {
            this.StartCoroutine(this.registerStartPosition());
        }
    }

    public virtual IEnumerator registerStartPosition()
    {
        yield return new WaitForSeconds(0.5f); // give object a chance to settle
        this.startPos = this.transform.position;
        this.startRot = this.transform.rotation;
        this.foundStartPos = true;
    }

    public virtual void NewTap(TapParams @params)
    {
        if ((@params.tap == this.tapType) || (this.tapType == 3))
        {
            if (this.ready)
            {
                this.floating = !this.floating;
                this.ready = false;
                this.StartCoroutine(this.takeBreather());
            }
            if (this.floating)
            {
                if (this.GetComponent<Rigidbody>())
                {
                    this.GetComponent<Rigidbody>().isKinematic = true;
                }
                this.originalParent = this.transform.parent;
                this.tempParent = new GameObject("tempParent");
                this.tempParent.transform.position = this.transform.localPosition;
                this.tempParent.transform.rotation = this.transform.localRotation;
                this.transform.parent = this.tempParent.transform;
                this.tempParent.transform.parent = this.originalParent;
                this.cursorScript.followActiveObject = this.followWhenActive; // tell cursorScript whether it should follow this object
                this.cursorScript.activeObj = this.gameObject; // tell cursorScript that we have an active object
            }
            else
            {
                this.cursorScript.activeObj = null;
                this.transform.parent = this.originalParent;
                UnityEngine.Object.Destroy(this.tempParent);
            }
        }
    }

    public virtual IEnumerator takeBreather()
    {
        yield return new WaitForSeconds(0.25f);
        this.ready = true;
    }

    public virtual void Deactivate()
    {
        this.floating = false;
        this.cursorScript.activeObj = null;
        this.transform.parent = this.originalParent;
        UnityEngine.Object.Destroy(this.tempParent);
    }

    public virtual void Update()
    {
        if (this.floating)
        {
            Ray ray = new Ray(this.mainCamObj.transform.position, this.mainCamObj.transform.forward);
            Vector3 floatPos = ray.GetPoint(this.floatDistance);
            Vector3 obRot = this.mainCamObj.transform.position - floatPos;
            Quaternion floatRot = Quaternion.LookRotation(obRot, this.mainCamObj.transform.up);
            if (this.tempParent.transform.position != floatPos)
            {
                this.tempParent.transform.position = Vector3.Lerp(this.tempParent.transform.position, floatPos, Time.deltaTime * this.objectSpeed);
                this.tempParent.transform.rotation = Quaternion.Lerp(this.tempParent.transform.rotation, floatRot, Time.deltaTime * this.objectSpeed);
                this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, this.floatPosOffset, Time.deltaTime * this.objectSpeed);
                this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(this.floatRotOffset), Time.deltaTime * this.objectSpeed);
            }
        }
        else
        {
            if (!this.GetComponent<Rigidbody>() || ((this.GetComponent<Rigidbody>() && this.returnToOrigin) && this.foundStartPos))
            {
                if (this.transform.position != this.startPos)
                {
                    this.transform.position = Vector3.Lerp(this.transform.position, this.startPos, Time.deltaTime * this.objectSpeed);
                    if (this.returnToOriginalRotation)
                    {
                        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, this.startRot, Time.deltaTime * this.objectSpeed);
                    }
                }
                else
                {
                    if (this.GetComponent<Rigidbody>())
                    {
                        this.GetComponent<Rigidbody>().isKinematic = false;
                    }
                }
            }
            else
            {
                if (this.GetComponent<Rigidbody>().isKinematic)
                {
                    this.GetComponent<Rigidbody>().WakeUp();
                    this.GetComponent<Rigidbody>().isKinematic = false;
                }
            }
        }
    }

    public centerToCamera()
    {
        this.floatDistance = 2;
        this.objectSpeed = 1;
        this.returnToOriginalRotation = true;
        this.returnToOrigin = true;
        this.tapType = 3;
        this.ready = true;
        this.followWhenActive = true;
    }

}