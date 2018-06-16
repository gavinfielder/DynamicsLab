using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {

	// Use this for initialization
	public void GotoMotion(){
		SceneManager.LoadScene("3dMotion");
	}
	public void GotoMotion2(){
		SceneManager.LoadScene("MainScene");
	}

}
