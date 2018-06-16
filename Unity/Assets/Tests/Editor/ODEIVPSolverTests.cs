using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using DynamicsLab.Solvers;
using DynamicsLab.Vector;
using B83.ExpressionParser;

public class ODEIVPSolverTests {

    private const double required_accuracy = 1e-12;

    [Test]
	public void RungeKutta_works() {

        //3 dimensional function of 2 variables
        ExpressionParser parser = new ExpressionParser();
        FunctionVectorND fv1 = new FunctionVectorND(3);
        fv1.funcs[0] = parser.EvaluateExpression("x+y-7").ToDelegate("x", "y");
        fv1.funcs[1] = parser.EvaluateExpression("x*y+y").ToDelegate("x", "y");
        fv1.funcs[2] = parser.EvaluateExpression("y^x-17*x").ToDelegate("x", "y");
        VectorND v1 = new VectorND(1, 2, 3);

        ODEIVPSolver solver = new ODEIVPSolver(3);
        VectorND result = solver.RungeKutta(0, 0.1, v1, fv1);

        Assert.AreEqual(result[0], 0.374145833333333, required_accuracy);
        Assert.AreEqual(result[1], 2.072081250000000, required_accuracy);
        Assert.AreEqual(result[2], 3.012229986742000, required_accuracy);
    }


}
