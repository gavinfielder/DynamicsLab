/**
 * This file defines data structures used to manage solution data generated
 * by DynamicsLab. 
 * 
 * @author Gavin Fielder
 * @date 2/19/2018
 */
using DynamicsLab.Vector;
using System.Collections.Generic;
using System.IO;

namespace DynamicsLab.Data
{
    
    // *************************************************************************
    // **************************** Public Classes *****************************
    // *************************************************************************

    // There are currently no public classes in this namespace.


    // *************************************************************************
    // *************************** Internal Classes ****************************
    // *************************************************************************
    // Internal classes are those inaccessible outside the assembly. The way I
    // use them here, they shouldn't need to be used outside the math library.

    /**
     * This class wraps a table of 1000xN values for use in dynamic memory storage
     */
    public class DataChunk //making public for testing only
    {

        //Fields - everything is public since it's an internal class
        public VectorND[] data;
        public byte dim;
        public double dataLowerBound; //stores the t-value assigned to data[0]
        public double dataUpperBound; //stores the t-value assigned to data[999]

        //constructor
        public DataChunk(byte dim_in, double from, double to)
        {
            dim = dim_in;
            //Set the upper and lower bound values for this chunk
            dataLowerBound = from;
            dataUpperBound = to;
            //Instantiate the collection
            data = new VectorND[1000];
            //Instantiate the data vectors
            for (uint i = 0; i < 1000; i++)
            {
                data[i] = new VectorND(dim);
            }
        }
        //destructor
        ~DataChunk() { data = null; }
    }

    /**
     * This class handles a dynamically-sized data set
     */
    public class DataSet //making public for testing only
    {
        //Fields - everything is public since it's an internal class
        public List<DataChunk> data; //the underlying collection of data tables
        public double dataLowerBound; //stores the lowest t-value with valid solution data
        public double dataUpperBound; //stores the highest t-value with valid solution data
        public int maxIndex; //stores the highest index with valid solution data
        public int minIndex; //stores the lowest index with valid solution data
        public double tSlope; //stores the slope of t over the discrete array locations
        public double tIntercept; //stores the t-value of data[0].data[0]
        public double h; //solution data resolution
        public byte dim; //number of dimensions

        //Constructor
        public DataSet(byte dim_in, double tIntercept_in, double tSlope_in)
        {
            //set basic parameters
            dim = dim_in;
            tSlope = tSlope_in;
            tIntercept = tIntercept_in;
            h = tSlope;
            //currently no data has been generated
            dataLowerBound = tIntercept;
            dataUpperBound = tIntercept;
            maxIndex = 0;
            minIndex = 0;
            //instantiate the data collection
            data = new List<DataChunk>();
            //create first chunk so it's ready to go
            DataChunk chunk = new DataChunk
                (dim, tIntercept, tIntercept + tSlope * 1000);
            data.Add(chunk);
        }


        //Access and manipulation : Does not error check

        //Indexer - lookup by independent variable value
        public VectorND this[double t]
        {
            get
            {
                //compute the absolute index
                int i = Lookup(t);
                //retrieve the data: does not error check
                return data[ChunkIndex(i)].data[LocalIndex(i)];
            }
        }
        //Indexer for lookup by absolute index
        public VectorND this[int i]
        {
            get
            {
                //retrieve the data: does not error check
                return data[ChunkIndex(i)].data[LocalIndex(i)];
            }
        }
        //Indexer for lookup by chunk and local indices
        public VectorND this[int chunk, int local]
        {
            get
            {
                //retrieve the data: does not error check
                return data[chunk].data[local];
            }
        }
        //Returns the absolute index
        public int Lookup(double t)
        {
            return (int)(((t - tIntercept) / h) + 0.5);
        }
        //Takes an absolute index and returns the chunk index
        public int ChunkIndex(int i)
        {
            return i / 1000;
        }
        //Takes an absolute index and returns the local index
        public int LocalIndex(int i)
        {
            return i % 1000;
        }
        //Calculates the independent variable of an index
        public double Tval(int i)
        {
            return tIntercept + tSlope * i;
        }


        //Writes a state to a vector. Used for setting initial conditions
        public void WriteState(int chunk, int local, VectorND newState)
        {
            data[chunk].data[local] = newState;
        }


        //Writes the next value to the data set
        //Used in sequential runtime solving
        public void WriteNext(VectorND val)
        {
            int c = ChunkIndex(maxIndex + 1);
            //Check if we need to make a new chunk
            if (c >= data.Count)
            {
                NewChunk();
            }
            data[c].data[LocalIndex(maxIndex + 1)] = val; //write data
            maxIndex = maxIndex + 1;
            dataUpperBound = Tval(maxIndex);
        }


        //Data Chunk management

        //Adds a new chunk (forward on the independent variable)
        public void NewChunk()
        {
            int chunkNumber = data.Count;
            DataChunk chunk = new DataChunk
                (dim, tIntercept + tSlope * chunkNumber * 1000,
                 tIntercept + tSlope * (chunkNumber + 1) * 1000);
            data.Add(chunk);
        }
        //Removes a chunk from the beginning of the list
        public void RemoveFirstChunk()
        {
            data.RemoveAt(0);
            //Check if there is a chunk remaining
            if (data.Count > 0)
            {
                //update to new intercept for data set
                tIntercept = data[0].dataLowerBound;
                //update max index as all indexing has changed
                maxIndex -= 1000;
                if (maxIndex < 0) maxIndex = 0;
            }
        }
        //Removes a chunk from the end of the list
        public void RemoveLastChunk()
        {
            data.RemoveAt(data.Count - 1);
            //check if maxIndex now points to a spot off the array
            /*
            if (ChunkIndex(maxIndex) > data.Count)
            {
                //reset max index
                maxIndex = data.Count * 1000 - 1;
            }
            //check if minIndex now points to a spot off the array
            if (ChunkIndex(maxIndex) > data.Count)
            {
                //reset min index
                minIndex = data.Count * 1000 - 1;
            }
            */
            //NOTE: user of DataSet must check maxIndex and minIndex themselves 
            //after all call to RemoveLastChunk()
            //TODO: refactor so this is easier to use.
        }


        //Saves all the valid data to file in CSV format
        public void SaveData(StreamWriter fout)
        {
            for (int i = minIndex; i < maxIndex; i++)
            {
                //time,var1,var2,var3 ...
                fout.WriteLine(Tval(i) + "," + VectorNDToCSV(data[ChunkIndex(i)].data[LocalIndex(i)]));
            }
        }

        //Forms a VectorND into a string in CSV format
        private string VectorNDToCSV(VectorND v)
        {
            string r = "";
            for (byte i = 0; i < v.GetDim(); i++)
                r += v[i] + ",";
            return r;
        }
    }







    // *************************************************************************
    // ************************** Deprecated Classes ***************************
    // *************************************************************************
    // These classes are included for legacy support. Move away from using them
    // as soon as possible.

    /**
    * DEPRECATED LEGACY CODE
	* This class handles a single data series for use in DynamicsLab simulations
	* implemented as a wrapper of a float array.
    * DEPRECATED LEGACY CODE
	*/
    /*
    public class DataSeries
    {
        /* 
		 * Public Interface:
		 * 
		 * 		Constructors:
		 * 
		 *			public DataSeries() : default
		 *			public DataSeries(uint size_in) : preallocates size
		 *			public DataSeries(float start, float end, uint numberOfPoints)
		 *            	: sets up linear space for independent variables
		 * 
		 * 		Primary Methods:
		 * 
		 * 			uint getSize() : returns size
		 * 			myDataSeries[i] : index with a uint like an array to access values
		 * 				(outputs float)
		 * 			myDataSeries[t] : index with a float to access values 
		 * 				(outputs float)
		 * 			float getInitialValue() : returns value at the lower bound
		 * 			float getTerminalValue() : returns value at the upper bound
		 * 			string setSymbol : sets the symbol that was used in math expressions, 
		 * 				for informational purposes. 
		 * 			string getSymbol : returns the symbol used in math expressions
		 * 
		 * 		Other Accessible Methods. These shouldn't need to be used much:
		 * 
		 *  		DataSeries getIndependnetVariable : returns a reference to
		 * 				the DataSeries which holds the associated independent variable
		 * 			bool getIsLinear() : returns whether set up as a linear space (i.e. as
		 * 				an independent variable)
		 * 			float getSlope() : returns the slope of an independent variable series
		 * /

        //Fields
        private uint size;
        private float[] values;
        private DataSeries independentVariable; //note this is a reference variable
        string symbol; //symbol used within math expressions, e.g. 'x'
                       //The following fields are for independent variables specifically
        private bool isLinear;
        private float slope;


        //Accessors
        public uint getSize() { return size; }
        public DataSeries getIndependentVariable()
        { //Note this returns a reference
            return independentVariable;
        }
        public bool getIsLinear() { return isLinear; }
        public float getSlope() { return slope; }
        public string getSymbol() { return symbol; }
        public float getInitialValue() { return values[0]; } //does not check allocation!
        public float getTerminalValue() { return values[size - 1]; } //does not check allocation!

        //Manipulators
        public void setIndependentVariable(DataSeries t_in)
        {
            independentVariable = t_in;
        }

        //constructors
        //Default constructor
        public DataSeries()
        {
            size = 0; //when 0, values is unallocated.
            values = null;
            independentVariable = null;
        }
        //Dimmed and allocated constructor
        public DataSeries(uint size_in)
        {
            size = size_in;
            if (size > 0) values = new float[size];
            independentVariable = null;
        }
        //Linear setup constructor for independent variables
        public DataSeries(float start, float end, uint numberOfPoints)
        {
            size = 0; //when 0, values is unallocated.
            independentVariable = null;
            setupLinear(start, end, numberOfPoints);
        }
        //Destructor to hopefully speed up garbage collection
        ~DataSeries()
        {
            values = null;
        }


        //Indexer
        //Note this does not check bounds or even allocation status. program carefully.
        public float this[uint i]
        {
            get
            {
                return values[i];
            }
            set
            {
                //We don't want a general set function--this should be limited to the
                //the math algorithm which can write data to this structure another way
                //NOTE: enabling this temporarily
                values[i] = value; //'value' seems to be reserved in this context
            }
        }
        //Indexer EXPERIMENTAL - Can I overload the indexer for lookup by independent
        //variable value? It's worth a try. 
        public float this[float t]
        {
            get
            {
                return lookup(t);
            }
        }


        //The following methods enable lookup by independent variable value
        //This method is private--use the linear setup constructor to create linear series
        private void setupLinear(float start, float end, uint numberOfPoints)
        {
            //just in case it's been already allocated
            if (size > 0)
            {
                values = null; //do we need this? I don't know
                size = 0;
            }
            //Allocate proper size
            size = numberOfPoints;
            values = new float[size];
            //Set up linear space
            for (int i = 0; i < size; i++)
            {
                values[i] = ((end * i - start * i) / size) + start;
            }
            //Calculate slope
            slope = (end - start) / size;
        }
        //Lookup by value of the independent variable
        //Note this assumes an independent variable data series has been hooked in through
        //the independentVariable pointer
        private float lookup(float t)
        { //if the overloaded indexer doesn't work, this 
          //will be changed to public access
            float m = independentVariable.getSlope();
            float b = independentVariable[0];
            uint i = (uint)(((t - b) / m) + 0.5); //round to nearest index
            return values[i];
        }

    }*/
}







