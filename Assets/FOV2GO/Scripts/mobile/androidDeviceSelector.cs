using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* automatically set FOV2GO layout for Android devices
 * works together with s3dDeviceManager.js
 * checks resolution of device and - if resolution matches any supported Android device -
 * switches to that screen layout
 * so a single app can run on all supported devices
 * checks both orientations - layouts are for landscape orientation, but checks for portrait
 * just in case app is starting up in portrait, assuming it will be rotated to landscape in a moment
 * (s3dGyroCam.js monitors AutoRotation & updates display when it changes) 
 */
[UnityEngine.RequireComponent(typeof(s3dDeviceManager))]
public partial class androidDeviceSelector : MonoBehaviour
{
    private s3dDeviceManager s3dDeviceMan;
    public virtual void Start()
    {
        this.StartCoroutine(this.checkResolutionToDetermineDevice());
    }

    public virtual IEnumerator checkResolutionToDetermineDevice()
    {
        yield return new WaitForSeconds(1f);
        this.s3dDeviceMan = (s3dDeviceManager) this.gameObject.GetComponent(typeof(s3dDeviceManager));
        if (this.s3dDeviceMan)
        {
            if (((Screen.width == 800) && (Screen.height == 480)) || ((Screen.width == 480) && (Screen.height == 800)))
            {
                this.s3dDeviceMan.phoneLayout = phoneType.Thrill_LandLeft;
            }
            else
            {
                if (((Screen.width == 960) && (Screen.height == 540)) || ((Screen.width == 540) && (Screen.height == 960)))
                {
                    this.s3dDeviceMan.phoneLayout = phoneType.OneS_LandLeft;
                }
                else
                {
                    if (((Screen.width == 1280) && (Screen.height == 720)) || ((Screen.width == 720) && (Screen.height == 1280)))
                    {
                        this.s3dDeviceMan.phoneLayout = phoneType.GalaxyNexus_LandLeft;
                    }
                    else
                    {
                        if (((Screen.width == 1280) && (Screen.height == 800)) || ((Screen.width == 800) && (Screen.height == 1280)))
                        {
                            this.s3dDeviceMan.phoneLayout = phoneType.GalaxyNote_LandLeft;
                        }
                        else
                        {
                        }
                    }
                }
            }
            // (resolution doesn't match any iOS device, so leave it alone)
            this.s3dDeviceMan.setPhoneLayout();
            this.s3dDeviceMan.camera3D.initStereoCamera();
        }
    }

}