using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ClickSound : MonoBehaviour
{
    public AudioClip mouseClick;

    private Button ButtonStart { get { return GetComponent<Button>(); } }

    private AudioSource source { get { return GetComponent<AudioSource>(); } }

    // Use this for initialization

    void Start()

    {

        gameObject.AddComponent<AudioSource>();

        source.clip = mouseClick;

        source.playOnAwake = false;
        
        ButtonStart.onClick.AddListener(() => PlaySound());
    }

    void PlaySound()

    {
        if (mouseClick != null)
        {
            source.PlayOneShot(mouseClick);
        }       
    }
}
