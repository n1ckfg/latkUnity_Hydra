using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simple3DCam : MonoBehaviour {

	public Camera cameraL;
	public Camera cameraR;
	public bool alwaysUpdate = false;
	public enum StereoConfig { OU_LR, OU_RL, SBS_LR, SBS_RL, Anaglyph };
	public StereoConfig stereoConfig = StereoConfig.OU_LR;
	public float ioDistance = 0.065f;
	public float fov = 60f;
	public float nearClip = 0.01f;
	public float farClip = 1000f;

	private AudioListener alL;
	private AudioListener alR;
	private AudioListener alCenter;
	private s3dCamera fxAnaglyph;

	void Awake() {
		fxAnaglyph = cameraL.GetComponent<s3dCamera>();
		alL = cameraL.GetComponent<AudioListener>();
		alR = cameraR.GetComponent<AudioListener>();
		alCenter = GetComponent<AudioListener>();
		alL.enabled = false;
		alR.enabled = false;
		if (alCenter == null) {
			alCenter = gameObject.AddComponent<AudioListener> ();
		} else {
			alCenter.enabled = true;
		}
	}

	void Start() {
		configureCams();
	}
	
	void Update() {
		if (alwaysUpdate) configureCams();
	}

	void configureCams() {
		if (stereoConfig != StereoConfig.Anaglyph) {
			cameraR.gameObject.SetActive(true);
			cameraL.transform.position = new Vector3 (transform.position.x - (ioDistance / 2f), transform.position.y, transform.position.z);
			cameraR.transform.position = new Vector3 (transform.position.x + (ioDistance / 2f), transform.position.y, transform.position.z);
			fxAnaglyph.enabled = false;
		} else {
			cameraR.gameObject.SetActive(false);
			fxAnaglyph.enabled = true;
		}

		if (stereoConfig == StereoConfig.OU_LR) {
			camSettings (cameraL, 0f, 0f, 1f, 0.5f);
			camSettings (cameraR, 0f, 0.5f, 1f, 0.5f);
		} else if (stereoConfig == StereoConfig.OU_RL) {
			camSettings (cameraL, 0f, 0.5f, 1f, 0.5f);	
			camSettings (cameraR, 0f, 0f, 1f, 0.5f);
		} else if (stereoConfig == StereoConfig.SBS_LR) {
			camSettings (cameraL, 0f, 0f, 0.5f, 1f);
			camSettings (cameraR, 0.5f, 0f, 0.5f, 1f);			
		} else if (stereoConfig == StereoConfig.SBS_RL) {
			camSettings (cameraL, 0.5f, 0f, 0.5f, 1f);			
			camSettings (cameraR, 0f, 0f, 0.5f, 1f);
		} else if (stereoConfig == StereoConfig.Anaglyph) {
			camSettings (cameraL, 0f, 0f, 1f, 1f);
		}
	}

	void camSettings(Camera cam, float x, float y, float w, float h) {
		cam.fieldOfView = fov;
		cam.farClipPlane = farClip;
		cam.nearClipPlane = nearClip;
		cam.rect = new Rect(x, y, w, h);
	}
}
