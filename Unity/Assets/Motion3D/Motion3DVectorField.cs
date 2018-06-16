using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicsLab.Vector;
using DynamicsLab.Peripherals;

namespace DynamicsLab.VectorField {

//Handles a vector field for first order motion
public class Motion3DVectorField : MonoBehaviour
{

    //Public fields
    public FunctionVectorND F; //3 dimensional vector function

    //Private fields
    private double currentTime;
    private GameObject[] instance;
    public GameObject arrow;
    public int vectorField = 0; //active flag
    public float vectorDist = 3f;
    private Motion3DSimulationController simController;
    private bool initialized;
    //Update frequency manager
    private UpdateFrequencyManager updateFrequencyManager;
    private const float UPDATE_FREQUENCY = 10f;
    private float max_Velocity_x = 1f;
	private float max_Velocity_y = 1f;
	private float max_Velocity_z = 1f;

    // Use this for initialization
    public void Start()
    {
        //Initialize update frequency
        updateFrequencyManager = new UpdateFrequencyManager(UPDATE_FREQUENCY);
        //Hook simulation controller reference
        simController = gameObject.GetComponent<Motion3DSimulationController>();
        //Hook arrow reference
        arrow = GameObject.Find("VectorModel");
        arrow.SetActive(false);
        //lazy initialization of vector field. Will initialize on first enable
        vectorField = 0;
        initialized = false;
        F = null;
    }

    //Instantiates vectors
    public void InitializeVectorField()
    {
        instance = new GameObject[1000];
        for (int z = 0; z < 10; z++)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {

                    float d = vectorDist;

                    //Position
                    instance[x + y * 10 + z * 100] = Instantiate(arrow,
                        new Vector3(x * d - d * 5, y * d, z * d), transform.rotation);
                    //Color
                    instance[x + y * 10 + z * 100].GetComponent<MeshRenderer>().sharedMaterial.color =
                            new Color(1.0f, 1.0f, 1.0f, 0.0f);

                } //x
            } //y
        } //z
        initialized = true;
    }

    // Update is called once per frame
    public void Update()
    {
        if (updateFrequencyManager.Ready() && (vectorField == 1)) // vectorField==1 => intialized
        {
            //Update current time
            currentTime = simController.CurrentTime;

            //Update vectors
            for (int z = 0; z < 10; z++)
            {
                for (int y = 0; y < 10; y++)
                {
                    for (int x = 0; x < 10; x++)
                    {

                        //Return velocities at all Vector Field arrow positions.
                        VectorND result = F.Eval(currentTime,
                            (x - 5) * vectorDist,
                            (y - 5) * vectorDist,
                                z);

						float res_x = Mathf.Abs((float)result[0]);
						float res_y = Mathf.Abs((float)result[1]);
						float res_z = Mathf.Abs((float)result[2]);


						//Max force of equation defines proportions of vector strengths
						if (res_x > max_Velocity_x){
							max_Velocity_x = Mathf.Max( Mathf.Max(res_x, res_y), res_y);
						}
						if (res_y > max_Velocity_y){
							max_Velocity_y = Mathf.Max( Mathf.Max(res_x, res_y), res_y);
						}
						if (res_z > max_Velocity_z){
							max_Velocity_z = Mathf.Max( Mathf.Max(res_x, res_y), res_y);
						}

						res_x = Mathf.Clamp( res_x, .5f, max_Velocity_x );
						res_y = Mathf.Clamp( res_y, .5f, max_Velocity_y );
						res_z = Mathf.Clamp( res_z, .5f, max_Velocity_z );

						res_x /= max_Velocity_x;
						res_y /= max_Velocity_y;
						res_z /= max_Velocity_z;


                        //Change Color intensity based on abs(Velocity of result)
                        instance[x + y * 10 + z * 100].GetComponent<MeshRenderer>().material.color =
                                //new Color(res_x,.9f,.9f, 0.1f);
								new Color(res_x, res_y, res_z, 0.1f);
								//new Color(res_x / 3f, res_y / 3f, res_z / 3f, 0.1f);

                        //Scaling volume based on intensity of ^ above
                        instance[x + y * 10 + z * 100].transform.localScale = new Vector3(
                            .5f,
                            Mathf.Clamp((res_y + res_x + res_z), -vectorDist, vectorDist),
                            .5f);

                        //Points Arrow at direction of result Velocity
                        instance[x + y * 10 + z * 100].transform.LookAt(new Vector3(
                            instance[x + y * 10 + z * 100].transform.position.x + (float)result.values[0],
                            instance[x + y * 10 + z * 100].transform.position.y + (float)result.values[1],
                            instance[x + y * 10 + z * 100].transform.position.z + (float)result.values[2])
                            );

                        //Transform 90 across x-axis to make the arrow point to "forward"
                        instance[x + y * 10 + z * 100].transform.Rotate(90, 0, 0);

                    } //z
                } //y
            } //x
        } //if vector field active and update frequency manager is ready
    }

    //Enables or disables the vector field
    public void ToggleVectorField()
    {
        if (vectorField == 0)
        {
            EnableVectorField();
        }
        else
        {
            DisableVectorField();
        }
    }
    public void EnableVectorField()
    {
        //Only allowed on first order problems
        if (simController != null){
	        if (simController.GetMotion3DSetup.Order == 1)
	        {
	            if (!(initialized)) InitializeVectorField();
	            if (F == null) F = simController.GetMotion3DSetup.F;
	            //Enable vector field
	            vectorField = 1;
	            for (int z = 0; z < 10; z++)
	            {
	                for (int y = 0; y < 10; y++)
	                {
	                    for (int x = 0; x < 10; x++)
	                    {
	                        //var d = vectorDist;
	                        //instance[x + y * 10 + z * 100].GetComponent<MeshRenderer>().material.color =
	                        //new Color(0f, 0f, 0f, 0.1f);
	                        instance[x + y * 10 + z * 100].SetActive(true);
	                    }
	                }
	            }
	        }
        }

    }
    public void DisableVectorField()
    {
        vectorField = 0;
        if (initialized)
        {
            for (int z = 0; z < 10; z++)
            {
                for (int y = 0; y < 10; y++)
                {
                    for (int x = 0; x < 10; x++)
                    {
                        //var d = vectorDist;
                        //instance[x + y * 10 + z * 100].GetComponent<MeshRenderer>().material.color =
                        //new Color(0f, 0f, 0f, 0.1f);
                        instance[x + y * 10 + z * 100].SetActive(false);
                    } //x
                } //y
            } //z
        } //if initialized
    }

    //Increases or Decreases space between vectors
    public void IncreaseSpacing()
    {
        vectorDist *= 1.5f;
		max_Velocity_x = 1f;
		max_Velocity_y = 1f;
		max_Velocity_z = 1f;
        UpdateVectorDist();
    }
    public void DecreaseSpacing()
    {
        vectorDist /= 1.5f;
		max_Velocity_x = 1f;
		max_Velocity_y = 1f;
		max_Velocity_z = 1f;
        UpdateVectorDist();
    }
    private void UpdateVectorDist()
    {
        if (initialized)
        {
            for (int z = 0; z < 10; z++)
            {
                for (int y = 0; y < 10; y++)
                {
                    for (int x = 0; x < 10; x++)
                    {
                        var d = vectorDist;
                        instance[x + y * 10 + z * 100].transform.position =
                            new Vector3(x * d - d * 5, y * d, z * d);
                    }
                }
            }
        }
    }
}

}