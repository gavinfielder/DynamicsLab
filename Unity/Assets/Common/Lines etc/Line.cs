using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicsLab.CreateLines;

public class Line : MonoBehaviour {

	public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    int lengthOfLineRenderer = 250; //50;
    int numLines = 10;
	LineRenderer lineRenderer;
    GameObject CL;

    // 0 is not pause
    int paused = 0;

	public float segY;
	float[] segYCoord;

    public void Start()
    {
    	//SegYCoord is an array of variables that specify that
    	//height of all the segments in the line. Remember, there are
    	//20 or so of these line objects running this same line script.
    	segYCoord = new float[lengthOfLineRenderer];
		for (int i = 0; i < lengthOfLineRenderer; i++){
			segYCoord[i] = 0;
		}

		//Using unity magic to create a line component to this otherwise
		//blank GameObject
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

        CL = GameObject.Find("LineMaster");
    }

	public int count = 0;

    public void Update()
    {
       
        paused = CL.GetComponent<CreateLines>().pause;
        //paused = 1;
        if (paused == 0)
        {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            var t = Time.time;

            //The SegYCoord array updates over time based on SegY, 
            //which is updated in the CreateLines.cs script.
            //SegY is public so it can be accessed by other scripts.
            if (true)
            {
                segYCoord[count] = segY;
                segY = 0;
                count += 1;
                if (count >= lengthOfLineRenderer)
                {
                    count = 0;
                }
            }
            segYCoord[0] = segYCoord[1];

            //Loops through the number of segments in the line being 
            //created by this script.
            //SegYCoord[i] is the height of the line at segment i
            for (int i = 0; i < lengthOfLineRenderer; i++)
            {
                lineRenderer.SetPosition(i, new Vector3(
                    i * 0.1f - lengthOfLineRenderer * 0.05f,
                    segYCoord[i] * this.transform.position.z / 5.0f,
                    this.transform.position.z));
            }
        }
	    
    }
}
