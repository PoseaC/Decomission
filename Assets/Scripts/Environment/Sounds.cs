using UnityEngine;

[System.Serializable]
public class Sound
{
    public string soundName;
    public AudioClip clip;
    public bool loop = false;
    public float beatSensitivity = 1.3f;

    [Range(0f,1f)]
    public float volume = 0.3f;
    [Range(.1f, 3f)]
    public float pitch = 1f;

    [HideInInspector] public float[] energySamplesPerSecond;
    [HideInInspector] public int energySampleCount;
}
