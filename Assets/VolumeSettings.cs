using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    // Área para acesso dos audio mixers
    [Header("Mixers Groups")]
    [SerializeField] AudioMixer mixer;

    // Área para acesso dos sliders
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

    // Método responsável pela mudança de volume do som geral
    public void SetGeneralVolume()
    {
        float volume = generalVolSlider.value;
        mixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);

        // Salva última configuração de som definida pelo jogador
        PlayerPrefs.SetFloat("GeneralVolume", volume);
    }
    
    // Método responsável pela mudança de volume do som da música
    public void SetMusicVolume()
    {
        float volume = musicVolSlider.value;
        mixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);

        // Salva última configuração de som definida pelo jogador
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
    
    // Método responsável pela mudança de volume do som da música
    public void SetVFXVolume()
    {
        float volume = VFXVolSlider.value;
        mixer.SetFloat("VFXVolume", Mathf.Log10(volume) * 20);

        // Salva última configuração de som definida pelo jogador
        PlayerPrefs.SetFloat("VFXVolume", volume);
    }

    // Método para carregar última configuração de som definida pelo jogador
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
