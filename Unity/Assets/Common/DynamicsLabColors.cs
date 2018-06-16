using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicsLab.DefaultColors
{
    //Defines the default Motion3DProblem
    static class DynamicsLabColors
    {
        static private Vector4[] colors =
        {
            new Vector4(1.0f, 0.0f, 0.0f, 1.0f), //red
            new Vector4(0.0f, 1.0f, 0.0f, 1.0f), //bright green
            new Vector4(0.0f, 0.0f, 1.0f, 1.0f), //blue
            new Vector4(0.0f, 0.68f, 1.0f, 1.0f), //green
            new Vector4(1.0f, 0.0f, 1.0f, 1.0f), //magenta
            new Vector4(0.0f, 1.0f, 1.0f, 1.0f), //cyan
            new Vector4(1.0f, 1.0f, 0.0f, 1.0f), //yellow
            new Vector4(1.0f, 0.6f, 0.0f, 1.0f), //orange
            new Vector4(0.68f, 0.1f, 0.85f, 1.0f), //purple
            new Vector4(0.65f, 1.0f, 0.0f, 1.0f), //yellow green
            new Vector4(0.2f,0.8f,0.67f,1.0f), //teal
            new Vector4(0.5f,0.6f,0.75f,1.0f), //steel blue
        };

        public static Color GetRandomColor()
        {
            int index = (int)(Random.value * colors.Length);
            return new Color(colors[index][0], colors[index][1], colors[index][2], colors[index][3]);
        }
    }
}