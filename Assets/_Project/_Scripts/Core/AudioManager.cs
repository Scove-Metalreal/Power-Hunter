using BloodlinesUI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
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
    public AudioClip victory;            // victory.mp3
    public AudioClip gameOver;           // Game over.mp3
    public AudioClip nextStage;          // qua màn.mp3
    public AudioClip takeDamage;         // >>> MỚI THÊM: nhận damage

    private AudioSource sfxPlayer;
    private AudioSource oneShotAreaPlayer; // >>> MỚI THÊM: dùng cho vùng âm thanh chỉ phát 1 lần

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // tạo sfxPlayer để phát hiệu ứng nhanh
            sfxPlayer = gameObject.AddComponent<AudioSource>();

            // >>> MỚI THÊM
            oneShotAreaPlayer = gameObject.AddComponent<AudioSource>();
            oneShotAreaPlayer.loop = false; // phát một lần rồi dừng

            // Tìm AudioMixerGroup "Master" để gán cho AudioSource
            UnityEngine.Audio.AudioMixerGroup[] sfxGroups = SFXMixer.FindMatchingGroups("Master");

            if (sfxGroups.Length > 0)
            {
                sfxPlayer.outputAudioMixerGroup = sfxGroups[0];
                oneShotAreaPlayer.outputAudioMixerGroup = sfxGroups[0];
            }
            else
            {
                Debug.LogError("AudioManager: KHÔNG TÌM THẤY AudioMixerGroup CÓ TÊN 'SFX' trong SFXMixer!");
            }
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

    // ------------------ QUẢN LÝ PHÁT ÂM THANH ------------------

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxPlayer != null)
            sfxPlayer.PlayOneShot(clip);
    }

    // >>> MỚI THÊM: phát âm thanh chỉ một lần khi player vào vùng
    public void PlayAreaSoundOnce(AudioClip clip)
    {
        if (clip == null || oneShotAreaPlayer.isPlaying) return;
        oneShotAreaPlayer.clip = clip;
        oneShotAreaPlayer.Play();
    }

    // >>> MỚI THÊM: dừng vùng âm thanh khi ra khỏi
    public void StopAreaSound()
    {
        if (oneShotAreaPlayer.isPlaying)
            oneShotAreaPlayer.Stop();
    }

    // ------------------ CÁC HÀM GỌI NHANH ------------------
    public void PlayWind() => PlaySFX(windSound);
    public void PlaySlashEnemy() => PlaySFX(slashEnemy);
    public void PlaySlashBoss() => PlaySFX(slashBoss);
    public void PlayPotion() => PlaySFX(potion);
    public void PlayTrap() => PlaySFX(hitTrap);
    public void PlayLava() => PlaySFX(lava);
    public void PlayVictory() => PlaySFX(victory);
    public void PlayGameOver() => PlaySFX(gameOver);
    public void PlayNextStage() => PlaySFX(nextStage);

    // >>> MỚI THÊM: âm thanh nhận damage
    public void PlayTakeDamage() => PlaySFX(takeDamage);
}
