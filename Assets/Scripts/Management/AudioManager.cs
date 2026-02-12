using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Sound Lists")]
    public Sound[] musicSounds;
    public Sound[] sfxSounds;

    [Header("Audio Sources")]
    public AudioSource mainMusicSource;
    public AudioSource ambientMusicSource;
    public AudioSource sfxSource;

    [Header("Start Music")]
    public string startMainMusicName;
    public string startAmbientMusicName;

    private void Awake()
    {
        // Tách khỏi parent để tránh lỗi DontDestroyOnLoad
        transform.parent = null;

        // Nếu chưa có instance thì gán và giữ lại
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // Nếu scene mới có AudioManager khác
            bool newHasMusic = !string.IsNullOrEmpty(startMainMusicName) || !string.IsNullOrEmpty(startAmbientMusicName);

            if (newHasMusic)
            {
                // Hủy bản cũ, dùng bản mới
                Destroy(instance.gameObject);
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // Scene mới không có nhạc → giữ nhạc cũ, hủy bản mới
                Destroy(gameObject);
                return;
            }
        }
    }

    private void Start()
    {
        // Nếu có tên nhạc được chỉ định thì phát
        if (!string.IsNullOrEmpty(startMainMusicName))
            PlayMusic(startMainMusicName, MusicType.Main);

        if (!string.IsNullOrEmpty(startAmbientMusicName))
            PlayMusic(startAmbientMusicName, MusicType.Ambient);
    }

    public enum MusicType { Main, Ambient }

    public void PlayMusic(string name, MusicType type)
    {
        Sound s = Array.Find(musicSounds, x => x.soundName == name);
        if (s == null)
        {
            Debug.LogWarning("Music not found: " + name);
            return;
        }

        AudioSource source = (type == MusicType.Main) ? mainMusicSource : ambientMusicSource;
        source.clip = s.soundClip;
        source.loop = true;
        source.Play();
    }

    public void StopMusic(MusicType type)
    {
        AudioSource source = (type == MusicType.Main) ? mainMusicSource : ambientMusicSource;
        source.Stop();
    }

    public void PlaySFX(string name, Transform spawnTransform = null)
    {
        Sound s = Array.Find(sfxSounds, x => x.soundName == name);
        if (s == null)
        {
            Debug.LogWarning("SFX not found: " + name);
            return;
        }

        AudioSource audioSource = Instantiate(sfxSource, spawnTransform ? spawnTransform.position : Vector3.zero, Quaternion.identity);
        audioSource.PlayOneShot(s.soundClip);
        Destroy(audioSource.gameObject, s.soundClip.length);
    }

    public void SetMusicVolume(float volume)
    {
        mainMusicSource.volume = volume;
        ambientMusicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
