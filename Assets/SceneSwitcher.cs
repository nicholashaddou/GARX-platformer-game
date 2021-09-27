using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SceneSwitcher : MonoBehaviour
{
    public int index;
    public string levelName;
    public Image black;
    public Animator anim;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator Fading(){
        anim.SetBool("fade", true);
        yield return new WaitUntil(()=>black.color.a==1);
        SceneManager.LoadScene(index);
    }
}
