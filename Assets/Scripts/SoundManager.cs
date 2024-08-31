using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    [Header("Volume info")] [Space(20)] public float masterVol;
    public bool masterVolMute;
    [SerializeField] Sound currentTheme;

    [Space(20)] [Header("Sounds")] [Space(20)] [SerializeField]
    Sound[] sounds;

    public static SoundManager Instance;


    private void Awake()
    {
        Initialize();
        DontDestroyOnLoad(gameObject);
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        foreach (Sound s in sounds)
        {
            s.sound.source = gameObject.AddComponent<AudioSource>();
            s.sound.source.clip = s.sound.clip;
            s.sound.source.volume = s.sound.volume;
            s.sound.source.pitch = s.sound.pitch;
            s.sound.source.loop = s.sound.loop;
        }

        InitRandomTheme();
    }

    public AudioSource GetSourceOf(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        return s.sound.source;
    }

    public void MuteGeneral()
    {
        masterVol = 20;
        audioMixer.SetFloat("MasterVolume", -80f);
    }

    public void UnMuteGeneral()
    {
        audioMixer.SetFloat("MasterVolume", masterVol);
        masterVol = 20;
    }


    public void PauseLoopedAudios()
    {
        foreach (Sound s in sounds)
        {
            if (s.sound.loop)
            {
                Pause(s.name);
            }
        }
    }

    public void LowerCurrentTheme()
    {
        currentTheme.sound.volume -= 0.04f;
        currentTheme.sound.pitch -= 0.1f;
        foreach (AudioSource aus in GetComponents<AudioSource>())
        {
            if (aus.clip.name == currentTheme.sound.clip.name)
            {
                aus.volume = currentTheme.sound.volume;
                aus.pitch = currentTheme.sound.pitch;
            }
        }
    }

    public void RestoreCurrentTheme()
    {
        currentTheme.sound.volume += 0.04f;
        currentTheme.sound.pitch += 0.1f;
        foreach (AudioSource aus in GetComponents<AudioSource>())
        {
            if (aus.clip.name == currentTheme.sound.clip.name)
            {
                aus.volume = currentTheme.sound.volume;
                aus.pitch = currentTheme.sound.pitch;
            }
        }
    }

    private void InitRandomTheme()
    {
        var musicSounds = sounds.Where(sound => sound.sound.type == AudioTypeGroup.MUSIC).ToArray();
        if (musicSounds.Length > 0)
        {
            int randomIndex = new Random().Next(0, musicSounds.Length);
            var randomMusicSound = musicSounds[randomIndex].name;
            SetCurrentTheme(randomMusicSound);
        }
    }

    public void SetCurrentTheme(string theme)
    {
        currentTheme = Array.Find(sounds, sound => sound.name == theme);
        Play(theme, false, false, false);
        StartCoroutine(WaitToThemeEndToPlayNewOne(currentTheme.sound.clip.length));
    }

    private IEnumerator WaitToThemeEndToPlayNewOne(float duration)
    {
        Debug.Log(duration);
        yield return new WaitForSecondsRealtime(duration);
        InitRandomTheme();
    }


    public void Play(string name, bool checkIsPlaying,bool ignoreIfIsPlaying, bool randomizePitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sound {name} not found!");
            return;
        }
        if(ignoreIfIsPlaying && IsPlaying(name))
            return;

        if (checkIsPlaying && IsPlaying(name))
        {
            AudioSource source = s.sound.GetOrCreateAudioSource(gameObject);
            source.pitch = randomizePitch ? UnityEngine.Random.Range(1.0f, 1.9f) : s.sound.source.pitch;
            source.Play();
            StartCoroutine(s.sound.ReturnAudioSourceToPool(source, s.sound.clip.length));
            return;
        }

        s.sound.source.pitch = randomizePitch ? UnityEngine.Random.Range(1.0f, 1.9f) : s.sound.source.pitch; //1f;
        s.sound.source.Play();
        Debug.Log("Played sound!");
    }

    public void Play(string name, bool checkIsPlaying, bool randomizePitch, float newPitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sound {name} not found!");
            return;
        }

        if (checkIsPlaying && IsPlaying(name))
        {
            AudioSource source = s.sound.GetOrCreateAudioSource(gameObject);
            source.pitch = randomizePitch ? UnityEngine.Random.Range(1.0f, 1.9f) : newPitch;
            source.Play();
            StartCoroutine(s.sound.ReturnAudioSourceToPool(source, s.sound.clip.length));
            return;
        }

        s.sound.source.pitch = randomizePitch ? UnityEngine.Random.Range(1.0f, 1.9f) : s.sound.source.pitch; //1f;
        s.sound.source.Play();
        Debug.Log("Played sound!");
    }

    public void Pause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.sound.source.Pause();
    }

    public bool IsPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        return s.sound.source.isPlaying;
    }

    public void PauseAllOthers(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        foreach (Sound sound in sounds)
        {
            if (sound.sound.type == AudioTypeGroup.MUSIC)
            {
                if (sound != s)
                {
                    Pause(sound.name);
                }
            }
        }
    }
}

[Serializable]
public class Sound
{
    public string name;
    public SoundConfig sound;

    public Sound(Sound s)
    {
        name = s.name;
        sound = new SoundConfig(s.sound);
    }
}

[System.Serializable]
public class SoundConfig
{
    private Queue<AudioSource> audioSourcePool;
    public AudioTypeGroup type;
    public bool loop;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume;
    [Range(.1f, 3f)] public float pitch;
    public AudioSource source;

    public SoundConfig()
    {
        audioSourcePool = new Queue<AudioSource>();
    }
    public SoundConfig(SoundConfig s)
    {
        type = s.type;
        loop = s.loop;
        clip = s.clip;
        volume = s.volume;
        pitch = s.pitch;
        source = null;
        audioSourcePool = new Queue<AudioSource>();
    }

    public void InitializeAudioSourcePool() => audioSourcePool.Enqueue(source);

    public AudioSource GetOrCreateAudioSource(GameObject gameObject)
    {
        if (audioSourcePool.Count > 0)
            return audioSourcePool.Dequeue();

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = clip;
        newSource.volume = volume;
        newSource.pitch = pitch;
        newSource.loop = loop;
        source = newSource;
        return newSource;
    }

    public IEnumerator ReturnAudioSourceToPool(AudioSource source, float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        source.Stop();
        audioSourcePool.Enqueue(source);
    }
}

public enum AudioTypeGroup
{
    MUSIC,
    UI,
    OTHER
}