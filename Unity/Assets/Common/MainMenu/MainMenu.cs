using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    
    //Gui Handlers
    public void SpringMassButton_OnPress()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void NBodyButton_OnPress()
    {
        SceneManager.LoadScene("NBody");
    }

    public void Motion3DButton_OnPress()
    {
        SceneManager.LoadScene("3dMotion");
    }

    public void CloseMenuButton_OnPress()
    {
        gameObject.SetActive(false);
    }

    public void QuitButton_OnPress()
    {
        Application.Quit();
    }
}
