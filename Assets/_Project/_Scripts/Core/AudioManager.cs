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

    // ------------------ SINGLETON ------------------
    public static AudioManager Instance;

    [Header("Danh sách âm thanh")]
    public AudioClip windSound;
    public AudioClip slashEnemy;
    public AudioClip slashBoss;
    public AudioClip potion;
    public AudioClip hitTrap;
    public AudioClip lava;
    public AudioClip victory;
    public AudioClip gameOver;
    public AudioClip nextStage;
    public AudioClip takeDamage;

    public AudioClip walk;
    public AudioClip jump;
    public AudioClip fall;
    public AudioClip pauseOn;
    public AudioClip pauseOff;
    public AudioClip bossSkill1;
    public AudioClip bossSkill2;
    public AudioClip bossSkill3;
    public AudioClip hitTower;
    public AudioClip bushWalk;
    public AudioClip menuBGM; // <<< THÊM MỚI: Nhạc nền Menu
    public AudioClip level1BGM; // <<< THÊM MỚI: Nhạc nền Level 1

    private AudioSource sfxPlayer;
    private AudioSource oneShotAreaPlayer;
    private AudioSource musicPlayer; // <<< THÊM MỚI: AudioSource cho Music/BGM

    // ------------------ QUẢN LÝ TRẠNG THÁI ÂM THANH ------------------
    public enum SoundType
    {
        None,
        Jump,
        Fall,
        Walk,
        Damage,
        BossSkill,
        Other
    }

    private SoundType currentSoundType = SoundType.None;

    private void Awake()
    {
        Instance = this;

       

        sfxPlayer = gameObject.AddComponent<AudioSource>();
        oneShotAreaPlayer = gameObject.AddComponent<AudioSource>();
        oneShotAreaPlayer.loop = false;

        musicPlayer = gameObject.AddComponent<AudioSource>();
        musicPlayer.loop = true;

        var sfxGroups = SFXMixer.FindMatchingGroups("Master");
        var musicGroups = musicMixer.FindMatchingGroups("Master");

        if (sfxGroups.Length > 0)
        {
            sfxPlayer.outputAudioMixerGroup = sfxGroups[0];
            oneShotAreaPlayer.outputAudioMixerGroup = sfxGroups[0];
            musicPlayer.outputAudioMixerGroup = musicGroups[0];
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
            SetVolume(MusicSlider.value);
        else
            musicMixer.SetFloat("MusicVolume", -80f);
    }

    private void OnToggleSFX(bool isOn)
    {
        if (SFXSlider != null)
            SFXSlider.interactable = isOn;
        if (isOn)
            SetSFXVolume(SFXSlider.value);
        else
            SFXMixer.SetFloat("SFXVolume", -80f);
    }

    // ------------------ HỆ THỐNG PHÁT ÂM THANH ------------------
    private void PlaySFX(AudioClip clip, SoundType type)
    {
        if (clip == null) return;

        // Nếu đang phát loại khác thì dừng luôn
        if (currentSoundType != type && sfxPlayer.isPlaying)
        {
            sfxPlayer.Stop();
        }

        sfxPlayer.clip = clip;
        sfxPlayer.Play();
        currentSoundType = type;
    }
    public void StopMusic()
    {
        if (musicPlayer != null && musicPlayer.isPlaying)
            musicPlayer.Stop();
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicPlayer == null) return;
        StopMusic();
        musicPlayer.clip = clip;
        musicPlayer.Play();
    }

    public void StopCurrentSound()
    {
        if (sfxPlayer.isPlaying)
            sfxPlayer.Stop();
        currentSoundType = SoundType.None;
    }

    // ------------------ PHÁT ÂM THANH KHU VỰC ------------------
    public void PlayAreaSoundOnce(AudioClip clip)
    {
        if (clip == null || oneShotAreaPlayer.isPlaying) return;
        oneShotAreaPlayer.clip = clip;
        oneShotAreaPlayer.Play();
    }

    public void StopAreaSound()
    {
        if (oneShotAreaPlayer.isPlaying)
            oneShotAreaPlayer.Stop();
    }

    // ------------------ CÁC HÀM GỌI NHANH ------------------
    public void PlayWind() => PlaySFX(windSound, SoundType.Other);
    public void PlaySlashEnemy() => PlaySFX(slashEnemy, SoundType.Other);
    public void PlaySlashBoss() => PlaySFX(slashBoss, SoundType.BossSkill);
    public void PlayPotion() => PlaySFX(potion, SoundType.Other);
    public void PlayTrap() => PlaySFX(hitTrap, SoundType.Other);
    public void PlayLava() => PlaySFX(lava, SoundType.Other);
    public void PlayVictory() => PlaySFX(victory, SoundType.Other);
    public void PlayGameOver() => PlaySFX(gameOver, SoundType.Other);
    public void PlayNextStage() => PlaySFX(nextStage, SoundType.Other);
    public void PlayTakeDamage() => PlaySFX(takeDamage, SoundType.Damage);

    public void PlayWalk() => PlaySFX(walk, SoundType.Walk);
    public void PlayJump() => PlaySFX(jump, SoundType.Jump);
    public void PlayFall() => PlaySFX(fall, SoundType.Fall);
    public void PlayPauseOn() => PlaySFX(pauseOn, SoundType.Other);
    public void PlayPauseOff() => PlaySFX(pauseOff, SoundType.Other);
    public void PlayBossSkill1() => PlaySFX(bossSkill1, SoundType.BossSkill);
    public void PlayBossSkill2() => PlaySFX(bossSkill2, SoundType.BossSkill);
    public void PlayBossSkill3() => PlaySFX(bossSkill3, SoundType.BossSkill);
    public void PlayHitTower() => PlaySFX(hitTower, SoundType.Other);
    public void PlayBushWalk() => PlaySFX(bushWalk, SoundType.Walk);
}
