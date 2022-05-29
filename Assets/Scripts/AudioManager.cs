using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource1, _audioSource2;
    [SerializeField] private AudioClip[] _audioClips;

    private int _currentSong = -1;
    private bool _secondaryAudioSource = false;
    public static AudioManager instance;
    [SerializeField] private AudioMixer _audioMixer;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            if (_audioClips.Length > 0)
            {
                _audioSource1.clip = _audioClips[0];
                _audioSource1.Play();
            }
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public float ConvertToDecibel(float _value)
    {
        return Mathf.Log10(Mathf.Max(_value, 0.0001f))*20f;
    }

    public void ChangeSong(int index, float fadeOutTime = 0.2f, float fadeInTime = 0.2f)
    {
        if (index != -1)
        {
            CrossFade(index, fadeOutTime, fadeInTime);
        }
        else
        {
            StartCoroutine(FadeOut(fadeOutTime));
        }
    }

    public void SetSpeed(int speed)
    {
        _audioSource1.pitch = speed;
        _audioSource2.pitch = speed;
    }

    private IEnumerator FadeOut(float fadeOutTime)
    {
        AudioSource affectedAudioSource;
        if (!_secondaryAudioSource) affectedAudioSource = _audioSource1;
        else affectedAudioSource = _audioSource2;

        float elapsedTime = 0;
        while (elapsedTime < fadeOutTime)
        {
            affectedAudioSource.volume = 1 - (elapsedTime/fadeOutTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        affectedAudioSource.volume = 0;
        affectedAudioSource.Stop();
    }
    
    private IEnumerator FadeIn(int index, float fadeInTime)
    {
        AudioSource affectedAudioSource;
        if (!_secondaryAudioSource) affectedAudioSource = _audioSource2;
        else affectedAudioSource = _audioSource1;

        affectedAudioSource.clip = _audioClips[index];
        affectedAudioSource.volume = 0;
        affectedAudioSource.Play();

        float elapsedTime = 0;
        while (elapsedTime < fadeInTime)
        {
            affectedAudioSource.volume = (elapsedTime/fadeInTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        affectedAudioSource.volume = 1;
        _secondaryAudioSource = !_secondaryAudioSource;
    }

    private void CrossFade(int index, float fadeOutTime, float fadeInTime)
    {
        StartCoroutine(FadeOut(fadeOutTime));
        StartCoroutine(FadeIn(index, fadeInTime));
    }

    public void SetVolumes(float globalVol, float musicVol, float sfxVol)
    {
        _audioMixer.SetFloat("Audio", ConvertToDecibel(globalVol));
        _audioMixer.SetFloat("Music", ConvertToDecibel(musicVol));
        _audioMixer.SetFloat("Sounds", ConvertToDecibel(sfxVol));
    }
}
