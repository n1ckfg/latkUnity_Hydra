using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d GUI Cursor Script revised 12.30.12
 * Usage: Attach to a GUI Texture, along with  s3dGuiTexture.js
 * Creates a stereoscopic cursor (s3d_Gui_Pointer)
 * Assign 3 textures (default, click and pick) and 2 sounds (click and pick).
 * assign s3dTouchpad for cursor input on mobile devices
 * s3dTouchpad mimics touchpad input with mouse on desktop (leave trackMouseXYPosition OFF)
 * cursor triggers clickAction function in Interactor.js (attached to interactive objects)
 * interactive objects need to be placed in interactiveLayer (default is layer 9) 
 */
// hide default pointer so it doesn't compete with 3D cursor
// *** interaction ***
// layer for clickable objects
 // interactive layer
// main texture
// texture for click
// texture for grab
// maximum distance for object clicks
// click sound
// pick sound
// *** Mouse Input ***
// track mouse position - leave off if using touchpad input
// track mouse position only when held down
// *** Touchscreen Joystick ***
// select joystick
// toggle joystick
// touchpad tracking speed
// make joystick area like a trackpad
 // can be set to false so flashlight, gun etc don't follow certain objects - working?
 // most recent rollover object
[UnityEngine.RequireComponent(typeof(AudioSource))]
// @script RequireComponent(s3dGuiTexture); 
// generates this error: "Can't remove s3dGuiTexture (Script) because s3dGuiCursor (Script) depends on it"
// which is odd because it's exactly the opposite of what I'm trying to do!
[UnityEngine.AddComponentMenu("Stereoskopix/s3d GUI Cursor")]
public partial class s3dGuiCursor : MonoBehaviour
{
    public bool hidePointer;
    private s3dCamera camera3D;
    private s3dGuiTexture s3dTexture;
    public int interactiveLayer;
    public Texture defaultTexture;
    public Texture clickTexture;
    public Texture pickTexture;
    public float clickDistance;
    public AudioClip clickSound;
    public AudioClip pickSound;
    public bool trackMouseXYPosition;
    public bool onlyWhenMouseDown;
    public s3dTouchpad touchpad;
    public bool useTouchpad;
    public Vector2 touchpadSpeed;
    public bool uniformTouchpadMovement;
    private Vector2 touchpadPrevPosition;
    private LayerMask interactiveLayerMask;
    public GameObject activeObj;
    private bool readyForTap;
    private float unitWidth;
    public bool followActiveObject;
    private GameObject prevRolloverObject;
    private bool initialized;
    private s3dInteractor actScript;

    public virtual void Start()
    {
        findS3dCamera();
        if (hidePointer)
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
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

    public virtual void initialize() // called from s3dGuiTexture on startup, after it's been initialized;
    {
        s3dTexture = (s3dGuiTexture) gameObject.GetComponent(typeof(s3dGuiTexture));
        if (trackMouseXYPosition || useTouchpad)
        {
            float xInset = s3dTexture.GetComponent<GUITexture>().pixelInset.width / -2;
            float yInset = s3dTexture.GetComponent<GUITexture>().pixelInset.height / -2;

            {
                float _25 = xInset;
                Rect _26 = s3dTexture.GetComponent<GUITexture>().pixelInset;
                _26.x = _25;
                s3dTexture.GetComponent<GUITexture>().pixelInset = _26;
            }

            {
                float _27 = yInset;
                Rect _28 = s3dTexture.GetComponent<GUITexture>().pixelInset;
                _28.y = _27;
                s3dTexture.GetComponent<GUITexture>().pixelInset = _28;
            }

            {
                float _29 = xInset;
                Rect _30 = s3dTexture.objectCopyR.GetComponent<GUITexture>().pixelInset;
                _30.x = _29;
                s3dTexture.objectCopyR.GetComponent<GUITexture>().pixelInset = _30;
            }

            {
                float _31 = yInset;
                Rect _32 = s3dTexture.objectCopyR.GetComponent<GUITexture>().pixelInset;
                _32.y = _31;
                s3dTexture.objectCopyR.GetComponent<GUITexture>().pixelInset = _32;
            }
        }
        if (defaultTexture)
        {
            setTexture(defaultTexture);
        }
        interactiveLayerMask = (LayerMask) (1 << interactiveLayer);
        initialized = true;
    }

    public virtual void Update()
    {
        if (initialized)
        {
            if (s3dTexture.on)
            {
                if (trackMouseXYPosition)
                {
                    if (!onlyWhenMouseDown || (onlyWhenMouseDown && Input.GetMouseButton(0)))
                    {
                        s3dTexture.obPosition = matchMousePos();
                    }
                }
                if (useTouchpad)
                {
                    doTouchpad();
                }
                castForObjects();
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

    public virtual void castForObjects()
    {
        Vector2 dPosition = default(Vector2);
        RaycastHit hit = default(RaycastHit);
        s3dInteractor actScript = null;
        if ((camera3D.format3D == (mode3D) 0) && !camera3D.sideBySideSqueezed)
        {
            dPosition = new Vector2((s3dTexture.obPosition.x / 2) + 0.25f, s3dTexture.obPosition.y); // 0 = left, 0.5 = center, 1 = right
        }
        else
        {
            dPosition = s3dTexture.obPosition;
        }
        Ray ray = camera3D.GetComponent<Camera>().ViewportPointToRay(dPosition);
        if (Physics.Raycast(ray, out hit, 100f))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, new Color(0, 1, 0, 0));
            // if there's currently an activeObj, notify it of hit position
            if (activeObj && followActiveObject)
            {
                actScript = (s3dInteractor) activeObj.GetComponent(typeof(s3dInteractor));
                if (actScript)
                {
                    actScript.updatePosition(hit.point);
                }
                // if activeObj, tell any attached aimObject.js scripts to point at it
                gameObject.SendMessage("PointAt", activeObj.transform.position, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                // if no activeObj, tell any attached aimObject.js scripts to point at hitpoint
                gameObject.SendMessage("PointAt", hit.point, SendMessageOptions.DontRequireReceiver);
            }
        }
        // next, raycast against objects in interactive layer (for taps)
        ray = camera3D.GetComponent<Camera>().ViewportPointToRay(dPosition);
        if (trackMouseXYPosition)
        {
            if (Physics.Raycast(ray, out hit, clickDistance, (int) interactiveLayerMask))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    processTap(hit, true, 1); // tapped on object
                }
                else
                {
                    processRollover(hit, true); // rolled over object
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    processTap(hit, false, 1); // if there's no hit, process clicks anyway to deactivate active objects
                }
                else
                {
                    if (prevRolloverObject != null)
                    {
                        processRollover(hit, false); // lost rolled over object
                    }
                }
            }
        }
        if (useTouchpad && readyForTap)
        {
            if (Physics.Raycast(ray, out hit, clickDistance, (int) interactiveLayerMask))
            {
                if (touchpad.tap > 0)
                {
                    processTap(hit, true, touchpad.tap); // tapped on object
                }
                else
                {
                    processRollover(hit, true); // rolled over object
                }
            }
            else
            {
                if (touchpad.tap > 0)
                {
                    processTap(hit, false, touchpad.tap); // if there's no hit, process clicks anyway to deactivate active objects
                }
                else
                {
                    if (prevRolloverObject != null)
                    {
                        processRollover(hit, false); // lost rolled over object
                    }
                }
            }
            touchpad.reset();
            readyForTap = false;
            StartCoroutine(pauseAfterTap());
        }
    }

    // update parallax
    public virtual void doTouchpad()
    {
        if (uniformTouchpadMovement)
        {
            if (touchpad.position != Vector2.zero)
            {
                s3dTexture.obPosition.x = Mathf.Clamp(s3dTexture.obPosition.x + ((touchpad.position.x - touchpadPrevPosition.x) * touchpadSpeed.x), 0.05f, 0.95f);
                s3dTexture.obPosition.y = Mathf.Clamp(s3dTexture.obPosition.y + ((touchpad.position.y - touchpadPrevPosition.y) * touchpadSpeed.y), 0.05f, 0.95f);
                touchpadPrevPosition = touchpad.position;
            }
            else
            {
                touchpadPrevPosition = Vector2.zero;
            }
        }
        else
        {
            s3dTexture.obPosition.x = Mathf.Clamp(s3dTexture.obPosition.x + (touchpad.position.x * touchpadSpeed.x), 0.05f, 0.95f);
            s3dTexture.obPosition.y = Mathf.Clamp(s3dTexture.obPosition.y + (touchpad.position.y * touchpadSpeed.y), 0.05f, 0.95f);
        }
    }

    public virtual void processTap(RaycastHit theHit, bool gotHit, int tapType)
    {
        setTexture(clickTexture);
        if (clickSound)
        {
            GetComponent<AudioSource>().PlayOneShot(clickSound);
        }
        if (activeObj && (!gotHit || (activeObj != theHit.transform.gameObject))) // if there's currently an active object and there was a tap but no hit - then deactivate this object
        {
            actScript = (s3dInteractor) activeObj.GetComponent(typeof(s3dInteractor)); // or if there's currently an active object and there was a tap that hit another object - then deactivate this object
            if (actScript)
            {
                actScript.deactivateObject();
            }
        }
        else
        {
            if (gotHit) // if there's not a currently active object and there was a tap with a hit on an active object - then activate it
            {
                actScript = (s3dInteractor) theHit.transform.gameObject.GetComponent(typeof(s3dInteractor));
                if (actScript)
                {
                    actScript.tapAction(theHit, tapType);
                }
            }
        }
        StartCoroutine(unclickTexture());
    }

    public virtual void processRollover(RaycastHit theHit, bool onObject)
    {
        if (onObject)
        {
            s3dInteractor actScript = (s3dInteractor) theHit.transform.gameObject.GetComponent(typeof(s3dInteractor));
            actScript.rolloverText(theHit, true, s3dTexture.obPosition);
            prevRolloverObject = theHit.transform.gameObject;
        }
        else
        {
            actScript = (s3dInteractor) prevRolloverObject.transform.gameObject.GetComponent(typeof(s3dInteractor));
            actScript.rolloverText(theHit, false, s3dTexture.obPosition);
            prevRolloverObject = null;
        }
    }

    public virtual void setTexture(Texture tex)
    {
        GetComponent<GUITexture>().texture = tex;
        if (s3dTexture.objectCopyR)
        {
            s3dTexture.objectCopyR.GetComponent<GUITexture>().texture = tex;
        }
    }

    public virtual IEnumerator unclickTexture()
    {
        yield return new WaitForSeconds(0.2f);
        if (!activeObj)
        {
            setTexture(defaultTexture);
        }
        else
        {
            setTexture(pickTexture);
            if (pickSound)
            {
                GetComponent<AudioSource>().PlayOneShot(pickSound);
            }
        }
    }

    public virtual IEnumerator pauseAfterTap()
    {
        yield return new WaitForSeconds(0.25f);
        readyForTap = true;
    }

    public s3dGuiCursor()
    {
        interactiveLayer = 23;
        clickDistance = 20;
        touchpadSpeed = new Vector2(1f, 1f);
        uniformTouchpadMovement = true;
        touchpadPrevPosition = new Vector2(0, 0);
        readyForTap = true;
        followActiveObject = true;
    }

}