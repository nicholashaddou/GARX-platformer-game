using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public static bool onSettingsPage = false;
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;
    private Scene activeScene;

    //mainMenu mainMenu;
    private void Start()
    {
        activeScene = SceneManager.GetActiveScene();
        Cursor.visible = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.visible = false;
        //Debug.Log("resuming");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.visible = true;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(activeScene.name);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    //-----------------------------------
    public void goToSettings()
    {
        //DO NOT GO TO MAIN MENU SETTINGS, MAKE OWN SETTINGS CANVAS FOR EACH LEVEL
        if (onSettingsPage == false)
        {
            enableSettingsWindow();
        }
        else
        {
            backToPauseMenu();
        }
    }
    public void enableSettingsWindow()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }
    public void backToPauseMenu()
    {
        onSettingsPage = false;
        if(onSettingsPage == false)
        {
            settingsMenuUI.SetActive(false);
            pauseMenuUI.SetActive(true);
        }
    }
    //----------------------------
    public void LoadMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void QuitGameFromPause()
    {
        Application.Quit();
    }

}
