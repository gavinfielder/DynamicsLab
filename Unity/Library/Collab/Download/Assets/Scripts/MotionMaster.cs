using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicsLab.Solvers;
using DynamicsLab.Vector;
using B83.ExpressionParser;



public class MotionWrapper{
        
    //Fields
	public ODEInitialValueProblem ivp; //temporarily public for debugging
	private ExpressionParser parser;
	public Motion3DSetup m3Ds;
    //Reset vector fields
    public double icX, icY, icZ, icTime, icXp, icYp, icZp;
    public double time; //public so the controller can write to it
    public MotionMaster parent; //reference to the MotionMaster which contains me
    private bool initialized; //TODO: make sure there's a default problem instead of having this flag

    //Constructor
    public MotionWrapper(){
        initialized = false; 
	}

    //Configures the IVP system according to a passed Motion3DSetup object
    //Should only be called when the user clicks OK on the 3D Motion Config menu
    public void ApplyConfiguration(Motion3DSetup setup){ 
        if (setup != null)
        {
            m3Ds = setup;
            if (m3Ds.order == 1)
            {
                ivp = new ODEInitialValueProblem(3, 0.01, 0);
                parser = new ExpressionParser();
                Parse1stOrder();
            }
            else //2nd order
            {
                ivp = new ODEInitialValueProblem(6, 0.01, 0);
                parser = new ExpressionParser();
                Parse2ndOrder();
            }
            solveIVP(time + 1); //solve for upcoming second of data
            initialized = true;
        }
	}

    //Updates one or more parameters values
    public void ChangeParameter(int index, double value)
    {
        //Change parameter value in the setup
        m3Ds.parameterValues[index] = value;
        //Re-evaluate expression
        Reparse();
        //Invalidate data at current time
        ivp.InvalidateData(time);
        //Solve for upcoming second of data
        ivp.SolveTo(time + 1);
    }


    //Reparses the expressions. Used when new parameters are input.
    //Called by a function which updates one or more parameter values
    //within m3Ds.
    private void Reparse()
    {
        if (m3Ds.order == 1)
        {
            Parse1stOrder();
        }
        else
        {
            Parse2ndOrder();
        }
    }

    //Evaluates 1st order expressions with updated user parameters
    private void Parse1stOrder()
    {
        IncludeParameters();
        ivp.F.funcs[0] = parser.EvaluateExpression(m3Ds.expressionX).ToDelegate("t", "x", "y", "z"); //x 
        ivp.F.funcs[1] = parser.EvaluateExpression(m3Ds.expressionY).ToDelegate("t", "x", "y", "z"); //y
        ivp.F.funcs[2] = parser.EvaluateExpression(m3Ds.expressionZ).ToDelegate("t", "x", "y", "z"); //z
    }
    //Evaluates 2nd order expressions with updated user parameters
    private void Parse2ndOrder()
    {
        IncludeParameters();
        ivp.F.funcs[0] = parser.EvaluateExpression("x'").ToDelegate("t", "x", "x'", "y", "y'", "z", "z'");
        ivp.F.funcs[1] = parser.EvaluateExpression(m3Ds.expressionX).ToDelegate("t", "x", "x'", "y", "y'", "z", "z'");
        ivp.F.funcs[2] = parser.EvaluateExpression("y'").ToDelegate("t", "x", "x'", "y", "y'", "z", "z'");
        ivp.F.funcs[3] = parser.EvaluateExpression(m3Ds.expressionY).ToDelegate("t", "x", "x'", "y", "y'", "z", "z'");
        ivp.F.funcs[4] = parser.EvaluateExpression("z'").ToDelegate("t", "x", "x'", "y", "y'", "z", "z'");
        ivp.F.funcs[5] = parser.EvaluateExpression(m3Ds.expressionZ).ToDelegate("t", "x", "x'", "y", "y'", "z", "z'");
    }

    //Reads the parameters in m3Ds and adds them to the expression parser
    //with their current values.
    //This is a helper function called by the parse functions above
    private void IncludeParameters()
    {
        for (int i = 0; i < m3Ds.parameters.Count; i++)
        {
            parser.AddConst(m3Ds.parameters[i], () => m3Ds.parameterValues[i]);
        }
    }


    //Sets initial conditions and saves the values to the reset settings
    public void SetInitialCondition1stOrder(double x, double y, double z, double t)
    {
            //set the state
            ivp.SetState(new VectorND(x, y, z), t);
            time = t;
            //change the reset values to this state
            icX = x;
            icY = y;
            icZ = z;
            icTime = t;
            //Solve for the upcoming second of data. This avoids a zero-length interval error when the
            //MotionMaster tries to grab solution data at the precise time value when paused
            solveIVP(t + 1);
    }

    //Sets initial conditions and saves the values to the reset settings
    public void SetInitialCondition2ndOrder(double x, double xprime, double y, double yprime, 
        double z, double zprime, double t)
    {
        //set the state
        ivp.SetState(new VectorND(x, xprime, y, yprime, z, zprime), t);
        time = t;
        //change the reset values to this state
        icX = x;
        icXp = xprime;
        icY = y;
        icYp = yprime;
        icZ = z;
        icZp = zprime;
        icTime = t;
        //Solve for the upcoming second of data. This avoids a zero-length interval error when the
        //MotionMaster tries to grab solution data at the precise time value when paused
        solveIVP(t + 1);
    }

    //Solves IVP to specified time
    public void solveIVP(double toTime){
        if (initialized)
        {
            ivp.SolveTo(toTime);
        }
	}

    //Retrieves position x at time t
    public float X(double t)
    {
        if (initialized)
        {
            if (m3Ds.order == 1)
                return (float)ivp.SolutionData(0, t);
            else
                return (float)ivp.SolutionData(0, t);
        }
        else
        {
            return 0;
        }
    }

    //Retrieves position y at time t
    public float Y(double t)
    {
        if (initialized)
        {
            if (m3Ds.order == 1)
                return (float)ivp.SolutionData(1, t);
            else
                return (float)ivp.SolutionData(2, t);
        }
        else
        {
            return 0;
        }
    }

    //Retrieves position z at time t
    public float Z(double t)
    {
        if (initialized)
        {
            if (m3Ds.order == 1)
                return (float)ivp.SolutionData(2, t);
            else
                return (float)ivp.SolutionData(4, t);
        }
        else
        {
            return 0;
        }
    }

    //For second order, retrieves xprime, yprime, or zprime at time t
    public double GetPrime(char dim, double t)
    {
        if (m3Ds.order == 1) return 0;
        else
        {
            switch (dim)
            {
                case 'x':
                    return ivp.SolutionData(1, t);
                case 'y':
                    return ivp.SolutionData(3, t);
                case 'z':
                    return ivp.SolutionData(5, t);
                default:
                    return 0;
            }
        }
    }

    //Returns upper, lower time bounds on available data
    public float GetAvailableDataLowerBound() {
        //Debug.Log("Returning lower bound of: " + ivp.GetDataLowerBound());
        return (float)ivp.GetDataLowerBound();
    }
    public float GetAvailableDataUpperBound()
    {
        //Debug.Log("Returning upper bound of: " + ivp.GetDataUpperBound());
        return (float)ivp.GetDataUpperBound();
    }

    //Resets to the saved initial condition
    public void Reset()
    {
        if (m3Ds.order == 1)
        {
            SetInitialCondition1stOrder(icX, icY, icZ, icTime);
        }
        else if (m3Ds.order == 2)
        {
            SetInitialCondition2ndOrder(icX, icXp, icY, icYp, icZ, icZp, icTime);
        }
        parent.OverridePosition((float)icX, (float)icY, (float)icZ);
        parent.ResetTrail();
        parent.UpdateTrail();
    }
}






public class MotionMaster : MonoBehaviour {

    //Fields
	public Color c1;
	public Color c2;
	List<float> lineX;
	List<float> lineY;
	List<float> lineZ;
	int lineNum;
    public float mx, my, mz;
    int count = 0;
    GameObject mass;
	GameObject plane;
	GameObject[] instance; 
	LineRenderer lineRenderer;
	public MotionWrapper mw; //public so the controller can access it
    public bool paused = true;

    public int ID = 0;

    //Default equations
	//private const string x_default = "5.4*cos(t)";
    //private const string y_default = "1.2*sin(t/2.2)";
    //private const string z_default = "cos(t/3.73)";

	private const string x_default = "10*(y-x)";
    private const string y_default = "28*x-y-x*z";
    private const string z_default = "x*y-(8/3)*z";

    //Called once upon creation
	void Start () {
		mw = new MotionWrapper();
        mw.parent = this;
		mw.ApplyConfiguration(new Motion3DSetup(x_default, y_default, z_default, 
            new List<string>(), 1)); //set up the default problem
        mw.SetInitialCondition1stOrder(mx, my, mz, 0);

        //Hook object references
        if (ID == 0){
        	mass = GameObject.Find("Motion3DSceneController");
        }else{
        	mass = this.gameObject;
        }
        plane = GameObject.Find("Plane");

        //Set color of floor
        plane.GetComponent<MeshRenderer>().material.color =
            new Color(1.0f, 1.0f, 1.0f, 0.5f);

        //Mass initial position
        mx = 0;
        my = 0; 
        mz = 0;

        //Initialize object trail
        InitializeTrail();
            
    }

    //Initializes the object trail graphics implementation
    private void InitializeTrail()
    {
        c1 = Color.yellow;
        c2 = Color.red;
        lineNum = 1;

        //Initialize Lines
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.positionCount = lineNum;

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        lineRenderer.colorGradient = gradient;

        //Initializes the trail itself
        NewTrail();
    }

    //Allocates and initializes the trail
    private void NewTrail()
    {
        lineX = new List<float>();
        lineY = new List<float>();
        lineZ = new List<float>();
        for (int i = 0; i < lineNum; i++)
        {
            lineX.Add(0);
			lineY.Add(0);
			lineZ.Add(0);
        }
    }

    //Resets the object trail to nothing
    //This is called by the controller whenever the simulation is reset 
    //or a new initial condition is entered. 
    public void ResetTrail()
    {
        //Gavin: I don't know the most sophisticated way to do this, so I'm just doing this for now.
        //TODO: implement a way to remove the current trail that's more efficient than this
        count = 0;
        NewTrail();
    }

    //Sets mx, my, mz manually to force the renderer to update
    //to the correct position while paused.
    //TODO: we might want to way to overcome that limitation that doesn't require
    //this type of band-aid approach. 
    public void OverridePosition(float mx_in, float my_in, float mz_in)
    {
        mx = mx_in;
        my = my_in;
        mz = mz_in;
    }

    //Called every frame
	void Update () {

        //Update object position
        UpdateObjectPosition();

        //Update object trail
        if (!(paused)) UpdateTrail();
	}

    //Updates the object's position
    public void UpdateObjectPosition()
    {

        //Some error checking, just in case. If error, simulation retains previous position.
        if ((mw.time < mw.GetAvailableDataLowerBound()) || (mw.time > mw.GetAvailableDataUpperBound())) {
            if (!(paused))
                Debug.Log("Data out of bounds error at time = " + mw.time 
                    + "; Available data is on [" + mw.GetAvailableDataLowerBound() 
                    + ", " + mw.GetAvailableDataUpperBound() + "]");
        }
        else {
            mx = mw.X(mw.time);
            my = mw.Y(mw.time);
            mz = mw.Z(mw.time);
        }

        //Debug.Log(mx);

        mass.transform.position = new Vector3(mx, my, mz);
    }

    //Updates the object trail
    public void UpdateTrail() 
    {

        //Doubles lineX/Y/Z vectors when count reaches the vector limit.
        //Copies previous values into extended vector
        //(done with vectors because C# has no array deallocation)
        if (count >= lineNum){

			float[] lineX_old = new float[lineNum];
	        float[] lineY_old = new float[lineNum];
	        float[] lineZ_old = new float[lineNum];
			for (int i = 0; i < lineNum; i++)
	        {
				lineX_old[i] = lineX[i];
				lineY_old[i] = lineY[i];
				lineZ_old[i] = lineZ[i];
	        }

            lineNum = lineNum * 2;
            //Debug.Log(lineNum);
			lineRenderer.positionCount = lineNum;

			for (int i = 0; i < lineNum; i++)
	        {
	            if (i < lineNum/2){
		            lineX[i] = lineX_old[i];
					lineY[i] = lineX_old[i];
					lineZ[i] = lineX_old[i];
				}else{
					lineX.Add(0);
					lineY.Add(0);
					lineZ.Add(0);	
				}
	        }

        }

            
        //Update line segments as mass moves
        lineX[count] = mx;
        lineY[count] = my;
        lineZ[count] = mz;
        for (int i = count + 1; i < lineNum; i++)
        {
            lineX[i] = lineX[count];
            lineY[i] = lineY[count];
            lineZ[i] = lineZ[count];
        }
        count += 1;

        //Re-position line segments
        for (int i = count - 1; i < lineNum; i++)
        {
            lineRenderer.SetPosition(i, new Vector3(
                lineX[i],
                lineY[i],
                lineZ[i]));
        }
    }
}
