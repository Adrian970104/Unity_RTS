using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void PlayGame()
	{
	    SceneManager.LoadScene("MapMenuScene");
	}

    public void SelectScene()
    {
        var map = EventSystem.current.currentSelectedGameObject;

        if (map != null)
            SceneManager.LoadScene(map.name);
        else
            Debug.Log("currentSelectedGameObject is null");
    }

    public void QuitGame()
	{
	    Application.Quit();
	}
}
