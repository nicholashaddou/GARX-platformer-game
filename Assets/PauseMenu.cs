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
    MobileHealthController2D mobileHealthController2D;
    //private Scene mainMenu;

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
        //mainMenu = SceneManager.GetSceneByBuildIndex(0);
        SceneManager.LoadScene(0);
    }

    public void QuitGameFromPause()
    {
        Application.Quit();
    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(mobileHealthController2D);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        mobileHealthController2D.level = data.level;
        mobileHealthController2D.health = data.health;

        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];

        mobileHealthController2D.transform.position = position;
        Debug.Log("level is = " + mobileHealthController2D.level);
    }

}
