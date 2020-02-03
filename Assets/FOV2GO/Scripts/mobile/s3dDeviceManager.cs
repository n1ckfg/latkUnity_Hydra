using UnityEngine;
using System.Collections;

[System.Serializable]
/* This file is part of Stereoskopix FOV2GO for Unity V2.
 * URL: http://diy.mxrlab.com/ * Please direct any bugs/comments/suggestions to hoberman@usc.edu.
 * Stereoskopix FOV2GO for Unity Copyright (c) 2011-12 Perry Hoberman & MxR Lab. All rights reserved.
 */
/* s3d Device Manager.
 * Usage: Manages layout and interface elements for Android & iOS phones and tablets 
 * Has companion Editor script to create custom inspector 
 */
[UnityEngine.ExecuteInEditMode]
public partial class s3dDeviceManager : MonoBehaviour
{
    // enum phoneType {GalaxyNexus_LandLeft, GalaxyNote_LandLeft, iPad2_LandLeft, iPad2_Portrait, iPad3_LandLeft, iPad3_Portrait, iPhone4_LandLeft, OneS_LandLeft, Rezound_LandLeft, Thrill_LandLeft, my3D_LandLeft};
    // declared in s3dEnums.js
    public phoneType phoneLayout;
    private phoneType prevPhoneLayout;
    public Vector2 phoneResolution;
    public s3dCamera camera3D;
    // cursor controls
    public bool use3dCursor;
    public s3dGuiCursor cursor3D;
    private float cursorSize;
    // interface controls
    //enum controlPos {off,left,center,right}
    public controlPos movePadPosition;
    public controlPos turnPadPosition;
    public controlPos pointPadPosition;
    public s3dTouchpad moveTouchpad;
    public s3dTouchpad turnTouchpad;
    public s3dTouchpad pointTouchpad;
    // stereo parameter controls
    public bool useStereoParamsTouchpad;
    public s3dTouchpad stereoParamsTouchpad;
    private Rect stereoParamsRect;
    public s3dTouchpad interaxialTouchpad;
    private Rect interaxialRect;
    public s3dTouchpad zeroPrlxTouchpad;
    private Rect zeroPrlxRect;
    public s3dTouchpad hitTouchpad;
    private Rect hitRect;
    // load new scene control
    public bool showLoadNewScenePad;
    public s3dTouchpad loadNewSceneTouchpad;
    private Rect loadNewSceneRect;
    // first-person tool controls
    public bool showFpsTool01;
    public s3dTouchpad fpsTool01;
    public bool showFpsTool02;
    public s3dTouchpad fpsTool02;
    private Rect fpsTool01Rect;
    private Rect fpsTool02Rect;
    public virtual void Start()
    {
        this.findS3dCamera();
        this.prevPhoneLayout = this.phoneLayout;
        this.setPhoneLayout();
    }

    public virtual void findS3dCamera()
    {
        s3dCamera[] cameras3D = ((s3dCamera[]) UnityEngine.Object.FindObjectsOfType(typeof(s3dCamera))) as s3dCamera[];
        if (cameras3D.Length == 1)
        {
            this.camera3D = cameras3D[0];
        }
        else
        {
            if (cameras3D.Length > 1)
            {
                MonoBehaviour.print("There is more than one s3dCamera in this scene.");
            }
            else
            {
                MonoBehaviour.print("There is no s3dCamera in this scene.");
            }
        }
    }

    public virtual void setPhoneLayout()
    {
        Vector3 leftControlPos = default(Vector3);
        Rect leftControlRect = default(Rect);
        Vector3 middleControlPos = default(Vector3);
        Rect middleControlRect = default(Rect);
        Vector3 rightControlPos = default(Vector3);
        Rect rightControlRect = default(Rect);
        this.camera3D.useStereoShader = false;
        this.camera3D.format3D = mode3D.SideBySide;
        this.camera3D.sideBySideSqueezed = false;
        if (!this.camera3D.usePhoneMask)
        {
            this.camera3D.usePhoneMask = true;
        }
        switch (this.phoneLayout)
        {
            case phoneType.GalaxyNexus_LandLeft:
                this.camera3D.leftViewRect = new Vector4(0f, 0.19f, 0.49f, 0.81f);
                this.camera3D.rightViewRect = new Vector4(0.51f, 0.19f, 0.49f, 0.81f);
                this.setCameraRect(this.camera3D.leftViewRect, this.camera3D.rightViewRect);
                this.camera3D.H_I_T = 2;
                this.cursorSize = 64;
                leftControlPos = new Vector3(0, 0, 0);
                leftControlRect = new Rect(0, 0, 400, 200);
                middleControlPos = new Vector3(0.5f, 0, 0);
                middleControlRect = new Rect(-200, 0, 400, 200);
                rightControlPos = new Vector3(1, 0, 0);
                rightControlRect = new Rect(-400, 0, 400, 200);
                this.stereoParamsRect = new Rect(-64, 280, 128, 128);
                this.interaxialRect = new Rect(150, 200, 300, 80);
                this.zeroPrlxRect = new Rect(-150, 200, 300, 80);
                this.hitRect = new Rect(-450, 200, 300, 80);
                this.loadNewSceneRect = new Rect(-64, 440, 128, 128);
                this.fpsTool01Rect = new Rect(0, 200, 128, 128);
                this.fpsTool02Rect = new Rect(-128, 200, 128, 128);
                break;
            case phoneType.GalaxyNote_LandLeft:
                this.camera3D.leftViewRect = new Vector4(0f, 0.22f, 0.49f, 0.78f);
                this.camera3D.rightViewRect = new Vector4(0.51f, 0.22f, 0.49f, 0.78f);
                this.setCameraRect(this.camera3D.leftViewRect, this.camera3D.rightViewRect);
                this.camera3D.H_I_T = 0;
                this.cursorSize = 64;
                leftControlPos = new Vector3(0, 0, 0);
                leftControlRect = new Rect(0, 0, 400, 200);
                middleControlPos = new Vector3(0.5f, 0, 0);
                middleControlRect = new Rect(-200, 0, 400, 200);
                rightControlPos = new Vector3(1, 0, 0);
                rightControlRect = new Rect(-400, 0, 400, 200);
                this.stereoParamsRect = new Rect(-64, 300, 128, 128);
                this.interaxialRect = new Rect(150, 200, 300, 80);
                this.zeroPrlxRect = new Rect(-150, 200, 300, 80);
                this.hitRect = new Rect(-450, 200, 300, 80);
                this.loadNewSceneRect = new Rect(-64, 450, 128, 128);
                this.fpsTool01Rect = new Rect(0, 200, 128, 128);
                this.fpsTool02Rect = new Rect(-128, 200, 128, 128);
                break;
            case phoneType.iPad2_LandLeft:
                this.camera3D.leftViewRect = new Vector4(0, 0.33f, 0.5f, 0.67f);
                this.camera3D.rightViewRect = new Vector4(0.5f, 0.33f, 0.5f, 0.67f);
                this.setCameraRect(this.camera3D.leftViewRect, this.camera3D.rightViewRect);
                this.camera3D.H_I_T = -12;
                this.cursorSize = 48;
                leftControlPos = new Vector3(0, 0, 0);
                leftControlRect = new Rect(0, 256, 300, 150);
                middleControlPos = new Vector3(0.5f, 0, 0);
                middleControlRect = new Rect(-100, 160, 200, 100);
                rightControlPos = new Vector3(1, 0, 0);
                rightControlRect = new Rect(-300, 256, 300, 150);
                this.stereoParamsRect = new Rect(-24, 96, 48, 48);
                this.interaxialRect = new Rect(120, 64, 240, 32);
                this.zeroPrlxRect = new Rect(-120, 64, 240, 32);
                this.hitRect = new Rect(-360, 64, 240, 32);
                this.loadNewSceneRect = new Rect(-24, 16, 48, 48);
                this.fpsTool01Rect = new Rect(128, 192, 64, 64);
                this.fpsTool02Rect = new Rect(-192, 192, 64, 64);
                break;
            case phoneType.iPad2_Portrait:
                this.camera3D.leftViewRect = new Vector4(0.08f, 0.6875f, 0.42f, 0.3125f);
                this.camera3D.rightViewRect = new Vector4(0.5f, 0.6875f, 0.42f, 0.3125f);
                this.setCameraRect(this.camera3D.leftViewRect, this.camera3D.rightViewRect);
                this.camera3D.H_I_T = 0;
                this.cursorSize = 48;
                leftControlPos = new Vector3(0, 1, 0);
                leftControlRect = new Rect(0, -640, 384, 192);
                middleControlPos = new Vector3(0.5f, 1, 0);
                middleControlRect = new Rect(-128, -768, 256, 128);
                rightControlPos = new Vector3(1, 1, 0);
                rightControlRect = new Rect(-384, -640, 384, 192);
                this.stereoParamsRect = new Rect(-32, 164, 64, 64);
                this.interaxialRect = new Rect(0, 92, 256, 64);
                this.zeroPrlxRect = new Rect(-128, 92, 256, 64);
                this.hitRect = new Rect(-256, 92, 256, 64);
                this.loadNewSceneRect = new Rect(-32, 16, 64, 64);
                this.fpsTool01Rect = new Rect(0, 256, 128, 128);
                this.fpsTool02Rect = new Rect(-128, 256, 128, 128);
                break;
            case phoneType.iPad3_LandLeft:
                this.camera3D.leftViewRect = new Vector4(0, 0.33f, 0.5f, 0.67f);
                this.camera3D.rightViewRect = new Vector4(0.5f, 0.33f, 0.5f, 0.67f);
                this.setCameraRect(this.camera3D.leftViewRect, this.camera3D.rightViewRect);
                this.camera3D.H_I_T = -12;
                this.cursorSize = 64;
                leftControlPos = new Vector3(0, 0, 0);
                leftControlRect = new Rect(0, 640, 600, 300);
                middleControlPos = new Vector3(0, 0, 0);
                middleControlRect = new Rect(0, 256, 400, 200);
                rightControlPos = new Vector3(1, 0, 0);
                rightControlRect = new Rect(-600, 640, 600, 300);
                this.stereoParamsRect = new Rect(-64, 280, 128, 128);
                this.interaxialRect = new Rect(240, 180, 480, 80);
                this.zeroPrlxRect = new Rect(-240, 180, 480, 80);
                this.hitRect = new Rect(-720, 180, 480, 80);
                this.loadNewSceneRect = new Rect(-64, 32, 128, 128);
                this.fpsTool01Rect = new Rect(384, 512, 128, 128);
                this.fpsTool02Rect = new Rect(-512, 512, 128, 128);
                break;
            case phoneType.iPad3_Portrait:
                this.camera3D.leftViewRect = new Vector4(0.08f, 0.6875f, 0.42f, 0.3125f);
                this.camera3D.rightViewRect = new Vector4(0.5f, 0.6875f, 0.42f, 0.3125f);
                this.setCameraRect(this.camera3D.leftViewRect, this.camera3D.rightViewRect);
                this.camera3D.H_I_T = 0;
                this.cursorSize = 64;
                leftControlPos = new Vector3(0, 1, 0);
                //leftControlRect = Rect(120, -1280, 512, 512);
                leftControlRect = new Rect(0, -1280, 768, 384);
                middleControlPos = new Vector3(0.5f, 1, 0);
                middleControlRect = new Rect(-256, -1536, 512, 256);
                rightControlPos = new Vector3(1, 1, 0);
                rightControlRect = new Rect(-768, -1280, 768, 384);
                this.stereoParamsRect = new Rect(-64, 328, 128, 128);
                this.interaxialRect = new Rect(0, 184, 512, 128);
                this.zeroPrlxRect = new Rect(-256, 184, 512, 128);
                this.hitRect = new Rect(-512, 184, 512, 128);
                this.loadNewSceneRect = new Rect(-64, 32, 128, 128);
                this.fpsTool01Rect = new Rect(0, 512, 256, 256);
                this.fpsTool02Rect = new Rect(-256, 512, 256, 256);
                break;
            case phoneType.iPhone4_LandLeft:
                this.camera3D.leftViewRect = new Vector4(0f, 0.27f, 0.49f, 0.73f);
                this.camera3D.rightViewRect = new Vector4(0.51f, 0.27f, 0.49f, 0.73f);
                this.setCameraRect(this.camera3D.leftViewRect, this.camera3D.rightViewRect);
                this.camera3D.H_I_T = 4;
                this.cursorSize = 64;
                leftControlPos = new Vector3(0, 0, 0);
                leftControlRect = new Rect(0, 0, 320, 160);
                middleControlPos = new Vector3(0.5f, 0, 0);
                middleControlRect = new Rect(-160, 0, 320, 160);
                rightControlPos = new Vector3(1, 0, 0);
                rightControlRect = new Rect(-320, 0, 320, 160);
                this.stereoParamsRect = new Rect(-50, 260, 100, 100);
                this.interaxialRect = new Rect(120, 180, 240, 60);
                this.zeroPrlxRect = new Rect(-120, 180, 240, 60);
                this.hitRect = new Rect(-360, 180, 240, 60);
                this.loadNewSceneRect = new Rect(-50, 380, 100, 100);
                this.fpsTool01Rect = new Rect(0, 172, 100, 100);
                this.fpsTool02Rect = new Rect(-100, 172, 100, 100);
                break;
            case phoneType.OneS_LandLeft:
                this.camera3D.leftViewRect = new Vector4(0f, 0.18f, 0.49f, 0.82f);
                this.camera3D.rightViewRect = new Vector4(0.51f, 0.18f, 0.49f, 0.82f);
                this.setCameraRect(this.camera3D.leftViewRect, this.camera3D.rightViewRect);
                this.camera3D.H_I_T = 8;
                this.cursorSize = 64;
                leftControlPos = new Vector3(0, 0, 0);
                leftControlRect = new Rect(0, 0, 240, 120);
                middleControlPos = new Vector3(0.5f, 0, 0);
                middleControlRect = new Rect(-120, 0, 240, 120);
                rightControlPos = new Vector3(1, 0, 0);
                rightControlRect = new Rect(-240, 0, 240, 120);
                this.stereoParamsRect = new Rect(-50, 200, 100, 100);
                this.interaxialRect = new Rect(100, 130, 240, 60);
                this.zeroPrlxRect = new Rect(-120, 130, 240, 60);
                this.hitRect = new Rect(-340, 130, 240, 60);
                this.loadNewSceneRect = new Rect(-50, 320, 100, 100);
                this.fpsTool01Rect = new Rect(0, 120, 100, 100);
                this.fpsTool02Rect = new Rect(-100, 120, 100, 100);
                break;
            case phoneType.Rezound_LandLeft:
                this.camera3D.leftViewRect = new Vector4(0f, 0.27f, 0.49f, 0.73f);
                this.camera3D.rightViewRect = new Vector4(0.51f, 0.27f, 0.49f, 0.73f);
                this.setCameraRect(this.camera3D.leftViewRect, this.camera3D.rightViewRect);
                this.camera3D.H_I_T = 2;
                this.cursorSize = 64;
                leftControlPos = new Vector3(0, 0, 0);
                leftControlRect = new Rect(0, 0, 400, 200);
                middleControlPos = new Vector3(0.5f, 0, 0);
                middleControlRect = new Rect(-200, 0, 400, 200);
                rightControlPos = new Vector3(1, 0, 0);
                rightControlRect = new Rect(-400, 0, 400, 200);
                this.stereoParamsRect = new Rect(-64, 280, 128, 128);
                this.interaxialRect = new Rect(150, 200, 300, 80);
                this.zeroPrlxRect = new Rect(-150, 200, 300, 80);
                this.hitRect = new Rect(-450, 200, 300, 80);
                this.loadNewSceneRect = new Rect(-64, 440, 128, 128);
                this.fpsTool01Rect = new Rect(0, 200, 128, 128);
                this.fpsTool02Rect = new Rect(-128, 200, 128, 128);
                break;
            case phoneType.Thrill_LandLeft:
                this.camera3D.leftViewRect = new Vector4(0f, 0.19f, 0.49f, 0.81f);
                this.camera3D.rightViewRect = new Vector4(0.51f, 0.19f, 0.49f, 0.81f);
                this.setCameraRect(this.camera3D.leftViewRect, this.camera3D.rightViewRect);
                this.camera3D.H_I_T = 2;
                this.cursorSize = 40;
                leftControlPos = new Vector3(0, 0, 0);
                leftControlRect = new Rect(0, 0, 256, 128);
                middleControlPos = new Vector3(0.5f, 0, 0);
                middleControlRect = new Rect(-128, 0, 256, 128);
                rightControlPos = new Vector3(1, 0, 0);
                rightControlRect = new Rect(-256, 0, 256, 128);
                this.stereoParamsRect = new Rect(-32, 200, 64, 64);
                this.interaxialRect = new Rect(100, 128, 200, 50);
                this.zeroPrlxRect = new Rect(-100, 128, 200, 50);
                this.hitRect = new Rect(-300, 128, 200, 50);
                this.loadNewSceneRect = new Rect(-32, 300, 64, 64);
                this.fpsTool01Rect = new Rect(0, 128, 64, 64);
                this.fpsTool02Rect = new Rect(-64, 128, 64, 64);
                break;
            case phoneType.my3D_LandLeft:
                this.camera3D.leftViewRect = new Vector4(0f, 0.075f, 0.4675f, 0.925f);
                this.camera3D.rightViewRect = new Vector4(0.529f, 0.075f, 0.4675f, 0.925f);
                this.setCameraRect(this.camera3D.leftViewRect, this.camera3D.rightViewRect);
                this.camera3D.H_I_T = 3;
                this.cursorSize = 64;
                leftControlPos = new Vector3(0, 0, 0);
                leftControlRect = new Rect(0, 0, 200, 100);
                middleControlPos = new Vector3(0.5f, 0, 0);
                middleControlRect = new Rect(-80, 100, 160, 80);
                rightControlPos = new Vector3(1, 0, 0);
                rightControlRect = new Rect(-200, 0, 200, 100);
                this.stereoParamsRect = new Rect(-40, 400, 80, 80);
                this.interaxialRect = new Rect(120, 100, 240, 60);
                this.zeroPrlxRect = new Rect(-120, 100, 240, 60);
                this.hitRect = new Rect(-360, 100, 240, 60);
                this.loadNewSceneRect = new Rect(-40, 500, 80, 80);
                this.fpsTool01Rect = new Rect(440, 200, 80, 80);
                this.fpsTool02Rect = new Rect(-520, 300, 80, 80);
                break;
        }
        if (this.moveTouchpad)
        {
            this.moveTouchpad.enabled = true;
            if (this.movePadPosition == controlPos.left)
            {
                this.moveTouchpad.transform.position = leftControlPos;
                ((GUITexture) this.moveTouchpad.GetComponent(typeof(GUITexture))).pixelInset = leftControlRect;
            }
            else
            {
                if (this.movePadPosition == controlPos.center)
                {
                    this.moveTouchpad.transform.position = middleControlPos;
                    ((GUITexture) this.moveTouchpad.GetComponent(typeof(GUITexture))).pixelInset = middleControlRect;
                }
                else
                {
                    if (this.movePadPosition == controlPos.right)
                    {
                        this.moveTouchpad.transform.position = rightControlPos;
                        ((GUITexture) this.moveTouchpad.GetComponent(typeof(GUITexture))).pixelInset = rightControlRect;
                    }
                    else
                    {
                        if (this.movePadPosition == controlPos.off)
                        {
                        }
                    }
                }
            }
            this.moveTouchpad.setUp();
        }
        if (this.turnTouchpad)
        {
            this.turnTouchpad.enabled = true;
            if (this.turnPadPosition == controlPos.left)
            {
                this.turnTouchpad.transform.position = leftControlPos;
                ((GUITexture) this.turnTouchpad.GetComponent(typeof(GUITexture))).pixelInset = leftControlRect;
            }
            else
            {
                if (this.turnPadPosition == controlPos.center)
                {
                    this.turnTouchpad.transform.position = middleControlPos;
                    ((GUITexture) this.turnTouchpad.GetComponent(typeof(GUITexture))).pixelInset = middleControlRect;
                }
                else
                {
                    if (this.turnPadPosition == controlPos.right)
                    {
                        this.turnTouchpad.transform.position = rightControlPos;
                        ((GUITexture) this.turnTouchpad.GetComponent(typeof(GUITexture))).pixelInset = rightControlRect;
                    }
                    else
                    {
                        if (this.turnPadPosition == controlPos.off)
                        {
                        }
                    }
                }
            }
            this.turnTouchpad.setUp();
        }
        if (this.pointTouchpad)
        {
            this.pointTouchpad.enabled = true;
            if (this.pointPadPosition == controlPos.left)
            {
                this.pointTouchpad.transform.position = leftControlPos;
                ((GUITexture) this.pointTouchpad.GetComponent(typeof(GUITexture))).pixelInset = leftControlRect;
            }
            else
            {
                if (this.pointPadPosition == controlPos.center)
                {
                    this.pointTouchpad.transform.position = middleControlPos;
                    ((GUITexture) this.pointTouchpad.GetComponent(typeof(GUITexture))).pixelInset = middleControlRect;
                }
                else
                {
                    if (this.pointPadPosition == controlPos.right)
                    {
                        this.pointTouchpad.transform.position = rightControlPos;
                        ((GUITexture) this.pointTouchpad.GetComponent(typeof(GUITexture))).pixelInset = rightControlRect;
                    }
                    else
                    {
                        if (this.pointPadPosition == controlPos.off)
                        {
                        }
                    }
                }
            }
            this.pointTouchpad.setUp();
        }
        if (this.stereoParamsTouchpad)
        {
            if (this.useStereoParamsTouchpad)
            {
                this.stereoParamsTouchpad.transform.position = new Vector3(0.5f, 0, 0);
                ((GUITexture) this.stereoParamsTouchpad.GetComponent(typeof(GUITexture))).pixelInset = this.stereoParamsRect;
                this.interaxialTouchpad.transform.position = new Vector3(0, 0, 0);
                ((GUITexture) this.interaxialTouchpad.GetComponent(typeof(GUITexture))).pixelInset = this.interaxialRect;
                this.zeroPrlxTouchpad.transform.position = new Vector3(0.5f, 0, 0);
                ((GUITexture) this.zeroPrlxTouchpad.GetComponent(typeof(GUITexture))).pixelInset = this.zeroPrlxRect;
                this.hitTouchpad.transform.position = new Vector3(1, 0, 0);
                ((GUITexture) this.hitTouchpad.GetComponent(typeof(GUITexture))).pixelInset = this.hitRect;
                this.stereoParamsTouchpad.setUp();
                this.interaxialTouchpad.setUp();
                this.zeroPrlxTouchpad.setUp();
                this.hitTouchpad.setUp();
            }
            else
            {
            }
        }
        if (this.loadNewSceneTouchpad)
        {
            if (this.showLoadNewScenePad)
            {
                this.loadNewSceneTouchpad.transform.position = new Vector3(0.5f, 0, 0);
                ((GUITexture) this.loadNewSceneTouchpad.GetComponent(typeof(GUITexture))).pixelInset = this.loadNewSceneRect;
                this.loadNewSceneTouchpad.setUp();
            }
            else
            {
            }
        }
        if (this.fpsTool01)
        {
            if (this.showFpsTool01)
            {
                this.fpsTool01.transform.position = new Vector3(0, 0, 0);
                ((GUITexture) this.fpsTool01.GetComponent(typeof(GUITexture))).pixelInset = this.fpsTool01Rect;
                this.fpsTool01.setUp();
            }
            else
            {
            }
        }
        if (this.fpsTool02)
        {
            if (this.showFpsTool02)
            {
                this.fpsTool02.transform.position = new Vector3(1, 0, 0);
                ((GUITexture) this.fpsTool02.GetComponent(typeof(GUITexture))).pixelInset = this.fpsTool02Rect;
                this.fpsTool02.setUp();
            }
            else
            {
            }
        }
        if (this.cursor3D)
        {
            s3dGuiTexture texture3D = null;
            texture3D = (s3dGuiTexture) this.cursor3D.GetComponent(typeof(s3dGuiTexture));
            if (this.use3dCursor)
            {
                texture3D.toggleVisible(true);
                this.cursor3D.transform.position = new Vector3(0.5f, 0.5f, 0);
                ((GUITexture) this.cursor3D.GetComponent(typeof(GUITexture))).pixelInset = new Rect(this.cursorSize / -2, this.cursorSize / -2, this.cursorSize, this.cursorSize);
            }
            else
            {
                texture3D.toggleVisible(false);
            }
        }
    }

    public virtual void setCameraRect(Vector4 l, Vector4 r)
    {
        // don't mess with screen layout if stereoShader is being used for anaglyph, etc
        if (!this.camera3D.useStereoShader)
        {
            this.camera3D.leftCam.GetComponent<Camera>().rect = new Rect(l.x, l.y, l.z, l.w);
            this.camera3D.rightCam.GetComponent<Camera>().rect = new Rect(r.x, r.y, r.z, r.w);
            this.camera3D.leftViewRect = this.camera3D.RectToVector4(this.camera3D.leftCam.GetComponent<Camera>().rect);
            this.camera3D.rightViewRect = this.camera3D.RectToVector4(this.camera3D.rightCam.GetComponent<Camera>().rect);
        }
    }

    public virtual void Update()
    {
        this.checkForChanges();
    }

    public virtual void checkForChanges()
    {
        // only run in edit mode
        if (this.prevPhoneLayout != this.phoneLayout)
        {
            this.setPhoneLayout();
        }
        this.prevPhoneLayout = this.phoneLayout;
    }

    // called from Editor script
    public virtual void forceUpdate()
    {
        this.setPhoneLayout();
        this.prevPhoneLayout = this.phoneLayout;
        this.camera3D.initStereoCamera();
    }

    public s3dDeviceManager()
    {
        this.phoneLayout = phoneType.iPhone4_LandLeft;
        this.use3dCursor = true;
        this.movePadPosition = controlPos.left;
        this.turnPadPosition = controlPos.center;
        this.pointPadPosition = controlPos.right;
    }

}