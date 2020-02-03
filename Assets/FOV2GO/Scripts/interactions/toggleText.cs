using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* This is an s3d Interaction script.
 * Requires a s3dInteractor component
 * simple script to toggle s3dGuiText on/off
 */
 // 1 = short tap 2 = long tap 3 = any tap
[UnityEngine.RequireComponent(typeof(s3dInteractor))]
public partial class toggleText : MonoBehaviour
{
    public GUIText textObject;
    private s3dGuiText textScript;
    public bool textOn;
    public int tapType;
    public virtual void Start()
    {
        this.textScript = (s3dGuiText) this.textObject.GetComponent(typeof(s3dGuiText));
    }

    public virtual void NewTap(TapParams @params)
    {
        if ((@params.tap == this.tapType) || (this.tapType == 3))
        {
            this.textOn = !this.textOn;
            this.textScript.toggleVisible(this.textOn);
        }
    }

    public toggleText()
    {
        this.tapType = 2;
    }

}