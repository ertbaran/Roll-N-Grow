using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance = null;

    [SerializeField] private AudioSource _musicSource, _effectSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlaySoundEffect(AudioClip audioClip)
    {
        _effectSource.PlayOneShot(audioClip);
    }
    public void PauseSoundEffect()
    {
        _effectSource.Stop();
    }

    public void PlayMusic()
    {
        _musicSource.UnPause();
    }
    public void PauseMusic()
    {
        _musicSource.Pause();
    }
    public void SetMusicVolume(float volume)
    {
        _musicSource.volume = volume;
    }
}
