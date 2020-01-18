using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Range(0, 1)]
    public float sfxVolume;
    [Range(0, 1)]
    public float backgroundVolume;

    private BackgroundMusic backgroundMusic;

    private List<AudioSource> currentSounds = new List<AudioSource>();

    #region Singleton
    public static SoundManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }else
        {
            Destroy(this);
        }
    }
    #endregion

    private void Start()
    {
        backgroundMusic = GetComponent<BackgroundMusic>();
    }

    private void Update()
    {
        List<AudioSource> toRemove = new List<AudioSource>();
        foreach(AudioSource sound in currentSounds)
        {
            if(!sound.isPlaying)
            {
                toRemove.Add(sound);
            }
        }
        foreach(AudioSource sound in toRemove)
        {
            currentSounds.Remove(sound);
            Destroy(sound);
        }
    }
    public void ChangeVolumes(float musicVolume, float sfxVolume)
    {
        ChangeBackgroundVolume(musicVolume);
        ChangeSfxVolume(sfxVolume);
    }
    public void ChangeSfxVolume(float newValue)
    {
        sfxVolume = newValue;
        ApplyVolumes();
    }
    public void ChangeBackgroundVolume(float newValue)
    {
        if (backgroundMusic != null && backgroundMusic.backgroundSource != null)
            backgroundMusic.backgroundSource.volume = backgroundVolume = newValue;
    }
    public void ApplyVolumes()
    {
        foreach (AudioSource sound in currentSounds)
        {
            sound.volume = sfxVolume;
        }
        if (backgroundMusic != null && backgroundMusic.backgroundSource != null)
            backgroundMusic.backgroundSource.volume = backgroundVolume;
    }
    public void PlayNewSound(AudioType type, bool loop = false)
    {
        AudioSource newSound = gameObject.AddComponent<AudioSource>();
        newSound.clip = AudioAssigner.Instance.GetAudio(type);
        newSound.volume = sfxVolume;
        newSound.Play();
        newSound.loop = loop;
        currentSounds.Add(newSound);
    }
}
