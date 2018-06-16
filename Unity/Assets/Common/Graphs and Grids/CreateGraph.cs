using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGraph : MonoBehaviour {
	public GameObject segment;
	public int distx = 10;
	public int disty = 10;
	public int distz = 10;
	public float lineWidth = 0.005f;
	int numLines; 
	GameObject[] instance;
	public float offsetX = 5;
	public float offsetY = 0;
	public float offsetZ = 0;
	// Use this for initialization
	void Start () {
		numLines= distx * disty + distx * distz + disty * distz;
		instance = new GameObject[numLines];
		for (int y = 0; y < disty; y++) {
			for (int x = 0; x < distx; x++) {
				instance [(y * distx) + x] = Instantiate (segment);
				instance [(y * distx) + x].GetComponent<LineRenderer> ().material = new Material (Shader.Find ("Particles/Additive"));
				instance [(y * distx) + x].GetComponent<LineRenderer> ().widthMultiplier =lineWidth;
				instance [(y * distx) + x].GetComponent<LineRenderer> ().SetPosition (0,new Vector3(x-offsetX , y-offsetY, 0-offsetZ));
				instance [(y * distx) + x].GetComponent<LineRenderer> ().SetPosition (1, new Vector3(x-offsetX, y-offsetY, distz-1-offsetZ));
			}
		}
		for (int x = 0; x < distx; x++) {
			for (int z = 0; z < distz; z++) {
				instance [(distx * disty) + (z * distx) + x] = Instantiate (segment);
				instance [(distx * disty) + (z * distx) + x].GetComponent<LineRenderer> ().material = new Material (Shader.Find ("Particles/Additive"));
				instance [(distx * disty) + (z * distx) + x].GetComponent<LineRenderer> ().widthMultiplier = lineWidth;
				instance [(distx * disty) + (z * distx) + x].GetComponent<LineRenderer> ().SetPosition (0,new Vector3(x-offsetX, 0-offsetY, z-offsetZ));
				instance [(distx * disty) + (z * distx) + x].GetComponent<LineRenderer> ().SetPosition (1, new Vector3(x-offsetX, disty-1-offsetY, z-offsetZ));
			}
		}
		for (int y = 0; y < disty; y++) {
			for (int z = 0; z < distz; z++) {
				instance [(distx * disty + distx * distz) + (z * disty) + y] = Instantiate (segment);
				instance [(distx * disty + distx * distz) + (z * disty) + y].GetComponent<LineRenderer> ().material = new Material (Shader.Find ("Particles/Additive"));
				instance [(distx * disty + distx * distz) + (z * disty) + y].GetComponent<LineRenderer> ().widthMultiplier = lineWidth;
				instance [(distx * disty + distx * distz) + (z * disty) + y].GetComponent<LineRenderer> ().SetPosition (0, new Vector3 (0-offsetX, y-offsetY, z-offsetZ));
				instance [(distx * disty + distx * distz) + (z * disty) + y].GetComponent<LineRenderer> ().SetPosition (1, new Vector3(distx-1-offsetX, y-offsetY, z-offsetZ));
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
