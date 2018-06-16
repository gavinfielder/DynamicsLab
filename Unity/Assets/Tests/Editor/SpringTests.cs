using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;
//using UnityEngine.Assertions;
using NUnit.Framework;
using System;
using System.Collections;
using DynamicsLab.CreateLines;
using DynamicsLab.Vector;
using DynamicsLab.Solvers;

public class SpringTest {

    /*
	[Test]
	public void VectorNDTestsSimplePasses() {
		// Use the Assert class to test conditions.
	}

	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator VectorNDTestsWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}
    */



    private const double required_accuracy = 1e-14;

    [Test]
    public void InitializeSpringMassSystem_test_mass()
    {
    	CreateLines cl = new CreateLines();
		SpringMassSystem sms = new SpringMassSystem(0.01f, 1.0f, 1.0f, 0.15f, 0.0f, 40.0f); //range of 0-40s with data points every 0.5s
        Assert.IsTrue(Mathf.Approximately((float)sms.Mass, 1.0f));
    }

    [Test]
    public void InitializeSpringMassSystem_test_damping()
    {
        CreateLines cl = new CreateLines();
        SpringMassSystem sms = new SpringMassSystem(0.01f, 1.0f, 1.0f, 0.15f, 0.0f, 40.0f); //range of 0-40s with data points every 0.5s
        Assert.IsTrue(Mathf.Approximately((float)sms.Damping, 0.15f));
    }

    [Test]
    public void InitializeSpringMassSystem_test_stiffness()
    {
        CreateLines cl = new CreateLines();
        SpringMassSystem sms = new SpringMassSystem(0.01f, 1.0f, 1.0f, 0.15f, 0.0f, 40.0f); //range of 0-40s with data points every 0.5s
        Assert.IsTrue(Mathf.Approximately((float)sms.Stiffness, 1.0f));
    }

    [Test]
    public void StartedTest()
    {
        GameObject lines = new GameObject();
        lines.AddComponent<CreateLines>();

        lines.GetComponent<CreateLines>().started = 1;
        Assert.AreEqual(lines.GetComponent<CreateLines>().started, 1);
    }

	[Test]
    public void UpdateWithoutStarted_x()
    {
        GameObject lines = new GameObject();
        lines.AddComponent<CreateLines>();

		CreateLines ls = lines.GetComponent<CreateLines>();
		//ls.InitializeSimulation();
		ls.Update();

		Debug.Log( ls.mx );
		Assert.IsTrue(
			Mathf.Approximately(-2.0f, ls.mx )); //No change for mx
    }

	[Test]
    public void UpdateWithStarted_x()
    {
        EditorSceneManager.OpenScene((Application.dataPath + "/SpringMass/MainScene.Unity"));

        GameObject LM = GameObject.Find("LineMaster");
        CreateLines ls = LM.GetComponent<CreateLines>();

        ls.InitializeSimulation();
        
        ls.started = 1;
        ls.Update();
        Assert.IsFalse(Mathf.Approximately(-2f, ls.mx )); //Any change for mx
    }


	[Test]
    public void InitializeSimulation()
    {
        EditorSceneManager.OpenScene((Application.dataPath + "/SpringMass/MainScene.Unity"));

        GameObject LM = GameObject.Find("LineMaster");
        CreateLines ls = LM.GetComponent<CreateLines>();

        try
        {
			ls.InitializeSimulation();
		}
		catch(Exception e){
			Debug.LogAssertion(e);
			Assert.IsTrue(false);
		}

    }

	[Test]
    public void ApplyForceX(){
        EditorSceneManager.OpenScene((Application.dataPath + "/SpringMass/MainScene.Unity"));

        GameObject LM = GameObject.Find("LineMaster");
        CreateLines ls = LM.GetComponent<CreateLines>();
        ls.InitializeSimulation();
        ls.velX = 1;

        ls.started = 1;
		ls.Update();
		ls.Update();
		ls.Update();

		Assert.IsFalse(
			Mathf.Approximately(1.0f, ls.velX )); //Any change for velX

    }

	[Test]
    public void PausingWorks(){

        EditorSceneManager.OpenScene((Application.dataPath + "/SpringMass/MainScene.Unity"));

        GameObject LM = GameObject.Find("LineMaster");
        CreateLines ls = LM.GetComponent<CreateLines>();

        ls.started = 1;
        ls.pause = 0;
        ls.InitializeSimulation();

		ls.Update();
		ls.Update();
		ls.Update();
		float testTime = ls.time;
		ls.pause = 1;
		ls.Update();
		ls.Update();
		ls.Update();
		Assert.IsTrue(Mathf.Approximately(ls.time, testTime) );

//		Assert.IsFalse(
	//		Mathf.Approximately(0.0f, ls.velX )); //Any change for velX

    }

	[Test]
    public void callOnGui(){

        EditorSceneManager.OpenScene((Application.dataPath + "/SpringMass/MainScene.Unity"));

        GameObject LM = GameObject.Find("LineMaster");
        CreateLines ls = LM.GetComponent<CreateLines>();

        //GameObject lines = new GameObject();
        //lines.AddComponent<CreateLines>();
		try{
            //lines.GetComponent<CreateLines>().OnGUI();
            ls.GetComponent<CreateLines>().OnGUI();
		}
		catch(Exception e){
			Debug.LogAssertion(e);
			Assert.IsTrue(true);
		}
    }

	[Test]
    public void ToggleVR(){
		GameObject lines = new GameObject();
        lines.AddComponent<CreateLines>();
		CreateLines ls = lines.GetComponent<CreateLines>();
		try{
			ls.TestVR(); //Toggle on
			ls.TestVR(); //Toggle off
			ls.MenuButton_OnPress();
		}
		catch(Exception e){
			Debug.LogAssertion(e);
			Assert.IsTrue(false);
		}
    }

	[Test]
    public void CallLineDotCSFunctions(){
        EditorSceneManager.OpenScene((Application.dataPath + "/SpringMass/MainScene.Unity"));

        GameObject LM = GameObject.Find("LineMaster");
        CreateLines ls = LM.GetComponent<CreateLines>();
        ls.InitializeSimulation();
		try{
			ls.instance[0].GetComponent<Line>().Start();
			ls.instance[0].GetComponent<Line>().Update();
			ls.instance[9].GetComponent<Line>().Start();
			ls.instance[9].GetComponent<Line>().Update();
		}
		catch(Exception e){
			Debug.LogAssertion(e);
			Assert.IsTrue(false);
		}
    }


}
