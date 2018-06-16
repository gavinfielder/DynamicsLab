using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicsLab.Peripherals;

public class PositionTextRenderer : MonoBehaviour
{
    //Set this to have the position text follow an object
    public GameObject Follows
    {
        set
        {
            myObject = value;
            if (value != null)
            {
                attached = true;
            }
            else
            {
                attached = false;
            }
        }
    }

    //Private fields
    private GameObject myObject; //follows this object
    private TextMesh text;
    private bool attached = false;
    private UpdateFrequencyManager updateFrequencyManager;
    private const float UPDATE_FREQUENCY = 2f;
    private Camera cameran; 

    // Use this for initialization
    void Start () {
        text = GetComponent<TextMesh>();
        updateFrequencyManager = new UpdateFrequencyManager(UPDATE_FREQUENCY);
        cameran = GameObject.Find("GVR Player").GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        if (updateFrequencyManager.Ready() && attached)
        {
            if (myObject == null)
            {
                //Object was deleted. No more updates necessary
                gameObject.SetActive(false);
                enabled = false;
                return;
            }
            //Update position text
            text.text = myObject.transform.position.ToString();
            //Update position
            transform.position = new Vector3
                (myObject.transform.position.x,
                myObject.transform.position.y + 1,
                myObject.transform.position.z);
            //Make sure we're looking at the current camera

            if ((Camera.current != null) && (cameran != Camera.current))
            {
                cameran = Camera.current;
            }
            transform.LookAt(
                transform.position + cameran.transform.rotation * Vector3.forward,
                cameran.transform.rotation * Vector3.up);
        }
    }
}
