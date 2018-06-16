using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour {

	public Color c1 = Color.yellow;
	public Color c2 = Color.red;
	int lengthOfLineRenderer = 2; //50;
	LineRenderer lineRenderer;

	public float segY;
	float[] segYCoord;

	// Use this for initialization
	void Start () {
		segYCoord = new float[lengthOfLineRenderer];
		for (int i = 0; i < lengthOfLineRenderer; i++){
			segYCoord[i] = 0;
		}

		lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
		lineRenderer.widthMultiplier = 0.1f;
		lineRenderer.positionCount = lengthOfLineRenderer;

		// A simple 2 color gradient with a fixed alpha of 1.0f.
		float alpha = 1.0f;
		Gradient gradient = new Gradient();
		gradient.SetKeys(
			new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
			new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
		);
		lineRenderer.colorGradient = gradient;

		lineRenderer.SetPosition(0, new Vector3(-5f, 0f, -5f));
		lineRenderer.SetPosition(1, new Vector3(5f, 0f, -5f));
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
