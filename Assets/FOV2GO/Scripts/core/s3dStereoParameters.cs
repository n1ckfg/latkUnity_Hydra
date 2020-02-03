using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * s3d Stereo Parameters revised 12-30-12
 * Usage: Provides an interface to adjust stereo parameters (interaxial, zero parallax distance, and horizontal image transform (HIT)
 * Option to use in conjunction with s3dDeviceManager.js or on its own 
 * Has companion Editor script to create custom inspector 
 */
[UnityEngine.ExecuteInEditMode]
public partial class s3dStereoParameters : MonoBehaviour
{
    private s3dCamera camera3D;
    public s3dDeviceManager s3dDeviceMan;
    public s3dTouchpad stereoParamsTouchpad;
    public bool saveStereoParamsToDisk;
    public Texture2D showStereoParamsTexture;
    public Texture2D dismissStereoParamsTexture;
    private bool showParamGui;
    public s3dGuiText stereoReadoutText;
    public s3dTouchpad interaxialTouchpad;
    private float interaxialStart;
    private float interaxialInc;
    public s3dTouchpad zeroPrlxTouchpad;
    private float zeroPrlxStart;
    private float zeroPrlxInc;
    public s3dTouchpad hitTouchpad;
    private float hitStart;
    private float hitInc;
    public virtual void Awake()
    {
        s3dDeviceMan = (s3dDeviceManager) gameObject.GetComponent(typeof(s3dDeviceManager));
        // if object has s3dDeviceManager.js, use that script's camera & touchpads
        if (s3dDeviceMan)
        {
            stereoParamsTouchpad = s3dDeviceMan.stereoParamsTouchpad;
            interaxialTouchpad = s3dDeviceMan.interaxialTouchpad;
            zeroPrlxTouchpad = s3dDeviceMan.zeroPrlxTouchpad;
            hitTouchpad = s3dDeviceMan.hitTouchpad;
        }
    }

    public virtual void Start()
    {
        findS3dCamera();
        if (saveStereoParamsToDisk)
        {
            if (PlayerPrefs.GetFloat(Application.loadedLevelName + "_interaxial") != 0f)
            {
                camera3D.interaxial = PlayerPrefs.GetFloat(Application.loadedLevelName + "_interaxial");
            }
            if (PlayerPrefs.GetFloat(Application.loadedLevelName + "_zeroPrlxDistance") != 0f)
            {
                camera3D.zeroPrlxDist = PlayerPrefs.GetFloat(Application.loadedLevelName + "_zeroPrlxDist");
            }
            if (PlayerPrefs.GetFloat(Application.loadedLevelName + "_H_I_T") != 0f)
            {
                camera3D.H_I_T = PlayerPrefs.GetFloat(Application.loadedLevelName + "_H_I_T");
            }
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
        if (stereoParamsTouchpad && (stereoParamsTouchpad.tap > 0))
        {
            showParamGui = !showParamGui;
            toggleStereoParamGui(showParamGui);
            stereoParamsTouchpad.reset();
            if (showParamGui)
            {
                interaxialStart = camera3D.interaxial;
                zeroPrlxStart = camera3D.zeroPrlxDist;
                hitStart = camera3D.H_I_T;
            }
            else
            {
                 // showParamGui has just been dismissed, write new values to disk
                if (saveStereoParamsToDisk)
                {
                    PlayerPrefs.SetFloat(Application.loadedLevelName + "_interaxial", camera3D.interaxial);
                    PlayerPrefs.SetFloat(Application.loadedLevelName + "_zeroPrlxDist", camera3D.zeroPrlxDist);
                    PlayerPrefs.SetFloat(Application.loadedLevelName + "_H_I_T", camera3D.H_I_T);
                }
                interaxialStart = camera3D.interaxial;
                zeroPrlxStart = camera3D.zeroPrlxDist;
                hitStart = camera3D.H_I_T;
            }
        }
        if (showParamGui)
        {
            // touchpad should be set to moveLikeJoystick = false, actLikeJoystick = true
            // so that a tapdown generates a position change
            // grab position while touchpad is being dragged - because when we actually get the tap (at TouchPhase.Ended) 
            // or the click (on Input.GetMouseButtonUp), position has already been reset to 0
            if (interaxialTouchpad.position.x != 0f)
            {
                // position values are between -1.0 and 1.0 - values < 1.0 are converted to -1.0, values > 1.0 are converted to 1.0
                interaxialInc = Mathf.Round(interaxialTouchpad.position.x + (0.49f * Mathf.Sign(interaxialTouchpad.position.x)));
            }
            if (interaxialTouchpad.tap > 0)
            {
                camera3D.interaxial = camera3D.interaxial + interaxialInc;
                camera3D.interaxial = Mathf.Max(camera3D.interaxial, 0);
                interaxialTouchpad.reset();
            }
            if (zeroPrlxTouchpad.position.x != 0f)
            {
                zeroPrlxInc = Mathf.Round(zeroPrlxTouchpad.position.x + (0.49f * Mathf.Sign(zeroPrlxTouchpad.position.x))) * 0.25f;
            }
            if (zeroPrlxTouchpad.tap > 0)
            {
                camera3D.zeroPrlxDist = camera3D.zeroPrlxDist + zeroPrlxInc;
                camera3D.zeroPrlxDist = Mathf.Max(camera3D.zeroPrlxDist, 1f);
                zeroPrlxTouchpad.reset();
            }
            if (hitTouchpad.position.x != 0f)
            {
                hitInc = Mathf.Round(hitTouchpad.position.x + (0.49f * Mathf.Sign(hitTouchpad.position.x))) * 0.1f;
            }
            if (hitTouchpad.tap > 0)
            {
                camera3D.H_I_T = camera3D.H_I_T + hitInc;
                hitTouchpad.reset();
            }
            stereoReadoutText.setText((((("Interaxial: " + (Mathf.Round(camera3D.interaxial * 10) / 10)) + "mm \nZero Prlx: ") + (Mathf.Round(camera3D.zeroPrlxDist * 10) / 10)) + "M \nH.I.T.: ") + (Mathf.Round(camera3D.H_I_T * 10) / 10));
        }
    }

    public virtual void toggleStereoParamGui(bool on)
    {
        if (on)
        {
            ((GUITexture) stereoParamsTouchpad.gameObject.GetComponent(typeof(GUITexture))).texture = dismissStereoParamsTexture;
            stereoReadoutText.toggleVisible(true);
        }
        else
        {
            ((GUITexture) stereoParamsTouchpad.gameObject.GetComponent(typeof(GUITexture))).texture = showStereoParamsTexture;
            stereoReadoutText.toggleVisible(false);
        }
    }

}