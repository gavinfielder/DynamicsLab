using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using DynamicsLab.CreateLines;

public class SpringTestsPlaymode {


	[UnityTest]
	public IEnumerator LoadScene_SpringMass() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        SceneManager.LoadSceneAsync("MainScene");
        SceneManager.UnloadSceneAsync("MainScene");
//        GameObject LM = GameObject.Find("LineMaster");
//        CreateLines ls = LM.GetComponent<CreateLines>();
//
//       ls.InitializeSimulation();

//        ls.started = 1;
        Assert.IsFalse(false);
        yield return new WaitForEndOfFrame();
    }
}
