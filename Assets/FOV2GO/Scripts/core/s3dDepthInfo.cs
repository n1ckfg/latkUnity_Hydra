using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.

 * s3d Depth Info Script revised 12.30.12
 * Usage:
 * This script performs an array of raycasts into the view frustum and finds:
 * A. the distance of the nearest object in the scene.
 * B. the distance of the farthest object in the scene.
 * C. the distance to the object under the mouse position in the scene.
 * D. the distance to a selected object (set either by script or by mouse click) in the scene.
 * There are currently three additional scripts that can use this information to control various stereoscopic parameters:
 * 1. s3DdepthScene: automatically adjusts interaxial, convergence and parallax based on total scene depth
 * 2. s3DdepthMouse: automatically adjust convergence based on mouse position
 * 3. s3DdepthGUI: attached to a GUIelement, automatically adjusts its depth - can be used to create a 3D mouse pointer
*/
// number of rays to cast (more = more accurate, especially horizontal)
// maximum distance to cast rays
// draw debug rays in scene
 // distance to the nearest visible object
 // distance to the farthest visible object
 // distance to center of scene
[UnityEngine.RequireComponent(typeof(s3dCamera))]
[UnityEngine.AddComponentMenu("Stereoskopix/s3d Depth Info")]
public partial class s3dDepthInfo : MonoBehaviour
{
    public int raysH;
    public int raysV;
    public float maxSampleDistance;
    public bool drawDebugRays;
    public float nearDistance;
    public float farDistance;
    public float distanceAtCenter;
    public float distanceUnderMouse;
    public bool clickToSelect;
    public GameObject selectedObject;
    private GameObject objectUnderMouse;
    public float objectDistance;
    private Vector3 nearPoint;
    private Vector3 farPoint;
    private float interaxial;
    private Camera mainCam;
    private s3dCamera camScript;
    private object[][] rays;
    public bool showScreenPlane;
    public bool showNearFarPlanes;
    public virtual void Start()
    {
        this.mainCam = (Camera) this.gameObject.GetComponent(typeof(Camera)); // Main Stereo Camera Component
        this.camScript = (s3dCamera) this.gameObject.GetComponent(typeof(s3dCamera)); // Main Stereo Script
    }

    public virtual void Update()
    {
        this.findNearAndFarDistances();
        this.findDistanceUnderMousePosition();
        if (this.clickToSelect)
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.selectedObject = this.objectUnderMouse;
            }
        }
        if (this.selectedObject)
        {
            this.findObjectDistance();
        }
        if (this.showScreenPlane)
        {
            if (!this.camScript.planeZero.GetComponent<Renderer>().enabled)
            {
                this.camScript.planeZero.GetComponent<Renderer>().enabled = true;
            }
            float cameraWidth = (Mathf.Tan((this.camScript.horizontalFOV / 2) * Mathf.Deg2Rad) * this.camScript.zeroPrlxDist) * 2;
            float cameraHeight = (Mathf.Tan((this.camScript.verticalFOV / 2) * Mathf.Deg2Rad) * this.camScript.zeroPrlxDist) * 2;
            Vector2 screenSize = new Vector2(cameraWidth, cameraHeight);
            this.camScript.planeZero.transform.localPosition = new Vector3(0, 0, this.camScript.zeroPrlxDist);
            this.camScript.planeZero.transform.localScale = new Vector3(screenSize.x, screenSize.y, 0);
        }
        else
        {
            if (this.camScript.planeZero.GetComponent<Renderer>().enabled)
            {
                this.camScript.planeZero.GetComponent<Renderer>().enabled = false;
            }
        }
        if (this.showNearFarPlanes)
        {
            if (!this.camScript.planeNear.GetComponent<Renderer>().enabled)
            {
                this.camScript.planeNear.GetComponent<Renderer>().enabled = true;
                this.camScript.planeFar.GetComponent<Renderer>().enabled = true;
            }
            float nearWidth = (Mathf.Tan((this.camScript.horizontalFOV / 2) * Mathf.Deg2Rad) * this.nearDistance) * 2;
            float nearHeight = (Mathf.Tan((this.camScript.verticalFOV / 2) * Mathf.Deg2Rad) * this.nearDistance) * 2;
            Vector2 nearSize = new Vector2(nearWidth, nearHeight);
            this.camScript.planeNear.transform.localPosition = new Vector3(0, 0, this.nearDistance);
            this.camScript.planeNear.transform.localScale = new Vector3(nearSize.x, nearSize.y, 0);
            float farWidth = (Mathf.Tan((this.camScript.horizontalFOV / 2) * Mathf.Deg2Rad) * this.farDistance) * 2;
            float farHeight = (Mathf.Tan((this.camScript.verticalFOV / 2) * Mathf.Deg2Rad) * this.farDistance) * 2;
            Vector2 farSize = new Vector2(farWidth, farHeight);
            this.camScript.planeFar.transform.localPosition = new Vector3(0, 0, this.farDistance);
            this.camScript.planeFar.transform.localScale = new Vector3(farSize.x, farSize.y, 0);
        }
        else
        {
            if (this.camScript.planeNear.GetComponent<Renderer>().enabled)
            {
                this.camScript.planeNear.GetComponent<Renderer>().enabled = false;
                this.camScript.planeFar.GetComponent<Renderer>().enabled = false;
            }
        }
    }

    public virtual void findNearAndFarDistances()
    {
        RaycastHit hit = default(RaycastHit);
        // find the smallest and largest distance from any raycast
        this.rays[0] = new object[] {};
        this.rays[1] = new object[] {};
        int n = 0;
        this.nearDistance = Mathf.Infinity;
        this.farDistance = 0;
        string nearObjectName = "none";
        string farObjectName = "none";
        int col = 0;
        while (col < this.raysH)
        {
            int row = 0;
            while (row < this.raysV)
            {
                Ray ray = this.mainCam.ScreenPointToRay(new Vector3((col * this.mainCam.pixelWidth) / (this.raysH - 1), (row * this.mainCam.pixelHeight) / (this.raysV - 1), 0));
                // draw all the rays in gray
                if (this.drawDebugRays)
                {
                    Debug.DrawRay(ray.origin, ray.direction * this.maxSampleDistance, new Color(0.5f, 0.5f, 0.5f, 0.25f));
                }
                if (Physics.Raycast(ray, out hit, this.maxSampleDistance))
                {
                    // draw the rays that actually hit valid objects in yellow
                    if (this.drawDebugRays)
                    {
                        Debug.DrawLine(ray.origin, ray.GetPoint(hit.distance), new Color(1, 1, 0, 0.5f));
                    }
                    this.rays[0][n] = ray.origin;
                    this.rays[1][n] = ray.GetPoint(hit.distance);
                    // plane of camera position (perpendicular to camera z axis)
                    Plane camPlane = new Plane(this.mainCam.transform.forward, this.mainCam.transform.position);
                    // get hit point of ray (world coordinates)
                    this.nearPoint = ray.GetPoint(hit.distance);
                    // calculate distance measured along camera's local z axis
                    float currentDistance = camPlane.GetDistanceToPoint(this.nearPoint);
                    n++;
                    if (currentDistance < this.nearDistance)
                    {
                        this.nearDistance = currentDistance;
                    }
                    else
                    {
                        if (currentDistance > this.farDistance)
                        {
                            this.farDistance = currentDistance;
                        }
                    }
                    if ((col == (this.raysH / 2)) && (row == (this.raysV / 2)))
                    {
                        if (this.drawDebugRays)
                        {
                            Debug.DrawLine(ray.origin, ray.GetPoint(hit.distance), new Color(0, 0, 1, 1));
                        }
                        this.distanceAtCenter = currentDistance;
                    }
                }
                row++;
            }
            col++;
        }
    }

    public virtual void findDistanceUnderMousePosition()
    {
        RaycastHit hit = default(RaycastHit);
        Ray ray = this.mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100f)) // converge to clicked point
        {
            this.objectUnderMouse = hit.collider.gameObject;
            Plane camPlane = new Plane(this.mainCam.transform.forward, this.mainCam.transform.position);
            Vector3 thePoint = ray.GetPoint(hit.distance);
            this.distanceUnderMouse = camPlane.GetDistanceToPoint(thePoint);
        }
    }

    public virtual void findObjectDistance()
    {
        RaycastHit hit = default(RaycastHit);
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(this.mainCam);
        //test whether object is on camera
        if (GeometryUtility.TestPlanesAABB(planes, this.selectedObject.GetComponent<Collider>().bounds))
        {
            Vector3 vec = this.mainCam.WorldToViewportPoint(this.selectedObject.transform.position);
            //alternate to bounds - just check object center
            //if(vec.x>0 && vec.x<1 && vec.y>0 && vec.y<1 && vec.z>0) { 
            Ray ray = Camera.main.ViewportPointToRay(vec);
            if (Physics.Raycast(ray, out hit, 100f))
            {
                //make sure object isn't hidden by another object
                if ((hit.collider.gameObject == this.selectedObject) && (hit.distance > this.mainCam.nearClipPlane))
                {
                    Plane camPlane = new Plane(this.mainCam.transform.forward, this.mainCam.transform.position);
                    Vector3 thePoint = ray.GetPoint(hit.distance);
                    this.objectDistance = camPlane.GetDistanceToPoint(thePoint);
                }
            }
        }
    }

    public s3dDepthInfo()
    {
        this.raysH = 25;
        this.raysV = 12;
        this.maxSampleDistance = 100;
        this.drawDebugRays = true;
        this.rays = new object[][] {new object[0], new object[0]};
    }

}