using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audios Sourc")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audios Clip")]
    public AudioClip[] SFXClips;
    public AudioClip[] musicClips;

    public void ChangeMusic(int _song)
    {
        musicSource.Stop();

        musicSource.PlayOneShot(musicClips[_song]);
    }
}
