using UnityEngine;
using System;
using System.Collections;
//using one audio manager that stores all the sounds and can be called to play one at any point makes it easier to add and play new sounds rather than setting up manually an audio clip on each object
public class AudioManager : MonoBehaviour
{
    public Sounds[] sounds; //an array of sounds to play
    public Sounds[] music; //an array of music to play
    public bool onShuffle = false;

    public static AudioLowPassFilter filter;
    public static AudioSource source;
    public static AudioManager instance; //we use only one audio manager, otherwise background music will cut between scene transitions

    int songIndex = 0;
    Transform player;
    LevelManager levelManager;
    private void Awake()
    {
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
        StartCoroutine(PlayMusic());
        onShuffle = PlayerPrefs.GetInt("OnShuffle", 0) == 1;
    }
    private void Update()
    {
        if (player == null)
            player = FindObjectOfType<PhysicsPlayerMovement>().transform;

        if (levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();

        transform.position = player.position;
    }
    public void ChangeMusicVolume()
    {
        foreach (Sounds sound in music)
        {
            sound.volume = PlayerPrefs.GetFloat("MusicVolume", .6f);
        }
        source.volume = PlayerPrefs.GetFloat("MusicVolume", .6f);
    }

    public void ChangeSFXVolume()
    {
        foreach (Sounds sound in sounds)
        {
            sound.volume = PlayerPrefs.GetFloat("SfxVolume", .3f);
        }
    }
    public void PlaySound(string name, AudioSource source)
    {
        Sounds sound = Array.Find(sounds, sounds => sounds.soundName == name); //search the list of sounds for the one we need
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
    public void PlaySong(string name)
    {
        Sounds song = Array.Find(music, music => music.soundName == name); //search the list of sounds for the one we need
        if (song == null)
        {
            Debug.LogWarning("Song " + name + " not found"); //throw a warning if the sound wasn't found
            return;
        }
        levelManager.musicTitle.text = TransitionInfo.instance.songTitle = song.soundName;
        source.clip = song.clip;
        source.volume = song.volume;
        source.pitch = song.pitch;
        source.loop = song.loop;
        source.Play();
    }
    IEnumerator PlayMusic()
    {
        Sounds song = onShuffle ? music[UnityEngine.Random.Range(0, music.Length)] : music[songIndex];

        songIndex += 1;
        if (songIndex >= music.Length)
            songIndex = 0;

        PlaySong(song.soundName);

        //wait for the current song to end before playing the next one while still listening for input if the player wishes to skip it
        float timer = 0;
        while (timer < song.clip.length)
        {
            timer += Time.deltaTime;

            if (Input.GetButtonDown("SkipSong"))
                break;

            yield return null;
        }

        levelManager.uiAnimator.Play("Base Layer.MusicSwitch", 0, 0);
        StartCoroutine(FadeOutMusic());
        yield return new WaitForSeconds(levelManager.uiAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length / 2);
        StartCoroutine(PlayMusic());
    }

    IEnumerator FadeOutMusic()
    {
        float speed = source.volume/(levelManager.uiAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length / 2);

        while (source.volume > 0)
        {
            source.volume -= speed * Time.deltaTime;
            yield return null;
        }
    }
}
