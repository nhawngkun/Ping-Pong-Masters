using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager_PingPongMasters : Singleton<SoundManager_PingPongMasters>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip[] VFXSound;

    [Header("Volume Settings")]
    [SerializeField][Range(0f, 1f)] private float musicVolume = 0.3f;
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 0.5f;

    private bool _turnOn = true;
    public bool TurnOn
    {
        get { return _turnOn; }
        set
        {
            _turnOn = value;
            UpdateAudioVolumes();
        }
    }

    // Key để lưu PlayerPrefs
    private const string MUSIC_VOLUME_KEY = "Ms";
    private const string SFX_VOLUME_KEY = "SFX";

    // 🔊 Biến theo dõi sound đang loop
    private int currentLoopingSoundIndex = -1;

    public override void Awake()
    {
        base.Awake();
        LoadVolume();
        InitializeAudio();
        _turnOn = true;
    }

    private void LoadVolume()
    {
        musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, musicVolume);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, sfxVolume);
    }

    private void InitializeAudio()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = true;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false; // 🔊 Mặc định không loop
            sfxSource.playOnAwake = false;
        }

        UpdateAudioVolumes();

        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    private void UpdateAudioVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = _turnOn ? musicVolume : 0f;
        }
        if (sfxSource != null)
        {
            sfxSource.volume = _turnOn ? sfxVolume : 0f;
        }
    }

    /// <summary>
    /// Phát sound một lần (PlayOneShot) - dùng cho jump, pickup, etc.
    /// </summary>
    public void PlayVFXSound(int soundIndex)
    {
        if (VFXSound != null && soundIndex >= 0 && soundIndex < VFXSound.Length)
        {
            sfxSource.PlayOneShot(VFXSound[soundIndex], _turnOn ? sfxVolume : 0f);
        }
    }

    /// <summary>
    /// Phát sound loop liên tục - dùng cho âm thanh di chuyển
    /// </summary>
    public void PlayLoopingSound(int soundIndex)
    {
        if (VFXSound == null || soundIndex < 0 || soundIndex >= VFXSound.Length)
            return;

        // Nếu đang phát cùng sound này rồi thì không cần làm gì
        if (currentLoopingSoundIndex == soundIndex && sfxSource.isPlaying)
            return;

        // Dừng sound cũ và phát sound mới
        sfxSource.Stop();
        sfxSource.loop = true;
        sfxSource.clip = VFXSound[soundIndex];
        sfxSource.volume = _turnOn ? sfxVolume : 0f;
        sfxSource.Play();

        currentLoopingSoundIndex = soundIndex;
        Debug.Log($"🔊 Bắt đầu loop sound {soundIndex}");
    }

    /// <summary>
    /// Dừng sound đang loop
    /// </summary>
    public void StopLoopingSound()
    {
        if (currentLoopingSoundIndex != -1)
        {
            sfxSource.Stop();
            sfxSource.loop = false;
            sfxSource.clip = null;
            Debug.Log($"🔊 Dừng loop sound {currentLoopingSoundIndex}");
            currentLoopingSoundIndex = -1;
        }
    }

    /// <summary>
    /// Kiểm tra xem có sound nào đang loop không
    /// </summary>
    public bool IsLoopingSoundPlaying()
    {
        return currentLoopingSoundIndex != -1 && sfxSource.isPlaying;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null && _turnOn)
        {
            musicSource.volume = musicVolume;
        }

        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null && _turnOn)
        {
            sfxSource.volume = sfxVolume;
        }

        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.Save();
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }
}