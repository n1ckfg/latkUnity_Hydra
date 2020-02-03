using UnityEngine;
using System.Collections;

/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d Touchpad Script - revised 12-30-12 - based on Joystick.js
 * rewritten to combine elements of touchpad behavior with optional joystick visual movement, or joystick behavior without visual movement.
 * and to assume one touch per touchpad, so no multiple finger latching
 * and to deal only with single taps, so no tapCount - thus we can use TouchPhase to deal with taps
 * also, we don't need to worry about keeping track of all the touchpads, because they don't interfere with each other
 * if a TouchPhase has never been TouchPhase.Moved when it becomes TouchPhase.Ended (or if it hasn't moved more than tapDistanceLimit), then it's a tap
 * otherwise its a drag/swipe
 * distinguishes between short and long taps with shortTapTimeMax and longTapTimeMax
 */
// A simple class for bounding how far the GUITexture will move
[System.Serializable]
public class Boundary : object
{
    public Vector2 min;
    public Vector2 max;
    public Boundary()
    {
        min = Vector2.zero;
        max = Vector2.zero;
    }

}
[System.Serializable]
[UnityEngine.RequireComponent(typeof(GUITexture))]
 // [-1, 1] in x,y
 // 1 for short tap, 2 for long tap
 // Joystick graphic
 // Default position / extents of the joystick graphic
 // Boundary for joystick graphic
 // Offset to apply to touch input
 // Center of joystick
[UnityEngine.AddComponentMenu("Stereoskopix/s3d Touchpad")]
public partial class s3dTouchpad : MonoBehaviour
{
    public bool moveLikeJoystick;
    public bool actLikeJoystick;
    public float shortTapTimeMax;
    public float longTapTimeMax;
    public float tapDistanceLimit;
    private Rect touchZone;
    public Vector2 position;
    public int tap;
    private GUITexture gui;
    private Rect defaultRect;
    private Boundary guiBoundary;
    private Vector2 guiTouchOffset;
    private Vector2 guiCenter;
    private int thisTouchID;
    private float thisTouchDownTime;
    private bool thisTouchMoved;
    private Vector2 fingerDownPos;
    private Vector2 fingerUpPos;
    private float fingerDownTime;
    public virtual void Start()
    {
        setUp();
    }

    public virtual void setUp()
    {
        // Cache this component at startup instead of looking up every frame	
        gui = (GUITexture) GetComponent(typeof(GUITexture));
        // Store the default rect for the gui, so we can snap back to it
        defaultRect = gui.pixelInset;
        defaultRect.x = defaultRect.x + (transform.position.x * Screen.width);// + gui.pixelInset.x; // -  Screen.width * 0.5;
        defaultRect.y = defaultRect.y + (transform.position.y * Screen.height);// - Screen.height * 0.5;

        {
            float _53 = 0f;
            Vector3 _54 = transform.position;
            _54.x = _53;
            transform.position = _54;
        }

        {
            float _55 = 0f;
            Vector3 _56 = transform.position;
            _56.y = _55;
            transform.position = _56;
        }
        // If a texture has been assigned, then use the rect from the gui as our touchZone
        if (gui.texture)
        {
            touchZone = defaultRect;
        }
        // This is an offset for touch input to match with the top left corner of the GUI
        guiTouchOffset.x = defaultRect.width * 0.5f;
        guiTouchOffset.y = defaultRect.height * 0.5f;
        // Cache the center of the GUI, since it doesn't change
        guiCenter.x = defaultRect.x + guiTouchOffset.x;
        guiCenter.y = defaultRect.y + guiTouchOffset.y;
        // Let's build the GUI boundary, so we can clamp joystick movement
        guiBoundary.min.x = defaultRect.x - guiTouchOffset.x;
        guiBoundary.max.x = defaultRect.x + guiTouchOffset.x;
        guiBoundary.min.y = defaultRect.y - guiTouchOffset.y;
        guiBoundary.max.y = defaultRect.y + guiTouchOffset.y;
        gui.pixelInset = defaultRect;
    }

    public virtual void Update()
    {
        Vector2 guiTouchPos = (Vector2) Input.mousePosition - guiTouchOffset;
        if (touchZone.Contains(Input.mousePosition))
        {
            if (Input.GetMouseButtonDown(0))
            {
                thisTouchID = 1;
                fingerDownPos = Input.mousePosition;
                thisTouchDownTime = Time.time;
                thisTouchMoved = false;
            }
        }
        if (thisTouchID == 1)
        {
            if (!actLikeJoystick)
            {
                position.x = Mathf.Clamp((Input.mousePosition.x - fingerDownPos.x) / (touchZone.width / 2), -1, 1);
                position.y = Mathf.Clamp((Input.mousePosition.y - fingerDownPos.y) / (touchZone.height / 2), -1, 1);
            }
            if (moveLikeJoystick)
            {

                {
                    float _57 = Mathf.Clamp(guiTouchPos.x, guiBoundary.min.x, guiBoundary.max.x);
                    Rect _58 = gui.pixelInset;
                    _58.x = _57;
                    gui.pixelInset = _58;
                }

                {
                    float _59 = Mathf.Clamp(guiTouchPos.y, guiBoundary.min.y, guiBoundary.max.y);
                    Rect _60 = gui.pixelInset;
                    _60.y = _59;
                    gui.pixelInset = _60;
                }
            }
            if (actLikeJoystick)
            {
                float dummyInsetX = Mathf.Clamp(guiTouchPos.x, guiBoundary.min.x, guiBoundary.max.x);
                float dummyInsetY = Mathf.Clamp(guiTouchPos.y, guiBoundary.min.y, guiBoundary.max.y);
                position.x = ((dummyInsetX + guiTouchOffset.x) - guiCenter.x) / guiTouchOffset.x;
                position.y = ((dummyInsetY + guiTouchOffset.y) - guiCenter.y) / guiTouchOffset.y;
            }
        }
        if (Input.GetMouseButtonUp(0) && (thisTouchID == 1))
        {
            fingerUpPos = Input.mousePosition;
            float dist = Vector2.Distance(fingerDownPos, fingerUpPos);
            if (dist < tapDistanceLimit)
            {
                if (Time.time < (thisTouchDownTime + shortTapTimeMax))
                {
                    tap = 1;
                }
                else
                {
                    if (Time.time < (thisTouchDownTime + longTapTimeMax))
                    {
                        tap = 2;
                    }
                }
            }
            thisTouchID = -1;
            position = Vector2.zero;
            if (moveLikeJoystick)
            {
                gui.pixelInset = defaultRect;
            }
        }
    }

    /* The client that directly registers the tap is responsible for resetting touchpad. Currently, the following scripts call this function: s3dDeviceManager.js, s3dFirstPersonController.js, s3dGuiTexture.js, triggerObjectButton.js, triggerSceneChange.js. Note that since s3dInteractor.js (called by s3dGuiTexture.js) & all interaction scripts (called by s3dInteractor.js) are not direct clients, they aren't responsible for resetting touchpad */
    public virtual void reset()
    {
        tap = 0;
    }

    public s3dTouchpad()
    {
        moveLikeJoystick = true;
        shortTapTimeMax = 0.2f;
        longTapTimeMax = 0.5f;
        tapDistanceLimit = 10f;
        guiBoundary = new Boundary();
        thisTouchID = -1;
    }

}