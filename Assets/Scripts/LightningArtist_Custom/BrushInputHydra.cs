using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushInputHydra : MonoBehaviour {

	public SixenseCtl sixCtlMain;
	public SixenseCtl sixCtlAlt;
	public SixenseCam sixCam;
	public LightningArtist lightningArtist;
	//public s3dCamera anaglyph3d;
	public float alignTime = 5f;

	private float markTime = 0f;

	void Awake() {
		if (lightningArtist == null) lightningArtist = GetComponent<LightningArtist>();
		lightningArtist.enabled = false;
	}

	void Start() {
		markTime = Time.realtimeSinceStartup;
	}
	
	void Update () {
		if (!sixCtlMain.firstRun && !sixCtlAlt.firstRun) { // both controllers are calibrated
			if (Time.realtimeSinceStartup < markTime + alignTime || lightningArtist.isReadingFile) {
				sixCam.doAlign = true;
			} else {
				lightningArtist.enabled = true;
				sixCam.doAlign = false;
			}
		}
			
		// 1. draw
		if ((!sixCtlMain.firstRun && sixCtlMain.triggerPressed)) {// || Input.GetKeyDown(KeyCode.Space)) {
			lightningArtist.clicked = true;
		} else {
			lightningArtist.clicked = false;
		}

		if (sixCtlMain.triggerDown) {
			sixCtlMain.hideRen();
		} else if (sixCtlMain.triggerUp) {
			sixCtlMain.showRen();
		}

		if (sixCtlAlt.triggerDown || sixCtlMain.button3Down) {
			sixCtlAlt.hideRen();
		} else if (sixCtlAlt.triggerUp || sixCtlMain.button3Up) { 
			sixCtlAlt.showRen();	
		}

		if (!sixCtlMain.triggerPressed) {
			if (sixCtlMain.bumperPressed) lightningArtist.inputErase();

			if (sixCtlMain.button1Down) sixCam.verticalRotation = !sixCam.verticalRotation; //lightningArtist.inputPush();

			if (sixCtlMain.button3Down) sixCam.disableOffset();
			if (sixCtlMain.button3Pressed) lightningArtist.inputColorPick();
			if (sixCtlMain.button3Up) sixCam.enableOffset();
		}

		if (!sixCam.verticalRotation) lightningArtist.target.transform.Translate(new Vector3( 0f, 0f, -sixCtlMain.joystickVal.y * sixCam.joystickScale));

		if (sixCtlAlt.bumperDown) lightningArtist.inputNextLayer();
		if (sixCtlAlt.bumperPressed && sixCtlMain.button4Down) lightningArtist.inputNewLayer();

		// 2. new frame
		if (sixCtlMain.button4Down) {// || Input.GetKeyDown(KeyCode.F)) {
			lightningArtist.inputNewFrame();
			Debug.Log("Ctl: New Frame");
			} else if (sixCtlMain.button2Down) {// || Input.GetKeyDown(KeyCode.G)) {
			lightningArtist.inputNewFrameAndCopy();
			Debug.Log("Ctl: New Frame Copy");
		}

		//if ((!steamControllerMain.padPressed && blockMainPadButton) || Input.GetKeyUp(KeyCode.F)) {
		//blockMainPadButton = false;
		//}

		// 3. play
		if (sixCtlAlt.button3Down) {// || Input.GetKeyDown(KeyCode.P)) {
			lightningArtist.inputPlay();
			Debug.Log("Ctl: Play");
		}

		//if ((!steamControllerAlt.padPressed && blockAltPadButton) || Input.GetKeyUp(KeyCode.P)) {
		//blockAltPadButton = false;
		//}	

		// ~ ~ ~ ~ ~ ~ ~ ~ ~

		// 4. frame back
		if (sixCtlAlt.button1Down) {// || Input.GetKeyDown(KeyCode.LeftArrow)) {
			lightningArtist.inputFrameBack();
		}

		//if ((!steamControllerAlt.gripped && blockAltGripButton) || Input.GetKeyUp(KeyCode.LeftArrow)) {
		//blockAltGripButton = false;
		//}

		// 5. frame forward
		if (sixCtlAlt.button2Down) {// || Input.GetKeyDown(KeyCode.RightArrow)) {
			lightningArtist.inputFrameForward();
		}

		//if ((!steamControllerMain.gripped && blockMainGripButton) || Input.GetKeyUp(KeyCode.RightArrow)) {
		//blockMainGripButton = false;
		//}

		// 6. show / hide all frames
		if (sixCtlAlt.button4Down) {// || Input.GetKeyDown(KeyCode.UpArrow)) {
			//lightningArtist.inputShowFrames();
			//} else if (steamControllerAlt.menuUp || Input.GetKeyDown(KeyCode.DownArrow)) {
			//lightningArtist.inputHideFrames();

			lightningArtist.showOnionSkin = !lightningArtist.showOnionSkin;
			if (lightningArtist.showOnionSkin) {
				lightningArtist.inputShowFrames();
			} else {
				lightningArtist.inputHideFrames();
			}
		}

		/*
		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			if (!anaglyph3d.enabled) {
				anaglyph3d.enabled = true;
			} else {
				anaglyph3d.enabled = false;
			}
		}

		// ~ ~ ~ ~ ~ ~ ~ ~ ~

		if (Input.GetKeyDown(KeyCode.Z)) { // reset all
			lightningArtist.resetAll(); 
		}

		if (Input.GetKeyDown(KeyCode.X)) { // reset
			lightningArtist.layerList[lightningArtist.currentLayer].frameList[lightningArtist.layerList[lightningArtist.currentLayer].currentFrame].reset(); 
		}

		if (Input.GetKeyDown(KeyCode.T)) { // random
			//resetAll();
			lightningArtist.testRandomStrokes();
		}

		if (Input.GetKeyDown(KeyCode.O)) { // scale
			lightningArtist.applyScaleAndOffset();
		}

		if (Input.GetKeyDown(KeyCode.R) && !lightningArtist.isReadingFile) {
			lightningArtist.armReadFile = true;
		}

		if (Input.GetKeyDown(KeyCode.S) && !lightningArtist.isWritingFile) {
			lightningArtist.armWriteFile = true;
		}
		*/

	}

}
