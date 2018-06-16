using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveText : MonoBehaviour {
	TextMesh text;
	public Transform MassLocation;
	// Use this for initialization
	void Start () {
		text = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
		text.text = MassLocation.position.ToString();
		transform.position = new Vector3 (MassLocation.position.x, MassLocation.position.y + 1, MassLocation.position.z);
	}
}
