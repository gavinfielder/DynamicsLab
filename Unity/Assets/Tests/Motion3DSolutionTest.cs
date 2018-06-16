using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;

public class Motion3DSolutionTest {

    /*
	[Test]
	public void Motion3DSolultionTestSimplePasses() {
		// Use the Assert class to test conditions.
	}
    */
    /*
	// A UnityTest behaves like a coroutine in PlayMode
	// and allows you to yield null to skip a frame in EditMode
	[UnityTest]
	public IEnumerator Motion3DLoadsControllers() {
        // Use the Assert class to test conditions.
        // yield to skip a frame

        //Load 3D Motion
        SceneManager.LoadScene("3dMotion");
        //Wait for setup to complete
		yield return null;
        yield return null;
        yield return null;
        yield return null;
        //Grab references to the controllers
        Motion3DSceneController sceneController
            = GameObject.Find("Motion3DController").GetComponent<Motion3DSceneController>();
        Motion3DSimulationController simController
            = GameObject.Find("Motion3DController").GetComponent<Motion3DSimulationController>();
        Assert.IsNotNull(sceneController);
        Assert.IsNotNull(simController);
        yield break;
    }*/
}
