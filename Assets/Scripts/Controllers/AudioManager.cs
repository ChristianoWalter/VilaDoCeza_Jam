using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audios Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audios Clip")]
    public AudioClip[] SFXClips;
    public AudioClip[] musicClips;

    public void ChangeMusic(int _song)
    {
        musicSource.Stop();
        musicSource.clip = musicClips[_song];
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayMusic()
    {
        musicSource.Play();
    }

    public void PlayVFX(AudioClip _sfx)
    {
        SFXSource.PlayOneShot(_sfx);
    }
}
