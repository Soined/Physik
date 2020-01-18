using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioClip backgroundMusic;

    [HideInInspector]
    public AudioSource backgroundSource;

    private void Start()
    {
        if(backgroundMusic != null)
        {
            backgroundSource = gameObject.AddComponent<AudioSource>();
            backgroundSource.clip = backgroundMusic;
            backgroundSource.loop = true;
            backgroundSource.Play();
        }
    }
}
