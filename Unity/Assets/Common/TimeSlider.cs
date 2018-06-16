using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSlider : MonoBehaviour {

    Slider slider;
    Text lowerBoundText;
    Text upperBoundText;

    public float Value
    {
        get
        {
            return slider.value;
        }
        set
        {
            slider.value = value;
        }
    }

    public double LowerBound
    {
        set
        {
            lowerBoundText.text = value.ToString("####0.00");
            slider.minValue = (float)value;
        }
    }

    public double UpperBound
    {
        set
        {
            upperBoundText.text = value.ToString("####0.00");
            slider.maxValue = (float)value;
        }
    }

    // Use this for initialization
    void Start () {
        slider = GameObject.Find("TimeSlider").GetComponent<Slider>();
        lowerBoundText = GameObject.Find("LowerBoundDisplay").GetComponent<Text>();
        upperBoundText = GameObject.Find("UpperBoundDisplay").GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
