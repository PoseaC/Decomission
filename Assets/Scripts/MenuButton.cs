using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    //and for this game I thought it would look good if the outline of the objects in the stage bounced on the beat with the music
    public float beatThreshold = .25f; //how loud the current sample has to be to move the outline
    public float timeBetweenBeats = .1f; //the cooldown beetween beats so on constantly loud songs the outline still looks to be moving
    public float timeToBeat = .05f; //how long before the outline reaches the apex
    public float beatSpeed = 3; //how fast the outline moves

    float[] spectrumData = new float[128];
    float lastSample;
    float beatTimer;
    Outline outline;

    public Vector2 beatSize;
    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {
        beatTimer += Time.deltaTime;
        AudioManager.source.GetSpectrumData(spectrumData, 0, FFTWindow.Hamming);

        if (Mathf.Abs(spectrumData[0] - lastSample) > (beatThreshold * AudioManager.source.volume) && beatTimer > timeBetweenBeats)
        {
            StopAllCoroutines();
            StartCoroutine(Beat(true));
            beatTimer = 0;
        }

        lastSample = spectrumData[0];
    }

    //the bool up is used to use the same function for both going up the beat and cooling it back down
    IEnumerator Beat(bool up)
    {
        //when moving with the beat we interpolate between the current object scale and the target
        float size = outline.effectDistance.x;
        float target = up ? beatSize.x : beatSize.y;
        float timer = 0;

        while (timer < timeToBeat)
        {
            size = Mathf.Lerp(size, target, beatSpeed * Time.deltaTime);
            outline.effectDistance = new Vector2(size, size);
            timer += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(Beat(false));
    }
}
