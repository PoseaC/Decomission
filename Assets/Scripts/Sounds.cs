using UnityEngine;

[System.Serializable]
public class Sounds
{
    public string soundName; //name for the sound for searching in the array

    public AudioClip clip; //the actual sound

    [Range(0f,1f)] //tag to appear as a slider in the inspector
    public float volume; //volume for the sound
    [Range(.1f,3f)]
    public float pitch; //pitch of the sound

    public bool loop; //if the sound should loop or not
}
