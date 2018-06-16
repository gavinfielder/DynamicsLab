using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using B83.ExpressionParser;
using DynamicsLab.Vector;

public class FunctionVectorNDTests
{
    private const double required_accuracy = 1e-12;

    [Test]
    public void FunctionVectorND_Dim()
    {
        //Tests construction and dimensionality
        ExpressionParser parser = new ExpressionParser();
        FunctionVectorND fv1 = new FunctionVectorND(7);
        Assert.AreEqual(fv1.GetDim(), 7);
    }

    [Test]
    public void FunctionVectorND_Eval_1func1var()
    {
        //Tests basic evaluation of one function of one variable
        ExpressionParser parser = new ExpressionParser();
        FunctionVectorND fv1 = new FunctionVectorND(1);
        fv1.funcs[0] = parser.EvaluateExpression("-(y+1)*(y+3)").ToDelegate("y");
        Assert.AreEqual(fv1.Eval(2)[0], -15, required_accuracy);
    }

    [Test]
    public void FunctionVectorND_Eval_3funcs2vars()
    {
        //Tests evaluation of 3 dimensional function of 2 variables
        ExpressionParser parser = new ExpressionParser();
        FunctionVectorND fv1 = new FunctionVectorND(3);
        fv1.funcs[0] = parser.EvaluateExpression("x+y-7").ToDelegate("x", "y");
        fv1.funcs[1] = parser.EvaluateExpression("x*y+y").ToDelegate("x", "y");
        fv1.funcs[2] = parser.EvaluateExpression("y^x-17*x").ToDelegate("x", "y");
        VectorND result = fv1.Eval(2, 3);
        Assert.AreEqual(result[0], -2, required_accuracy);
        Assert.AreEqual(result[1], 9, required_accuracy);
        Assert.AreEqual(result[2], -25, required_accuracy);
    }

}
