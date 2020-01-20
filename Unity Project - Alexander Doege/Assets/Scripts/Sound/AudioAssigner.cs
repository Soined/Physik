using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAssigner : MonoBehaviour
{
    public AudioClip GunShot;

    public static AudioAssigner Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this);
        }
    }

    public AudioClip GetAudio(AudioType type)
    {
        switch(type)
        {
            case AudioType.gunShot:
                return GunShot;
            default:
                Debug.LogError($"AudioType \"{type}\" has not been assigned in \"GetAudio()\"");
                break;
        }
        return null;
    }
}

public enum AudioType
{
    gunShot,
}
