
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioManager : MonoBehaviour
{
    public Slider MusicSlider;
    public Slider SFXSlider;
    public AudioMixer musicMixer;
    public AudioMixer SFXMixer;
   

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
            SFXMixer.GetFloat("SfxVolume", out volume);
            SFXSlider.value = Mathf.Pow(10, volume / 20f);
            SFXSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        
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
        SFXMixer.SetFloat("SfxVolume", dB);
    }
    
  

    
}

