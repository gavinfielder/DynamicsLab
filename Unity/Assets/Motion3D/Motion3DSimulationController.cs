using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DynamicsLab.Solvers;
using DynamicsLab.Vector;
using Motion3DDefaultProblem;
using Motion3DConstants_namespace;
using System.IO;
using System;
using DynamicsLab.VectorField;
using DynamicsLab.SceneController;
using DynamicsLab.MotionSetup;

/**
 * Motion3DSimulationController creates, stores, and manages 
 * Motion3DObjects and their associated GameObjects.
 * 
 * It also controls the simulation itself, i.e. the current
 * time, play/pause/speed, etc. 
 * 
 * It is designed to take commands from Motion3DSceneController.
 */
public class Motion3DSimulationController : MonoBehaviour {

    //Fields
    private List<Motion3DObject> simulatedObjects; //contains all motion objects
    private int currentObjectIndex; //index of the currently selected object
    private bool simulationIsRunning; //currentTime only iterates if true
    private Motion3DSetup motion3Dsetup; //always holds current setup and parameter values
    private GameObject objectModel; //reference to the sphere we copy for new objects
    private double currentTime; //tracks the current simulation time
    public float speed = 1f; //multiplier of the speed of simulation
    private double forwardSolveTime;
    //Scene controller reference (used only limitedly, like for error messages, etc)
    private Motion3DSceneController sceneController;
    //Other GUI elements
    private Text startStopButtonText; //for switching button text between "Start" and "Stop"
    private InputField timeDisplayText; //for updating the current simulation time display
    private TimeSlider timeSlider; //for displaying data bounds and current time location 
    //Fields for special purpose, like waiting on callbacks from messageboxes
    private VectorND state_tempsave;
    private double time_tempsave;
    //Underlying fields that should only be accessed by their associated Property
    private double allObjectsDataLowerBound_internal; //Do not use
    private double allObjectsDataUpperBound_internal; //Do not use
    private double resetTime_internal; //Do not use
    //Private properties to simplify internal behavior 
    private double AllObjectsDataLowerBound 
    {
        set
        {
            allObjectsDataLowerBound_internal = value;
            timeSlider.LowerBound = value;
            //ensure reset time is within valid interval
            if (ResetTime < allObjectsDataLowerBound_internal)
            {
                ResetTime = allObjectsDataLowerBound_internal;
            }
        }
        get
        {
            return allObjectsDataLowerBound_internal;
        }
    }
    private double AllObjectsDataUpperBound 
    {
        set
        {
            allObjectsDataUpperBound_internal = value;
            timeSlider.UpperBound = value;
            //ensure reset time is within valid interval
            if (ResetTime > allObjectsDataUpperBound_internal)
            {
                ResetTime = allObjectsDataUpperBound_internal;
            }
        }
        get
        {
            //Debug.Log("RBF: AllObjectsDataUpperBound is read. Returning " + allObjectsDataUpperBound_internal);
            return allObjectsDataUpperBound_internal;
        }
    }
    private double ResetTime
    {
        get
        {
            return resetTime_internal;
        }
        set
        {
            resetTime_internal = value;
            //Ensure the reset time is inside the valid data interval
            if (resetTime_internal < AllObjectsDataLowerBound)
            {
                resetTime_internal = AllObjectsDataLowerBound;
            }
            if (resetTime_internal > AllObjectsDataUpperBound)
            {
                resetTime_internal = AllObjectsDataUpperBound;
            }
        }
    }
    //Public properties
    public double ForwardSolveTime {
        get
        {
            return forwardSolveTime;
        }
        set
        {
            forwardSolveTime = value;
            if (forwardSolveTime < Motion3DConstants.forwardSolveTime_minimum)
                forwardSolveTime = Motion3DConstants.forwardSolveTime_minimum;
            for (int i = 0; i < simulatedObjects.Count; i++)
                simulatedObjects[i].ForwardSolveTime = forwardSolveTime;
        }
    }
    //Read-only properties
    public Motion3DSetup GetMotion3DSetup
    {
        get
        {
            return motion3Dsetup; //TODO: check if this is actually read only as intended
        }
    }
    public double CurrentTime
    {
        get
        {
            return currentTime;
        }
        set 
        {
            if ((value >= AllObjectsDataLowerBound) && (value <= AllObjectsDataUpperBound))
            {
                //First, a discontinuity check
                const float DISCONTINUITY_THRESHHOLD = 1f;
                if (Mathf.Abs((float)(value - currentTime)) > DISCONTINUITY_THRESHHOLD)
                { 
                    //We've jumped through time. Reset the object trails. 
                    ResetAllTrails();
                }
                //Set time
                currentTime = value;
            }
        }
    }
    //Peripheral scene-level graphics
    public Motion3DVectorField vectorField;



    // Use this for initialization
    void Start () {
        //Instantiate simulated objects list
        simulatedObjects = new List<Motion3DObject>();
        //Hook other GUI elements
        startStopButtonText = GameObject.Find("StartButton").GetComponentInChildren<Text>();
        timeDisplayText = GameObject.Find("TimeInput").GetComponent<InputField>();
        timeSlider = GameObject.Find("TimeSlider").GetComponent<TimeSlider>(); //TODO: conflict add
        //Hook Scene Controller
        sceneController = gameObject.GetComponent<Motion3DSceneController>();
        //Initialize problem setup
        motion3Dsetup = new Motion3DSetup(DefaultProblem.exprX, DefaultProblem.exprY, DefaultProblem.exprZ, 
            new Dictionary<string, double>(), DefaultProblem.order);
        //Set default forward solve time
        forwardSolveTime = Motion3DConstants.forwardSolveTime_default;
        //Set the object model to copy from
        objectModel = GameObject.Find("ObjectModel");
        objectModel.SetActive(false);
        //Set valid data interval and reset time. Use internals to avoid bugs with one not being set yet
        allObjectsDataLowerBound_internal = 0;
        allObjectsDataUpperBound_internal = 0;
        resetTime_internal = 0;
        //Hook vector field
        vectorField = gameObject.GetComponent<Motion3DVectorField>();
        vectorField.DisableVectorField();
    }

    // Update is called once per frame
    void Update()
    {
        //Update simulation
        if (simulationIsRunning)
        {
            //Progress time
            currentTime += Time.deltaTime * speed;
        }
        //Update time display
        timeDisplayText.text = currentTime.ToString("####0.00") + " (" + speed.ToString("#0.00") + "x)";
        timeSlider.Value = (float)currentTime; //updates slider display for current time 
        //Update objects
        for (int i = 0; i < simulatedObjects.Count; i++)
        {
            simulatedObjects[i].CurrentTime = currentTime;
        }
        //Check if there's new data to track in the valid data interval
        if (currentTime + forwardSolveTime > AllObjectsDataUpperBound)
        { 
            AllObjectsDataUpperBound = currentTime + forwardSolveTime;
        }

    }

    //Applies a new problem configuration (equation set)
    public void ApplyConfiguration(Motion3DSetup setup)
    {
        if (setup != null)
        {
            motion3Dsetup = setup;
            //Apply new equations to each simulated object
            for (int i = 0; i < simulatedObjects.Count; i++)
            {
                ApplyConfigurationToObject(i);
            }
            vectorField.F = motion3Dsetup.F;
        }
    }

    //Applies the current configuration to the object located at index
    private void ApplyConfigurationToObject(int index)
    {
        if ((simulatedObjects[index].ivp == null) //is not yet initialized?
            || (simulatedObjects[index].ivp.GetDim() != motion3Dsetup.Dim)) //is wrong order?
        {
            simulatedObjects[index].ivp
                = new ODEInitialValueProblem(
                    motion3Dsetup.Dim, Motion3DConstants.h_default, currentTime);
        }
        if (motion3Dsetup.Order == 1)
            simulatedObjects[index].SetAs1stOrder();
        else
            simulatedObjects[index].SetAs2ndOrder();
        simulatedObjects[index].ivp.F = motion3Dsetup.F;
        simulatedObjects[index].ivp.InvalidateData(currentTime);
        simulatedObjects[index].CurrentTime = currentTime; //also solves forward
        if (AllObjectsDataLowerBound < currentTime )
            AllObjectsDataLowerBound = currentTime;
        if (AllObjectsDataUpperBound > currentTime + forwardSolveTime)
            AllObjectsDataUpperBound = currentTime + forwardSolveTime;
    }

    //Changes a parameter value
    public void ChangeParameter(string parameter, double value)
    {
        motion3Dsetup.ChangeParameter(parameter, value);
        //Apply new equations to each simulated object
        for (int i = 0; i < simulatedObjects.Count; i++)
        {
            ApplyConfigurationToObject(i);
        }
    }

    //Sets the state of the currently selected object
    public void SetStateOnCurrentObject(VectorND state)
    {
        simulatedObjects[currentObjectIndex].SetState(state, currentTime); //also solves forward
        //Make sure the interval of all valid data is contained in this object's valid interval
        if (AllObjectsDataLowerBound < currentTime)
            AllObjectsDataLowerBound = currentTime;
        if (AllObjectsDataUpperBound > currentTime + forwardSolveTime)
            AllObjectsDataUpperBound = currentTime + forwardSolveTime;
    }

    //Returns the state of the currently selected object
    public double GetCurrentState(string variable)
    {
        return simulatedObjects[currentObjectIndex].GetCurrentState(variable);
    }

    //Resets all objects to their reset states
    public void ResetAll()
    {
        //Set simulator to the reset time. 
        currentTime = ResetTime;
        //Reset trails
        ResetAllTrails();
    }

    public void ResetAllTrails()
    {
        //Reset all object trails
        for (int i = 0; i < simulatedObjects.Count; i++)
        {
            simulatedObjects[i].ResetTrail();
        }
    }

    //Adds and removes objects (RemoveObject removes the current object)
    public void AddObject()
    {
        const float K = 1f;
        //Instantiate a new object and add it to the list
        GameObject newObj = Instantiate(objectModel, new Vector3(0, 0, 0),
            transform.rotation);
        newObj.AddComponent<Motion3DObject>();
        newObj.SetActive(true);
        Motion3DObject newObject = newObj.GetComponent<Motion3DObject>();
        simulatedObjects.Add(newObject);
		//Instantiate Position text for new object and add it to text list
		//GameObject posText = Instantiate(Motion3DObject.positionTextModel,
		//	new Vector3(0, 0, 0), transform.rotation);
		//posText.SetActive(true);
		//posText.GetComponent<PositionTextRenderer>().Follows = this.gameObject;
        //Select the newly added object
        SelectObject(simulatedObjects.Count - 1);
        //Apply the current configuration to the new object
        ApplyConfigurationToObject(currentObjectIndex);
        //Set the object's forward solve time to the same as the simulator's forward solve time
        simulatedObjects[currentObjectIndex].ForwardSolveTime = forwardSolveTime;
        //Set the state of the new object to a position based on current number of objects
        if (motion3Dsetup.Order == 1)
        {
            simulatedObjects[currentObjectIndex].SetState(new VectorND
                (K * (simulatedObjects.Count - 1), 0, K * (simulatedObjects.Count - 1)),
                currentTime); //also solves forward by setting CurrentTime
        }
        else //2nd order
        {
            simulatedObjects[currentObjectIndex].SetState(new VectorND
                (K * (simulatedObjects.Count - 1), 0, 0, 0, K * (simulatedObjects.Count - 1), 0),
                currentTime); //also solves forward by setting CurrentTime
        }
        //Make sure the interval of all valid data is contained in this object's valid interval
        if (AllObjectsDataLowerBound < currentTime)
        {
            AllObjectsDataLowerBound = currentTime;
        }
        if (AllObjectsDataUpperBound > currentTime + forwardSolveTime)
        {
            AllObjectsDataUpperBound = currentTime + forwardSolveTime;
        }
    }
    public void RemoveObject()
    {
		if(currentObjectIndex >= 0 && simulatedObjects.Count >= 1)
		{
			//Remove object
			simulatedObjects[currentObjectIndex].posText.SetActive(false); //Removes Position Text
	        simulatedObjects[currentObjectIndex].gameObject.SetActive(false); //causes no rendering or update cycle
	        simulatedObjects.RemoveAt(currentObjectIndex); //remove from my list of things to update
	        //don't Destroy(), just leave for garbage collector
	        SelectPrevObject(); //select the previous object
		}
    }

    //Selects next or previous object
    public void SelectNextObject()
    {
		if (currentObjectIndex < simulatedObjects.Count && currentObjectIndex >= 0)
            simulatedObjects[currentObjectIndex].Dehighlight(); //Renders neutral color
        currentObjectIndex++; //Go to next object
        if (currentObjectIndex >= simulatedObjects.Count) //wrap around if needed
            currentObjectIndex = 0;
        if (simulatedObjects.Count > 0) //if list is nonempty
        {
            simulatedObjects[currentObjectIndex].Highlight(); //Renders highlighted color
        }
        //else currentObject = null; //Gavin: gonna leave references for now just in case
    }
    public void SelectPrevObject()
    {
        if (currentObjectIndex < simulatedObjects.Count)
            simulatedObjects[currentObjectIndex].Dehighlight(); //Renders neutral color
        currentObjectIndex--; //Go to prev object
        if (currentObjectIndex < 0) //wrap around if needed
            currentObjectIndex = simulatedObjects.Count - 1;
        if (simulatedObjects.Count > 0) //if list is nonempty
        {
            simulatedObjects[currentObjectIndex].Highlight(); //Renders highlighted color
        }
        //else currentObject = null; //Gavin: gonna leave references for now just in case
    }
    //Selects a particular object in the collection by index lookup
    private void SelectObject(int index)
    {
		if (currentObjectIndex < simulatedObjects.Count && currentObjectIndex >= 0)
            simulatedObjects[currentObjectIndex].Dehighlight(); //Renders neutral color
        currentObjectIndex = index;
        if (index < simulatedObjects.Count)
            simulatedObjects[currentObjectIndex].Highlight(); //render highlighted color
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
    }
    public void StopSimulation()
    {
        simulationIsRunning = false;
        startStopButtonText.text = "Start";
    }

    //Increases or Decreases  speed of simulation
    public void IncreaseSpeed()
    {
        speed *= 2f;
    }
    public void DecreaseSpeed()
    {
        speed *= 0.5f;
    }
    
    //Saves Data to file
    public void SaveData()
    {
        DateTime dt = DateTime.Now;
        string baseFilename = "3DMotion_Data_" + dt.Year + "_" + dt.Month + "_" +
            dt.Day + "_" + dt.Hour + "_" + dt.Minute + "_" + dt.Second + "_Object_{0}.csv";
        StreamWriter fout;
        for (int i = 0; i < simulatedObjects.Count; i++)
        {
            fout = new StreamWriter(String.Format(baseFilename, i));
            simulatedObjects[i].SaveData(fout);
            fout.Close();
        }
    }


}
