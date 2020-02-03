using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d GUI Text Script revised 12.30.12
 * Usage: Attach to GUIText
 * Creates left and right copies of GUIText for stereoscopic view
 * Can adjust GUIText parallax automatically to keep the GUIText closer than anything it occludes.
 * Assumes that the GUIText Pixel Offset is set to the default (0,0) position, and that the
 * x and y position (each set between 0.0 and 1.0) are used to place the GUIText.
 * Dependencies: requires one s3dCamera.js with useLeftRightOnlyLayers boolean set to TRUE (default) 
 */
// should GUI Text track mouse position?
// should GUI Text track mouse only when down?
// set initial GUIElement distance from camera
// keep this GUIElement closer than anything behind it
// make GUIElement a bit closer than nearest object or object under mouse
// minimum distance for this GUIELement
// maximum distance for this GUIElement, no matter what's behind it
// how gradually to change depth (bigger numbers are slower - more than 25 is very slow);
// start with guiText visible;
// timer to turn off text if visible (0 to leave on)
// string to begin with
// text color
// shadows?
// shadowColor
// shadowOffset
[UnityEngine.AddComponentMenu("Stereoskopix/s3d GUI Text")]
public partial class s3dGuiText : MonoBehaviour
{
    public bool trackMouseXYPosition;
    public bool onlyWhenMouseDown;
    public float objectDistance;
    public bool keepCloser;
    public float nearPadding;
    public float minimumDistance;
    public float maximumDistance;
    public float lagTime;
    public bool beginVisible;
    public float timeToDisplay;
    public string initString;
    public Color TextColor;
    public bool shadowsOn;
    public Color ShadowColor;
    public float shadowOffset;
    private s3dCamera camera3D;
    private GameObject objectCopyR;
    private GameObject shadowL;
    private GameObject shadowR;
    private float screenWidth;
    public Vector3 obPosition;
    private float scrnPrlx;
    private float curScrnPrlx;
    private object[] rays;
    private Vector2[] corners;
    private Vector2 corner;
    private bool textOn;
    private float unitWidth;
    public virtual IEnumerator Start()//toggleVisible(false);
    {
        findS3dCamera();
        corners = new Vector2[4];
        objectCopyR = UnityEngine.Object.Instantiate(gameObject, transform.position, transform.rotation);
        UnityEngine.Object.Destroy((s3dGuiText) objectCopyR.GetComponent(typeof(s3dGuiText)));
        objectCopyR.name = gameObject.name + "_R";
        objectCopyR.transform.parent = gameObject.transform.parent;
        gameObject.name = gameObject.name + "_L";
        gameObject.layer = camera3D.leftOnlyLayer;
        gameObject.GetComponent<GUIText>().material.color = TextColor;
        objectCopyR.layer = camera3D.rightOnlyLayer;
        objectCopyR.gameObject.GetComponent<GUIText>().material.color = TextColor;
        if (shadowsOn)
        {
            shadowL = UnityEngine.Object.Instantiate(objectCopyR.gameObject, transform.position, transform.rotation);
            shadowL.name = gameObject.name + "_shadL";
            shadowL.gameObject.layer = camera3D.leftOnlyLayer;
            shadowL.GetComponent<GUIText>().material.color = ShadowColor;
            shadowL.transform.parent = transform;
            shadowR = UnityEngine.Object.Instantiate(objectCopyR.gameObject, transform.position, transform.rotation);
            shadowR.name = gameObject.name + "_shadR";
            shadowR.gameObject.layer = camera3D.rightOnlyLayer;
            shadowR.GetComponent<GUIText>().material.color = ShadowColor;
            shadowR.transform.parent = objectCopyR.transform;
        }
        obPosition = gameObject.transform.position;
        setText(initString);
        toggleVisible(beginVisible);
        float horizontalFOV = (2 * Mathf.Atan(Mathf.Tan((camera3D.GetComponent<Camera>().fieldOfView * Mathf.Deg2Rad) / 2) * camera3D.GetComponent<Camera>().aspect)) * Mathf.Rad2Deg;
        unitWidth = Mathf.Tan((horizontalFOV / 2) * Mathf.Deg2Rad); // need unit width to calculate cursor depth when there's a HIT
        screenWidth = (unitWidth * camera3D.zeroPrlxDist) * 2;
        if (timeToDisplay != 0f)
        {
            yield return new WaitForSeconds(timeToDisplay);
        }
    }

    public virtual void findS3dCamera()
    {
        s3dCamera[] cameras3D = ((s3dCamera[]) UnityEngine.Object.FindObjectsOfType(typeof(s3dCamera))) as s3dCamera[];
        if (cameras3D.Length == 1)
        {
            camera3D = cameras3D[0];
        }
        else
        {
            if (cameras3D.Length > 1)
            {
                MonoBehaviour.print("There is more than one s3dCamera in this scene.");
            }
            else
            {
                MonoBehaviour.print("There is no s3dCamera in this scene.");
            }
        }
    }

    public virtual void Update()
    {
        float obPrlx = 0.0f;
        if (textOn)
        {
            if (trackMouseXYPosition)
            {
                if (!onlyWhenMouseDown || (onlyWhenMouseDown && Input.GetMouseButton(0)))
                {
                    obPosition = matchMousePos();
                }
            }
            if (keepCloser)
            {
                findDistanceUnderObject();
                objectDistance = Mathf.Max(objectDistance, camera3D.GetComponent<Camera>().nearClipPlane);
            }
            setScreenParallax();
            if (curScrnPrlx != scrnPrlx)
            {
                curScrnPrlx = curScrnPrlx + ((scrnPrlx - curScrnPrlx) / (lagTime + 1));
            }
            gameObject.transform.position = new Vector3(obPosition.x + (curScrnPrlx / 2), obPosition.y, gameObject.transform.position.z + 1);
            if (shadowsOn)
            {
                shadowL.gameObject.transform.localPosition = new Vector3(shadowOffset / 1100, -shadowOffset / 1000, 0);
            }
            objectCopyR.transform.position = new Vector3(obPosition.x - (curScrnPrlx / 2), obPosition.y, objectCopyR.gameObject.transform.position.z + 1);
            if (shadowsOn)
            {
                shadowR.gameObject.transform.localPosition = new Vector3(shadowOffset / 900, -shadowOffset / 1000, 0);
            }
        }
    }

    public virtual void findDistanceUnderObject()
    {
        Vector2 dPosition = default(Vector2);
        RaycastHit hit = default(RaycastHit);
        float nearDistance = Mathf.Infinity;
        if ((camera3D.format3D == (mode3D) 0) && !camera3D.sideBySideSqueezed)
        {
            dPosition = new Vector2((obPosition.x / 2) + 0.25f, obPosition.y); // 0 = left, 0.5 = center, 1 = right
        }
        else
        {
            dPosition = obPosition;
        }
        Ray ray = camera3D.GetComponent<Camera>().ViewportPointToRay(dPosition);
        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, new Color(0, 1, 0, 1));
            Plane camPlane = new Plane(camera3D.GetComponent<Camera>().transform.forward, camera3D.GetComponent<Camera>().transform.position);
            Vector3 thePoint = ray.GetPoint(hit.distance);
            nearDistance = camPlane.GetDistanceToPoint(thePoint);
        }
        if (nearDistance < Mathf.Infinity)
        {
            objectDistance = Mathf.Clamp(nearDistance, minimumDistance, maximumDistance);
        }
    }

    public virtual void setScreenParallax()
    {
        float obPrlx = ((camera3D.interaxial / 1000) * (camera3D.zeroPrlxDist - objectDistance)) / objectDistance;
        if ((camera3D.format3D == (mode3D) 0) && !camera3D.sideBySideSqueezed)
        {
            scrnPrlx = (((obPrlx / screenWidth) * 2) + (nearPadding / 1000)) - (camera3D.H_I_T / (unitWidth * 15)); // why 15? no idea.
        }
        else
        {
            scrnPrlx = (((obPrlx / screenWidth) * 1) + (nearPadding / 1000)) - (camera3D.H_I_T / (unitWidth * 15));
        }
    }

    public virtual void toggleVisible(bool t)
    {
        textOn = t;
    }

    public virtual void setText(string theText)
    {
        gameObject.GetComponent<GUIText>().text = theText;
        if (objectCopyR)
        {
            objectCopyR.GetComponent<GUIText>().text = theText;
        }
        if (shadowsOn)
        {
            if (shadowL)
            {
                shadowL.GetComponent<GUIText>().text = theText;
            }
            if (shadowR)
            {
                shadowR.GetComponent<GUIText>().text = theText;
            }
        }
    }

    public virtual Vector2 matchMousePos()
    {
        Vector2 mousePos = Input.mousePosition;
        if (camera3D.format3D == (mode3D) 0) // side by side
        {
            mousePos.x = mousePos.x / (Screen.width / 2);
        }
        else
        {
            mousePos.x = mousePos.x / Screen.width;
        }
        mousePos.y = mousePos.y / Screen.height;
        return mousePos;
    }

    // set position from another script (rollover text)
    public virtual void setObPosition(Vector2 obPos)
    {
        obPosition.x = obPos.x;
        obPosition.y = obPos.y;
    }

    public s3dGuiText()
    {
        onlyWhenMouseDown = true;
        objectDistance = 1f;
        nearPadding = 1f;
        minimumDistance = 1f;
        maximumDistance = 3f;
        beginVisible = true;
        timeToDisplay = 2f;
        TextColor = Color.white;
        shadowsOn = true;
        ShadowColor = Color.black;
        shadowOffset = 5f;
        rays = new object[0];
    }

}