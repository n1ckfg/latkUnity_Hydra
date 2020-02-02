using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetHydra : MonoBehaviour {

	public Vector3 globalScale = Vector3.one;
	public Vector3 globalOffset = Vector3.zero;
	public LightningArtist la;

	void Awake() {
		if (la == null)	la = GetComponent<LightningArtist>();
		la.transform.localScale = globalScale;
		la.transform.position = globalOffset;
	}

}
