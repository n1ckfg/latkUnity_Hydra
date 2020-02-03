using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* This is an s3d Interaction script.
 * Requires a s3dInteractor component
 * first tap picks up object, object can be dragged around
 * next tap drops object
 * requires rigidbody
 */
// DragRigidBody
 //50
 //5
 //0.2  
 // 1 = short tap 2 = long tap 3 = any tap
// if we move forward, object stays where it is 
// position of the object when clicked on
[UnityEngine.RequireComponent(typeof(s3dInteractor))]
[UnityEngine.RequireComponent(typeof(Rigidbody))]
public partial class pickUpPutDown : MonoBehaviour
{
    public float spring;
    public float damper;
    public float drag;
    public float angularDrag;
    public float distance;
    public int tapType;
    public bool attachToCenterOfMass;
    private SpringJoint springJoint;
    public Vector3 customCenterOfMass;
    public bool moveTowardsObject;
    public float minDist;
    public float maxDist;
    public Vector3 grabOffset;
    private GameObject mainCamObj;
    private GameObject cursorObj;
    private s3dGuiCursor cursorScript;
    private bool activated;
    private Vector3 startPos;
    private Quaternion startRot;
    private bool readyForStateChange;
    private Vector3 newPosition;
    private Vector3 clickPosition;
    private float hitDistance;
    public virtual void Start()
    {
        mainCamObj = GameObject.FindWithTag("MainCamera"); // Main Camera
        cursorObj = GameObject.FindWithTag("cursor");
        cursorScript = (s3dGuiCursor) cursorObj.GetComponent(typeof(s3dGuiCursor)); // Main Stereo Camera Script
        startPos = transform.position;
        startRot = transform.rotation;
        GetComponent<Rigidbody>().centerOfMass = customCenterOfMass;
    }

    public virtual void NewTap(TapParams @params)
    {
        if (readyForStateChange)
        {
            if (!activated)
            {
                if ((@params.tap == tapType) || (tapType == 3))
                {
                    activated = true;
                }
            }
            else
            {
                activated = false;
            }
            //activated = !activated;
            hitDistance = @params.hit.distance;
            readyForStateChange = false;
            StartCoroutine(pauseAfterStateChange());
        }
        if (activated)
        {
            clickPosition = @params.hit.point;
            cursorScript.activeObj = gameObject; // tell cursorScript that we have an active object
            if (!springJoint)
            {
                GameObject go = new GameObject("Rigidbody dragger");
                Rigidbody body = ((Rigidbody) go.AddComponent(typeof(Rigidbody))) as Rigidbody;
                springJoint = (SpringJoint) go.AddComponent(typeof(SpringJoint));
                body.isKinematic = true;
            }
            springJoint.transform.position = @params.hit.point;
            if (attachToCenterOfMass)
            {
                Vector3 anchor = transform.TransformDirection(GetComponent<Rigidbody>().centerOfMass) + GetComponent<Rigidbody>().transform.position;
                anchor = springJoint.transform.InverseTransformPoint(anchor);
                springJoint.anchor = anchor;
            }
            else
            {
                springJoint.anchor = Vector3.zero;
            }
            springJoint.spring = spring;
            springJoint.damper = damper;
            springJoint.maxDistance = distance;
            springJoint.connectedBody = GetComponent<Rigidbody>();
            StartCoroutine(increaseSpringAfterPickup());
            StartCoroutine("DragObject");
        }
        else
        {
            cursorScript.activeObj = null;
        }
    }

    public virtual IEnumerator DragObject()
    {
        float oldDrag = springJoint.connectedBody.drag;
        float oldAngularDrag = springJoint.connectedBody.angularDrag;
        springJoint.connectedBody.drag = drag;
        springJoint.connectedBody.angularDrag = angularDrag;
        while (activated) // end when receive another double-click touch
        {
            springJoint.transform.position = newPosition + grabOffset;
            yield return null;
        }
        if (springJoint.connectedBody)
        {
            springJoint.connectedBody.drag = oldDrag;
            springJoint.connectedBody.angularDrag = oldAngularDrag;
            springJoint.connectedBody = null;
        }
    }

    public virtual void Deactivate()
    {
        activated = false;
        cursorScript.activeObj = null;
        readyForStateChange = false;
        springJoint.spring = springJoint.spring / 10;
        StartCoroutine(pauseAfterStateChange());
    }

    public virtual IEnumerator pauseAfterStateChange()
    {
        yield return new WaitForSeconds(0.25f);
        readyForStateChange = true;
    }

    public virtual IEnumerator increaseSpringAfterPickup()
    {
        yield return new WaitForSeconds(1);
        springJoint.spring = springJoint.spring * 10;
    }

    public virtual void NewPosition(Vector3 pos)
    {
        float currentDistance = 0.0f;
        if (activated)
        {
            Vector3 viewPos = mainCamObj.GetComponent<Camera>().WorldToViewportPoint(pos);
            Ray ray = mainCamObj.GetComponent<Camera>().ViewportPointToRay(viewPos);
            if (moveTowardsObject)
            {
                currentDistance = Vector3.Distance(mainCamObj.transform.position, clickPosition);
                currentDistance = Mathf.Clamp(currentDistance, minDist, maxDist);
            }
            else
            {
                currentDistance = hitDistance;
            }
            newPosition = ray.GetPoint(currentDistance);
        }
    }

    public pickUpPutDown()
    {
        spring = 100f;
        damper = 100f;
        drag = 10f;
        angularDrag = 5f;
        distance = 0.1f;
        tapType = 1;
        customCenterOfMass = Vector3.zero;
        moveTowardsObject = true;
        minDist = 1;
        maxDist = 10;
        grabOffset = new Vector3(0, 0.5f, 0);
        readyForStateChange = true;
    }

}