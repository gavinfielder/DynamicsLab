/**
 * This file holds classes and numerical methods for solving
 * initial value problems
 * 
 * @author Gavin Fielder
 * @date 2/19/2018
 */

using DynamicsLab.Vector;
using DynamicsLab.Data;
using System.Collections.Generic;
using B83.ExpressionParser;
using System.IO;


namespace DynamicsLab.Solvers
{
    // *************************************************************************
    // **************************** Public Classes *****************************
    // *************************************************************************
    /*
     * public class ODEInitialValueProblem
     * 
     *      Setup: 
     *      
     *          An ODEInitialValueProblem requires the following steps to set up:
     *          
     *              1. Constructor:
     *                  public ODEInitialValueProblem(byte dim_in, double h, double tIntercept)
     *                      dim_in      : specifies the dimensionality (number of state 
     *                                    variables) of the differential system.
     *                                    Note this refers to the ultimate number of first-order
     *                                    differential equations--see my ODE primer for details.
     *                      h           : specifies the resolution (time between data points)
     *                                    of the numerical solution
     *                      tIntercept  : specifies the initial time value--usually zero
     *           
     *              2. Define the system of ODEs.
     *                 After using an ExpressionParser to evaluate each expression,
     *                 set the function delegates like so:
     *                      myIVP.F.funcs[0] = myExpression_0.ToDelegate("t","x",...);
     *                      myIVP.F.funcs[1] = myExpression_1.ToDelegate("t","x",...);
     *                          ...
     *                      myIVP.F.funcs[n] = myExpression_n.ToDelegate("t","x",...);
     *                  for however many dimensions the system is
     * 
     *      Control:
     *      
     *              public void SetState(VectorND newState, double currentTime)
     *                  This function sets the initial conditions. This needs to be done
     *                  before any solving takes place. The first time you do this, you
     *                  can use whatever initial time you specify (usually zero) as the
     *                  currentTime. 
     *                  This function is also used to give it a new initial condition 
     *                  during runtime (ie if the user clicks and drags an object to 
     *                  a new location). Just note that every time this function is 
     *                  called, all existing data is invalidated.
     *              
     *              public void SolveTo(double time)
     *                  This function generates some solution data. The 'time' that you pass
     *                  to it specifies the value of the independent variable that you want
     *                  to solve up to. For example, once you create an 
     *                  ODEInitialValueProblem with initial tIntercept of 0 and a resolution
     *                  of h=0.1, then you can call myIVP.SolveTo(5), which will generate 
     *                  50 new data points from t=0.1 to t=5.0 using the Runge-Kutta method.
     * 
     *              Updating parameters during runtime
     *                  1. (external to this class) update the value in the Expression, 
     *                     evaluate the expression, and set the F.funcs array elements
     *                     to the new delegates (see #1 under Setup)
     *                  2. Invalidate the data using this call:
     *                          myIVP.InvalidateData(currentTime);
     *                     What this does is essentially deletes all the existing data, 
     *                     since it's no longer valid for the new parameter value.
     *                  3. Call myIVP.SolveTo(time) as normal to generate new solution
     *                     data.
     * 
     *      Reading the results:
     *      
     *              public double SolutionData(byte i, double t)
     *                  This function retrieves solution data. 
     *                  i : specifies which data series to grab data from. 
     *                  t : specifies the independent variable value to look up
     *                  
     *                  For example, myIVP.SolutionData(0,5.6) will retrieve the value of 
     *                  the first state variable at t=5.6
     *                  
     *      Some basic informational accessors:
     *              
     *              public byte GetDim() : returns system dimensionality
     *              public double GetH() : returns solution resolution h
     *              
     *              public double GetDataLowerBound() 
     *              public double GetDataUpperBound()
     *              
     *                  ^ These two functions will return the upper and lower bound on 
     *                  the valid solution data that has been generated. You might use these
     *                  to make sure you're not asking for data that doesn't exist, or that
     *                  is simply no longer valid.
     *                  
     *      Other:
     *      
     *              public void SetH(double h_in, double tIntercept)
     *                  This function will set a new solution data resolution. This basically 
     *                  resets the entire solution data structure, so try not to use it.
     *                  The only thing I can think this would come in handy for is if we put
     *                  in a feature that sensed if the system was lagging and decided to lower
     *                  the solution resolution on its own to speed things up. The tIntercept
     *                  argument sets the new starting point of the data, which might as well
     *                  be whatever the current time is on the solution calculation.
     *                 
     */
    /**
     * This class handles an ODE initial value problem
     */
    public class ODEInitialValueProblem
    {
        //Fields
        public FunctionVectorND F;
        private ODEIVPSolver solver;
        private DataSet solution;

        //Constructor
        public ODEInitialValueProblem(byte dim_in, double h, double tIntercept)
        {
            solution = new DataSet(dim_in, tIntercept, h);
            solver = new ODEIVPSolver(dim_in);
            F = new FunctionVectorND(dim_in);
        }

        //Accessors
        public byte GetDim() { return solution.dim; }
        public double GetH() { return solution.h; }
        public double GetDataLowerBound() { return solution.dataLowerBound; }
        public double GetDataUpperBound() { return solution.dataUpperBound; }

        //Access the solution data for series i at time t
        public double SolutionData(byte i, double t)
        {
            return solution[t][i];
        }

        //Manipulators
        //Resetting H resets the entire solution. Don't use unless necessary.
        public void SetH(double h_in, double tIntercept)
        {
            byte dim = solution.dim;
            solution = new DataSet(dim, tIntercept, h_in);
        }


        //Invalidates and discards the currently stored solution data
        //This sets maxIndex and minIndex to the current index benchmarked to
        //currentTime. This means in order to have valid data at this location,
        //the state may need to be be written using solution.WriteState(...) 
        //with the same time value directly after calling this function. 
        public bool InvalidateData(double currentTime)
        {
            bool validDataAtLocation = false;
            //Get requested index
            int index = solution.Lookup(currentTime);
            //Check if index is actually in the current data set
            if ((index < 0) || (index >= (solution.data.Count * 1000)))
            {
                //Needs a brand new chunk
                while (solution.data.Count > 0)
                    solution.RemoveFirstChunk();
                solution.data.Add(new DataChunk(solution.dim, currentTime, currentTime + (solution.h * 1000)));
                solution.minIndex = 0;
                solution.maxIndex = 0;
                index = 0;
                //set new solution intercept
                solution.tIntercept = currentTime;
                //Flag that data at current location is invalid
                validDataAtLocation = false;
            }
            else
            {
                //index is inside current data set allocation
                //check whether that vector has valid data
                if ((index <= solution.maxIndex) && (index >= solution.minIndex))
                {
                    validDataAtLocation = true;
                }
                else
                {
                    validDataAtLocation = false;
                }
                //Delete the data before the current chunk
                while (index >= 1000)
                {
                    solution.RemoveFirstChunk();
                    index -= 1000;
                }
                //Delete the data after the current chunk
                while (solution.data.Count > 1)
                    solution.RemoveLastChunk();
                //Mark all other data as invalid by setting min and max index
                solution.minIndex = index;
                solution.maxIndex = index;
            }
            //Set the informational bounds to current time
            solution.dataLowerBound = currentTime;
            solution.dataUpperBound = currentTime;
            //Return whether current location has valid data
            return validDataAtLocation;
        }

        //Make sure solution data is populated up to the specified time
        public void SolveTo(double time)
        {
            while (solution.dataUpperBound < time)
            {
                //Perform runge-kutta method of order 4 and write the results to the next data vector
                solution.WriteNext(solver.RungeKutta(solution.Tval(solution.maxIndex + 1),
                                         solution.h, solution[solution.maxIndex],
                                         F));
            }
        }

        //Sets new initial conditions and invalidates existing data
        public void SetState(VectorND newState, double currentTime)
        {
            InvalidateData(currentTime);
            int i = solution.Lookup(currentTime);
            solution.WriteState(solution.ChunkIndex(i), solution.LocalIndex(i), newState);
        }

        //Returns the VectorND state at the requested time
        public VectorND GetState(double currentTime)
        {
            int i = solution.Lookup(currentTime);
            return solution.data[solution.ChunkIndex(i)].data[solution.LocalIndex(i)];
        }
        
        //Saves the data to file
        public void SaveData(StreamWriter fout)
        {
            solution.SaveData(fout);
        }
    }





    /**
     * This is a basic Spring-Mass problem
     * 
     * It serves as an example of how the ODEInitialValueProblem class is used
     * as well as how to work with the ExpressionParser
     * 
     *      Example code:
     *      
     *          Setup/initialization:
     *              SpringMassSystem sms = new SpringMassSystem(0.1, 1.0, 1.0, 0.15, 0, 1);
     *              sms.SetInitialCondition(0, 1, 0);
     *              
     *          Solve for upcoming data during update cycles/frames:
     *              sms.Update(t);
     *              
     *          To read the data:
     *              double value1 = sms.Position(t);
     *              double value2 = sms.Velocity(t);
     * 
     *          To give it a new initial condition during runtime:
     *              sms.SetInitialCondition(currentTime, 3,-1);
     *              
     *          To change a parameter during runtime:
     *              sms.Stiffness = 0.65;
     *              
     */
    public class SpringMassSystem
    {
        //fields
        private double k; //spring stiffness
        private double m; //mass
        private double b; //damping
        private double forwardSolveTime; //The amount of time into the future to prepare solution data for
        public double currentTime; //tracks the current time of the simulation
        public double Stiffness
        {
            get
            {
                return k;
            }
            set // Setting parameters causes immediate recalculation
            {
                k = value;
                ivp.InvalidateData(currentTime); // delete existing data
                Compile(); //reparse expression
            }
        }
        public double Mass
        {
            get
            {
                return m;
            }
            set // Setting parameters causes immediate recalculation
            {
                m = value;
                ivp.InvalidateData(currentTime); // delete existing data
                Compile(); //reparse expression
            }
        }
        public double Damping
        {
            get
            {
                return b;
            }
            set // Setting parameters causes immediate recalculation
            {
                b = value;
                ivp.InvalidateData(currentTime); // delete existing data
                Compile(); //reparse expression
            }
        }
        public double ForwardSolveTime
        {
            get
            {
                return forwardSolveTime;
            }
            set
            {
                forwardSolveTime = value;
            }
        }
        private ODEInitialValueProblem ivp;
        ExpressionParser parser;
        //The following strings define the differential system
        private const string udot = "v";
        private const string vdot = "(((-b) / m) * v) + (((-k) / m) * u)";
        Expression udot_expr;
        Expression vdot_expr;


        //Constructor
        public SpringMassSystem(double h_in, double stiffness_in,
                double mass_in, double damping_in,
                double currentTime_in, double forwardSolveTime_in)
        {
            //Create IVP solver
            ivp = new ODEInitialValueProblem(2, h_in, 0);
            //Set parameters
            k = stiffness_in;
            m = mass_in;
            b = damping_in;
            //Compile(evaluate) the expressions
            parser = new ExpressionParser();
            Compile();
            forwardSolveTime = forwardSolveTime_in;
        }

        //Compiles the system into a FunctionVectorND
        private void Compile()
        {
            //Register the constant symbols with their current values
            parser.AddConst("k", () => k);
            parser.AddConst("m", () => m);
            parser.AddConst("b", () => b);
            //Evaluate the expressions
            udot_expr = parser.EvaluateExpression(udot);
            vdot_expr = parser.EvaluateExpression(vdot);
            //Generate function delegates and load them into the IVP
            ivp.F.funcs[0] = udot_expr.ToDelegate("t", "u", "v");
            ivp.F.funcs[1] = vdot_expr.ToDelegate("t", "u", "v");
        }

        //Mathematical update cycle during frames. Generates new 
        //solution data ahead of time. Here forwardSolveTime is
        //used to determine how far ahead the solution should be
        //generated.
        public void Update(double newCurrentTime)
        {
            currentTime = newCurrentTime;
            ivp.SolveTo(currentTime + forwardSolveTime);
        }

        //Retrieves position as a function of time
        public double Position(double t)
        {
            return ivp.SolutionData(0, t);
        }
        //Retrieves velocity as a function of time
        public double Velocity(double t)
        {
            return ivp.SolutionData(1, t);
        }

        //Sets the state. Used for setting initial conditions
        public void SetInitialCondition(double t, double pos, double vel)
        {
            VectorND IC = new VectorND(pos, vel); //Gavin: this was (0.0, 1.0) before. Didn't know why.
            ivp.SetState(IC, t);
        }

    }



    // *************************************************************************
    // *************************** Internal Classes ****************************
    // *************************************************************************
    // Internal classes are those inaccessible outside the assembly. The way I
    // use them here, they shouldn't need to be used outside tthe math library.


    /**
     * This class contains the 4th order Runge Kutta method for solving IVPs
     */
    public class ODEIVPSolver //making public for testing only
    {
        private VectorND nullVector; //zero valued vector
        private byte dim; //number of dimensions

        //Constructor
        public ODEIVPSolver(byte dim_in)
        {
            //Set dimensionality
            dim = dim_in;
            //set up the null vector
            nullVector = new VectorND(dim);
            for (byte i = 0; i < dim; i++)
                nullVector[i] = 0;
        }

        /*
         * Builds an array of function inputs for Runge Kutta delegate calls
         * The reason we need this is Bunny83's expression parser only takes
         * params arrays of doubles for its delegates, so we need to
         * concatenate t and the state vector into an array.
         */
        private double[] RKCallBuild(double t, double t_offset, VectorND v,
                                    double v_off_scalar, VectorND v_offset)
        {
            double[] arr = new double[dim + 1];
            arr[0] = t + t_offset; //t is the first value in the array
            for (byte i = 0; i < dim; i++) //the remaining slots are for the state vector
                arr[i + 1] = v[i] + v_off_scalar * v_offset[i];
            return arr;
        }

        //Generates a new mesh point
        public VectorND RungeKutta(double t, double h, VectorND v,
                                   FunctionVectorND f)
        {
            VectorND k1 = h * f.Eval(RKCallBuild(t, 0, v, 0, nullVector));
            VectorND k2 = h * f.Eval(RKCallBuild(t, h / 2, v, 0.5, k1));
            VectorND k3 = h * f.Eval(RKCallBuild(t, h / 2, v, 0.5, k2));
            VectorND k4 = h * f.Eval(RKCallBuild(t, h, v, 1.0, k3));
            return (v + (1.0 / 6.0) * (k1 + 2 * k2 + 2 * k3 + k4));
        }
    }



    // *************************************************************************
    // ************************** Deprecated Classes ***************************
    // *************************************************************************
    // These classes are included for legacy support. Move away from using them
    // as soon as possible.

        /*
    // DEPRECATED : This code is included for legacy support
    //              Move away from using it as soon as possible.
    public class SpringMassSystem_old
    {
        //fields
        public float k; //stiffness
        public float m; //mass
        public float b; //friction
        public float h; //time between data points
        public uint numberOfDataPoints;
        public DataSeries t;
        public DataSeries position;
        public DataSeries velocity;


        //Manipulators
        //just gonna make everything public for now.

        //constructor
        public SpringMassSystem_old(float tLowerBound, float tUpperBound, float h_in)
        {
            h = h_in;
            numberOfDataPoints = (uint)(((tUpperBound - tLowerBound) / h) + 1);
            //^ should be +0.5 for rounding, but need a little more to hit the upper bound
            //TODO: make this better
            position = new DataSeries(numberOfDataPoints);
            velocity = new DataSeries(numberOfDataPoints);
            t = new DataSeries(tLowerBound, tUpperBound, numberOfDataPoints);
            position.setIndependentVariable(t);
            velocity.setIndependentVariable(t);
            m = 1; //zero mass would be bad, so just in case someone forgets to set it.
        }

        //The ODE system
        private float udot(float tval, float u, float v) { return v; }
        private float vdot(float tval, float u, float v)
        {
            return (-b / m) * v + (-k / m) * u;
        }

        //Solver
        public void solve(float initialPosition, float initialVelocity)
        {
            position[0] = initialPosition;
            velocity[0] = initialVelocity;
            for (uint i = 0; i < t.getSize() - 1; i++)
            {
                rungeKutta(i);
            }
        }

        //Runge Kutta method
        //Takes a (u,v) data point, generates the next one. 
        private void rungeKutta(uint i)
        {
            float k11 = h * udot(t[i], position[i], velocity[i]);
            float k12 = h * vdot(t[i], position[i], velocity[i]);
            float k21 = h * udot(t[i] + (h / 2), (float)(position[i] + 0.5 * k11), (float)(velocity[i] + 0.5 * k12));
            float k22 = h * vdot(t[i] + (h / 2), (float)(position[i] + 0.5 * k11), (float)(velocity[i] + 0.5 * k12));
            float k31 = h * udot(t[i] + (h / 2), (float)(position[i] + 0.5 * k21), (float)(velocity[i] + 0.5 * k22));
            float k32 = h * vdot(t[i] + (h / 2), (float)(position[i] + 0.5 * k21), (float)(velocity[i] + 0.5 * k22));
            float k41 = h * udot(t[i] + h, position[i] + k31, velocity[i] + k32);
            float k42 = h * vdot(t[i] + h, position[i] + k31, velocity[i] + k32);

            position[i + 1] = (float)(position[i] + ((k11 + 2 * k21 + 2 * k31 + k41) / 6.0));
            velocity[i + 1] = (float)(velocity[i] + ((k12 + 2 * k22 + 2 * k32 + k42) / 6.0));
        }
    }*/
}






