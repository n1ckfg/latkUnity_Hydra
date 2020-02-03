using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class triggerObjectButton : MonoBehaviour
{
    /* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
    /* Usage: attach this script to s3dTouchpad
 * toggle chosen object on/off (default short tap) and/or
 * trigger action on chosen object ("Go" function on object) (default long tap)
 * s3dTouchpad.js mimics touchpad input using mouse on desktop. 
 */
    public GameObject theObject;
    // 0 = ignore (don't trigger)
    // 1 = trigger with short tap only 
    // 2 = trigger with long tap only
    // 3 = trigger with any tap
    public int toggleTapType;
    public int actionTapType;
    public bool startOn;
    private s3dTouchpad touchpad;
    private bool onOff;
    public virtual void Start()
    {
        this.touchpad = (s3dTouchpad) this.gameObject.GetComponent(typeof(s3dTouchpad));
        this.toggleOnOff(this.startOn);
    }

    public virtual void Update()
    {
        if (this.touchpad.tap > 0)
        {
            if ((this.touchpad.tap == this.toggleTapType) || (this.toggleTapType == 3))
            {
                this.onOff = !this.onOff;
                this.toggleOnOff(this.onOff);
            }
            if ((this.touchpad.tap == this.actionTapType) || (this.actionTapType == 3))
            {
                this.triggerAction();
            }
        }
    }

    public virtual void toggleOnOff(bool onoff)
    {
        this.onOff = onoff;
        this.touchpad.reset();
    }

    public virtual void triggerAction()
    {
        this.theObject.SendMessage("Go", SendMessageOptions.DontRequireReceiver);
        this.touchpad.reset();
    }

    public triggerObjectButton()
    {
        this.toggleTapType = 1;
        this.actionTapType = 2;
        this.onOff = true;
    }

}