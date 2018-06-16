using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DynamicsLab.Vector;
//for parsing string and generating fields
using System.Text.RegularExpressions;
using DynamicsLab.MotionSetup;

namespace DynamicsLab.SceneController {
/**
 * This Controller handles interactions with the user. It is the 
 * highest-level Controller for the 3D Motion scene.
 * 
 * The next highest controller is the Simulation Controller, which
 * this class sends commands to. Both Motion3DSceneController and
 * Motion3DSimulationController are components of the 
 * Motion3DController GameObject in the scene. 
 */
public class Motion3DSceneController : MonoBehaviour {

    //GUI menus
    private GameObject configView;
    private GameObject setState1stOrderView;
    private GameObject setState2ndOrderView;
    private GameObject mainMenuView;
    private TimeSlider timeSliderComponent;
    private GameObject userDefinedView;
    private GameObject fileManagerView;

    //VR Mode control fields
    private GameObject playerSwitcher;
    private GameObject NVRPlayer;
    private GameObject GVRPlayer;
    private Camera NVRCamera;
    private Camera GVRCamera;
    private bool vRModeActive;

    //Simulation Controller
    public Motion3DSimulationController simController;

    // Use this for initialization
    public void Start()
    {
        InitializeCameras();
        //Hook GUI menus
        configView = GameObject.Find("Motion3DConfigView");
        setState1stOrderView = GameObject.Find("SetState1stOrderView");
        setState2ndOrderView = GameObject.Find("SetState2ndOrderView");
        mainMenuView = GameObject.Find("MainMenuView");
        timeSliderComponent = GameObject.Find("TimeSlider").GetComponent<TimeSlider>();
        userDefinedView = GameObject.Find("UserDefinedView");
        fileManagerView = GameObject.Find("FileManagerView");
        //Close GUI menus
        configView.SetActive(false);
        setState1stOrderView.SetActive(false);
        setState2ndOrderView.SetActive(false);
        mainMenuView.SetActive(false);
        userDefinedView.SetActive(false);
        fileManagerView.SetActive(false);
        //Hook Simulation Controller
        simController = gameObject.GetComponent<Motion3DSimulationController>();
        //Hook Motion3DObject's static reference to PositionTextModel
        Motion3DObject.positionTextModel = GameObject.Find("PositionTextModel");
        Motion3DObject.positionTextModel.SetActive(false);

        instance = new InputField[100];
        instanceText = new Text[100];
    }

    // Update is called once per frame
    public void Update() {
        //User shortcut keys
        if (Input.GetKeyDown("space")) {
            simController.StartStopToggle();
        }
        if (Input.GetKeyDown("f")) {
            simController.IncreaseSpeed();
        }
        if (Input.GetKeyDown("s")) {
            simController.DecreaseSpeed();
        }
        if (Input.GetKeyDown("w")) {
            simController.SelectNextObject();
        }
        if (Input.GetKeyDown("q")) {
            simController.SelectPrevObject();
        }
        if (Input.GetKeyDown("h")) {
            ToggleVRView();
        }
        if (Input.GetKeyDown("v")) {
            simController.vectorField.ToggleVectorField();
        }
        if (Input.GetKeyDown("c")) {
            simController.vectorField.DecreaseSpacing();
        }
        if (Input.GetKeyDown("b")) {
            simController.vectorField.IncreaseSpacing();
        }
    }
    
    //Normal Gui Event Handlers
    public void MenuBar_MenuButtonPress()
    {
        mainMenuView.SetActive(true);
    }
    public void MenuBar_StartButtonPress()
    {
        simController.StartStopToggle();
    }
    public void MenuBar_ResetButtonPress()
    {
        simController.StopSimulation();
        simController.ResetAll();
    }
    public void MenuBar_ConfigButtonPress()
    {
        //Show the configuration menu with the currently loaded problem
        configView.SetActive(true);
        configView.GetComponent<Motion3DConfigGui>().PopulateFields(simController.GetMotion3DSetup);
    }
    public void MenuBar_SetStateButtonPress()
    {
        simController.StopSimulation();
        //Show the set state menu with the current data
        if (simController.GetMotion3DSetup.Order == 1)
        {
            //Show menu
            setState1stOrderView.SetActive(true);
            //Initialize menu with state of currently selected object
            setState1stOrderView.transform.Find("xInput").gameObject.GetComponent<InputField>().text 
                = simController.GetCurrentState("x").ToString("####0.0###");
            setState1stOrderView.transform.Find("yInput").gameObject.GetComponent<InputField>().text 
                = simController.GetCurrentState("y").ToString("####0.0###");
            setState1stOrderView.transform.Find("zInput").gameObject.GetComponent<InputField>().text 
                = simController.GetCurrentState("z").ToString("####0.0###");
        }
        else
        {
            //Show menu
            setState2ndOrderView.SetActive(true);
            //Initialize menu with state of currently selected object
            setState2ndOrderView.transform.Find("xInput").gameObject.GetComponent<InputField>().text
                = simController.GetCurrentState("x").ToString("####0.0###");
            setState2ndOrderView.transform.Find("yInput").gameObject.GetComponent<InputField>().text
                = simController.GetCurrentState("y").ToString("####0.0###");
            setState2ndOrderView.transform.Find("zInput").gameObject.GetComponent<InputField>().text
                = simController.GetCurrentState("z").ToString("####0.0###");
            setState2ndOrderView.transform.Find("xPrimeInput").gameObject.GetComponent<InputField>().text 
                = simController.GetCurrentState("x'").ToString("####0.0###");
            setState2ndOrderView.transform.Find("yPrimeInput").gameObject.GetComponent<InputField>().text 
                = simController.GetCurrentState("y'").ToString("####0.0###");
            setState2ndOrderView.transform.Find("zPrimeInput").gameObject.GetComponent<InputField>().text 
                = simController.GetCurrentState("z'").ToString("####0.0###");
        }
    }
    public void MenuBar_SaveDataButtonPress()
    {
        simController.SaveData();
    }
    public void MenuBar_SavedProblemsButtonPress()
    {
        fileManagerView.SetActive(true);
    }

    public InputField input;
    public Text inputText;

    public void ConfigView_OkButtonPress()
    {
        Motion3DSetup setup = configView.GetComponent<Motion3DConfigGui>().exportForSetup();
        if (setup != null)
        {
            //Accept new problem definition
            simController.ApplyConfiguration(setup);

            //code for generating input fields for userdefinedview
            for (int i = 0; i < instance.Length; i++)
            {
                if (instance[i] != null)
                {
                    Destroy(instance[i].gameObject);
                }
                if (instanceText[i] != null)
                {
                    Destroy(instanceText[i].gameObject);
                }
            }
            
            string str = configView.transform.Find("ParametersListInput").gameObject.GetComponent<InputField>().text;

            Regex rgx = new Regex("[a-zA-Z]+");

            foreach (Match match in rgx.Matches(str))
            {
                if (match.Success)
                {
                    if (match.Value == "x" || match.Value == "y" || match.Value == "z")
                        Debug.Log("error reserved variable used");
                    else
                    {
                        Debug.Log(match.Value);
                        instanceText[count] = Instantiate(inputText, new Vector3(0, 0, 0), transform.rotation, userDefinedView.transform);
                        instanceText[count].text = match.Value + " = ";
                        instance[count] = Instantiate(input, new Vector3(0, 0, 0), transform.rotation, userDefinedView.transform);
                        instance[count].name = match.Value;
                        instance[count].GetComponent<InputField>().text = "1";

                        count++;
                    }
                }
                else
                    Debug.Log("no match found");
            }
            //end code for generating fields
            //Close config gui
            configView.SetActive(false);
        }
    }
    public void ConfigView_CancelButtonPress()
    {
        //close GUI
        configView.SetActive(false);
    }
    public void SetState1stOrderView_OkButtonPress()
    {
        //Parse user input
        double x, y, z;
        string xstr = setState1stOrderView.transform.Find("xInput").gameObject.GetComponent<InputField>().text;
        string ystr = setState1stOrderView.transform.Find("yInput").gameObject.GetComponent<InputField>().text;
        string zstr = setState1stOrderView.transform.Find("zInput").gameObject.GetComponent<InputField>().text;
        if (!(double.TryParse(xstr, out x))) return;
        if (!(double.TryParse(ystr, out y))) return;
        if (!(double.TryParse(zstr, out z))) return;
        //All valid, now set state of currently selected object
        simController.SetStateOnCurrentObject(new VectorND(x, y, z));
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
        double x, y, z, xprime, yprime, zprime;
        string xstr = setState2ndOrderView.transform.Find("xInput").gameObject.GetComponent<InputField>().text;
        string ystr = setState2ndOrderView.transform.Find("yInput").gameObject.GetComponent<InputField>().text;
        string zstr = setState2ndOrderView.transform.Find("zInput").gameObject.GetComponent<InputField>().text;
        string xprimestr = setState2ndOrderView.transform.Find("xPrimeInput").gameObject.GetComponent<InputField>().text;
        string yprimestr = setState2ndOrderView.transform.Find("yPrimeInput").gameObject.GetComponent<InputField>().text;
        string zprimestr = setState2ndOrderView.transform.Find("zPrimeInput").gameObject.GetComponent<InputField>().text;
        if (!(double.TryParse(xstr, out x))) return;
        if (!(double.TryParse(ystr, out y))) return;
        if (!(double.TryParse(zstr, out z))) return;
        if (!(double.TryParse(xprimestr, out xprime))) return;
        if (!(double.TryParse(yprimestr, out yprime))) return;
        if (!(double.TryParse(zprimestr, out zprime))) return;
        //All valid, now set state of currently selected object
        simController.SetStateOnCurrentObject(new VectorND(x, xprime, y, yprime, z, zprime));
        //Close GUI
        setState2ndOrderView.SetActive(false);
    }
    public void SetState2ndOrderView_CancelButtonPress()
    {
        //Close GUI
        setState2ndOrderView.SetActive(false);
    }
    int count = 0;
    InputField[] instance;
    Text[] instanceText;
    public void MenuBar_UserDefinedButtonPress()
    {
        if (userDefinedView.activeSelf == false)
        {
            userDefinedView.SetActive(true);
        }      
        else
        {
            count = 0;
            userDefinedView.SetActive(false);
        }
    }
    /*
     *@brief This is a button that when pressed parses the string with a regular expression
     *       then fills a dictionary with input from the matched expressions as the keys and
     *       InputFields as the values.
     */

    public void UserDefinedOKButtonPress()
    {
        double generated_input_value;
        for (int i = 0; i < instance.Length; i++)
        {
            if (instance[i] != null)
            {
                Debug.Log(instance[i].GetComponent<InputField>().name);
                if (!(double.TryParse(instance[i].GetComponent<InputField>().text, out generated_input_value))) return;
                simController.ChangeParameter(instance[i].GetComponent<InputField>().name, generated_input_value);
            }
        }
    }
    public void MenuBarBottom_AddObjectButtonPress()
    {
        simController.AddObject();
    }
    public void MenuBarBottom_RemoveObjectButtonPress()
    {
        simController.RemoveObject();
    }
    public void MenuBarBottom_NextObjectButtonPress()
    {
        simController.SelectNextObject();
    }
    public void MenuBarBottom_PrevObjectButtonPress()
    {
        simController.SelectPrevObject();
    }
    public void TimeSlider_OnValueChanged()
    {
        //TODO: This is called when the value is changed in code too,
        //resulting in excessive calls. Is there another way to set this up
        //so that this function is called only when the user changes the value?
        //Debug.Log("Time Slider changed."); 
        simController.CurrentTime = timeSliderComponent.Value; //set the new time in the sim controller
        //simController.ResetAllTrails(); //Can't do this while the above bug exists
    }

    public void FileManager_LoadButtonPress()
    {
        Motion3DFileManager fileManager = fileManagerView.GetComponent<Motion3DFileManager>();
        Motion3DProblemIOManager input = new Motion3DProblemIOManager();
        //Load the data
        input.Load(fileManager.selectedFilename);
        //Apply the new problem configuration
        input.Apply(simController);
        //Close the file manager menu
        fileManagerView.SetActive(false);
    }
    public void FileManager_SaveButtonPress()
    {
        //TODO
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
        NVRCamera = NVRPlayer.GetComponentInChildren<Camera>();
        GVRCamera = GVRPlayer.GetComponentInChildren<Camera>();

        //Initial state is VR disabled
        VROff();

    }

}

}