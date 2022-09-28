using UnityEngine;

public class HandsMovement : MonoBehaviour
{
    public Transform head;
    public float speed = 15;
    AudioSource source;
    private void Start()
    {
        source = FindObjectOfType<PlayerMovementManager>().feet;
        transform.rotation = Quaternion.Euler(TransitionInfo.instance.cameraRotationY, TransitionInfo.instance.cameraRotationX, 0);
    }
    //to give a more fluid feeling to moving the camera, the rotation of the hands lags behind the camera and then moves back into position, rather than being stuck to it
    void Update()
    {
        transform.position = head.position;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, head.rotation, speed * Time.deltaTime * Quaternion.Angle(transform.rotation, head.rotation));
    }

    public void StepSound()
    {
        AudioManager.instance.PlaySound("step", source);
    }
}
