using UnityEngine;
using System.Collections;

/* This file is part of Stereoskopix FOV2GO for Unity V3.
 * URL: http://www.stereoskopix.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 *
 * S3D Interactor Script revised 12.30.12
 * To make any object interactive:
  - put it in the Interactive layer
  - add this script
 * This script receives interaction messages from object with s3d_Gui_Pointer - with s3dGuiTexture.js script
  - add interaction scripts to object that implement functions sent from this script
 */
// called from s3dGuiTexture.js
 // 1 = short tap 2 = long tap
// called from: s3dGuiTexture.js, aimObject.js
// called from s3dGuiTexture.js
// called from s3dGuiTexture.js
[System.Serializable]
public class TapParams : object
{
    public RaycastHit hit;
    public int tap;
    //constructor
    public TapParams(RaycastHit hit, int tap)
    {
        this.hit = hit;
        this.tap = tap;
    }

}
[System.Serializable]
public partial class s3dInteractor : MonoBehaviour
{
    public virtual void tapAction(RaycastHit theHit, int theTap)
    {
        this.gameObject.SendMessage("NewTap", new TapParams(theHit, theTap), SendMessageOptions.DontRequireReceiver);
    }

    public virtual void updatePosition(Vector3 thePos)
    {
        this.gameObject.SendMessage("NewPosition", thePos, SendMessageOptions.DontRequireReceiver);
    }

    public virtual void deactivateObject()
    {
        this.gameObject.SendMessage("Deactivate", SendMessageOptions.DontRequireReceiver);
    }

    public virtual void rolloverText(RaycastHit theHit, object onObject, Vector2 obPosition)
    {
        if (onObject != null)
        {
            this.gameObject.SendMessage("ShowText", obPosition, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            this.gameObject.SendMessage("HideText", obPosition, SendMessageOptions.DontRequireReceiver);
        }
    }

}