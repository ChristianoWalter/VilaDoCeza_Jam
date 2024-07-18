using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    // �rea para acesso dos audio mixers
    [Header("Mixers Groups")]
    [SerializeField] AudioMixer mixer;

    // �rea para acesso dos sliders
    [Header("Sliders Groups")]
    [SerializeField] Slider generalVolSlider;
    [SerializeField] Slider musicVolSlider;
    [SerializeField] Slider VFXVolSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("GeneralVolume"))
        {
            LoadMusicPrefs();
        }
        else
        {
            SetGeneralVolume();
            SetMusicVolume();
            SetVFXVolume();
        }
    }

    // M�todo respons�vel pela mudan�a de volume do som geral
    public void SetGeneralVolume()
    {
        float volume = generalVolSlider.value;
        mixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

        // Salva �ltima configura��o de som definida pelo jogador
        PlayerPrefs.SetFloat("GeneralVolume", volume);
    }
    
    // M�todo respons�vel pela mudan�a de volume do som da m�sica
    public void SetMusicVolume()
    {
        float volume = musicVolSlider.value;
        mixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);

        // Salva �ltima configura��o de som definida pelo jogador
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
    
    // M�todo respons�vel pela mudan�a de volume do som da m�sica
    public void SetVFXVolume()
    {
        float volume = VFXVolSlider.value;
        mixer.SetFloat("VFXVolume", Mathf.Log10(volume) * 20);

        // Salva �ltima configura��o de som definida pelo jogador
        PlayerPrefs.SetFloat("VFXVolume", volume);
    }

    // M�todo para carregar �ltima configura��o de som definida pelo jogador
    public void LoadMusicPrefs()
    {
        generalVolSlider.value = PlayerPrefs.GetFloat("GeneralVolume");
        musicVolSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        VFXVolSlider.value = PlayerPrefs.GetFloat("VFXVolume");

        SetGeneralVolume();
        SetMusicVolume();
        SetVFXVolume();
    }
}
