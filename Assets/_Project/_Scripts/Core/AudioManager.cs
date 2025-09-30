
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class AudioManager : MonoBehaviour
{
    public Slider MusicSlider;
    public Slider SFXSlider;
    public AudioMixer musicMixer;
    public AudioMixer SFXMixer;
    public Toggle musicToggle;
    public Toggle SFXToggle;
   

    void Start()
    {
        if (MusicSlider != null)
        {
            float volume;
            musicMixer.GetFloat("MusicVolume", out volume);
            MusicSlider.value = Mathf.Pow(10, volume / 20f);
            MusicSlider.onValueChanged.AddListener(SetVolume);
        }
        if (SFXSlider != null)
        {
            float volume;
            SFXMixer.GetFloat("SFXVolume", out volume);
            SFXSlider.value = Mathf.Pow(10, volume / 20f);
            SFXSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        if (musicToggle != null)
        {
            musicToggle.onValueChanged.AddListener(OnToggleMusic);
        }
        if (SFXToggle != null)
        {
            SFXToggle.onValueChanged.AddListener(OnToggleSFX);
        }
        musicToggle.isOn = true;
        SFXToggle.isOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SetVolume(float value)
    {

        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        musicMixer.SetFloat("MusicVolume", dB);
    }
    void SetSFXVolume(float value)
    {

        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        SFXMixer.SetFloat("SFXVolume", dB);
    }


    private void OnToggleMusic(bool isOn)
    {
        if (MusicSlider != null)
            MusicSlider.interactable = isOn;
        if (isOn)
        {
            SetVolume(MusicSlider.value); 
        }
        else
        {
            musicMixer.SetFloat("MusicVolume", -80f); 
        }
    }

    private void OnToggleSFX(bool isOn)
    {
        if (SFXSlider != null)
            SFXSlider.interactable = isOn;
        if (isOn)
        {
            SetSFXVolume(SFXSlider.value);
        }
        else
        {
            SFXMixer.SetFloat("SFXVolume", -80f);
        }
    }

}

