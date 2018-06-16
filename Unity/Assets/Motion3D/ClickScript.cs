using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void onMouseOver(){
		Debug.Log("hi");
		this.gameObject.GetComponent<MeshRenderer>().material.color = 
	 		new Color(1.0f, 1.0f, 0.0f, 1.0f);
	}
}
