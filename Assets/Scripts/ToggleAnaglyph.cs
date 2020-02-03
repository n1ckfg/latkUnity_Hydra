using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleAnaglyph : MonoBehaviour {

	public s3dCamera anaglyph3d;

	void Awake() {
		if (anaglyph3d == null) anaglyph3d = GetComponent<s3dCamera>();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Alpha3)) {
			if (!anaglyph3d.enabled) { 
				anaglyph3d.enabled = true; 
			} else {
				anaglyph3d.enabled = false; 
			}
		}
	}

}
