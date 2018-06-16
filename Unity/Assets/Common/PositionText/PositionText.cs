using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionText : MonoBehaviour {
	public Camera UserCam;
	// Use this for initialization
	void Awake () {
		UserCam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
	}
	// Update is called once per frame
	void Update () {
		transform.LookAt (transform.position + UserCam.transform.rotation * Vector3.forward, UserCam.transform.rotation * Vector3.up);
	}
}
