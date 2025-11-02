using BloodlinesUI;
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

    // ------------------ PHẦN MỚI THÊM ------------------
    public static AudioManager Instance;

    [Header("Danh sách âm thanh")]
    public AudioClip windSound;          // Gió.mp3
    public AudioClip slashEnemy;         // chém enemy.mp3
    public AudioClip slashBoss;          // chém boss.mp3
    public AudioClip potion;             // Check potion.mp3
    public AudioClip hitTrap;            // chạm trap.mp3
    public AudioClip lava;               // dính lava.mp3
    public AudioClip victory;            // vitory.mp3
    public AudioClip gameOver;           // Game over.mp3
    public AudioClip nextStage;          // qua màn.mp3

    private AudioSource sfxPlayer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // tạo sfxPlayer để phát hiệu ứng nhanh
            sfxPlayer = gameObject.AddComponent<AudioSource>();
            sfxPlayer.outputAudioMixerGroup = SFXMixer.FindMatchingGroups("SFX")[0];
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
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

    // ------------------ HÀM MỚI QUẢN LÝ ÂM THANH ------------------

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxPlayer != null)
            sfxPlayer.PlayOneShot(clip);
    }

    // Các hàm gọi nhanh
    public void PlayWind() => PlaySFX(windSound);
    public void PlaySlashEnemy() => PlaySFX(slashEnemy);
    public void PlaySlashBoss() => PlaySFX(slashBoss);
    public void PlayPotion() => PlaySFX(potion);
    public void PlayTrap() => PlaySFX(hitTrap);
    public void PlayLava() => PlaySFX(lava);
    public void PlayVictory() => PlaySFX(victory);
    public void PlayGameOver() => PlaySFX(gameOver);
    public void PlayNextStage() => PlaySFX(nextStage);
}
