using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class aimObject : MonoBehaviour
{
    /* This file is part of Stereoskopix FOV2GO for Unity. * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu */
    /* Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman All rights reserved.
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
   * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
   * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. */
    /* attach this script to s3d_GUI_Pointer
 * requires s3dGuiTexture
 * assign an object to notify - that object requires an s3dInteractor script, as well as a pointAtPoint script 
 */
    public GameObject objectToNotify;
    private s3dInteractor actScript;
    public virtual void Start()
    {
        if (this.objectToNotify)
        {
            this.actScript = (s3dInteractor) this.objectToNotify.GetComponent(typeof(s3dInteractor));
        }
    }

    // triggered from s3dGuiTexture.js
    public virtual void PointAt(Vector3 thePoint)
    {
        if (this.actScript)
        {
            this.actScript.updatePosition(thePoint);
        }
    }

}