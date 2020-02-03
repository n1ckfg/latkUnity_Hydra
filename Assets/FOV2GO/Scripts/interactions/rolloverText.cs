using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* This is an s3d Interaction script.
 * Requires a s3dInteractor component
 * add to an interactive object, assign a s3dGuiText
 * set rollover message to appear when s3d Gui Pointer rolls over object
 */
[UnityEngine.RequireComponent(typeof(s3dInteractor))]
public partial class rolloverText : MonoBehaviour
{
    public s3dGuiText theText;
    public Vector2 offset;
    public string message;
    public virtual void ShowText(Vector2 obPosition)
    {
        Vector2 textPos = obPosition + offset;
        theText.setObPosition(textPos);
        theText.setText(message);
        theText.toggleVisible(true);
    }

    public virtual void HideText(Vector2 obPosition)
    {
        theText.toggleVisible(false);
    }

    public rolloverText()
    {
        offset = new Vector2(0, 0.15f);
        message = "rollover message";
    }

}