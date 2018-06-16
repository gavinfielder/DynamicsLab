using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; // for exceptions
using B83.ExpressionParser;
using DynamicsLab.Solvers; //for setup structure
using DynamicsLab.MotionSetup;
using Motion3DDefaultProblem;
using System.IO;

public class Motion3DConfigGui : MonoBehaviour {

    //Gui elements accessors
    private InputField parametersInput;
    private InputField expressionXInput;
    private InputField expressionYInput;
    private InputField expressionZInput;
    private Text variablesDisplay;
    private Text validationStatusDisplay;
    private Text equationLabelX;
    private Text equationLabelY;
    private Text equationLabelZ;
    private Slider orderToggle;
    //Input field changed flags
    private bool parametersChanged;
    private bool expressionXChanged;
    private bool expressionYChanged;
    private bool expressionZChanged;
    //Problem definition data
    private bool[] valid;
    private int order;
    private Expression exprX;
    private Expression exprY;
    private Expression exprZ;
    List<string> variablesAvailable;
    Dictionary<string, double> parameters;
    //Other
    private bool initialized = false;
    
    // Use this for initialization
    void Start ()
    {
        hookObjectReferences();
        parameters = new Dictionary<string, double>();
        valid = new bool[3];
        valid[0] = false;
        valid[1] = false;
        valid[2] = false;
        parametersChanged = false;
        order = 2;
        OnOrderSwitch(); //updates variables list based on order selection
        //flag all for revalidation right away just to clear any weird bugs
        expressionXChanged = true;
        expressionYChanged = true;
        expressionZChanged = true;
        initialized = true;
    }

    //Initialize object references
    public void hookObjectReferences()
    {
        parametersInput = transform.Find("ParametersListInput").gameObject.GetComponent<InputField>();
        expressionXInput = transform.Find("EquationInputX").gameObject.GetComponent<InputField>();
        expressionYInput = transform.Find("EquationInputY").gameObject.GetComponent<InputField>();
        expressionZInput = transform.Find("EquationInputZ").gameObject.GetComponent<InputField>();
        variablesDisplay = transform.Find("VariablesDisplay").gameObject.GetComponent<Text>();
        validationStatusDisplay = transform.Find("ValidationStatusDisplay").gameObject.GetComponent<Text>();
        equationLabelX = transform.Find("EquationLabelX").gameObject.GetComponent<Text>();
        equationLabelY = transform.Find("EquationLabelY").gameObject.GetComponent<Text>();
        equationLabelZ = transform.Find("EquationLabelZ").gameObject.GetComponent<Text>();
        orderToggle = transform.Find("OrderSelectToggle").gameObject.GetComponent<Slider>();

    }
	

	// Update is called once per frame
	void Update () {
        //Check for input changes. Only handle one of these per update cycle.
        if (parametersChanged)
        {
            //Get new parameters list
            List<string> paramsList = SeparateCSV(parametersInput.text);
            parameters = new Dictionary<string, double>();
            for (int i = 0; i < paramsList.Count; i++) {
                parameters.Add(paramsList[i], 0);
            }
            //Flag to re-validate all expressions
            expressionXChanged = true;
            expressionYChanged = true;
            expressionZChanged = true;
            parametersChanged = false;
        }
        else if (expressionXChanged)
        {
            ValidateExpression(1);
            expressionXChanged = false;
        }
        else if (expressionYChanged)
        {
            ValidateExpression(2);
            expressionYChanged = false;
        }
        else if (expressionZChanged)
        {
            ValidateExpression(3);
            expressionZChanged = false;
        }

	}

    //Event handlers
    public void OnParametersChanged()
    {
        parametersChanged = true;
    }
    public void OnExpressionXChanged()
    {
        expressionXChanged = true;
    }
    public void OnExpressionYChanged()
    {
        expressionYChanged = true;
    }
    public void OnExpressionZChanged()
    {
        expressionZChanged = true;
    }
    public void OnOrderSwitch()
    {
        if (orderToggle.value == 1)
        {
            //switch to second order problem
            variablesDisplay.text = "x, x', y, y', z, z', t";
            equationLabelX.text = "x''(t) =";
            equationLabelY.text = "y''(t) =";
            equationLabelZ.text = "z''(t) =";
            order = 2;
        }
        else
        {
            //switch to first order problem
            variablesDisplay.text = "x, y, z, t";
            equationLabelX.text = "x'(t) =";
            equationLabelY.text = "y'(t) =";
            equationLabelZ.text = "z'(t) =";
            order = 1;
        }
        variablesAvailable = SeparateCSV(variablesDisplay.text);
        //Flag to re-validate all expressions
        expressionXChanged = true;
        expressionYChanged = true;
        expressionZChanged = true;
    }


    //Separates a string in CSV format into a list. Ignores math symbols.
    public List<string> SeparateCSV(string text) {
        List<string> list = new List<string>();
        string str = "";
        //Populate list
        for (int i = 0; i < text.Length; i++) {
            switch (text.Substring(i, 1)) {
                case ",": //new symbol to add
                    if (str.Length > 0) {
                        list.Add(str);
                        str = ""; //reset
                    }
                    break;
                //Ignore cases
                case " ":
                    break;
                case "*":
                    break;
                case "-":
                    break;
                case "+":
                    break;
                case "/":
                    break;
                case "^":
                    break;
                case ")":
                    break;
                case "(":
                    break;
                default:
                    //concatenate to current symbol definition
                    str = str + text.Substring(i, 1);
                    break;
            }
        }
        //Get last symbol
        if (str.Length > 0) {
            list.Add(str);
        }
        return list;
    }

    //Used by validation process
    internal class UnknownIdentiferException : System.Exception
    { public UnknownIdentiferException(string aMessage) : base(aMessage) { } }
    internal class EmptyExpressionException : System.Exception
    { public EmptyExpressionException(string aMessage) : base(aMessage) { } }

    //int debug_iter = 0;

    //Attempts to parse an expression. Sets invalidity flags and updates the status display
    //fieldNumber: X=1, Y=2, Z=3
    public void ValidateExpression(int fieldNumber)
    {
        string expr;
        //Set the field we're looking at
        switch (fieldNumber)
        {
            case 1:
                expr = expressionXInput.text;
                //Debug.Log("Validation Field number" + fieldNumber + ". X equation input='" + expr + "'");
                break;
            case 2:
                expr = expressionYInput.text;
                //Debug.Log("Validation Field number" + fieldNumber + ". Y equation input='" + expr + "'");
                break;
            case 3:
                expr = expressionZInput.text;
                //Debug.Log("Validation Field number" + fieldNumber + ". Z equation input='" + expr + "'");
                break;
            default:
                //shouldn't happen
                //Debug.Log("fieldNumber using default.");
                expr = expressionXInput.text;
                break;
        }

        //debug_iter += 1;
        //Debug.Log("Validating: Current expr at (" + debug_iter + "): <" + expr + ">");
        
        //Validate
        try
        {
            //First check for empty expression
            if (expr == "")
            {
                //Debug.Log("Empty string detected. Current expr at (" + debug_iter + "): <" + expr + ">");
                throw new EmptyExpressionException("Please enter all equations.");
            }
            ExpressionParser parser = new ExpressionParser();
            //First, read in the user parameters as constants and give it test values
            int i = 1;
            foreach (KeyValuePair<string, double> p in parameters)
            {
                //each test value should be unique and nonzero to avoid divide by zero
                parser.AddConst(p.Key, () => 0.00013 * (i * 17));
                i++;
            }
            //Next, try to evaluate the expression
            Expression tempExpr = parser.EvaluateExpression(expr);
            //If no ParseException was thrown, expression parsed correctly
            //This is where I'd check for custom functions, if that was handled.
            //Now let's check the parameters of the Expression
            foreach (KeyValuePair<string, Parameter> p in tempExpr.Parameters)
            {
                if (!(variablesAvailable.Contains(p.Key)))
                {
                    throw new UnknownIdentiferException("Unknown identifier '" + p.Key + "'.");
                }
            }
            //Now for an invocation test:
            if (order == 1)
            {
                ExpressionDelegate deleg = tempExpr.ToDelegate("x", "y", "z", "t");
                double[] values = { 0.125778, 0.13456928, 0.2944782, 0.41698793 };
                deleg.Invoke(values);
            }
            else //order == 2
            {
                ExpressionDelegate deleg = tempExpr.ToDelegate("x", "x'", "y", "y'", "z", "z'", "t");
                double[] values = { 0.155926, 0.13562489, 0.2990285, 0.412947352, 1.8308463, 1.0329475, 2.7386352};
                deleg.Invoke(values);
            }
            //If no exception, then expression is validated.
                                //Debug.Log("Expression validated: fieldNumber=" + fieldNumber);
            valid[(fieldNumber - 1)] = true;
            //All good in this expression. Check the other invalid flags
            CheckInvalidFlags();
            UpdateStatusDisplay(validationStatusDisplay.text);
        }
        catch (ExpressionParser.ParseException ex)
        {
            //validation unsuccessful due to parsing error
            valid[(fieldNumber - 1)] = false;
            UpdateStatusDisplay(ex.Message);
        }
        catch (UnknownIdentiferException ex)
        {
            //validation unsuccessful due to unknown identifier
            valid[(fieldNumber - 1)] = false;
            UpdateStatusDisplay(ex.Message);
        }
        catch (EmptyExpressionException ex)
        {
            //validation unsuccessful due to empty expression
            valid[(fieldNumber - 1)] = false;
            UpdateStatusDisplay(ex.Message);
        }
        catch (Exception ex) {
            //validation unsuccessful on invocation test
            valid[(fieldNumber - 1)] = false;
            UpdateStatusDisplay("Unknown error. Check inputs.");
            Debug.Log(ex.Message);
        }
    }

    //Checks the invalid flags and sets the status display and field coloring
    public void UpdateStatusDisplay(string message)
    {
        //Update field coloring
        if (valid[0])
            expressionXInput.textComponent.color = new Color(0f, 0f, 0f, 1f); //black
        else
            expressionXInput.textComponent.color = new Color(0.9f, 0f, 0f, 1f); //red
        if (valid[1])
            expressionYInput.textComponent.color = new Color(0f, 0f, 0f, 1f); //black
        else
            expressionYInput.textComponent.color = new Color(0.9f, 0f, 0f, 1f); //red
        if (valid[2])
            expressionZInput.textComponent.color = new Color(0f, 0f, 0f, 1f); //black
        else
            expressionZInput.textComponent.color = new Color(0.9f, 0f, 0f, 1f); //red
        //Update status display
        if (valid[0] && valid[1] && valid[2]) { 
            validationStatusDisplay.text = "Ok.";
            validationStatusDisplay.color = new Color(0f, 0.7f, 0f, 1f); //green
        }
        else
        {
            validationStatusDisplay.text = message;
            validationStatusDisplay.color = new Color(0.9f, 0f, 0f, 1f); //red
        }
    }

    //Attempts to resolve expression invalid flags. Returns true if all resolved
    public bool CheckInvalidFlags()
    {
        if (!(valid[0]))
        { // if X expression currently marked as invalid
            ValidateExpression(1); // check if resolved
            if (!(valid[0])) return false;
        }
        if (!(valid[1]))
        { // if Y expression currently marked as invalid
            ValidateExpression(2); // check if resolved
            if (!(valid[1])) return false;
        }
        if (!(valid[2]))
        { // if Z expression currently marked as invalid
            ValidateExpression(3); // check if resolved
            if (!(valid[2])) return false;
        }
        return true;
    }
    

    //Exports user inputs on this form to set up a 3D motion problem
    public Motion3DSetup exportForSetup()
    {
        if (valid[0] && valid[1] && valid[2])
        {
            Motion3DSetup setup =
                new Motion3DSetup(expressionXInput.text, expressionYInput.text,
                expressionZInput.text, parameters, order);
            return setup;
        }
        Motion3DSetup empty = null;
        return empty;
    }

    //Imports a Motion3DSetup structure to populate the GUI fields
    public void PopulateFields(Motion3DSetup setup)
    {
        if (!(initialized)) Start(); //TODO: we can avoid needing this by changing the render order I think
        if (setup.Order == 1)
            orderToggle.value = 0;
        else
            orderToggle.value = 1;
        OnOrderSwitch();
        expressionXInput.text = setup.ExpressionX;
        expressionYInput.text = setup.ExpressionY;
        expressionZInput.text = setup.ExpressionZ;
        string paramStr = "";
        foreach (KeyValuePair<string, double> p in setup.Parameters) {
            paramStr += p.Key + ",";
        }
        if (paramStr.Length > 0) //strip off last comma
            paramStr = paramStr.Substring(0, paramStr.Length - 1);
        parametersInput.text = paramStr;
        //Validate all
        ValidateExpression(1);
        ValidateExpression(2);
        ValidateExpression(3);
    }

    //Changes the default problem
    public void ChangeDefaultEquationButton_OnPress()
    {
        PopulateFields(new Motion3DSetup(StrangeProblem.exprX, StrangeProblem.exprY, StrangeProblem.exprZ,
               new Dictionary<string, double>(), StrangeProblem.order));
    }

    public Text field_x;
	public Text field_y;
	public Text field_z;

	//Saves 3 input fields into text file
	public void Save_Equation(){

    	string path = "Assets/Motion3D/SavedProblems/saved_equation.txt";
		System.IO.File.WriteAllText(path,"");
		//File.Delete(path);
    	StreamWriter writer = new StreamWriter(path, true);
    	writer.WriteLine(field_x.text.ToString());
		writer.WriteLine(field_y.text.ToString());
		writer.WriteLine(field_z.text.ToString());
    	writer.Close();

    	return;
    }

    //Loads input fields from text file and overwrites Motion3DSetup with new equations
    public void Load_Equation(){
		string path = "Assets/Motion3D/SavedProblems/saved_equation.txt";
    	StreamReader reader = new StreamReader(path);
    	string x = reader.ReadLine();
		string y = reader.ReadLine();
		string z = reader.ReadLine();

		PopulateFields(new Motion3DSetup(x, y, z,
               new Dictionary<string, double>(), StrangeProblem.order));
		
    	reader.Close();

    	return;
    }

    //TODO: this is dead code
    public void Overwrite_Equations(string x, string y, string z, int order){

		PopulateFields(new Motion3DSetup(x, y, z,
               new Dictionary<string, double>(), order)); 
    }

}
