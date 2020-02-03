using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* This is an s3d Interaction script.
 * Requires a s3dInteractor component
 * First tap starts audioClip playing
 * If toggleOnOff = true, next tap stops audioClip
 - If rewindOnStop = true, next tap starts audioClip from beginning
 - If rewindOnStop = false, next tap starts audioClip from where it stopped
 * If toggleOnOff = false, audioClip plays through to the end
 - additional taps have no effect until clip is finished, then next tap starts clip again
*/
[UnityEngine.RequireComponent(typeof(AudioSource))]
[UnityEngine.RequireComponent(typeof(s3dInteractor))]
public partial class playSoundSimple : MonoBehaviour
{
    public bool toggleOnOff;
    public bool rewindOnStop;
    public int tapType; // 1 = short tap 2 = long tap 3 = any tap
    public virtual void Start()
    {
    }

    public virtual void NewTap(TapParams @params)
    {
        if ((@params.tap == this.tapType) || (this.tapType == 3))
        {
            if (!this.GetComponent<AudioSource>().isPlaying)
            {
                this.GetComponent<AudioSource>().Play();
            }
            else
            {
                if (this.toggleOnOff)
                {
                    if (this.rewindOnStop)
                    {
                        this.GetComponent<AudioSource>().Stop();
                    }
                    else
                    {
                        this.GetComponent<AudioSource>().Pause();
                    }
                }
            }
        }
    }

    public playSoundSimple()
    {
        this.toggleOnOff = true;
        this.rewindOnStop = true;
        this.tapType = 3;
    }

}