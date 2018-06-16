using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
//using UnityEngine.Assertions;
using NUnit.Framework;
using System;
using System.Collections;
using DynamicsLab.SceneController;
using DynamicsLab.Vector;
using DynamicsLab.Solvers;
using DynamicsLab.VectorField;

public class MotionVectorTests {

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


    [Test]
    public void SanityCheck()
    {
		try{
			GameObject control = new GameObject();
			control.AddComponent<Motion3DVectorField>();
			Motion3DVectorField sim = control.GetComponent<Motion3DVectorField>();
			sim.Start();
		}
		catch(Exception e){
			Debug.LogAssertion(e);
			Assert.IsTrue(false);
		}
    }

	[Test]
	public void InitializeVectorField()
    {
		try{
			GameObject control = new GameObject();
			control.AddComponent<Motion3DVectorField>();
			Motion3DVectorField sim = control.GetComponent<Motion3DVectorField>();
			sim.Start();
			sim.InitializeVectorField();
		}
		catch(Exception e){
			Debug.LogAssertion(e);
			Assert.IsTrue(false);
		}
    }

	[Test]
	public void Update()
    {
		try{
			GameObject control = new GameObject();
			control.AddComponent<Motion3DVectorField>();
			Motion3DVectorField sim = control.GetComponent<Motion3DVectorField>();
			sim.Start();
			sim.InitializeVectorField();
			sim.Update();
		}
		catch(Exception e){
			Debug.LogAssertion(e);
			Assert.IsTrue(false);
		}
    }
	[Test]
	public void UpdateWithVectorField1()
    {
		try{
			GameObject control = new GameObject();
			control.AddComponent<Motion3DVectorField>();
			Motion3DVectorField sim = control.GetComponent<Motion3DVectorField>();
			sim.Start();
			sim.InitializeVectorField();
			sim.vectorField = 1;
			sim.Update();
		}
		catch(Exception e){
			Debug.LogAssertion(e);
			Assert.IsTrue(false);
		}
    }

	[Test]
	public void ToggleVectorField()
    {
		GameObject control = new GameObject();
		control.AddComponent<Motion3DVectorField>();
		Motion3DVectorField sim = control.GetComponent<Motion3DVectorField>();
		sim.Start();
		sim.InitializeVectorField();
		sim.Update();

		ToggleVectorField();
		Assert.AreEqual(sim.vectorField, 1);

    }

	[Test]
	public void IncreaseSpacing()
    {
		GameObject control = new GameObject();
		control.AddComponent<Motion3DVectorField>();
		Motion3DVectorField sim = control.GetComponent<Motion3DVectorField>();
		sim.Start();
		sim.InitializeVectorField();
		sim.Update();

		sim.IncreaseSpacing();

		Assert.IsFalse(Mathf.Approximately(sim.vectorField, 3f));

    }

	[Test]
	public void DecreaseSpacing()
    {
		GameObject control = new GameObject();
		control.AddComponent<Motion3DVectorField>();
		Motion3DVectorField sim = control.GetComponent<Motion3DVectorField>();
		sim.Start();
		sim.InitializeVectorField();
		sim.Update();

		sim.DecreaseSpacing();
		Assert.IsFalse(Mathf.Approximately(sim.vectorField, 3f));

    }


}
