using UnityEngine;

public class MovingPlatform : BeatListener
{
    public float distance = 5;
    public Vector3 movingAxis = Vector3.up;
    Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
        AudioManager.instance.AddListener(this);
        animatedValueStart = 0;
        animatedValueEnd = 1;
    }
    private void Update()
    {
        transform.position = startPosition + distance * animatedValue * movingAxis;
    }
}
