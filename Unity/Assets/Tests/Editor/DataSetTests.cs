using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using DynamicsLab.Vector;
using DynamicsLab.Data;
using System;
using System.IO;

public class DataSetTests {

    const double doubleComparisonAccuracy = 1e-14;
    const double doubleComparison10decimalPlaces = 1e-10;

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
    public void DataSet_Constructor()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        //Check if there's a data chunk allocated now
        Assert.Greater(ds.data.Count, 0);
    }
    
    [Test]
	public void DataSet_AbsoluteIndexing_0() {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        //write to the underlying data directly to have something to check the function against
        ds.data[0].data[0].values[0] = 3;
        ds.data[0].data[0].values[1] = 4;
        ds.data[0].data[0].values[2] = 5;
        //Check if we can index by absolute index to get this vector
        Assert.IsTrue(VectorsAreEqual(ds[0], new VectorND(3, 4, 5)));
    }

    [Test]
    public void DataSet_ChunkLocalIndexing_0()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        //write to the underlying data directly to have something to check the function against
        ds.data[0].data[0].values[0] = 3;
        ds.data[0].data[0].values[1] = 4;
        ds.data[0].data[0].values[2] = 5;
        //Check if we can index by chunk,local index to get this vector
        Assert.IsTrue(VectorsAreEqual(ds[0,0], new VectorND(3, 4, 5)));
    }

    [Test]
    public void DataSet_tValueIndexing_nearIntercept()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        //write to the underlying data directly to have something to check the function against
        //t=5.6 <=> i=56
        ds.data[0].data[56].values[0] = 3;
        ds.data[0].data[56].values[1] = 4;
        ds.data[0].data[56].values[2] = 5;
        //Check if we can index by t value to get this vector
        Assert.IsTrue(VectorsAreEqual(ds[5.6], new VectorND(3, 4, 5)));
    }

    [Test]
    public void DataSet_AbsoluteIndexing_chunk2()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        //write to the underlying data directly to have something to check the function against
        ds.data[1].data[732].values[0] = 3;
        ds.data[1].data[732].values[1] = 4;
        ds.data[1].data[732].values[2] = 5;
        //Check if we can index by absolute index to get this vector
        Assert.IsTrue(VectorsAreEqual(ds[1732], new VectorND(3, 4, 5)));
    }

    [Test]
    public void DataSet_ChunkLocalIndexing_chunk2()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        //write to the underlying data directly to have something to check the function against
        ds.data[1].data[732].values[0] = 3;
        ds.data[1].data[732].values[1] = 4;
        ds.data[1].data[732].values[2] = 5;
        //Check if we can index by chunk,local index to get this vector
        Assert.IsTrue(VectorsAreEqual(ds[1,732], new VectorND(3, 4, 5)));
    }

    [Test]
    public void DataSet_tValueIndexing_chunk2()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        //write to the underlying data directly to have something to check the function against
        ds.data[1].data[732].values[0] = 3;
        ds.data[1].data[732].values[1] = 4;
        ds.data[1].data[732].values[2] = 5;
        //Check if we can index by t value to get this vector
        Assert.IsTrue(VectorsAreEqual(ds[173.2], new VectorND(3, 4, 5)));
    }

    [Test]
    public void DataSet_tValueIndexing_approximateLow()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        //write to the underlying data directly to have something to check the function against
        ds.data[1].data[732].values[0] = 3;
        ds.data[1].data[732].values[1] = 4;
        ds.data[1].data[732].values[2] = 5;
        //Check if we can index by approximate t value to get this vector
        Assert.IsTrue(VectorsAreEqual(ds[173.17], new VectorND(3, 4, 5)));
    }

    [Test]
    public void DataSet_tValueIndexing_approximateHigh()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        //write to the underlying data directly to have something to check the function against
        ds.data[1].data[732].values[0] = 3;
        ds.data[1].data[732].values[1] = 4;
        ds.data[1].data[732].values[2] = 5;
        //Check if we can index by approximate t value to get this vector
        Assert.IsTrue(VectorsAreEqual(ds[173.214], new VectorND(3, 4, 5)));
    }

    [Test]
    public void DataSet_ChunkIndex_0()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        Assert.AreEqual(ds.ChunkIndex(73), 0);
    }

    [Test]
    public void DataSet_ChunkIndex_High()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        Assert.AreEqual(ds.ChunkIndex(6379), 6);
    }

    [Test]
    public void DataSet_ChunkIndex_UpperEdge()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        Assert.AreEqual(ds.ChunkIndex(999), 0);
    }

    [Test]
    public void DataSet_ChunkIndex_LowerEdge()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        Assert.AreEqual(ds.ChunkIndex(1000), 1);
    }

    [Test]
    public void DataSet_Lookup_chunk1()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        Assert.AreEqual(ds.Lookup(65.3), 653);
    }

    [Test]
    public void DataSet_Lookup_chunk2()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        Assert.AreEqual(ds.Lookup(165.3), 1653);
    }

    [Test]
    public void DataSet_Lookup_approximateLow()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        Assert.AreEqual(ds.Lookup(165.27), 1653);
    }

    [Test]
    public void DataSet_Lookup_approximateHigh()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        Assert.AreEqual(ds.Lookup(165.32), 1653);
    }

    [Test]
    public void DataSet_LocalIndex_0()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        Assert.AreEqual(ds.LocalIndex(73), 73);
    }

    [Test]
    public void DataSet_LocalIndex_High()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        Assert.AreEqual(ds.LocalIndex(6379), 379);
    }

    [Test]
    public void DataSet_LocalIndex_UpperEdge()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        Assert.AreEqual(ds.LocalIndex(999), 999);
    }

    [Test]
    public void DataSet_LocalIndex_LowerEdge()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        Assert.AreEqual(ds.LocalIndex(1000), 0);
    }

    [Test]
    public void DataSet_TVal_0_withNonzeroIntercept()
    {
        //3 dimensional data starting at t=5.3 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 5.3, 0.1);
        Assert.AreEqual(ds.Tval(0), 5.3, doubleComparisonAccuracy);
    }

    [Test]
    public void DataSet_TVal_chunk2()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        Assert.AreEqual(ds.Tval(1739), 173.9, doubleComparisonAccuracy);
    }

    [Test]
    public void DataSet_WriteState_chunk1()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        //Write a vector
        ds.WriteState(0, 732, new VectorND(3, 4, 5));
        //read the underlying data directly to have something to check against
        VectorND verif = new VectorND(3);
        verif[0] = ds.data[0].data[732].values[0];
        verif[1] = ds.data[0].data[732].values[1];
        verif[2] = ds.data[0].data[732].values[2];
        //Check if the vector read from data is the same as the initial vector
        Assert.IsTrue(VectorsAreEqual(verif, new VectorND(3, 4, 5)));
    }

    [Test]
    public void DataSet_WriteState_chunk2()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        //Write a vector
        ds.WriteState(1, 732, new VectorND(3, 4, 5));
        //read the underlying data directly to have something to check against
        VectorND verif = new VectorND(3);
        verif[0] = ds.data[1].data[732].values[0];
        verif[1] = ds.data[1].data[732].values[1];
        verif[2] = ds.data[1].data[732].values[2];
        //Check if the vector read from data is the same as the initial vector
        Assert.IsTrue(VectorsAreEqual(verif, new VectorND(3, 4, 5)));
    }


    //Gavin: I believe this test is written correctly but the implementation is not correct
    //at least defining its behavior this way. We should leave it alone for now.
    [Test]
    public void DataSet_WriteNext_firstCallWritesToZero()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        //Write a vector
        ds.WriteNext(new VectorND(3, 4, 5));
        //read the underlying data directly to have something to check against
        VectorND verif = new VectorND(3);
        verif[0] = ds.data[0].data[0].values[0];
        verif[1] = ds.data[0].data[0].values[1];
        verif[2] = ds.data[0].data[0].values[2];
        //Check if the vector read from data is the same as the initial vector
        Assert.IsTrue(VectorsAreEqual(verif, new VectorND(3, 4, 5)));
    }

    [Test]
    public void DataSet_WriteNext_firstCallAfterWriteState()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        //Write a vector
        ds.WriteState(0, 0, new VectorND(7, 3, -5));
        ds.WriteNext(new VectorND(3, 4, 5));
        //read the underlying data directly to have something to check against
        VectorND verif = new VectorND(3);
        verif[0] = ds.data[0].data[1].values[0];
        verif[1] = ds.data[0].data[1].values[1];
        verif[2] = ds.data[0].data[1].values[2];
        //Check if the vector read from data is the same as the initial vector
        Assert.IsTrue(VectorsAreEqual(verif, new VectorND(3, 4, 5)));
    }

    [Test]
    public void DataSet_WriteNext_thirdCallAfterWriteState()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        //Write a vector
        ds.WriteState(0, 0, new VectorND(7, 3, -5)); //0
        ds.WriteNext(new VectorND(1, 2, 3)); //1
        ds.WriteNext(new VectorND(7, -1, 0)); //2
        ds.WriteNext(new VectorND(3, 4, 5)); //3
        //read the underlying data directly to have something to check against
        VectorND verif = new VectorND(3);
        verif[0] = ds.data[0].data[3].values[0];
        verif[1] = ds.data[0].data[3].values[1];
        verif[2] = ds.data[0].data[3].values[2];
        //Check if the vector read from data is the same as the initial vector
        Assert.IsTrue(VectorsAreEqual(verif, new VectorND(3, 4, 5)));
    }

    [Test]
    public void DataSet_SaveTest_First()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        //Write a vector
        ds.WriteState(0, 0, new VectorND(7, 3, -5)); //0
        ds.WriteNext(new VectorND(1, 2, 3)); //1
        ds.WriteNext(new VectorND(7, -1, 0)); //2
        ds.WriteNext(new VectorND(3, 4, 5)); //3
        //save this data to file
        StreamWriter fout = new StreamWriter("testing.csv");
        ds.SaveData(fout);
        fout.Close();
        //Read the data file and check its output
        string line = "";
        StreamReader fin = new StreamReader("testing.csv");
        line = fin.ReadLine();
        fin.Close();
        //Delete the file
        File.Delete("testing.csv");
        Assert.AreEqual(line, "0,7,3,-5,");
    }

    [Test]
    public void DataSet_SaveTest_Middle()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        //Write a vector
        ds.WriteState(0, 0, new VectorND(7, 3, -5)); //0
        ds.WriteNext(new VectorND(1, 2, 3)); //1
        ds.WriteNext(new VectorND(7, -1, 0)); //2
        ds.WriteNext(new VectorND(3, 4, 5)); //3
        //save this data to file
        StreamWriter fout = new StreamWriter("testing.csv");
        ds.SaveData(fout);
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
        Assert.AreEqual(line, "0.2,7,-1,0,");
    }

    [Test]
    public void DataSet_SaveTest_End()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        //Write a vector
        ds.WriteState(0, 0, new VectorND(7, 3, -5)); //0
        ds.WriteNext(new VectorND(1, 2, 3)); //1
        ds.WriteNext(new VectorND(7, -1, 0)); //2
        ds.WriteNext(new VectorND(3, 4, 5)); //3
        //save this data to file
        StreamWriter fout = new StreamWriter("testing.csv");
        ds.SaveData(fout);
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
        Assert.AreEqual(line, "0.3,3,4,5,");
    }

    [Test]
    public void DataSet_NewChunk_makesNewChunk()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        Assert.AreEqual(ds.data.Count, 2);
    }

    [Test]
    public void DataSet_NewChunk_makesNewChunkForward()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        Assert.Greater(ds.Tval(1000), ds.Tval(0));
    }

    [Test]
    public void DataSet_RemoveLastChunk_removesAChunk()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        ds.RemoveLastChunk();
        Assert.AreEqual(ds.data.Count, 1);
    }

    [Test]
    public void DataSet_RemoveLastChunk_removesTheLastChunk()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        //mark the first chunk
        ds.WriteState(0, 51, new VectorND(3, 4, 5));
        ds.NewChunk();
        ds.RemoveLastChunk();
        //check the first chunk to see if the data is ok
        Assert.IsTrue(VectorsAreEqual(ds[51], new VectorND(3, 4, 5)));
    }

    [Test]
    public void DataSet_RemoveLastChunk_willNotRemoveAllChunks()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.RemoveLastChunk();
        Assert.AreEqual(ds.data.Count, 1);
    }

    [Test]
    public void DataSet_RemoveFirstChunk_willNotRemoveAllChunks()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.RemoveFirstChunk();
        Assert.AreEqual(ds.data.Count, 1);
    }

    [Test]
    public void DataSet_RemoveFirstChunk_hasOneRemaining()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        ds.RemoveFirstChunk();
        Assert.AreEqual(ds.data.Count, 1);
    }

    [Test]
    public void DataSet_RemoveFirstChunk_updatesIntercept()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        ds.RemoveFirstChunk();
        Assert.AreEqual(ds.tIntercept, 100, doubleComparisonAccuracy);
    }

    [Test]
    public void DataSet_RemoveFirstChunk_thenAbsoluteIndex()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        //write to the underlying data directly to have something to check the function against
        ds.data[1].data[732].values[0] = 3;
        ds.data[1].data[732].values[1] = 4;
        ds.data[1].data[732].values[2] = 5;
        //Remove the first chunk
        ds.RemoveFirstChunk();
        //absolute index of the data should have shifted down by 1000
        //Check if we can index by absolute index to get this vector
        Assert.IsTrue(VectorsAreEqual(ds[732], new VectorND(3, 4, 5)));
    }

    [Test]
    public void DataSet_RemoveFirstChunk_thenChunkLocalIndex()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        //write to the underlying data directly to have something to check the function against
        ds.data[1].data[732].values[0] = 3;
        ds.data[1].data[732].values[1] = 4;
        ds.data[1].data[732].values[2] = 5;
        //Remove the first chunk
        ds.RemoveFirstChunk();
        //chunk index should be shifted down by 1, local index should be same
        //Check if we can index by chunk local index to get this vector
        Assert.IsTrue(VectorsAreEqual(ds[0, 732], new VectorND(3, 4, 5)));
    }
    
    [Test]
    public void DataSet_RemoveFirstChunk_thenTValueIndex()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        //write to the underlying data directly to have something to check the function against
        ds.data[1].data[732].values[0] = 3;
        ds.data[1].data[732].values[1] = 4;
        ds.data[1].data[732].values[2] = 5;
        //Remove the first chunk
        ds.RemoveFirstChunk();
        //t value index should be unchanged
        //Check if we can index by t value to get this vector
        Assert.IsTrue(VectorsAreEqual(ds[173.2], new VectorND(3, 4, 5)));
    }

    [Test]
    public void DataSet_RemoveFirstChunk_checkMinIndex()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        //artificially set min and max data ranges to flag valid data on an interval
        ds.minIndex = 532;
        ds.maxIndex = 1481;
        ds.dataLowerBound = 53.2;
        ds.dataUpperBound = 148.1;
        //Remove the first chunk
        ds.RemoveFirstChunk();
        //min index should now be zero as data was truncated to t=100.0 starting point
        Assert.AreEqual(0, ds.minIndex);
    }

    [Test]
    public void DataSet_RemoveFirstChunk_checkDataLowerBound()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        //artificially set min and max data ranges to flag valid data on an interval
        ds.minIndex = 532;
        ds.maxIndex = 1481;
        ds.dataLowerBound = 53.2;
        ds.dataUpperBound = 148.1;
        //Remove the first chunk
        ds.RemoveFirstChunk();
        //data was truncated to t=100.0 starting point
        Assert.AreEqual(ds.dataLowerBound, 100.0, doubleComparisonAccuracy);
    }

    [Test]
    public void DataSet_WriteState_MaxIndexTracking()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        //artificially set min and max data ranges to flag valid data on an interval
        ds.minIndex = 532;
        ds.maxIndex = 1481;
        ds.dataLowerBound = 53.2;
        ds.dataUpperBound = 148.1;
        //Write the next state
        ds.WriteNext(new VectorND(3, 4, 5));
        //max index should have increased by 1
        Assert.AreEqual(ds.maxIndex, 1482); 
    }

    [Test]
    public void DataSet_WriteState_UpperBoundTracking()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        //artificially set min and max data ranges to flag valid data on an interval
        ds.minIndex = 532;
        ds.maxIndex = 1481;
        ds.dataLowerBound = 53.2;
        ds.dataUpperBound = 148.1;
        //Write the next state
        ds.WriteNext(new VectorND(3, 4, 5));
        //upper bound should have increased by 0.1
        Assert.AreEqual(ds.dataUpperBound, 148.2, doubleComparisonAccuracy);
    }

    [Test]
    public void DataSet_RemoveFirstChunk_checkTIntercept_preciseEquality()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        //artificially set min and max data ranges to flag valid data on an interval
        ds.minIndex = 532;
        ds.maxIndex = 1481;
        ds.dataLowerBound = 53.2;
        ds.dataUpperBound = 148.1;
        //Remove the first chunk
        ds.RemoveFirstChunk();
        //data was truncated to t=100.0 starting point
        Assert.AreEqual(ds.tIntercept, 100.0, doubleComparisonAccuracy);
    }

    [Test]
    public void DataSet_RemoveFirstChunk_checkTIntercept_10decimalPlaces()
    {
        //3 dimensional data starting at t=0 and progressing in increments of dt=0.1
        DataSet ds = new DataSet(3, 0, 0.1);
        ds.NewChunk();
        //artificially set min and max data ranges to flag valid data on an interval
        ds.minIndex = 532;
        ds.maxIndex = 1481;
        ds.dataLowerBound = 53.2;
        ds.dataUpperBound = 148.1;
        //Remove the first chunk
        ds.RemoveFirstChunk();
        //data was truncated to t=100.0 starting point
        Assert.AreEqual(ds.tIntercept, 100.0, doubleComparison10decimalPlaces);
    }
}
