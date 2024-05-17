using UnityEngine;
using System.Collections;

public abstract class BeatListener : MonoBehaviour
{
    public enum InterpolationFunction
    {
        Linear, EaseIn, EaseOut, EaseInOut
    }
    public InterpolationFunction interpolationFunction = InterpolationFunction.EaseInOut;
    public bool holdBeat = false;
    [HideInInspector] public float animatedValueStart = 0;
    [HideInInspector] public float animatedValueEnd = 0;
    [HideInInspector] public float animatedValue = 0;
    bool coroutineRunning = false;
    bool returnBeat = true;

    public void HandleBeat()
    {
        if (!coroutineRunning && gameObject.activeInHierarchy)
        {
            if (holdBeat)
            {
                StartCoroutine(Beat(returnBeat));
            }
            else
            {
                StartCoroutine(Beat(true));
            }
        }
    }

    IEnumerator Beat(bool rising)
    {
        coroutineRunning = true;

        float start = rising ? animatedValueStart : animatedValueEnd;
        float target = rising ? animatedValueEnd : animatedValueStart;
        float timer = 0;

        while (timer < 1)
        {
            timer += AudioManager.instance.beatSensitivity * Time.deltaTime;
            animatedValue = Interpolate(start, target, timer);
            yield return null;
        }

        if (holdBeat)
        {
            returnBeat = !returnBeat;
            coroutineRunning = false;
        }
        else
        {
            if (rising)
            {
                StartCoroutine(Beat(false));
            }
            else
            {
                coroutineRunning = false;
            }
        }
    }

    float Interpolate(float start, float target, float t)
    {
        switch (interpolationFunction)
        {
            case InterpolationFunction.Linear:
                return Mathf.Lerp(start, target, t);

            case InterpolationFunction.EaseOut:
                return start + (target - start) * (1 - Mathf.Pow(2, -10 * t));

            case InterpolationFunction.EaseIn:
                return start + (target - start) * (Mathf.Pow(2, 10 * t - 10));

            case InterpolationFunction.EaseInOut:
                return start + (target - start) * (t < 0.5 ? Mathf.Pow(2, 20 * t - 10) / 2
                                                    : (2 - Mathf.Pow(2, -20 * t + 10)) / 2);
            default:
                return Mathf.Lerp(start, target, t);
        }
    }
}
