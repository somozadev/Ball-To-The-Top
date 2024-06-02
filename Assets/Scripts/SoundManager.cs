using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;
using System;
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


    public AudioMixer GetAudioMixer
    {
        get { return audioMixer; }
    }

    private void Awake()
    {
        Initialize();
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
        Play(theme);
        StartCoroutine(WaitToThemeEndToPlayNewOne(currentTheme.sound.clip.length));
    }

    private IEnumerator WaitToThemeEndToPlayNewOne(float duration)
    {
        Debug.Log(duration);
        yield return new WaitForSecondsRealtime(duration);
        InitRandomTheme();
    }

    public void PlayRandomJump()
    {
        var jumpSounds = sounds.Where(sound => sound.sound.type == AudioTypeGroup.JUMPS).ToArray();
        int randomIndex = UnityEngine.Random.Range(0, jumpSounds.Length);
        string rndClip = jumpSounds[randomIndex].name;
        Play(rndClip);

    }  
    public void PlayRandomKick()
    {
        var kickSounds = sounds.Where(sound => sound.sound.type == AudioTypeGroup.KICKS).ToArray();
        int randomIndex = UnityEngine.Random.Range(0, kickSounds.Length);
        string rndClip = kickSounds[randomIndex].name;
        Play(rndClip);
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.sound.source.Play();
    }

    public void Pause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.sound.source.Pause();
    }

    public bool isPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        bool result = s.sound.source.isPlaying;
        return result;
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

[System.Serializable]
public class Sound
{
    public string name;
    public SoundConfig sound;
}

[System.Serializable]
public class SoundConfig
{
    public AudioTypeGroup type;
    public bool loop;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume;
    [Range(.1f, 3f)] public float pitch;
    public AudioSource source;
}

public enum AudioTypeGroup
{
    JUMPS,
    KICKS,
    MUSIC,
    UI,
    OTHER
}