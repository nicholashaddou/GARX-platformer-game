using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+2);
    }

    public void settings()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void goCredits()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 17);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
