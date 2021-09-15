using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class canvasControl : MonoBehaviour
{
    public GameObject ButtonCanvas;
    // Start is called before the first frame update
    void Start()
    {
        //ButtonCanvas.SetActive(false);
        //buttonTimer();
    }

   /* public void buttonTimer()
    {
        float timeLeftUntilActive = 2;
        if (timeLeftUntilActive > 0)
        {
            timeLeftUntilActive -= Time.deltaTime;
        }
        else
        {
            timeLeftUntilActive = 0;
            enableCanvas();
            Debug.Log("working");
        }
    }
    public void enableCanvas()
    {
        ButtonCanvas.SetActive(true);
    }*/
    private void Update()
    {
        Debug.Log("update works");
        float timeLeftUntilActive = 2;
        if (timeLeftUntilActive > 0)
        {
            timeLeftUntilActive -= Time.deltaTime;
        }
        else
        {
            timeLeftUntilActive = 0;
            ButtonCanvas.SetActive(true);
            Debug.Log("working");
        }
    }
}
