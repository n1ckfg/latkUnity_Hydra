using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* This is an s3d Interaction script.
 * Requires a s3dInteractor component
 * toggles between lighting on and off states
 * option to swap out skyboxes along with lighting states
 * supports multiple switches, all toggling same light
 * designate one switch as the masterSwitch, make sure it's the same on all instances of the script
*/
 // 1 = short tap 2 = long tap 3 = any tap
// audio clip to play for switch
[UnityEngine.RequireComponent(typeof(AudioSource))]
[UnityEngine.RequireComponent(typeof(s3dInteractor))]
public partial class toggleLighting : MonoBehaviour
{
    public GameObject masterSwitch;
    public Light roomlight;
    public float roomlightIntensityOn;
    public float roomlightIntensityOff;
    public bool useSkyBox;
    public GameObject skyboxDay;
    public GameObject skyboxNight;
    public int tapType;
    private toggleLighting thisScript;
    public bool roomLightOn;
    public AudioClip clickSound;
    public virtual void Start()
    {
        if (this.gameObject == this.masterSwitch)
        {
            if (!this.skyboxDay || !this.skyboxNight)
            {
                this.useSkyBox = false;
            }
            this.roomLightOn = !this.roomLightOn; // initial double swap
            this.switchLighting();
        }
        this.thisScript = (toggleLighting) this.masterSwitch.GetComponent(typeof(toggleLighting));
    }

    public virtual void NewTap(TapParams @params)
    {
        if ((@params.tap == this.tapType) || (this.tapType == 3))
        {
            if (this.clickSound)
            {
                this.GetComponent<AudioSource>().PlayOneShot(this.clickSound);
            }
            this.thisScript.switchLighting();
        }
    }

    public virtual void switchLighting()
    {
        this.roomLightOn = !this.roomLightOn;
        if (this.roomLightOn)
        {
            this.roomlight.GetComponent<Light>().intensity = this.roomlightIntensityOn;
            if (this.useSkyBox)
            {
            }
        }
        else
        {
            this.roomlight.GetComponent<Light>().intensity = this.roomlightIntensityOff;
            if (this.useSkyBox)
            {
            }
        }
    }

    public toggleLighting()
    {
        this.roomlightIntensityOn = 1.5f;
        this.roomlightIntensityOff = 0.5f;
        this.tapType = 3;
        this.roomLightOn = true;
    }

}