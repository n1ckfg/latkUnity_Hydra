using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMeshLine : MonoBehaviour {

	public MeshLineRenderer m;

	void Awake() {
		if (m == null) m = GetComponent<MeshLineRenderer> ();
	}

	void Start () {
		m.AddPoint (new Vector3 (0f, 0f, 0f)); 
		m.AddPoint (new Vector3 (1f, 0f, 0f)); 
		m.AddPoint (new Vector3 (2f, 1f, 0f)); 
		m.AddPoint (new Vector3 (3f, 0f, 0f)); 
		m.AddPoint (new Vector3 (4f, 0f, 0f)); 
		m.AddPoint (new Vector3 (5f, 2f, 0f)); 
		m.AddPoint (new Vector3 (6f, 0f, 0f)); 
		m.AddPoint (new Vector3 (7f, 0f, 0f)); 
		m.AddPoint (new Vector3 (8f, 0f, 0f)); 
	}

}
