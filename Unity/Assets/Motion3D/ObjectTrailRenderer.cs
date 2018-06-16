using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicsLab.Peripherals;
using DynamicsLab.DefaultColors;


public class ObjectTrailRenderer : MonoBehaviour {

    //Fields
    private TrailRenderer trail;

    // Use this for initialization
    void Start()
    {
        //Instantiate components
        gameObject.AddComponent<TrailRenderer>();
        trail = gameObject.GetComponent<TrailRenderer>();

        //Set material
        trail.material = new Material(Shader.Find("Particles/Additive"));

        //Set colors
        Color startColor = DynamicsLabColors.GetRandomColor();
        Color endColor = DynamicsLabColors.GetRandomColor();
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(startColor, 0.0f), new GradientColorKey(endColor, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
            );
        trail.colorGradient = gradient;

        //Set width
        trail.startWidth = 0.16f;
        trail.endWidth = 0.08f;

        //Set trail size and resolution parameters
        trail.minVertexDistance = 0.05f;
        trail.time = 6f; //seconds to fade out
    }

    //Resets the object trail
    public void ResetTrail()
    {
        trail.Clear();
    }

    //Update is called once per frame
    void Update() { }
}
