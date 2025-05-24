using UnityEngine;

public class MovingPlatform : BeatListener
{
    public Vector3 targetPosition;
    Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
        AudioManager.instance.AddListener(this);
        animatedValueStart = 0;
        animatedValueEnd = 1;
    }
    private void Update()
    {
        transform.localPosition = Vector3.Lerp(startPosition, targetPosition, animatedValue);
    }
}
