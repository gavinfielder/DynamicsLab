/**
 * This file holds generalized n-dimensional structures for vector arithmetic and
 * vector delegate functions
 * 
 * @author Gavin Fielder
 * @date 2/19/2018
 */
using B83.ExpressionParser; //for ExpressionDelegate

namespace DynamicsLab.Vector {


    // *************************************************************************
    // **************************** Public Classes *****************************
    // *************************************************************************

    /*
     * public class VectorND
     *      This class represents a generalized n-dimensional vector 
     *      for vector arithmetic.
     *      
     *      Constructors:
     *              public VectorND(byte dim_in)
     *              public VectorND(params double[] list)
     *              public VectorND(VectorND tocopy)
     *          Example usages:
     *              myVec = new VectorND(2); //creates 2-dimensional vector
     *              myVec = new VectorND(1.0,3.5,0.7); //creates 3-dimensional
     *                                                 //vector with initial values
     *              vec2 = new VectorND(vec1); //copies vec1 to a new vec2
     *              
     *      Access:
     *          Directly indexable like an array. For example:
     *              vec1[0] = vec2[0];
     * 
     *      Operations:
     *          +   addition between vectors
     *          -   subtration between vectors
     *          *   element-wise multiplication between vectors
     *                  or scalar multiplication by double
     *                  
     *      Note:
     *          This class doesn't do any exception checking. Use carefully. 
     *          
     */
    /**
	 * This class represents a generalized n-dimensional vector for vector arithmetic
	 */
    public class VectorND
    {
        
        //Fields
        private byte dim;
        public double[] values; // TODO: why is this public? my design says it should be private. To investigate.

        //Accessors
        public byte GetDim() { return dim; }


        //constructors

        //Dimmed and allocated constructor
        public VectorND(byte dim_in)
        {
            dim = dim_in;
            values = new double[dim];
        }
        //Params array constructor
        public VectorND(params double[] list)
        {
            dim = (byte)list.Length;
            values = new double[dim];
            for (int i = 0; i < dim; i++)
                values[i] = list[i];
        }
        //Copy constructor
        public VectorND(VectorND tocopy)
        {
            dim = tocopy.GetDim();
            values = new double[dim];
        }
        //Destructor to hopefully speed up garbage collection
        ~VectorND()
        {
            values = null;
        }

        //Indexer
        //Note this does not check bounds or even allocation status. program carefully.
        public double this[byte i]
        {
            get
            {
                return values[i];
            }
            set
            {
                values[i] = value; //value is reserved in this context
            }
        }

        //Arithmetic operations
        //Addition. Note this assumes vectors are same dimension. program carefully.
        public static VectorND operator +(VectorND a, VectorND b)
        {
			if (b.GetDim () != a.GetDim ())
				return a;
			VectorND r = new VectorND(a.GetDim());
            for (byte i = 0; i < a.GetDim(); i++)
                r[i] = a[i] + b[i];
            return r;
        }
        //Subtraction. Note this assumes vectors are same dimension. program carefully.
        public static VectorND operator -(VectorND a, VectorND b)
        {
			if (b.GetDim () != a.GetDim ())
				return a;
            VectorND r = new VectorND(a.GetDim());
            for (byte i = 0; i < a.GetDim(); i++)
                r[i] = a[i] - b[i];
            return r;
        }
        //Element-wise multiplication. Note this assumes vectors are same dimension. program carefully.
        public static VectorND operator *(VectorND a, VectorND b)
        {
			if (b.GetDim () != a.GetDim ())
				return a;
            VectorND r = new VectorND(a.GetDim());
            for (byte i = 0; i < a.GetDim(); i++)
                r[i] = a[i] * b[i];
            return r;
        }
        //Scalar multiplication with scalar on left.
        public static VectorND operator *(double k, VectorND b)
        {
            VectorND r = new VectorND(b.GetDim());
            for (byte i = 0; i < b.GetDim(); i++)
                r[i] = k * b[i];
            return r;
        }
        //Scalar multiplication with scalar on right.
        public static VectorND operator *(VectorND a, double k)
        {
            VectorND r = new VectorND(a.GetDim());
            for (byte i = 0; i < a.GetDim(); i++)
                r[i] = k * a[i];
            return r;
        }

    }



    /*
     * public class FunctionVectorND
     *      This class wraps an array of function delegates to be used as a
     *      system or a vector function. 
     *      It is designed to be used with Bunny83's expression parser.
     * 
     *      Invoking the functions individually involve calling 
     *          funcs[i].Invoke(list)
     *      where list is a params double[] containing the t value first,
     *      and the state vector values after that.
     * 
     *      You can also invoke all of the functions together with this function:
     *          public VectorND Eval(params double[] list)
     *              Evaluates the vector function at the passed values 
     *              and returns the result in vector form
     *          Note that when used by the Runge-Kutta function,
     *          list is n+1 dimensional while the return value is n dimensional.
     *          The very first dimension in list is the t value, and after that
     *          is the state vector values.
     *          
     *      If anyone needs to set up a publically accessible function to
     *      generate n+1 dimensional params double[] arrays from a t-value and
     *      associated VectorND state vector, see the RKCallBuild function in 
     *      ODEIVPSolver in dlab-ivp.cs to use as a reference.
     */
    /**
	 * This class handles an n-dimensional vector function using 
	 * an array of delegates
	 */
    public class FunctionVectorND {
        //Fields
        private byte dim;
        public ExpressionDelegate[] funcs;

        //Dimmed and allocated constructor
        public FunctionVectorND(byte dim_in) {
            dim = dim_in;
            if (dim > 0) funcs = new ExpressionDelegate[dim];
        }

        //Accessor
        public byte GetDim() { return dim;  }

        /**
         * Evaluates the vector function at the passed values and returns
         * the result in vector form
         * 
         * @param  list  the input values. t first, then state variables
         * 
         * @return  the value of the vector function
         */
        public VectorND Eval(params double[] list) {
            VectorND result = new VectorND(dim);
            for (byte i = 0; i < dim; i++) {
                result[i] = funcs[i].Invoke(list);
            }
            return result;

        }




    }

}