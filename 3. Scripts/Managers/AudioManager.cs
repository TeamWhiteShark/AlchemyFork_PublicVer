using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoSingleton<AudioManager>
{
    protected override bool isDestroy => false;

    [Header("BGM")]
    [SerializeField] private AudioSource BgmSource;

    [Header("SFX")]
    [SerializeField] private AudioSource SfxPrefab;  
    [SerializeField] private int sfxPoolSize = 10;
    private List<AudioSource> sfxSources = new List<AudioSource>();

    private Slider BgmSlider;
    private Slider SfxSlider;   

    public void Start()
    {
        BgmSource.Play();
        InitSfxPool();
        BgmSource.volume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        foreach (var sfx in sfxSources)
            sfx.volume = sfxVolume;             
    }

    private void InitSfxPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource sfx = Instantiate(SfxPrefab, transform);
            sfx.playOnAwake = false;
            sfx.volume = 0.5f;
            sfxSources.Add(sfx);
        }
    }

    public void SetSliders(Slider bgm, Slider sfx)
    {
        BgmSlider = bgm;
        BgmSlider.value = BgmSource.volume;
        BgmSlider.onValueChanged.AddListener((v) =>
        {
            BgmSource.volume = v;
            PlayerPrefs.SetFloat("BGMVolume", v); 
        });

        SfxSlider = sfx;
        SfxSlider.value = sfxSources[0].volume;
        SfxSlider.onValueChanged.AddListener((v) =>
        {
            foreach (var s in sfxSources)
                s.volume = v;

            PlayerPrefs.SetFloat("SFXVolume", v); 
        });
    }

    public void PlayBGM(AudioClip clip)
    {
        if (BgmSource.clip == clip) return;
        BgmSource.clip = clip;
        BgmSource.loop = true;
        BgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        AudioSource source = GetAvailableSfxSource();
        source.clip = clip;
        source.Play();
    }

    public void StopSFX(AudioClip clip)
    {
        foreach (var s in sfxSources)
        {
            if (s.clip == clip && s.isPlaying)
            {
                s.Stop();
                break;
            }
        }
    }

    private AudioSource GetAvailableSfxSource()
    {
        foreach (var s in sfxSources)
        {
            if (!s.isPlaying)
                return s;
        }
        
        return sfxSources[0];
    }
    public void PlaySFX3D(AudioClip clip, Vector3 position)
    {
        AudioSource source = GetAvailableSfxSource();
        source.transform.position = position;
        source.clip = clip;
        source.spatialBlend = 1.0f; // 3D 사운드
        source.minDistance = 1f;    // 가까이서 최대 볼륨
        source.maxDistance = 1.1f;   // 이 거리 이상이면 안 들림
        source.Play();
    }
}