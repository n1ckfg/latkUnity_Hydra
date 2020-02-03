using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class triggerSceneChange : MonoBehaviour
{
    /* This file is part of Stereoskopix FOV2GO for Unity. * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu */
    /* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
    /* Usage: attach this script to s3dTouchpad
 * tap on touchpad to load next level
 * s3dTouchpad mimics touchpad input with mouse on desktop.
 */
    private s3dTouchpad touchpad;
    // 0 = ignore (don't trigger)
    // 1 = trigger with short tap only 
    // 2 = trigger with long tap only
    // 3 = trigger with any tap
    public int tapType;
    // seconds to pause before loading next level
    // GUI Text for loading message
    public s3dGuiText loadingText;
    public string loadingMessage;
    public float pauseBeforeLoad;
    private bool ready;
    public virtual void Start()
    {
        this.touchpad = (s3dTouchpad) this.gameObject.GetComponent(typeof(s3dTouchpad));
        this.StartCoroutine(this.waitABit());
    }

    public virtual IEnumerator waitABit()
    {
        yield return new WaitForSeconds(0.5f);
        this.ready = true;
    }

    public virtual void Update()
    {
        if (this.ready)
        {
            if (this.touchpad.tap > 0)
            {
                if ((this.touchpad.tap == this.tapType) || (this.tapType == 3))
                {
                    this.StartCoroutine(this.loadNewScene());
                }
            }
        }
    }

    public virtual IEnumerator loadNewScene()
    {
        this.touchpad.reset();
        if (this.loadingText)
        {
            this.loadingText.setText(this.loadingMessage);
            this.loadingText.toggleVisible(true);
        }
        yield return new WaitForSeconds(this.pauseBeforeLoad);
        if (this.loadingText)
        {
            this.loadingText.toggleVisible(false);
        }
        int nextLevel = Application.loadedLevel + 1;
        if (nextLevel > (Application.levelCount - 1))
        {
            nextLevel = 0;
        }
        Application.LoadLevel(nextLevel);
    }

    public triggerSceneChange()
    {
        this.tapType = 3;
        this.loadingMessage = "Loading...";
        this.pauseBeforeLoad = 1;
    }

}