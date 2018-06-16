using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using DynamicsLab.Vector;
using DynamicsLab.Solvers;
using B83.ExpressionParser;
using System;
using System.IO;

public class ODEInitialValueProblemTests {
    
    public const double solution_total_error_benchmark = 1e-6;
    public const double single_value_required_accuracy = 1e-12;
    public const double general_purpose_required_accuracy = 1e-10;

    //Helper function which tests if two vectors are equal
    private bool VectorsAreEqual(VectorND v1, VectorND v2)
    {
        const double vectorEqualityRequiredAccuracy = 1e-12;
        if (v1.GetDim() != v2.GetDim())
            return false;
        for (byte i = 0; i < v1.GetDim(); i++)
        {
            if (Math.Abs(v1[i] - v2[i]) > vectorEqualityRequiredAccuracy)
                return false;
        }
        return true;
    }

    [Test]
    public void DataLowerBound_init0()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Check the lower bound
        Assert.AreEqual(0, ivp.GetDataLowerBound(), general_purpose_required_accuracy);
    }

    [Test]
    public void DataLowerBound_init0butSet5()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 5);
        //Solve the problem
        ivp.SolveTo(8);
        //Check the lower bound
        Assert.AreEqual(5, ivp.GetDataLowerBound(), general_purpose_required_accuracy);
    }

    [Test]
    public void DataLowerBound_init5()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 5);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 5);
        //Solve the problem
        ivp.SolveTo(8);
        //Check the lower bound
        Assert.AreEqual(5, ivp.GetDataLowerBound(), general_purpose_required_accuracy);
    }


    [Test]
    public void DataUpperBoundSimpleTest()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Check the upper bound
        Assert.GreaterOrEqual(ivp.GetDataUpperBound(), 3);
    }




    [Test]
    public void InvalidateData_checkLowerBound()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Invalidate the data at t=10
        ivp.InvalidateData(2);
        //Check the lower bound
        Assert.AreEqual(ivp.GetDataLowerBound(), 2, general_purpose_required_accuracy);
    }

    [Test]
    public void InvalidateData_checkUpperBound()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Invalidate the data at t=10
        ivp.InvalidateData(2);
        //Check the upper bound
        Assert.AreEqual(ivp.GetDataUpperBound(), 2, general_purpose_required_accuracy);
    }

    //TODO: this is the wrong behavior! This test needs to be rewritten
    [Test]
    public void InvalidateData_OutOfBounds()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Invalidate the data at t=25 where there is no data
        ivp.InvalidateData(25);
        //Check the valid data interval to see if it is unchanged
        Assert.AreEqual(ivp.GetDataLowerBound(), 0, general_purpose_required_accuracy);
        Assert.AreEqual(ivp.GetDataUpperBound(), 3, general_purpose_required_accuracy);
    }

    //TODO: this is the wrong behavior! This test needs to be rewritten
    [Test]
    public void InvalidateData_OutOfBoundsNegative()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Invalidate the data at t=25 where there is no data
        ivp.InvalidateData(-1);
        //Check the valid data interval to see if it is unchanged
        Assert.AreEqual(ivp.GetDataLowerBound(), 0, general_purpose_required_accuracy);
        Assert.AreEqual(ivp.GetDataUpperBound(), 3, general_purpose_required_accuracy);
    }

    [Test]
    public void InvalidateData_Twice_CheckUpperBound()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Invalidate the data at t=1
        ivp.InvalidateData(1);
        //Now solve to 5
        ivp.SolveTo(5);
        //Invalidate data at t=0.5
        ivp.InvalidateData(0.5);
        //Check the upper bound
        Assert.AreEqual(ivp.GetDataUpperBound(), 5, general_purpose_required_accuracy);
    }

    [Test]
    public void InvalidateData_Twice_CheckLowerBound()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Invalidate the data at t=1
        ivp.InvalidateData(1);
        //Now solve to 5
        ivp.SolveTo(5);
        //Invalidate data at t=0.5
        ivp.InvalidateData(0.5);
        //Check the upper bound
        Assert.AreEqual(ivp.GetDataUpperBound(), 0.5, general_purpose_required_accuracy);
    }

    [Test]
    public void InvalidateData_OutOfBoundsOfDataSet_CheckLowerBound()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Invalidate the data at t=2000
        ivp.InvalidateData(2000);
        //Check the valid data interval
        Assert.AreEqual(ivp.GetDataLowerBound(), 2000, general_purpose_required_accuracy);
    }

    [Test]
    public void InvalidateData_OutOfBoundsOfDataSet_CheckUpperBound()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Invalidate the data at t=2000
        ivp.InvalidateData(2000);
        //Check the valid data interval
        Assert.AreEqual(ivp.GetDataUpperBound(), 2000, general_purpose_required_accuracy);
    }
    


    [Test]
    public void GetState_initialState()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Check the solution data at t=0
        Assert.IsTrue(VectorsAreEqual(ivp.GetState(0), new VectorND(3, 4, 5)));
    }

    [Test]
    public void GetState_middle()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Check the solution data at t=10
        Assert.IsTrue(VectorsAreEqual(ivp.GetState(2), new VectorND(3, 4, 5)));
    }

    [Test]
    public void GetState_atUpperBound()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Check the solution data at t=20
        Assert.IsTrue(VectorsAreEqual(ivp.GetState(3), new VectorND(3, 4, 5)));
    }

    //Gavin: This behavior is not implemented, so I expect this to fail
    [Test]
    public void GetState_OutOfBounds()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Check the solution data at t=25 where there is no data
        Assert.IsNull(ivp.GetState(25));
    }

    //Gavin: This behavior is not implemented, so I expect this to fail
    [Test]
    public void GetState_OutOfBoundsNegative()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Check the solution data at t=-1 where there is no data
        Assert.IsNull(ivp.GetState(-1));
    }

    [Test]
    public void SolveTo_backwards()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem to -1. Note this should not be possible.
        ivp.SolveTo(-1);
        //Check the interval to see if reads [0,0]
        Assert.AreEqual(0, ivp.GetDataLowerBound(), general_purpose_required_accuracy);
        Assert.AreEqual(0, ivp.GetDataUpperBound(), general_purpose_required_accuracy);
    }


    [Test]
    public void SolveTo_noMoreThanNecessary()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem to t=3
        ivp.SolveTo(3);
        Assert.AreEqual(3, ivp.GetDataUpperBound(), general_purpose_required_accuracy);
    }


    [Test]
    public void SetState_ThenReadState()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //See if it set the initial state correctly
        Assert.IsTrue(VectorsAreEqual(ivp.GetState(0), new VectorND(3, 4, 5)));
    }

    [Test]
    public void SetState_ThenReadState_atNonzero()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 5);
        //See if it set the initial state correctly
        Assert.IsTrue(VectorsAreEqual(ivp.GetState(5), new VectorND(3, 4, 5)));
    }

    [Test]
    public void SetState_ThenReadState_atNonzero_Imprecise()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 4.963);
        //See if it set the initial state correctly
        Assert.IsTrue(VectorsAreEqual(ivp.GetState(5.071), new VectorND(3, 4, 5)));
    }


    [Test]
    public void SetState_AfterSolving_RedefinesInitial()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 4.963);        
        //Solve the problem to t=3
        ivp.SolveTo(3);
        //Set the state 
        ivp.SetState(new VectorND(7, 6, 5), 0);
        //See if it set the initial state correctly
        Assert.IsTrue(VectorsAreEqual(ivp.GetState(0), new VectorND(7, 6, 5)));
    }

    [Test]
    public void SetState_AfterSolving_MiddleOfInterval()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 4.963);
        //Solve the problem to t=3
        ivp.SolveTo(3);
        //Set the state 
        ivp.SetState(new VectorND(7, 6, 5), 1);
        //See if it set the initial state correctly
        Assert.IsTrue(VectorsAreEqual(ivp.GetState(1), new VectorND(7, 6, 5)));
    }

    [Test]
    public void SetState_AfterSolving_EndOfInterval()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 4.963);
        //Solve the problem to t=3
        ivp.SolveTo(3);
        //Set the state 
        ivp.SetState(new VectorND(7, 6, 5), 3);
        //See if it set the initial state correctly
        Assert.IsTrue(VectorsAreEqual(ivp.GetState(3), new VectorND(7, 6, 5)));
    }

    [Test]
    public void SetState_AfterSolving_CheckLowerBound()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 4.963);
        //Solve the problem to t=3
        ivp.SolveTo(3);
        //Set the state 
        ivp.SetState(new VectorND(7, 6, 5), 1);
        //See if it set the initial state correctly
        Assert.AreEqual(1, ivp.GetDataLowerBound(), general_purpose_required_accuracy);
    }


    [Test]
    public void SetState_AfterSolving_CheckUpperBound()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 4.963);
        //Solve the problem to t=3
        ivp.SolveTo(3);
        //Set the state 
        ivp.SetState(new VectorND(7, 6, 5), 1);
        //See if it set the initial state correctly
        Assert.AreEqual(1, ivp.GetDataUpperBound(), general_purpose_required_accuracy);
    }




    [Test]
    public void SolutionData_initialState()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Check the solution data at t=0
        Assert.AreEqual(3, ivp.SolutionData(0, 0), general_purpose_required_accuracy);
        Assert.AreEqual(4, ivp.SolutionData(1, 0), general_purpose_required_accuracy);
        Assert.AreEqual(5, ivp.SolutionData(2, 0), general_purpose_required_accuracy);
    }

    [Test]
    public void SolutionData_middle()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Check the solution data at t=2
        Assert.AreEqual(3, ivp.SolutionData(0, 2), general_purpose_required_accuracy);
        Assert.AreEqual(4, ivp.SolutionData(1, 2), general_purpose_required_accuracy);
        Assert.AreEqual(5, ivp.SolutionData(2, 2), general_purpose_required_accuracy);
    }

    [Test]
    public void SolutionData_atUpperBound()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Check the solution data at t=3
        Assert.AreEqual(3, ivp.SolutionData(0, 3), general_purpose_required_accuracy);
        Assert.AreEqual(4, ivp.SolutionData(1, 3), general_purpose_required_accuracy);
        Assert.AreEqual(5, ivp.SolutionData(2, 3), general_purpose_required_accuracy);
    }

    //Gavin: This behavior is not implemented, so I expect this to fail
    [Test]
    public void SolutionData_OutOfBounds()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Check the solution data at t=25 where there is no data
        Assert.IsNaN(ivp.SolutionData(0, 25));
        Assert.IsNaN(ivp.SolutionData(1, 25));
        Assert.IsNaN(ivp.SolutionData(2, 25));
    }

    //Gavin: This behavior is not implemented, so I expect this to fail
    [Test]
    public void SolutionData_OutOfBoundsNegative()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);
        //Check the solution data at t=25 where there is no data
        Assert.IsNaN(ivp.SolutionData(0, -1));
        Assert.IsNaN(ivp.SolutionData(1, -1));
        Assert.IsNaN(ivp.SolutionData(2, -1));
    }








    [Test]
    public void SaveTest_First()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);

        //save this data to file
        StreamWriter fout = new StreamWriter("testing.csv");
        ivp.SaveData(fout);
        fout.Close();

        //Read the data file and check its output
        string line = "";
        StreamReader fin = new StreamReader("testing.csv");
        line = fin.ReadLine();
        fin.Close();
        //Delete the file
        File.Delete("testing.csv");
        Assert.AreEqual("0,3,4,5,", line);
    }

    [Test]
    public void SaveTest_Middle()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);

        //save this data to file
        StreamWriter fout = new StreamWriter("testing.csv");
        ivp.SaveData(fout);
        fout.Close();

        //Read the data file and check its output
        string line = "";
        StreamReader fin = new StreamReader("testing.csv");
        fin.ReadLine();
        fin.ReadLine();
        line = fin.ReadLine();
        fin.Close();
        //Delete the file
        File.Delete("testing.csv");
        Assert.AreEqual("0.4,3,4,5,", line);
    }

    [Test]
    public void SaveTest_End()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ExpressionParser parser = new ExpressionParser();
        //Define a system in which the solution will be (3,4,5) at all times
        ivp.F.funcs[0] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[1] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.F.funcs[2] = parser.EvaluateExpression("0").ToDelegate("t");
        ivp.SetState(new VectorND(3, 4, 5), 0);
        //Solve the problem
        ivp.SolveTo(3);

        //save this data to file
        StreamWriter fout = new StreamWriter("testing.csv");
        ivp.SaveData(fout);
        fout.Close();


        //Read the data file and check its output
        string line = "sentinal";
        string lastLine = "";
        StreamReader fin = new StreamReader("testing.csv");
        while (line != null)
        {
            lastLine = line;
            line = fin.ReadLine();
        }
        line = lastLine;
        fin.Close();
        //Delete the file
        File.Delete("testing.csv");
        Assert.AreEqual("3,3,4,5,", line);
    }

    [Test]
    public void SetH_GetH()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ivp.SetH(0.3, 0);
        Assert.AreEqual(0.3, ivp.GetH(), single_value_required_accuracy);
    }

    [Test]
    public void Constructor_SetsH() {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        Assert.AreEqual(0.2, ivp.GetH(), single_value_required_accuracy);
    }

    [Test]
    public void Constructor_WillNotSetHToZero()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0, 0);
        Assert.AreNotEqual(0, ivp.GetH());
    }

    [Test]
    public void Constructor_WillNotSetHToNegative()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, -0.1, 0);
        Assert.Greater(ivp.GetH(), 0);
    }

    [Test]
    public void Constructor_WillNotSetDimToZero()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(0, 0.1, 0);
        Assert.AreNotEqual(0, ivp.GetDim());
    }

    [Test]
    public void Constructor_SetsDim()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        Assert.AreEqual(3, ivp.GetDim());
    }

    [Test]
    public void SetH_WillNotSetHToZero()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ivp.SetH(0, 0);
        Assert.AreNotEqual(0, ivp.GetH());
    }

    [Test]
    public void SetH_WillNotSetHToNegative()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.2, 0);
        ivp.SetH(-0.1, 0);
        Assert.Greater(ivp.GetH(), 0);
    }




    //************************************************************************
    //******************* Accuracy Performance Testing ***********************
    //************************************************************************

    [Test]
    public void AccuracyPerformanceTest_dim1()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(1, 0.015625, 0);
        ExpressionParser parser = new ExpressionParser();
        //1 dimensional first order nonlinear, nonhomogeneous, autonomous
        ivp.F.funcs[0] = parser.EvaluateExpression("-(y+1)*(y+3)").ToDelegate("t", "y"); 
        ivp.SetState(new VectorND(-2.0), 0);
        //Solve the problem
        ivp.SolveTo(20);
        //Assess the error
        double error = 0;
        Func<double, double> actualSolution = (double tt) =>
        {
            return (-3 + 2 * Math.Pow((1 + Math.Exp(-2 * tt)), -1));
        };
        double a = 1; //test error on [a,b]
        double b = 20;
        double htest = 1; //test at every this
        for (double t = a; t <= b; t += htest)
        {
            //Debug.Log("Estimated solution at t = " + t + ":" + ivp.SolutionData(0, t)
                //+ " ; versus  Actual solution at t = " + t + ":" + actualSolution(t)
                //+ " ; Error = " + (ivp.SolutionData(0, t) - actualSolution(t)));
            error += Math.Abs(ivp.SolutionData(0, t) - actualSolution(t));
        }
        //Test error against required total precision over the interval
        Assert.LessOrEqual(error, solution_total_error_benchmark * 20);

    }


    [Test]
    public void AccuracyPerformanceTest_dim3()
    {
        //Create an ODE initial value problem
        ODEInitialValueProblem ivp = new ODEInitialValueProblem(3, 0.0001, 0);
        ExpressionParser parser = new ExpressionParser();
        //3 dimensional first order linear non-homogenous non-autonomous
        ivp.F.funcs[0] = parser.EvaluateExpression("u1+2*u2-2*u3+e^(-t)").ToDelegate("t", "u1", "u2", "u3");
        ivp.F.funcs[1] = parser.EvaluateExpression("u2+u3-2*e^(-t)").ToDelegate("t", "u1", "u2", "u3");
        ivp.F.funcs[2] = parser.EvaluateExpression("u1+2*u2+e^(-t)").ToDelegate("t", "u1", "u2", "u3");
        ivp.SetState(new VectorND(3, -1, 1), 0);
        //Solve the problem
        ivp.SolveTo(20);
        //Assess the error
        double error = 0;
        Func<double, VectorND> actualSolution = (double tt) =>
        {
            VectorND r = new VectorND(3);
            r[0] = -3 * Math.Exp(-tt) - 3 * Math.Sin(tt) + 6 * Math.Cos(tt);
            r[1] = 1.5 * Math.Exp(-tt) + 0.3 * Math.Sin(tt) - 2.1 * Math.Cos(tt) - 0.4 * Math.Exp(2 * tt);
            r[2] = -Math.Exp(-tt) + 2.4 * Math.Cos(tt) + 1.8 * Math.Sin(tt) - 0.4 * Math.Exp(2 * tt);
            return r;
        };
        double a = 0; //test error on [a,b]
        double b = 1;
        double htest = 0.1; //test at every this
        for (double t = a; t <= b; t += htest)
        {
            error += Math.Abs(ivp.SolutionData(0, t) - actualSolution(t)[0]);
            error += Math.Abs(ivp.SolutionData(1, t) - actualSolution(t)[1]);
            error += Math.Abs(ivp.SolutionData(2, t) - actualSolution(t)[2]);
        }
        //Test error against required total precision over the interval
        Assert.LessOrEqual(error, solution_total_error_benchmark * 30); //3D * 10 points = 30 values
        
    }



}
