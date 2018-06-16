using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicsLab.Solvers;
using DynamicsLab.Vector;
using Motion3DConstants_namespace;
using System.IO;

/**
 * Motion3DObject handles a simulated object in a 3D motion problem.
 * Motion3DObjects are created and attached to spheres by the Simulation
 * Controller. After this, this script will update the sphere's location
 * based on the time variable assigned to it by the Simulation
 * Controller. 
 * 
 * It also attaches to itself a ObjectTrailRenderer script, which will
 * automatically handle object trails. 
 * 
 * (TODO) It also attaches to itself a PositionTextRenderer script, which
 * will automatically handle a position label above the object.
 */
public class Motion3DObject : MonoBehaviour
{

    //Simulation control
    public ODEInitialValueProblem ivp; //public so controller can access it
    private byte xIndex, yIndex, zIndex; //Holds index of x, y, z positions within data vectors
    private double currentTime;
    public double CurrentTime
    {
        get
        {
            return currentTime;
        }
        set
        {
            currentTime = value;
            //Solves ahead of current time
            ivp.SolveTo(currentTime + forwardSolveTime);
        }
    }
    private double forwardSolveTime = Motion3DConstants.forwardSolveTime_default; 
    public double ForwardSolveTime
    {
        get
        {
            return forwardSolveTime;
        }
        set
        {
            //check against a minimum value
            forwardSolveTime = value;
            if (forwardSolveTime < Motion3DConstants.forwardSolveTime_minimum)
                forwardSolveTime = Motion3DConstants.forwardSolveTime_minimum;
        }
    }
 
    //Peripheral graphics component references
    private ObjectTrailRenderer trailRenderer;
    public static GameObject positionTextModel;
	public GameObject posText;

    // Use this for initialization
    void Start()
    {
		
        gameObject.AddComponent<ObjectTrailRenderer>();
        trailRenderer = gameObject.GetComponent<ObjectTrailRenderer>();
        posText = Instantiate(positionTextModel,
            new Vector3(0, 0, 0), transform.rotation);
        posText.SetActive(true);
        posText.GetComponent<PositionTextRenderer>().Follows = this.gameObject;
    }

    //Called every frame
    void Update()
    {
        UpdateObjectPosition();
        //Trail will update on its own now
    }

    //Updates the object's position
    public void UpdateObjectPosition()
    {
        //Some error checking, just in case. If error, simulation retains previous position.
        if ((currentTime < ivp.GetDataLowerBound()) || (currentTime > ivp.GetDataUpperBound()))
        {
            //if (!(paused))
            Debug.Log("Data out of bounds error at time = " + currentTime
                + "; Available data is on [" + ivp.GetDataLowerBound()
                + ", " + ivp.GetDataUpperBound() + "]");
        }
        else
        {

            if (!float.IsNaN((float)ivp.SolutionData(xIndex, currentTime)) && !float.IsNaN((float)ivp.SolutionData(yIndex, currentTime)) && !float.IsNaN((float)ivp.SolutionData(zIndex, currentTime))
                 && !float.IsInfinity((float)ivp.SolutionData(xIndex, currentTime)) && !float.IsInfinity((float)ivp.SolutionData(yIndex, currentTime)) && !float.IsInfinity((float)ivp.SolutionData(zIndex, currentTime)))
            gameObject.transform.position = new Vector3(
                (float)ivp.SolutionData(xIndex, currentTime),
                (float)ivp.SolutionData(yIndex, currentTime),
                (float)ivp.SolutionData(zIndex, currentTime));
        }
        
    }

    //Sets x, y, z position indices depending on order
    public void SetAs1stOrder()
    {
        xIndex = 0;
        yIndex = 1;
        zIndex = 2;
    }
    public void SetAs2ndOrder()
    {
        xIndex = 0;
        yIndex = 2;
        zIndex = 4;
    }

    //Returns the current value of the requested variable
    public double GetCurrentState(string variable)
    {
        switch (variable)
        {
            case "x":
                return ivp.SolutionData(xIndex, currentTime);
            case "y":
                return ivp.SolutionData(yIndex, currentTime);
            case "z":
                return ivp.SolutionData(zIndex, currentTime);
            case "t":
                return currentTime;
            case "x'": //if you request this it'd better be second order
                return ivp.SolutionData(1, currentTime);
            case "y'": //if you request this it'd better be second order
                return ivp.SolutionData(3, currentTime);
            case "z'": //if you request this it'd better be second order
                return ivp.SolutionData(5, currentTime);
            default:
                Debug.Log("Error: Unexpected default case.");
                return 0;
        }
    }
    //Returns the current state vector
    public VectorND GetCurrentState()
    {
        if (ivp.GetDim() == 3)
        {
            return new VectorND(GetCurrentState("x"),
                GetCurrentState("y"),
                GetCurrentState("z"));
        }
        else
        {
            return new VectorND(GetCurrentState("x"),
                GetCurrentState("x'"),
                GetCurrentState("y"),
                GetCurrentState("y'"),
                GetCurrentState("z"),
                GetCurrentState("z'"));
        }
    }

    //Resets the object trail
    public void ResetTrail()
    {
        trailRenderer.ResetTrail();
        //trailRenderer.NewTrail();
    }

    //Sets the state/initial condition. Also redefines the reset state.
    public void SetState(VectorND state, double t)
    {
        ivp.SetState(state, t);
        CurrentTime = currentTime; //Not setting a new value, but by
                                   //setting the Property we will 
                                   //solve forward for some data
        //trailRenderer.ResetTrail(); //TODO: fix these. These were throwing a null reference exception when a new object
        //trailRenderer.NewTrail(); //was added, causing the AddObject function to terminate prematurely.
        //OR: it seems to be working fine for the moment. Should we leave this commented out? To test...
    }

    //Highlights and dehighlights the object
    public void Highlight()
    {
        gameObject.GetComponent<MeshRenderer>().material.color =
                     new Color(1.0f, 1.0f, 0.0f, 1.0f);
    }
    public void Dehighlight()
    {
        gameObject.GetComponent<MeshRenderer>().material.color =
                     new Color(1.0f, 1.0f, 1.0f, .5f);
    }

    //Saves object motion data to file
    public void SaveData(StreamWriter fout)
    {
        ivp.SaveData(fout);
    }
}