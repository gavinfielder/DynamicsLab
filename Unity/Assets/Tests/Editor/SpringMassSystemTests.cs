using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using DynamicsLab.Solvers;


public class SpringMassSystemTests {



    private const double general_purpose_required_accuracy = 1e-10;
    private const double single_value_required_accuracy = 1e-14;


    [Test]
	public void ConstructAndReadStiffness() {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Read parameters
        Assert.AreEqual(0, sms.Stiffness, single_value_required_accuracy);
    }

    [Test]
    public void ConstructAndReadDamping()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Read parameters
        Assert.AreEqual(0, sms.Damping, single_value_required_accuracy);
    }

    [Test]
    public void ConstructAndReadMass()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Read parameters
        Assert.AreEqual(1, sms.Mass, single_value_required_accuracy);
    }

    [Test]
    public void ConstructAndReadForwardSolveTime()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Read parameters
        Assert.AreEqual(0.1, sms.ForwardSolveTime, single_value_required_accuracy);
    }

    [Test]
    public void SetStiffness()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Change parameter
        sms.Stiffness = 5;
        //Read parameters
        Assert.AreEqual(5, sms.Stiffness, single_value_required_accuracy);
    }

    [Test]
    public void SetDamping()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Change parameter
        sms.Damping = 5;
        //Read parameters
        Assert.AreEqual(5, sms.Damping, single_value_required_accuracy);
    }

    [Test]
    public void SetMass()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Change parameter
        sms.Mass = 5;
        //Read parameters
        Assert.AreEqual(5, sms.Mass);
    }

    //Gavin: I don't believe this behavior is implemented, so I expect this to fail
    [Test]
    public void SetMass_doesNotSetToZero()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 7, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Change parameter
        sms.Mass = 0;
        //Read parameters
        Assert.AreEqual(7, sms.Mass);
    }

    //Gavin: I don't believe this behavior is implemented, so I expect this to fail
    [Test]
    public void Constructor_doesNotSetToZero()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 0, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Read parameters
        Assert.AreNotEqual(0, sms.Mass);
    }

    [Test]
    public void SetForwardSolveTime()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Change parameter
        sms.ForwardSolveTime = 5;
        //Read parameters
        Assert.AreEqual(5, sms.ForwardSolveTime, single_value_required_accuracy);
    }

    [Test]
    public void Update_andCheckPosition()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Update to t=1
        sms.Update(1);
        //check position at t=1
        Assert.AreEqual(1, sms.Position(1), general_purpose_required_accuracy);
    }

    [Test]
    public void Update_andCheckVelocity0()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Update to t=1
        sms.Update(1);
        //check velocity at t=1
        Assert.AreEqual(0, sms.Velocity(1), general_purpose_required_accuracy);
    }

    [Test]
    public void Update_andCheckVelocity1()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 1); //should travel linearly in time
        //Update to t=1
        sms.Update(1);
        //check velocity at t=1
        Assert.AreEqual(1, sms.Velocity(1), general_purpose_required_accuracy);
    }

    [Test]
    public void Update_andCheckPosition_afterMoving()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 1); //should travel linearly in time
        //Update to t=1
        sms.Update(1);
        //check velocity at t=1. 
        Assert.AreEqual(2, sms.Position(1), general_purpose_required_accuracy);
    }


    [Test]
    public void ForwardSolveTime_viaConstructor()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.2);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Update to t=1
        sms.Update(1);
        //check velocity at t=1.2
        Assert.AreEqual(1, sms.Position(1.2), general_purpose_required_accuracy);
    }

    [Test]
    public void ForwardSolveTime_viaSet()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        sms.ForwardSolveTime = 0.2;
        //Update to t=1
        sms.Update(1);
        //check velocity at t=1.2
        Assert.AreEqual(1, sms.Position(1.2), general_purpose_required_accuracy);
    }


    [Test]
    public void ForwardSolveTime_zero()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        sms.ForwardSolveTime = 0;
        //Update to t=1
        sms.Update(1);
        //check velocity at t=1
        Assert.AreEqual(1, sms.Position(1), general_purpose_required_accuracy);
    }

    [Test]
    public void ForwardSolveTime_negative()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        sms.ForwardSolveTime = -0.5;
        //Update to t=1
        sms.Update(1);
        //check velocity at t=1
        Assert.AreEqual(1, sms.Position(1), general_purpose_required_accuracy);
    }

    [Test]
    public void Update_reverse()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Update to t=-1
        sms.Update(-1);
        //check position at t=0
        Assert.AreEqual(1, sms.Position(0), general_purpose_required_accuracy);
    }

    //Gavin: this behagvior not implemented. expect to fail.
    [Test]
    public void ReadPosition_OutOfBounds()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Update to t=1
        sms.Update(1);
        //check position at t=2
        Assert.IsNaN(sms.Position(2));
    }

    //Gavin: this behagvior not implemented. expect to fail.
    [Test]
    public void ReadVelocity_OutOfBounds()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Update to t=1
        sms.Update(1);
        //check velocity at t=2
        Assert.IsNaN(sms.Velocity(2));
    }

    //Gavin: this behagvior not implemented. expect to fail.
    [Test]
    public void ReadPosition_OutOfBoundsNegative()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Update to t=1
        sms.Update(1);
        //check position at t=-2
        Assert.IsNaN(sms.Position(-2));
    }

    //Gavin: this behagvior not implemented. expect to fail.
    [Test]
    public void ReadVelocity_OutOfBoundsNegative()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(0, 1, 0); //should sit at x=1
        //Update to t=1
        sms.Update(1);
        //check velocity at t=-2
        Assert.IsNaN(sms.Velocity(-2));
    }

    //Gavin: this behagvior not implemented. expect to fail.
    [Test]
    public void SetInitialCondition_timeOutOfBounds()
    {
        //Create a SpringMassSystem with zero damping and stiffness.
        //This should produce solution of constant velocity
        SpringMassSystem sms = new SpringMassSystem(0.1, 0, 1, 0, 0, 0.1);
        sms.SetInitialCondition(-1, 1, 0); //should sit at x=1
        //check velocity at t=0
        Assert.IsNaN(sms.Velocity(0));
    }


}
