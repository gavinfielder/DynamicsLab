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
using DynamicsLab.MotionSetup;

public class MotionSetupTests {

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
    public void LoadFileNameCheck()
    {
		Motion3DProblemIOManager sim = new Motion3DProblemIOManager();
		sim.Load("rosslerAttractor.3dmotion");
		Assert.AreEqual("Rossler Attractor", sim.name);
    }


	[Test]
    public void LoadFileDescCheck()
    {
		Motion3DProblemIOManager sim = new Motion3DProblemIOManager();
		sim.Load("rosslerAttractor.3dmotion");
		Assert.AreEqual("The Rossler strange attractor", sim.description);
    }

	[Test]
    public void LoadFileXCheck()
    {
		Motion3DProblemIOManager sim = new Motion3DProblemIOManager();
		sim.Load("rosslerAttractor.3dmotion");
		Assert.AreEqual("-(y+z)", sim.expressionX);
    }

	[Test]
    public void LoadFileYCheck()
    {
		Motion3DProblemIOManager sim = new Motion3DProblemIOManager();
		sim.Load("rosslerAttractor.3dmotion");
		Assert.AreEqual("x+A*y", sim.expressionY);
		//Assert.AreEqual("B+x*z-C*z", sim.expressionZ);
    }

	[Test]
    public void LoadFileZCheck()
    {
		Motion3DProblemIOManager sim = new Motion3DProblemIOManager();
		sim.Load("rosslerAttractor.3dmotion");
		Assert.AreEqual("B+x*z-C*z", sim.expressionZ);
    }
















	[Test]
    public void ConstructorWithParse1stOrder()
    {
    	try {
			Motion3DProblemIOManager sim = new Motion3DProblemIOManager();
			sim.Load("rosslerAttractor.3dmotion");

			Motion3DSetup ms =
				new Motion3DSetup(
					sim.expressionX, sim.expressionY, sim.expressionZ,
					sim.parameters, sim.order
				);

			
		}
		catch(Exception e){
			Debug.LogAssertion(e);
			Assert.IsTrue(false);
		}
    }

	[Test]
    public void ConstructorWithParse1stOrder_CheckX()
    {
		Motion3DProblemIOManager sim = new Motion3DProblemIOManager();
		sim.Load("rosslerAttractor.3dmotion");

		Motion3DSetup ms =
			new Motion3DSetup(
				sim.expressionX, sim.expressionY, sim.expressionZ,
				sim.parameters, sim.order
			);

		Assert.AreEqual(ms.ExpressionX, "-(y+z)");
    }

	[Test]
    public void ConstructorWithParse1stOrder_CheckY()
    {
		Motion3DProblemIOManager sim = new Motion3DProblemIOManager();
		sim.Load("rosslerAttractor.3dmotion");

		Motion3DSetup ms =
			new Motion3DSetup(
				sim.expressionX, sim.expressionY, sim.expressionZ,
				sim.parameters, sim.order
			);

		Assert.AreEqual(ms.ExpressionY, "x+A*y");
    }

	[Test]
    public void ConstructorWithParse1stOrder_CheckZ()
    {
		Motion3DProblemIOManager sim = new Motion3DProblemIOManager();
		sim.Load("rosslerAttractor.3dmotion");

		Motion3DSetup ms =
			new Motion3DSetup(
				sim.expressionX, sim.expressionY, sim.expressionZ,
				sim.parameters, sim.order
			);

		Assert.AreEqual(ms.ExpressionZ, "B+x*z-C*z");
    }

	[Test]
    public void ParseWith2ndOrder()
    {
    	try {
			Motion3DProblemIOManager sim = new Motion3DProblemIOManager();
			sim.Load("rosslerAttractor.3dmotion");

			Motion3DSetup ms =
				new Motion3DSetup(
					sim.expressionX, sim.expressionY, sim.expressionZ,
					sim.parameters, 2
				);
		}
		catch(Exception e){
			Debug.LogAssertion(e);
			Assert.IsTrue(false);
		}
    }


	[Test]
    public void ChangeParameterX()
    {
		Motion3DProblemIOManager sim = new Motion3DProblemIOManager();
		sim.Load("rosslerAttractor.3dmotion");

		Motion3DSetup ms =
			new Motion3DSetup(
				sim.expressionX, sim.expressionY, sim.expressionZ,
				sim.parameters, sim.order
			);

		ms.ChangeParameter("expressionX", 3);
		Assert.AreNotEqual(ms.ExpressionX, 3);

    }


}
