using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLines : MonoBehaviour {

	public Color c1 = Color.blue;
    public Color c2 = Color.blue;
	LineRenderer lineRenderer;

	public GameObject Line;
	int numLines = 25;

	GameObject mass;
	GameObject plane;
	GameObject[] instance;

	float mx = -2f; //mass X-coord
	float my = 3.5f;
	float mz = -.5f;

	float initialX = -2f;
	float velX = 0.0f;


	// Use this for initialization
	void Start () {
		instance = new GameObject[numLines];
		for (int n = 0; n < numLines; n++){
			instance[n] = Instantiate(Line, new Vector3(0,0,n*.8f), transform.rotation);

		}
		plane =  GameObject.Find("Plane");
		plane.GetComponent<MeshRenderer>().material.color = 
	 		new Color(1.0f, 1.0f, 1.0f, 0.5f);

	 	mass = GameObject.Find("Mass");
	 	mx = initialX;


	 	//line that travels with updating segments
		lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.positionCount = numLines;

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        lineRenderer.colorGradient = gradient;

	}

	float time = 0f;
	float oldmx = 0f;

	void Update () {
		time += .1f;

		mx += velX;
		velX = velX * .9f; // * ground friction
		mass.transform.position = new Vector3(mx,my,mz);

		mx = Mathf.Sin(time);

		float accel = mx - oldmx;
		oldmx = mx;

		for (int n = 0; n < numLines; n++){
			//yCoord = Acceleration
			instance[n].GetComponent<Line>().segY = accel*20f;
		}

		if (Input.GetKeyDown("space")){
			velX += .1f;
		}

		//Draw line at edge of updating segments
		int count = instance[0].GetComponent<Line>().count;

		for (int i = 0; i < numLines; i++){
			lineRenderer.SetPosition(i, new Vector3(
				count*.1f-(float)numLines*.5f, 
				accel*20f,
            	numLines*0.5f * i));
        }

	}

}
