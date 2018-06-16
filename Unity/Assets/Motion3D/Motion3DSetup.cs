using System.Collections.Generic;
using B83.ExpressionParser;
using DynamicsLab.Vector;
using System.IO;
using UnityEngine; //for Debug.Log


namespace Motion3DDefaultProblem
{
    //Defines the default Motion3DProblem
    static class DefaultProblem
    {
		static public string exprX = "5.4*cos(t)";
        static public string exprY = "1.2*sin(t/2.2)";
        static public string exprZ = "cos(t/3.73)";
        static public int order = 1;
    }

	static class StrangeProblem
    {
		static public string exprX = "10*(y-x)";
        static public string exprY = "28*x-y-x*z";
        static public string exprZ = "x*y-(8/3)*z";
        static public int order = 1;
    }
}
namespace Motion3DConstants_namespace
{
    //Holds constants for 3D Motion
    static class Motion3DConstants
    {
        public static double h_default = 0.015625; //2e-6
        public static double forwardSolveTime_default = 0.125; //2e-3
        public static double forwardSolveTime_minimum = 0.0625; //2e-4
    }
}

/**
 * Motion3DProblemIOManager allows 3D Motion problems to be loaded from
 * and saved to disk. 
 */

namespace DynamicsLab.MotionSetup {


public class Motion3DProblemIOManager
{
    //Fields for problem/file information
    public string name; //a name for the problem
    public string description; //a description of the problem 
    //Gavin: Could you keep these two strings as the top lines in the file
    //       format? I'm going to use them for the overall file management.

    //Fields used in problem setup
    public string expressionX;
    public string expressionY;
    public string expressionZ;
    public Dictionary<string, double> parameters; //holds names and values
    public int order; //1=first order, 2=second order
    private int numberOfObjects;
    private List<VectorND> initialConditions; //each VectorND is the starting state of an object

    //Loads from disk (reads from file and sets all the fields)
    public void Load(string filename)
    {
        //Open file
        string path = "Assets/Motion3D/SavedProblems/" + filename;// + ".3dmotion"; //file manager makes sure there's a file extension
        StreamReader reader = new StreamReader(path);
        //Get name and description
        name = reader.ReadLine();
        description = reader.ReadLine();
        //Get equations
        expressionX = reader.ReadLine();
        expressionY = reader.ReadLine();
        expressionZ = reader.ReadLine();
        //Get order
        order = int.Parse(reader.ReadLine());
        Debug.Log("Read order " + order);
        //Get parameter list
        List<string> paramsSymbols = SeparateCSV(reader.ReadLine());
        //Get parameter values
        List<string> paramsValues = SeparateCSV(reader.ReadLine());
        //Merge into a single dictionary
        parameters = new Dictionary<string, double>();
        for (int i = 0; i < paramsSymbols.Count; i++)
        {
            parameters.Add(paramsSymbols[i], double.Parse(paramsValues[i]));
        }
        //Get objects
        initialConditions = new List<VectorND>();
        numberOfObjects = 0;
        while (!(reader.EndOfStream))
        {
            //get initial condition list
            List<string> objectIC = SeparateCSV(reader.ReadLine());
            if ((order == 1 && objectIC.Count == 3) ||
                    (order == 2 && objectIC.Count == 6))
            {
                //make a new VectorND
                VectorND vec = new VectorND((byte)objectIC.Count);
                for (byte i = 0; i < vec.GetDim(); i++)
                {
                    vec[i] = double.Parse(objectIC[i]);
                }
                //Add object with this initial condition
                numberOfObjects++;
                initialConditions.Add(vec);
            }
        }
        Debug.Log("Exiting with " + numberOfObjects + " objects defined.");
    }

    //Saves to disk (saves all the fields to file)
    public void Save(string filename)
    {

    }

    //Applies the problem to a Motion3DSimulationController
    public void Apply(Motion3DSimulationController simController)
    {
        //1. Make a Motion3DSetup
        Motion3DSetup setup = new Motion3DSetup(expressionX, expressionY, expressionZ, parameters, order);
        //Just by calling the above constructor all parameter values are entered and equations parsed

        //2. Apply the configuration (simController.ApplyConfiguration(...))
        simController.ApplyConfiguration(setup);

        //3. Add Object and set initial conditions
        // Since an object is selected after it is added, you can do this by
        // AddObject() and then SetStateOnCurrentObject() iteratively. 
        for (int i = 0; i < numberOfObjects; i++)
        {
            simController.AddObject();
            simController.SetStateOnCurrentObject(initialConditions[i]);
        }
    }


    //TODO: was copied from config gui. Might be better with josh's regex implementation
    //Separates a string in CSV format into a list. 
    private List<string> SeparateCSV(string text)
    {
        List<string> list = new List<string>();
        string str = "";
        //Populate list
        for (int i = 0; i < text.Length; i++)
        {
            switch (text.Substring(i, 1))
            {
                case ",": //new symbol to add
                    if (str.Length > 0)
                    {
                        list.Add(str);
                        str = ""; //reset
                    }
                    break;
                default:
                    //concatenate to current symbol definition
                    str = str + text.Substring(i, 1);
                    break;
            }
        }
        //Get last symbol
        if (str.Length > 0)
        {
            list.Add(str);
        }
        return list;
    }


}

/**
 * Motion3DSetup handles the expression parsing for a 3D Motion
 * problem. It also holds parameters values and reparses
 * whenever parameter values are changed. 
 */
public class Motion3DSetup
{
    //Fields
    private string expressionX;
    private string expressionY;
    private string expressionZ;
    private Dictionary<string, double> parameters;
    private int order; //1=first order, 2=second order
    private byte dim;
    private ExpressionParser parser;
    private FunctionVectorND F_internal;
    //Read-Only Properties
    public string ExpressionX
    {
        get
        {
            return expressionX;
        }
    }
    public string ExpressionY
    {
        get
        {
            return expressionY;
        }
    }
    public string ExpressionZ
    {
        get
        {
            return expressionZ;
        }
    }
    public int Order
    {
        get
        {
            return order;
        }
    }
    public byte Dim
    {
        get
        {
            return dim;
        }
    }
    public FunctionVectorND F
    {
        get
        {
            return F_internal;
        }
    }
    public Dictionary<string, double> Parameters
    {
        get
        {
            Dictionary<string, double> parameters_copy = parameters;
            return parameters_copy;
        }
    }

    //Constructor
    public Motion3DSetup(string expressionX_in, string expressionY_in,
        string expressionZ_in, Dictionary<string, double> params_in, int order_in)
    {
        expressionX = expressionX_in;
        expressionY = expressionY_in;
        expressionZ = expressionZ_in;
        parameters = params_in;
        order = order_in;
        if (order == 1) dim = 3;
        else dim = 6;
        F_internal = new FunctionVectorND(dim);
        parser = new ExpressionParser();
        Parse();
    }

    //Changes a parameter value
    public void ChangeParameter(string parameter, double value)
    {
        //if (parameters.ContainsKey(parameter)) //Gavin: removing checks for speed
        //{
        parameters[parameter] = value;
        //}
        Parse();
    }

    //Retrieves a parameter value
    public double GetParameterValue(string parameter)
    {
        //if (parameters.ContainsKey(parameter)) //Gavin: removing checks for speed
        //{
        return parameters[parameter];
        //}
    }

    //These functions parse the expressions
    private void Parse()
    {
        if (order == 1)
        {
            Parse1stOrder();
        }
        else
        {
            Parse2ndOrder();
        }
    }
    private void Parse1stOrder()
    {
        IncludeParameters();
        F_internal.funcs[0] = parser.EvaluateExpression(expressionX).ToDelegate("t", "x", "y", "z"); //x 
        F_internal.funcs[1] = parser.EvaluateExpression(expressionY).ToDelegate("t", "x", "y", "z"); //y
        F_internal.funcs[2] = parser.EvaluateExpression(expressionZ).ToDelegate("t", "x", "y", "z"); //z
    }
    private void Parse2ndOrder()
    {
        IncludeParameters();
        F_internal.funcs[0] = parser.EvaluateExpression("x'").ToDelegate("t", "x", "x'", "y", "y'", "z", "z'");
        F_internal.funcs[1] = parser.EvaluateExpression(expressionX).ToDelegate("t", "x", "x'", "y", "y'", "z", "z'");
        F_internal.funcs[2] = parser.EvaluateExpression("y'").ToDelegate("t", "x", "x'", "y", "y'", "z", "z'");
        F_internal.funcs[3] = parser.EvaluateExpression(expressionY).ToDelegate("t", "x", "x'", "y", "y'", "z", "z'");
        F_internal.funcs[4] = parser.EvaluateExpression("z'").ToDelegate("t", "x", "x'", "y", "y'", "z", "z'");
        F_internal.funcs[5] = parser.EvaluateExpression(expressionZ).ToDelegate("t", "x", "x'", "y", "y'", "z", "z'");
    }
    //Reads in parameters and adds them to the expression parser with their current values.
    //This is a helper function called by the parse functions above
    private void IncludeParameters()
    {
        foreach (KeyValuePair<string, double> p in parameters)
        {
            parser.AddConst(p.Key, () => p.Value);
        }
    }

}

}