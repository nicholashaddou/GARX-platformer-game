using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MobileHealthController2D : MonoBehaviour
{
    Scene currentScene;
    private int index;
    public int level = 2;
    public int health = 100;

    public Slider healthSlider;

    public void setMaxHealth(int health)
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;
    }
    public void setPlayerHealth(int health)
    {
        healthSlider.value = health;
    }

    //for save/load
    /*private int passIndexValue()
    {
        currentScene = SceneManager.GetActiveScene();
        index = currentScene.buildIndex;
        level = index;
        Debug.Log("level is index = " + level);
        return level;
    }
    private void Update()
    {
        passIndexValue();       
    }*/
   //for save/load



}


    
