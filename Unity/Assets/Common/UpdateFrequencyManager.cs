using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DynamicsLab.Peripherals
{
    //Handles nonstandard refresh rates
    class UpdateFrequencyManager
    {
        //Fields
        private float period; //seconds per cycle
        private float timeOfLastUpdate;

        //Gets or sets a frequency of update in cycles per second
        public float UpdateFrequency
        {
            get
            {
                return (1f / period);
            }
            set
            {
                period = 1f / value;
            }
        }

        //Constructor requires a an initial frequency
        public UpdateFrequencyManager(float frequency)
        {
            UpdateFrequency = frequency;
            timeOfLastUpdate = -1; //sentinel value. Should flag for update immediately
        }

        //Returns true if ready for an update, false if not ready
        public bool Ready()
        {
            if (Time.time > timeOfLastUpdate + period)
            {
                timeOfLastUpdate = Time.time;
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
