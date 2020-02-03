using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d GUI Texture Script revised 12.30.12
 * Usage: Attach to a GUI Texture.
 * Creates left and right copies of GUITexture for stereoscopic view. 
 * Can adjust GUI Texture parallax automatically to keep the GUITexture closer than anything it occludes.
 * GUITextures are placed in leftOnlyLayer and rightOnlyLayer (layers are set in s3dCamera.js).
 * Assumes that the GUI Texture Pixel Inset is set to the default (center) position, and that the
   x and y position (each set between 0.0 and 1.0) are used to place the GUITexture.
 * Dependencies: 
 * s3dCamera.js (on main camera) with useLeftRightOnlyLayers boolean set to TRUE (default)  
 */
// *** Z Depth ***
// set initial GUIElement distance from camera
// keep this GUIElement closer than anything behind it
// make GUIElement this a bit closer than nearest object or object under mouse
// minimum distance for this GUIELement
// maximum distance for this GUIElement, no matter what's behind it
// *** Display & Movement ***
// start with guiText visible?
// timer to turn off texture if visible (0 = stays on forever)
// how gradually to change depth (bigger numbers are slower - more than 25 is very slow);
[UnityEngine.AddComponentMenu("Stereoskopix/s3d GUI Texture")]
public partial class s3dGuiTexture : MonoBehaviour
{
    public float objectDistance;
    public bool keepCloser;
    public float nearPadding;
    public float minimumDistance;
    public float maximumDistance;
    public bool beginVisible;
    public float timeToDisplay;
    public float lagTime;
    private s3dCamera camera3D;
    public GameObject objectCopyR;
    private float screenWidth;
    public Vector3 obPosition;
    private float scrnPrlx;
    private float curScrnPrlx;
    private Vector2[] checkpoints;
    private Vector2 corner;
    public bool on;
    private float unitWidth;
    private s3dGuiCursor s3dCursor;
    private float xWidth, xInset;

    public virtual IEnumerator Start()
    {
        findS3dCamera();
        checkpoints = new Vector2[5];
        objectCopyR = UnityEngine.Object.Instantiate(gameObject, transform.position, transform.rotation);
        UnityEngine.Object.Destroy((s3dGuiTexture) objectCopyR.GetComponent(typeof(s3dGuiTexture)));
        objectCopyR.name = gameObject.name + "_R";
        gameObject.name = gameObject.name + "_L";
        gameObject.layer = camera3D.leftOnlyLayer;
        objectCopyR.layer = camera3D.rightOnlyLayer;
        obPosition = gameObject.transform.position;
        
        // if using stereo shader + side-by-side + not squeezed, double width of guiTexture
        if ((camera3D.useStereoShader && (camera3D.format3D == (mode3D) 0)) && !camera3D.sideBySideSqueezed)
        {
            xWidth = GetComponent<GUITexture>().pixelInset.width * 2;

            {
                float _33 = xWidth;
                Rect _34 = gameObject.GetComponent<GUITexture>().pixelInset;
                _34.width = _33;
                gameObject.GetComponent<GUITexture>().pixelInset = _34;
            }

            {
                float _35 = xWidth;
                Rect _36 = objectCopyR.GetComponent<GUITexture>().pixelInset;
                _36.width = _35;
                objectCopyR.GetComponent<GUITexture>().pixelInset = _36;
            }
            xInset = gameObject.GetComponent<GUITexture>().pixelInset.width / -2;

            {
                float _37 = xInset;
                Rect _38 = gameObject.GetComponent<GUITexture>().pixelInset;
                _38.x = _37;
                gameObject.GetComponent<GUITexture>().pixelInset = _38;
            }

            {
                float _39 = xInset;
                Rect _40 = objectCopyR.GetComponent<GUITexture>().pixelInset;
                _40.x = _39;
                objectCopyR.GetComponent<GUITexture>().pixelInset = _40;
            }
        }
        else
        {
            // if not using stereo shader + squeezed, halve width of guiTexture
            if (!camera3D.useStereoShader && camera3D.sideBySideSqueezed)
            {
                xWidth = gameObject.GetComponent<GUITexture>().pixelInset.width * 0.5f;

                {
                    float _41 = xWidth;
                    Rect _42 = gameObject.GetComponent<GUITexture>().pixelInset;
                    _42.width = _41;
                    gameObject.GetComponent<GUITexture>().pixelInset = _42;
                }

                {
                    float _43 = xWidth;
                    Rect _44 = objectCopyR.GetComponent<GUITexture>().pixelInset;
                    _44.width = _43;
                    objectCopyR.GetComponent<GUITexture>().pixelInset = _44;
                }
                xInset = gameObject.GetComponent<GUITexture>().pixelInset.width / -2;

                {
                    float _45 = xInset;
                    Rect _46 = gameObject.GetComponent<GUITexture>().pixelInset;
                    _46.x = _45;
                    gameObject.GetComponent<GUITexture>().pixelInset = _46;
                }

                {
                    float _47 = xInset;
                    Rect _48 = objectCopyR.GetComponent<GUITexture>().pixelInset;
                    _48.x = _47;
                    objectCopyR.GetComponent<GUITexture>().pixelInset = _48;
                }
            }
        }
        // find corner offset
        corner = new Vector2((gameObject.GetComponent<GUITexture>().pixelInset.width / 2) / Screen.width, (gameObject.GetComponent<GUITexture>().pixelInset.height / 2) / Screen.height);
        toggleVisible(beginVisible);
        float horizontalFOV = (2 * Mathf.Atan(Mathf.Tan((camera3D.GetComponent<Camera>().fieldOfView * Mathf.Deg2Rad) / 2) * camera3D.GetComponent<Camera>().aspect)) * Mathf.Rad2Deg;
        unitWidth = Mathf.Tan((horizontalFOV / 2) * Mathf.Deg2Rad); // need unit width to calculate cursor depth when there's a HIT (horizontal image transform)
        screenWidth = (unitWidth * camera3D.zeroPrlxDist) * 2;
        setScreenParallax();
        if (timeToDisplay != 0f)
        {
            yield return new WaitForSeconds(timeToDisplay);
            toggleVisible(false);
        }
        s3dCursor = (s3dGuiCursor) gameObject.GetComponent(typeof(s3dGuiCursor));
        if (s3dCursor)
        {
            s3dCursor.initialize();
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
        if (on)
        {
            if (keepCloser)
            {
                findDistanceUnderObject();
            }
            if (curScrnPrlx != scrnPrlx)
            {
                curScrnPrlx = curScrnPrlx + ((scrnPrlx - curScrnPrlx) / (lagTime + 1));
            }
            gameObject.transform.position = new Vector3(obPosition.x + (curScrnPrlx / 2), obPosition.y, gameObject.transform.position.z);
            objectCopyR.transform.position = new Vector3(obPosition.x - (curScrnPrlx / 2), obPosition.y, objectCopyR.gameObject.transform.position.z);
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

    public virtual void findDistanceUnderObject()
    {
        Vector2 dPosition = default(Vector2);
        RaycastHit hit = default(RaycastHit);
        float nearDistance = Mathf.Infinity;
        s3dInteractor actScript = null;
        if ((camera3D.format3D == (mode3D) 0) && !camera3D.sideBySideSqueezed)
        {
            dPosition = new Vector2((obPosition.x / 2) + 0.25f, obPosition.y); // 0 = left, 0.5 = center, 1 = right
        }
        else
        {
            dPosition = obPosition;
        }
        checkpoints[0] = dPosition; // raycast against object center
        checkpoints[1] = dPosition + new Vector2(-corner.x, -corner.y); // raycast against object corners
        checkpoints[2] = dPosition + new Vector2(corner.x, -corner.y);
        checkpoints[3] = dPosition + new Vector2(corner.x, corner.y);
        checkpoints[4] = dPosition + new Vector2(-corner.x, corner.y);
        // raycast against all objects
        int cor = 0;
        while (cor < 5)
        {
            Ray ray = camera3D.GetComponent<Camera>().ViewportPointToRay(checkpoints[cor]);
            if (Physics.Raycast(ray, out hit, 100f))
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, new Color(0, 1, 0, 1));
                Plane camPlane = new Plane(camera3D.GetComponent<Camera>().transform.forward, camera3D.GetComponent<Camera>().transform.position);
                Vector3 thePoint = ray.GetPoint(hit.distance);
                float currentDistance = camPlane.GetDistanceToPoint(thePoint);
                if (currentDistance < nearDistance)
                {
                    nearDistance = currentDistance;
                }
            }
            cor++;
        }
        if (nearDistance < Mathf.Infinity)
        {
            objectDistance = Mathf.Clamp(nearDistance, minimumDistance, maximumDistance);
        }
        objectDistance = Mathf.Max(objectDistance, camera3D.GetComponent<Camera>().nearClipPlane);
        setScreenParallax();
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
        on = t;
    }

    public s3dGuiTexture()
    {
        objectDistance = 1f;
        keepCloser = true;
        minimumDistance = 0.01f;
        maximumDistance = 100f;
        beginVisible = true;
    }

}