﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DynamicsLab.Solvers;
using System;
//using Motion;

public class Motion3DSceneController : MonoBehaviour {

    //GUI menus
    private GameObject configView;
    private GameObject setState1stOrderView;
    private GameObject setState2ndOrderView;
    private GameObject messageBox;

    //Other GUI elements
    private Text startStopButtonText;
    private InputField timeDisplayText;

    //Simulation control
    private List<MotionWrapper> simulatedObjects;
    private Motion3DSetup motion3Dsetup;
    private bool simulationIsRunning;
    private float simTime;
    private float forwardSolveTime = 30;
    public float speed = 1f;


    // Use this for initialization
    void Start()
    {
        //Hook GUI menus
        configView = GameObject.Find("Motion3DConfigView");
        setState1stOrderView = GameObject.Find("SetState1stOrderView");
        setState2ndOrderView = GameObject.Find("SetState2ndOrderView");
        messageBox = GameObject.Find("MessageBox");
        //Hook other GUI elements
        startStopButtonText = GameObject.Find("StartButton").GetComponentInChildren<Text>();
        timeDisplayText = GameObject.Find("TimeInput").GetComponent<InputField>();
        //Initialize problem setup
        motion3Dsetup = new Motion3DSetup("5.4*cos(t)", "1.2*sin(t/2.2)", "cos(t/3.73)", new List<string>(), 1);
        //Initialize GUI menus
        configView.GetComponent<Motion3DConfigGui>().PopulateFields(motion3Dsetup);
        setState1stOrderView.transform.Find("timeInput").gameObject.GetComponent<InputField>().text = "0.0";
        setState1stOrderView.transform.Find("xInput").gameObject.GetComponent<InputField>().text = "0.0";
        setState1stOrderView.transform.Find("yInput").gameObject.GetComponent<InputField>().text = "0.0";
        setState1stOrderView.transform.Find("zInput").gameObject.GetComponent<InputField>().text = "0.0";
        //Close GUI menus
        configView.SetActive(false);
        setState1stOrderView.SetActive(false);
        setState2ndOrderView.SetActive(false);
        messageBox.SetActive(false);
        //Initialize object list
        simulatedObjects = new List<MotionWrapper>();

        //Add the default/first simulated object to the list
        simulatedObjects.Add(GameObject.Find("MotionMaster").GetComponent<MotionMaster>().mw);

        //Setup default problem
        ApplyConfiguration(motion3Dsetup);
        simulationIsRunning = false;

        //Instancing for GameObjects
		instance = new List<GameObject>();
		instance.Add(this.gameObject);
    }


    // Update is called once per frame
    void Update() {
        //User shortcut keys
        if (Input.GetKeyDown("space"))
        {
            StartStopToggle();
        }
		if (Input.GetKeyDown("f")) //fast
        {
            speed = speed * 2f;
        }
		if (Input.GetKeyDown("s")) //slow
        {
            speed = speed * .5f;
        }

		if (Input.GetKeyDown("w")) //next obj
        {
        	if (instance.Count > currentObj){
            	currentObj ++;
            	UpdateCurrentObject();
            }
        }
		if (Input.GetKeyDown("q")) //select prev obj
        {
        	if (currentObj > 0){
            	currentObj --;
            	UpdateCurrentObject();
            }
        }

        UpdateCurrentObject_Text();

        //Update simulation
        if (simulationIsRunning)
        {
            //Update Time
            simTime += Time.deltaTime * speed;
            timeDisplayText.text = simTime.ToString("###0.0#") + " (" + speed.ToString() + "x)";
            //Update simulated objects
            foreach (MotionWrapper mw in simulatedObjects)
            {
                mw.time = simTime; //Why is this causing an error? Object refence not set?
                mw.solveIVP(simTime + forwardSolveTime);
                //Debug.Log("Done solving. Interval is now: [" + mw.ivp.GetDataLowerBound() + ", " + mw.ivp.GetDataUpperBound() + "]");
            }
        }
    }

    //Gavin: As a user input event, this function needs to stay here
	public void BottomMenuBar_NumberObjectsEdited()
    {

		GameObject NumberObjInput = GameObject.Find("NumberObjInput");
        ChangeNumberObjects( Convert.ToInt32( NumberObjInput.GetComponent<InputField>().text ) );
    }

    public void MenuBar_StartButtonPress()
    {
        StartStopToggle();
    }
    public void MenuBar_ResetButtonPress()
    {
        StopSimulation();
        foreach (MotionWrapper mw in simulatedObjects)
        {
            mw.Reset();
            simTime = (float)mw.icTime; //TODO: when we move to multi-object motion, this should not be in the loop
            timeDisplayText.text = simTime.ToString("###0.0#");
        }
    }
    public void MenuBar_ConfigButtonPress()
    {
        //Show the configuration menu with the currently loaded problem
        configView.SetActive(true);
        configView.GetComponent<Motion3DConfigGui>().PopulateFields(motion3Dsetup);
    }
    public void MenuBar_SetStateButtonPress()
    {
        StopSimulation();
        //Show the set state menu with the current data
        if (motion3Dsetup.order == 1)
        {
            setState1stOrderView.SetActive(true);
            //Note this button will only exist for single-object environments
            MotionWrapper mw = simulatedObjects[0];
            setState1stOrderView.transform.Find("xInput").gameObject.GetComponent<InputField>().text = mw.X(mw.time).ToString("####0.0###");
            setState1stOrderView.transform.Find("yInput").gameObject.GetComponent<InputField>().text = mw.Y(mw.time).ToString("####0.0###");
            setState1stOrderView.transform.Find("zInput").gameObject.GetComponent<InputField>().text = mw.Z(mw.time).ToString("####0.0###");
            setState1stOrderView.transform.Find("timeInput").gameObject.GetComponent<InputField>().text = mw.time.ToString("####0.0###");
        }
        else
        {
            setState2ndOrderView.SetActive(true);
            //Note this button will only exist for single-object environments
            MotionWrapper mw = simulatedObjects[0];
            setState2ndOrderView.transform.Find("xInput").gameObject.GetComponent<InputField>().text = mw.X(mw.time).ToString("####0.0###");
            setState2ndOrderView.transform.Find("yInput").gameObject.GetComponent<InputField>().text = mw.Y(mw.time).ToString("####0.0###");
            setState2ndOrderView.transform.Find("zInput").gameObject.GetComponent<InputField>().text = mw.Z(mw.time).ToString("####0.0###");
            setState2ndOrderView.transform.Find("xPrimeInput").gameObject.GetComponent<InputField>().text = mw.GetPrime('x',mw.time).ToString("####0.0###");
            setState2ndOrderView.transform.Find("yPrimeInput").gameObject.GetComponent<InputField>().text = mw.GetPrime('y', mw.time).ToString("####0.0###");
            setState2ndOrderView.transform.Find("zPrimeInput").gameObject.GetComponent<InputField>().text = mw.GetPrime('z', mw.time).ToString("####0.0###");
            setState2ndOrderView.transform.Find("timeInput").gameObject.GetComponent<InputField>().text = mw.time.ToString("####0.0###");
        }
    }
    public void MenuBar_GraphicsOptionsButtonPress()
    {


    }

    public void ConfigView_OkButtonPress()
    {
        Motion3DSetup temp = configView.GetComponent<Motion3DConfigGui>().exportForSetup();
        if (temp != null)
        {
            //Debug.Log("Redefining problem...");
            //Accept new problem definition
            motion3Dsetup = temp;
            //Close config gui
            configView.SetActive(false);
            //Restructure problem
            ApplyConfiguration(motion3Dsetup);
        }

    }
    public void ConfigView_CancelButtonPress()
    {
        //close GUI
        configView.SetActive(false);
    }

    public void MessageBox_CancelButtonPress()
    {

    }
    public void MessageBox_OkButtonPress()
    {

    }

    public void SetState1stOrderView_OkButtonPress()
    {
        //Parse user input
        double x, y, z, t;
        string xstr = setState1stOrderView.transform.Find("xInput").gameObject.GetComponent<InputField>().text;
        string ystr = setState1stOrderView.transform.Find("yInput").gameObject.GetComponent<InputField>().text;
        string zstr = setState1stOrderView.transform.Find("zInput").gameObject.GetComponent<InputField>().text;
        string tstr = setState1stOrderView.transform.Find("timeInput").gameObject.GetComponent<InputField>().text;
        if (!(double.TryParse(xstr, out x))) return;
        if (!(double.TryParse(ystr, out y))) return;
        if (!(double.TryParse(zstr, out z))) return;
        if (!(double.TryParse(tstr, out t))) return;
        //All valid, now set state
        foreach (MotionWrapper mw in simulatedObjects)
        {
            if (mw != null)
            {
                mw.SetInitialCondition1stOrder(x, y, z, t);
                mw.time = t;
                mw.parent.ResetTrail();
                mw.parent.OverridePosition((float)x, (float)y, (float)z);
                mw.parent.UpdateTrail();
            }
        }
        //Update the simTime in the controller
        simTime = (float)t;
        //Close GUI
        setState1stOrderView.SetActive(false);
    }
    public void SetState1stOrderView_CancelButtonPress()
    {
        //Close GUI
        setState1stOrderView.SetActive(false);
    }
    public void SetState2ndOrderView_OkButtonPress()
    {
        //Parse user input
        double x, y, z, t, xprime, yprime, zprime;
        string xstr = setState2ndOrderView.transform.Find("xInput").gameObject.GetComponent<InputField>().text;
        string ystr = setState2ndOrderView.transform.Find("yInput").gameObject.GetComponent<InputField>().text;
        string zstr = setState2ndOrderView.transform.Find("zInput").gameObject.GetComponent<InputField>().text;
        string tstr = setState2ndOrderView.transform.Find("timeInput").gameObject.GetComponent<InputField>().text;
        string xprimestr = setState2ndOrderView.transform.Find("xPrimeInput").gameObject.GetComponent<InputField>().text;
        string yprimestr = setState2ndOrderView.transform.Find("yPrimeInput").gameObject.GetComponent<InputField>().text;
        string zprimestr = setState2ndOrderView.transform.Find("zPrimeInput").gameObject.GetComponent<InputField>().text;
        if (!(double.TryParse(xstr, out x))) return;
        if (!(double.TryParse(ystr, out y))) return;
        if (!(double.TryParse(zstr, out z))) return;
        if (!(double.TryParse(tstr, out t))) return;
        if (!(double.TryParse(xprimestr, out xprime))) return;
        if (!(double.TryParse(yprimestr, out yprime))) return;
        if (!(double.TryParse(zprimestr, out zprime))) return;
        //All valid, now set state
        foreach (MotionWrapper mw in simulatedObjects)
        {
            if (mw != null)
            {
                mw.SetInitialCondition2ndOrder(x, xprime, y, yprime, z, zprime, t);
                mw.time = t;
                mw.parent.ResetTrail();
                mw.parent.OverridePosition((float)x, (float)y, (float)z);
                mw.parent.UpdateTrail();
            }
        }
        //Update the simTime in the controller
        simTime = (float)t;
        //Close GUI
        setState2ndOrderView.SetActive(false);
    }
    public void SetState2ndOrderView_CancelButtonPress()
    {
        //Close GUI
        setState2ndOrderView.SetActive(false);
    }



    private void ApplyConfiguration(Motion3DSetup setup)
    {
        foreach (MotionWrapper mw in simulatedObjects)
        {
            if (mw != null)
            {
                mw.ApplyConfiguration(setup);
                mw.parent.ResetTrail();
                mw.parent.UpdateTrail();
            }

        }
    }

    //Should be located in a more global/general script
    //public void MenuBar_MenuButtonPress() { }


	private int numObjects = 1;
	private List<GameObject> instance; 
	public GameObject mass;
	private int currentObj = 1; //First object holds Motion3dSceneController, cannot be deleted
                                //Gavin: I'm going to return Motion3DSceneController to an empty object.
                                //       Whatever wasn't working with it, I'll make it work. 

    //Gavin: I'm going to move this to an motion object collection class 
    //"Motion3DObjectController" is what I mentioned on slack.
    public void ChangeNumberObjects(int n)
    {

   		if  (n <= 0){ n = 1; }

   		numObjects = n;

   		if (n > instance.Count){

			for (var k = instance.Count; k < n; k++){
				currentObj = k;

				float initial_x = k;
				float initial_y = 0;
				float initial_z = k;

				instance.Add( Instantiate(mass, new Vector3(
					initial_x, 
					initial_y, 
					initial_z), transform.rotation) );
				instance[k].AddComponent<ClickScript>(); //Gavin: What is this? Is it still valid or can we remove it?
					//Can be removed as of now, but idealy it will allow the objects to be clicked in order
					//to be selected, eventually

				
				instance[k].AddComponent<MotionMaster>(); 
				instance[k].GetComponent<MotionMaster>().ID = k;
				instance[k].GetComponent<MotionMaster>().mx = initial_x;
				instance[k].GetComponent<MotionMaster>().my = initial_y;
				instance[k].GetComponent<MotionMaster>().mz = initial_z;
				simulatedObjects.Add(instance[k].GetComponent<MotionMaster>().mw);

			}

    	}else{

    		//Deletes all objects past the specified number n
			for (var k = instance.Count; k > n; k--){
				currentObj = k;

				Destroy(instance[k-1]); 
				instance.RemoveAt(k-1);
				simulatedObjects.RemoveAt(k-1);
			}

    	}

    	UpdateCurrentObject();

    }

    //Gavin: I'll split this functionality between Motion3DSimulationController and Motion3DObject
    //       The reason I'd split this is firstly because the SceneController should be skinnier
    //       and focused on user input, and graphics functionality are a part of View and 
    //       don't belong in the Controller, and the only "View" between these three are the
    //       Motion3DObject's themselves.

    //       So this controller (Motion3DSceneController) will accept user input and call 
    //       functions in Motion3DSimulationController. Motion3DSimulationController will contain 
    //       and manage all the Motion3DObject's, and call functions in each of them to
    //       update their graphics settings accordingly. 


    //Updates material attributes of created mass objects when 
    public void UpdateCurrentObject(){

		for (var k = 0; k < numObjects; k++){
    		if (k == currentObj){ 
				instance[k].gameObject.GetComponent<MeshRenderer>().material.color = 
	 				new Color(1.0f, 1.0f, 0.0f, 1.0f);
	 		}else{
				instance[k].gameObject.GetComponent<MeshRenderer>().material.color = 
	 				new Color(1.0f, 1.0f, 1.0f, .5f);
	 		}
	 	}
    }

	public GameObject posText;

    //Update hoving (x,y,z) text over object -- 
    //Bug... also removes position of Main Object. Maybe because of scripts MoveText or PositionText?
	public void UpdateCurrentObject_Text(){

		posText = GameObject.Find("Position Text");

		/*
		posText.GetComponent<MoveText>().MassLocation.transform.position = new Vector3(
			instance[currentObj].transform.position.x, 
			instance[currentObj].transform.position.y,
			instance[currentObj].transform.position.z);
		*/
	}

    //Starts or pauses the simulation
    public void StartStopToggle()
    {
        if (simulationIsRunning)
            StopSimulation();
        else
            StartSimulation();
    }
    public void StartSimulation()
    {
        simulationIsRunning = true;
        startStopButtonText.text = "Stop";
        foreach (MotionWrapper mw in simulatedObjects)
        {
            mw.parent.paused = false;
        }
    }
    public void StopSimulation()
    {
        simulationIsRunning = false;
        startStopButtonText.text = "Start";
        foreach (MotionWrapper mw in simulatedObjects)
        {
            mw.parent.paused = true;
        }
    }
}
