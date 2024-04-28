using UnityEngine;
using System.Collections;

public abstract class BeatListener : MonoBehaviour
{
    public float beatThreshold = .25f;
    public float timeBetweenBeats = .1f;
    public float beatSpeed = 3;
    public bool holdBeat = false;
    [HideInInspector] public float animatedValueStart = 0;
    [HideInInspector] public float animatedValueEnd = 0;
    [HideInInspector] public float animatedValue = 0;
    float cooldownTimer = 0;
    bool coroutineRunning = false;
    bool returnBeat = false;

    public void HandleBeat(float intensity)
    {
        cooldownTimer += Time.deltaTime;
        if (intensity >= beatThreshold && cooldownTimer >= timeBetweenBeats)
        {
            if (!coroutineRunning && gameObject.activeInHierarchy)
            {
                returnBeat = !returnBeat;
                StartCoroutine(Beat(returnBeat));
            }
        }
    }

    IEnumerator Beat(bool rising)
    {
        coroutineRunning = true;
        cooldownTimer = 0;

        float start = rising ? animatedValueStart : animatedValueEnd;
        float target = rising ? animatedValueEnd : animatedValueStart;
        float timer = 0;

        while (timer < 1)
        {
            animatedValue = EaseOutInterpolation(start, target, timer);
            timer += beatSpeed * Time.deltaTime;
            yield return null;
        }

        if (rising && !holdBeat)
        {
            returnBeat = !returnBeat;
            if (gameObject.activeInHierarchy)
                StartCoroutine(Beat(returnBeat));
            else
                coroutineRunning = false;
        }
        else
            coroutineRunning = false;
    }

    float EaseOutInterpolation(float start, float target, float t)
    {
        return start + (target - start) * (1 - Mathf.Pow(1 - t, 5));
    }
}
