using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DynamicsLab.Solvers;

namespace DynamicsLab.CreateLines {

public class CreateLines : MonoBehaviour {

    GameObject mainMenu;

	public Color c1 = Color.blue;
    public Color c2 = Color.blue;
	LineRenderer lineRenderer;

	public GameObject Line;
	int numLines = 25;

	GameObject spring;
	GameObject mass;
	GameObject plane;
	public GameObject[] instance; 

	public float mx = -2f; //mass X-coord
	public float my = 3.5f;
	public float mz = -.5f;

	float initialX = -2f;
	public float velX = 0.0f;

    public float friction;
    public float masss;
    public float stiffness;

    public int started = 0;
    public int pause = 0;

    SpringMassSystem sms;

    //VR Mode control fields
    private GameObject playerSwitcher;
    private GameObject NVRPlayer;
    private GameObject GVRPlayer;
    private Camera NVRCamera;
    private Camera GVRCamera;
    private bool vRModeActive;

    public void Start () {

        InitializeCameras();

        mainMenu = GameObject.Find("MainMenuView");
        try{
        	mainMenu.SetActive(false);
        }catch (Exception e){
        	;
        }
        
	}

    public void InitializeSimulation()
    {
        if (friction == 0)
            friction = .15f;
        if (masss == 0)
            masss = 1.0f;
        if (stiffness == 0)
            stiffness = 1.0f;
        sms = new SpringMassSystem(0.01f, 1.0f, 1.0f, 0.15f, 0.0f, 40.0f); //range of 0-40s with data points every 0.5s
        sms.SetInitialCondition(0f, 1.0f, 0f);
        sms.Mass = masss; //mass
        sms.Damping = friction; //friction .15f
        sms.Stiffness = stiffness; //spring stiffness

        //sms.solve(1.0f, 0.0f); //solve with these initial conditions (position, velocity)

        //Instance is an array of GameObjects
        instance = new GameObject[numLines];
        for (int n = 0; n < numLines; n++)
        {
            //Looping though a specified number of lines that I want, 
            //I create several line objects (the lines are objects saved in the
            //assets in unity.
            //These lines are all saved in the instance array for easy access after creation.
            //Each line object has a line.cs script attached.
            instance[n] = Instantiate(Line, new Vector3(0, 0, n * .8f), transform.rotation);

        }
        //Finds an object in the scene by name.
        plane = GameObject.Find("Plane");
        //Changes the color of the floor plane to make it transparent
        plane.GetComponent<MeshRenderer>().sharedMaterial.color =
             new Color(1.0f, 1.0f, 1.0f, 0.5f);

        //Find the mass by name. The coordinates of the mass are
        //changed in THIS script by this reference.
        mass = GameObject.Find("Mass");
        mx = initialX;

        spring = GameObject.Find("Spring");
        spring.GetComponent<MeshRenderer>().sharedMaterial.color =
             new Color(1.0f, 1.0f, 1.0f, 0.5f);


        //line that travels with updating segments
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.sharedMaterial = new Material(Shader.Find("Particles/Additive"));
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
    
	public float time = 0f;
	float oldmx = 0f;

    public void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "Time: " + time.ToString());
        GUI.Label(new Rect(10, 30, 100, 20), "mx: " + mx.ToString());
        GUI.Label(new Rect(10, 50, 100, 20), "friction: " + friction.ToString());
    }

    public void Update () {

        //Toggle view if requested
        if (Input.GetKeyDown("h"))
        {
            ToggleVRView();
        }

        if (Input.GetKeyDown("space")) {
            if (started == 0)
            {
                started = 1;
                InitializeSimulation();
            }
            else
            {
                pause = 1- pause;
            }
            //sms.Mass = masss; //mass
            //sms.Damping = friction; //friction .15f
            //sms.Stiffness = stiffness; //spring stiffness
        }
        if (started == 1)
        {
            if (sms.Mass != masss)
                sms.Mass = masss; //mass
            if (sms.Damping != friction)
                sms.Damping = friction; //friction .15f
            if (sms.Stiffness != stiffness)
                sms.Stiffness = stiffness; //spring stiffness
        }
        if (started == 1 && pause == 0)
        {
            time += .1f;
            sms.Update(time);

            //mx += velX;
            velX = velX * .9f; // * ground friction

            //Uncomment for sin movement
            //mx = Mathf.Sin(time);
            mx = System.Convert.ToSingle(sms.Position(time));
            //Debug.Log(mx.ToString());
            mass.transform.position = new Vector3(mx, my, mz);

            spring.transform.position = new Vector3(mx / 2f - 2.5f, my, mz);
            spring.transform.localScale = new Vector3(.05f, .05f, .05f - .035f * (mx - initialX + 2.5f));


            float accel = mx - oldmx;
            oldmx = mx;

            for (int n = 0; n < numLines; n++)
            {
                //yCoord = Acceleration
                //Updates the segY public variable in each Line GameObject
                //Look at the line.cs script to see the segY variable
                //and what it does.
                instance[n].GetComponent<Line>().segY = accel * 20f;
            }

            if (Input.GetKeyDown("space"))
            {
                velX += .1f;
            }

            //Draw line at edge of updating segments
            int count = instance[0].GetComponent<Line>().count;


            //This is the blue line that visualizes the updating graph.
            for (int i = 0; i < numLines; i++)
            {
                lineRenderer.SetPosition(i, new Vector3(
                    count * .1f - (float)numLines * .5f,
                    accel * 20f,
                    numLines * 0.5f * i));
            }

        }

	}

	public void TestVR(){
		ToggleVRView();
	}

    //Toggles whether VR view is active
    private void ToggleVRView()
    {
        if (vRModeActive)
        {
            VROff();
        }
        else
        {
            VROn();
        }
    }
    private void VROn()
    {
        NVRPlayer.SetActive(true);
        NVRCamera.enabled = true;
        GVRCamera.enabled = false;
        GVRPlayer.SetActive(false);
        vRModeActive = true;
    }
    private void VROff()
    {
        GVRPlayer.SetActive(true);
        GVRCamera.enabled = true;
        NVRCamera.enabled = false;
        NVRPlayer.SetActive(false);
        vRModeActive = false;
    }

    //Hooks player and camera references
    private void InitializeCameras()
    {
        playerSwitcher = GameObject.Find("PlayerSwitcher");
        //Get all children, active or inactive
        NVRPlayer = playerSwitcher.GetComponentInChildren<NewtonVR.NVRPlayer>(true).gameObject;
        GVRPlayer = GameObject.Find("GVR Player");
            /*
        foreach (GameObject obj in objs)
        {
            if (obj.name == "NVRPlayer")
            {
                NVRPlayer = obj;
            }
            if (obj.name == "GVR Player")
            {
                GVRPlayer = obj;
            }
        }*/
        NVRCamera = NVRPlayer.GetComponentInChildren<Camera>();
        GVRCamera = GVRPlayer.GetComponentInChildren<Camera>();

        //Initial state is VR disabled
        VROff();

    }



    //GUI handlers
    public void MenuButton_OnPress()
    {
        mainMenu.SetActive(true);
    }


}

}