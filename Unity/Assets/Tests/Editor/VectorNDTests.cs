using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using DynamicsLab.Vector;
using B83.ExpressionParser;

public class VectorNDTests {

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



    private const double required_accuracy = 1e-12;

	[Test]
	public void checkDim()
	{
		VectorND v1 = new VectorND(4, -8, 0);
		Assert.AreEqual (v1.GetDim (), 3);
	}

	[Test]
	public void checkDimIn()
	{
		VectorND v1 = new VectorND (10);
		Assert.AreEqual (v1.GetDim(), 10);
	}

	[Test]
	public void checkConstructor()
	{
		VectorND v1 = new VectorND (1, 2);
		Assert.AreEqual (v1 [0], 1);
		Assert.AreEqual (v1 [1], 2);
	}

    [Test]
    public void AddSameDimVectors()
    {
        VectorND v1 = new VectorND(1.4, 2.7, -3.1, 0);
        VectorND v2 = new VectorND(-3.7, 4, -1, 57.3);
        VectorND r = v1 + v2;
        Assert.AreEqual(r[0], -2.3, required_accuracy);
        Assert.AreEqual(r[1], 6.7, required_accuracy);
        Assert.AreEqual(r[2], -4.1, required_accuracy);
        Assert.AreEqual(r[3], 57.3, required_accuracy);
    }

	[Test]
	public void AddDifferentDimVectors()
	{
		VectorND v1 = new VectorND (2, -4, 0);
		VectorND v2 = new VectorND (7, 5, -6, 3);
		VectorND r = v1 + v2;
		Assert.AreEqual (r[0], 2, required_accuracy);
		Assert.AreEqual (r[1], -4, required_accuracy);
		Assert.AreEqual (r[2], 0, required_accuracy);
	}

    [Test]
    public void SubtractSameDimVectors()
    {
        VectorND v1 = new VectorND(27.2, 2.7, -30, 0);
        VectorND v2 = new VectorND(-3.7, 21, -5.99, 50);
        VectorND r = v1 - v2;
        Assert.AreEqual(r[0], 30.9, required_accuracy);
        Assert.AreEqual(r[1], -18.3, required_accuracy);
        Assert.AreEqual(r[2], -24.01, required_accuracy);
        Assert.AreEqual(r[3], -50, required_accuracy);
    }

	[Test]
	public void SubtractDifferentDimVectors()
	{
		VectorND v1 = new VectorND (2, -4, 0);
		VectorND v2 = new VectorND (7, 5, -6, 3);
		VectorND r = v1 - v2;
		Assert.AreEqual (r[0], 2, required_accuracy);
		Assert.AreEqual (r[1], -4, required_accuracy);
		Assert.AreEqual (r[2], 0, required_accuracy);
	}

	[Test]
	public void MultiplySameDimVectors()
	{
		VectorND v1 = new VectorND (2, -4, 3, 8);
		VectorND v2 = new VectorND (7, 5, -6, 0);
		VectorND r = v1 * v2;
		Assert.AreEqual (r[0], 14, required_accuracy);
		Assert.AreEqual (r[1], -20, required_accuracy);
		Assert.AreEqual (r[2], -18, required_accuracy);
		Assert.AreEqual (r[3], 0, required_accuracy);
	}

	[Test]
	public void MultiplyDifferentDimVectors()
	{
		VectorND v1 = new VectorND (2, -4, 0);
		VectorND v2 = new VectorND (7, 5, -6, 3);
		VectorND r = v1 * v2;
		Assert.AreEqual (r[0], 2, required_accuracy);
		Assert.AreEqual (r[1], -4, required_accuracy);
		Assert.AreEqual (r[2], 0, required_accuracy);
	}

	[Test]
	public void ScalarMultiplicationOnLeft()
	{
		VectorND v1 = new VectorND (4, -8, 0);
		double scalar = 2;
		VectorND r = scalar * v1;
		Assert.AreEqual (r[0], 8, required_accuracy);
		Assert.AreEqual (r [1], -16, required_accuracy);
		Assert.AreEqual (r [2], 0, required_accuracy);
	}

	[Test]
	public void ScalarMultiplicationOnRight()
	{
		VectorND v1 = new VectorND (4, -8, 0);
		double scalar = 2;
		VectorND r = v1 * scalar;
		Assert.AreEqual (r[0], 8, required_accuracy);
		Assert.AreEqual (r [1], -16, required_accuracy);
		Assert.AreEqual (r [2], 0, required_accuracy);
	}

    [Test]
    public void GetDimTest()
    {
        VectorND v1 = new VectorND(4, -8, 0);
        Assert.AreEqual(3, v1.GetDim());
    }

    [Test]
    public void CopyConstructorTest()
    {
        VectorND v1 = new VectorND(4, -8, 0);
        VectorND v2 = new VectorND(v1);
        Assert.AreEqual(3, v2.GetDim());
        Assert.AreEqual(v2[0], 4, required_accuracy);
        Assert.AreEqual(v2[1], -8, required_accuracy);
        Assert.AreEqual(v2[2], 0, required_accuracy);
    }

    [Test]
    public void IndexerTest()
    {
        VectorND v1 = new VectorND(4, -8, 0);
        Assert.AreEqual(v1[0], v1.values[0]);
        Assert.AreEqual(v1[1], v1.values[1]);
        Assert.AreEqual(v1[2], v1.values[2]);
    }


}
