using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motion3DPlayerController : MonoBehaviour {

    //public references set in inspector
    public Rigidbody rigidBody;
    public Camera playerCamera;

    //Fields
    private Vector3 direction; //tracks which direction the player is going
    private float speedMult = 1.0f;


    // Update is called once per frame
    void Update() {
        //Get movement input
        if (Input.GetKeyDown("up")) //go forward
        {
            DirectionInput(Vector3.forward);
        }
        if (Input.GetKeyDown("down")) //go back
        {
            DirectionInput(Vector3.back);
        }
        if (Input.GetKeyDown("left")) //go left
        {
            DirectionInput(Vector3.left);
        }
        if (Input.GetKeyDown("right")) //go right
        {
            DirectionInput(Vector3.right);
        }
        if (Input.GetKeyDown("backspace")) //go right
        {
            DirectionInput(Vector3.zero);
        }
    }

    //Accepts direction input and decides what to do with it
    private void DirectionInput(Vector3 dirInput)
    {
        if (dirInput == direction)
        {
            //Same direction pressed twice. Speed up.
            speedMult *= 1.5f;
            rigidBody.velocity = playerCamera.transform.rotation * direction * speedMult;
        }
        else
        {
            //New direction. Reset speed and set new velocity
            speedMult = 1.0f;
            direction = dirInput;
            rigidBody.velocity = playerCamera.transform.rotation * direction * speedMult;
        }
    }
    
}
