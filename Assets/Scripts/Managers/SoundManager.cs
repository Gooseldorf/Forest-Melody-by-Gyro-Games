using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using SO_Scripts;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource soundsAudioSource;
    [SerializeField] private AudioMixerGroup soundsMixerGroup;
    private List<AudioSource> loopedAudioSources = new();
    private bool isSoundsEnabled;
    private Tweener musicGain;

    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        ToggleMusic(PlayerPrefs.GetInt(PrefKeys.Music, 1) == 1);
        ToggleSounds(PlayerPrefs.GetInt(PrefKeys.Sound, 1) == 1);
    }

    public void ToggleMusic(bool isActive)
    {
        if (isActive)
        {
            backgroundAudioSource.volume = 0;
            musicGain = backgroundAudioSource.DOFade(0.2f, 1).SetDelay(0.3f);
            backgroundAudioSource.Play();
            musicGain.Play();
        }
        else
        {
            if(musicGain != null) musicGain.Kill();
            backgroundAudioSource.Pause();
        }
    }

    public void ToggleSounds(bool isActive)
    {
        isSoundsEnabled = isActive;
    }

    public void PlaySound(string soundName)
    {
        if(!isSoundsEnabled)return;
        AudioClip sound = DataHolder.Instance.soundsData.Find(x => x.name == soundName);
        soundsAudioSource.PlayOneShot(sound);
    }

    public void PlaySound(string soundName, float duration)
    {
        if(!isSoundsEnabled)return;
        AudioClip sound = DataHolder.Instance.soundsData.Find(x => x.name == soundName);
        for (int i = 0; i < loopedAudioSources.Count; i++)
        {
            if (!loopedAudioSources[i].isPlaying)
            {
                loopedAudioSources[i].clip = sound;
                loopedAudioSources[i].Play();
                StartCoroutine(StopSoundAfterDuration(loopedAudioSources[i], duration));
                return;
            }
        }
        AudioSource source = gameObject.AddComponent<AudioSource>();
        loopedAudioSources.Add(source);
        source.outputAudioMixerGroup = soundsMixerGroup;
        source.loop = true;
        source.clip = sound;
        source.Play();
        StartCoroutine(StopSoundAfterDuration(source, duration));
    }
    
    public void PlayBirdSound()
    {
        if(!isSoundsEnabled)return;
        soundsAudioSource.PlayOneShot(DataHolder.Instance.birdSounds[Random.Range(0,3)], 0.5f);
    }
    
    private IEnumerator StopSoundAfterDuration(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        source.DOFade(0, 1.5f).OnComplete((() =>
        {
            source.Stop();
            source.volume = 1;
        }));
    }
}

