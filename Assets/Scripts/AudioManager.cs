using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
//using one audio manager that stores all the sounds and can be called to play one at any point makes it easier to add and play new sounds rather than setting up manually an audio clip on each object
public class AudioManager : MonoBehaviour
{
    public float beatSensitivity = 1.3f;
    public int songSampleRate = 44100;

    public Sound[] sounds; //an array of sounds to play
    public Sound[] music; //an array of music to play
    public bool onShuffle = false;
    public bool playerPlaylist = false;
    public Animator uiAnimator;
    public Transform player;
    public TextMeshProUGUI musicTitle;

    public static AudioLowPassFilter filter;
    public static AudioSource source;
    public static AudioManager instance; //we use only one audio manager, otherwise background music will cut between scene transitions

    int songIndex = 0;
    LevelManager levelManager;
    List<BeatListener> listeners;
    Sound currentSong;

    private void Awake()
    {
        if (player == null)
            player = FindObjectOfType<PhysicsPlayerMovement>().transform;

        levelManager = FindObjectOfType<LevelManager>();

        if (instance == null) //check if we don't already have an audio manager, if we don't we assign this one
            instance = this;
        else
        {
            Destroy(gameObject); //if we do we destroy this audio manager as to not have two different sound sources
            return;
        }

        source = GetComponent<AudioSource>();
        filter = GetComponent<AudioLowPassFilter>();
        DontDestroyOnLoad(gameObject); //tell the inspector to keep this object between scenes

        ComputeSongsEnergy();
        ChangeMusicVolume();
        ChangeSFXVolume();
        StartCoroutine(PlayMusic());
        
        onShuffle = PlayerPrefs.GetInt("OnShuffle", 0) == 1;
        listeners = new List<BeatListener>();
    }

    private void Update()
    {
        if (player == null)
            player = FindObjectOfType<PhysicsPlayerMovement>().transform;

        if (levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();

        transform.position = player.position;

        NotifyListeners();
    }

    private void ComputeSongsEnergy()
    {
        foreach(Sound song in music)
        {
            song.energySampleCount = song.clip.samples / songSampleRate;
            song.energySamplesPerSecond = new float[song.energySampleCount];
            int buffer = songSampleRate * song.clip.channels;

            float[] samples = new float[song.clip.samples * song.clip.channels];
            song.clip.GetData(samples, 0);

            for (int i = 0; i < song.energySampleCount; i++)
            {
                float energyAvg = 0;

                for (int j = 0; j < buffer; j++)
                {
                    energyAvg += Mathf.Pow(samples[i * buffer + j], 2);
                }

                song.energySamplesPerSecond[i] = energyAvg / buffer;
            }
        }
    }

    private bool IsBeat()
    {
        float instantEnergy = 0;
        float[] samples = new float[1024 * source.clip.channels];
        source.GetSpectrumData(samples, 0, FFTWindow.Hamming);

        for(int i = 0; i < samples.Length; i++)
        {
            instantEnergy += samples[i];
        }
        
        return instantEnergy > currentSong.energySamplesPerSecond[(int)source.time] * beatSensitivity;
    }

    private void NotifyListeners()
    {
        if (IsBeat())
        {
            foreach(BeatListener listener in listeners)
            {
                listener.HandleBeat();
            }
        }
    }

    public void AddListener(BeatListener item)
    {
        listeners.Add(item);
    }

    public void ChangeMusicVolume()
    {
        foreach (Sound sound in music)
        {
            sound.volume = PlayerPrefs.GetFloat("MusicVolume", .6f);
        }
        source.volume = PlayerPrefs.GetFloat("MusicVolume", .6f);
    }

    public void ChangeSFXVolume()
    {
        foreach (Sound sound in sounds)
        {
            sound.volume = PlayerPrefs.GetFloat("SfxVolume", .3f);
        }
    }

    public void PlaySound(string name, AudioSource source)
    {
        Sound sound = Array.Find(sounds, sounds => sounds.soundName == name); //search the list of sounds for the one we need

        if (sound == null)
        {
            Debug.LogWarning("Sound " + name + " not found"); //throw a warning if the sound wasn't found
            return;
        }

        source.clip = sound.clip;
        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.loop = sound.loop;
        source.Play();
    }

    public void PlaySong(Sound song)
    {
        if (musicTitle == null)
            musicTitle = levelManager.musicTitle;

        beatSensitivity = song.beatSensitivity;
        musicTitle.text = TransitionInfo.instance.songTitle = song.soundName;
        source.clip = song.clip;
        source.volume = song.volume;
        source.pitch = song.pitch;
        source.loop = song.loop;
        source.Play();
    }

    IEnumerator PlayMusic()
    {
        currentSong = onShuffle ? music[UnityEngine.Random.Range(0, music.Length)] : music[songIndex];

        songIndex += 1;
        if (songIndex >= music.Length)
            songIndex = 0;

        PlaySong(currentSong);

        //wait for the current song to end before playing the next one while still listening for input if the player wishes to skip it
        float timer = 0;
        while (timer < currentSong.clip.length)
        {
            timer += Time.deltaTime;

            if (Input.GetButtonDown("SkipSong"))
                break;

            yield return null;
        }

        if (uiAnimator == null)
            uiAnimator = levelManager.uiAnimator;

        uiAnimator.Play("Base Layer.MusicSwitch", 0, 0);
        StartCoroutine(FadeOutMusic());
        yield return new WaitForSeconds(uiAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length / 2);
        StartCoroutine(PlayMusic());
    }

    IEnumerator FadeOutMusic()
    {
        if (uiAnimator == null)
            uiAnimator = levelManager.uiAnimator;

        float speed = source.volume/(uiAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length / 2);

        while (source.volume > 0)
        {
            source.volume -= speed * Time.deltaTime;
            yield return null;
        }
    }
}
