using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* automatically set FOV2GO layout for iOS devices
 * works together with s3dDeviceManager.js
 * checks resolution of device and - if resolution matches any supported iOS device -
 * switches to that screen layout
 * so a single app can run on all supported devices
 * note that there are FOV2GO viewers for iPad 2 & 3 in both landscape and portrait orientation
 * so the device's orientation at startup determines which layout will be selected
 */
[UnityEngine.RequireComponent(typeof(s3dDeviceManager))]
public partial class iosDeviceSelector : MonoBehaviour
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
            if (((Screen.width == 960) && (Screen.height == 640)) || ((Screen.width == 640) && (Screen.height == 960)))
            {
                this.s3dDeviceMan.phoneLayout = phoneType.iPhone4_LandLeft;
            }
            else
            {
                if ((Screen.width == 1024) && (Screen.height == 768))
                {
                    this.s3dDeviceMan.phoneLayout = phoneType.iPad2_LandLeft;
                }
                else
                {
                    if ((Screen.width == 768) && (Screen.height == 1024))
                    {
                        this.s3dDeviceMan.phoneLayout = phoneType.iPad2_Portrait;
                    }
                    else
                    {
                        if ((Screen.width == 2048) && (Screen.height == 1536))
                        {
                            this.s3dDeviceMan.phoneLayout = phoneType.iPad3_LandLeft;
                        }
                        else
                        {
                            if ((Screen.width == 1536) && (Screen.height == 2048))
                            {
                                this.s3dDeviceMan.phoneLayout = phoneType.iPad3_Portrait;
                            }
                            else
                            {
                            }
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