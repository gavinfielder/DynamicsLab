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

public class MotionTests {

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
			control.AddComponent<Motion3DSceneController>();
			Motion3DSceneController sim = control.GetComponent<Motion3DSceneController>();
			sim.Start();
		}
		catch(Exception e){
			Debug.LogAssertion(e);
			Assert.IsTrue(false);
		}
    }


    [Test]
    public void Update()
    {
		GameObject control = new GameObject();
		control.AddComponent<Motion3DSceneController>();
		Motion3DSceneController sim = control.GetComponent<Motion3DSceneController>();
		try{
			sim.Update();
		}
		catch(Exception e){
			Debug.LogAssertion(e);
			Assert.IsTrue(false);
		}

    }


	[Test]
    public void IncreaseSpeed()
    {
		GameObject control = new GameObject();
		control.AddComponent<Motion3DSceneController>();
		Motion3DSceneController sim = control.GetComponent<Motion3DSceneController>();

		sim.simController.IncreaseSpeed();

		Assert.AreNotEqual(1f, sim.simController.speed);

    }


}
