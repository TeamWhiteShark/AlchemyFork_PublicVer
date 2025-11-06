using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource BgmSource;
    public AudioSource SfxSource;    
    public Slider bgmSlider;
    public Slider sfxSlider;

    public void Start()
    {
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;

        BgmSource.volume = bgmVolume;
        SfxSource.volume = sfxVolume;

        bgmSlider.onValueChanged.AddListener((value) =>
        {
            BgmSource.volume = value;
            PlayerPrefs.SetFloat("BGMVolume", value);
        });

        sfxSlider.onValueChanged.AddListener((value) =>
        {
            SfxSource.volume = value;
            PlayerPrefs.SetFloat("SFXVolume", value);
        });
    }   
}

