using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicsLab.CreateLines;

public class viewModel : MonoBehaviour {

    GameObject LM;
    GameObject MSC;

    public void get_speed(string newText)
    {
        MSC = GameObject.Find("Motion3DSceneController");
        //MSC.GetComponent<Motion3DSceneController>().speed = float.Parse(newText); 
            //Gavin: Spring-Mass shouldn't be accessing the controller for 3D Motion
    }

    public void get_friction(string newText)
    {
        //get object
        LM = GameObject.Find("LineMaster");

        //gets script attached to LM
        LM.GetComponent<CreateLines>().friction = float.Parse(newText);

        Debug.Log(float.Parse(newText));
    }

    public void get_mass(string newText)
    {
        LM = GameObject.Find("LineMaster");
        LM.GetComponent<CreateLines>().masss = float.Parse(newText);
    }

    public void get_stiffness(string newText)
    {
        LM = GameObject.Find("LineMaster");
        LM.GetComponent<CreateLines>().stiffness = float.Parse(newText);
    }
}
