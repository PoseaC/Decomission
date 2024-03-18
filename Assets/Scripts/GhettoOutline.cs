using UnityEngine;
using System.Collections;
//the name "ghetto" comes from the fact that I couldn't get the desired effect using the shader graph so I made my own script for it
public class GhettoOutline : MonoBehaviour
{
    public Material outlineMaterial; //a material made in unity's shader graph that just renders the back faces of the mesh
    public float thickness = .1f;

    //while I am not an artist, I do wish for my games to look nice in a minimalistic way,
    //and for this game I thought it would look good if the outline of the objects in the stage bounced on the beat with the music
    public float beatThreshold = .25f; //how loud the current sample has to be to move the outline
    public float timeBetweenBeats = .1f; //the cooldown beetween beats so on constantly loud songs the outline still looks to be moving
    public float timeToBeat = .05f; //how long before the outline reaches the apex
    public float beatSpeed = 3; //how fast the outline moves

    PlayerMovementManager inputManager;
    GameObject outlineObj;
    float[] spectrumData = new float[128];
    float lastSample;
    float beatTimer;

    private void Start()
    {
        //we duplicate the current object, parent it to this one and remove the components we don't need, leaving only the mesh renderer
        inputManager = FindObjectOfType<PlayerMovementManager>();
        outlineObj = Instantiate(gameObject, transform);
        try
        {
            Destroy(outlineObj.GetComponent<GhettoOutline>());
            Destroy(outlineObj.GetComponent<BoxCollider>());
            Destroy(outlineObj.GetComponent<MeshCollider>());
        }
        finally
        {
            if (!outlineObj.GetComponent<MeshRenderer>().enabled)
                outlineObj.GetComponent<MeshRenderer>().enabled = true;

            outlineObj.GetComponent<MeshRenderer>().material = outlineMaterial;
            outlineObj.transform.position = transform.position;
            outlineObj.transform.rotation = transform.rotation;
            outlineObj.transform.localScale = (1 + thickness) * Vector3.one;
        }
    }

    private void Update()
    {
        //on a ground level, to detect a beat in the music we compare the current and previous sample, and if the difference is high enough we have a beat
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
        float scale = outlineObj.transform.localScale.x;
        float target = up ? 1.015f + (thickness * inputManager.speed * 2) : 1 + thickness;
        float timer = 0;

        while (timer < timeToBeat)
        {
            scale = Mathf.Lerp(scale, target, beatSpeed * Time.deltaTime);
            outlineObj.transform.localScale = Vector3.one * scale;
            timer += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(Beat(false));
    }
}
