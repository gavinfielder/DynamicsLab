using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {

	// Use this for initialization

	public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    int lengthOfLineRenderer = 250; //50;
    int numLines = 10;
	LineRenderer lineRenderer;

	public float segY;
	float[] segYCoord;

    void Start()
    {
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

    }

	public int count = 0;

    void Update()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        var t = Time.time;

        //if (segY != 0f){
        if (true){
       		segYCoord[count] = segY;
       		segY = 0;
       		count += 1;
       		if (count >= lengthOfLineRenderer){
       			count = 0;
       		}
       	}
       	segYCoord[0] = segYCoord[1];


        for (int i = 0; i < lengthOfLineRenderer; i++)
        {
			lineRenderer.SetPosition(i, new Vector3(
				i*0.1f - lengthOfLineRenderer*0.05f, 
				//Mathf.Sin((i - t*10)/2) * this.transform.position.z/5.0f,
				segYCoord[i] * this.transform.position.z/5.0f,
            	this.transform.position.z));
        }
	    
    }
}
