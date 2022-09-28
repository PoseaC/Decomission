using UnityEngine;
using System.Collections;

public class Wallrun : MonoBehaviour
{
    public float headRotationSpeed = 20;
    public float headTiltAngle = 15;
    public float thresholdAndle = 45;
    public float artificialGravity = 200;

    public PlayerMovementManager inputManager;
    public CameraMovement cameraMovement;
    Coroutine activeTilt;

    private void Update()
    {
        if (inputManager.isWallrunning)
        {
            inputManager.playerBody.AddForce(artificialGravity * Time.deltaTime * -transform.up, ForceMode.Acceleration);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (Vector3.Angle(transform.up, collision.GetContact(0).normal) > thresholdAndle && inputManager.movementState.Equals(PlayerMovementManager.MovementState.Running))
        {
            //when starting a wall run we turn off gravity and use a weaker artificial force to simulate it to allow for further travel on the wall
            inputManager.playerBody.useGravity = false;
            inputManager.playerBody.velocity = Vector3.ProjectOnPlane(new Vector3(inputManager.playerBody.velocity.x, 0, inputManager.playerBody.velocity.z), collision.GetContact(0).normal);
            inputManager.isWallrunning = true;
            inputManager.physicsMovement.wallNormal = collision.GetContact(0).normal;

            //a good indicator to the player that he is wallrunning is to tilt the head in the opposite direction of the wall,
            //which is done using the right hand rule by crossing the player position and contact point position and then cheking if the resulting vector is higher or lower than the origin
            try { StopCoroutine(activeTilt); } catch { };

            if (Vector3.Cross((inputManager.playerBody.position - collision.GetContact(0).point).normalized, inputManager.playerBody.transform.forward).y < 0)
                activeTilt = StartCoroutine(TiltHead(-headTiltAngle));
            else
                activeTilt = StartCoroutine(TiltHead(headTiltAngle));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (inputManager.isWallrunning)
        {
            StopWallRunning();
        }
    }

    void StopWallRunning()
    {
        try { StopCoroutine(activeTilt); } catch { };

        inputManager.playerBody.useGravity = true;
        inputManager.isWallrunning = false;
        activeTilt = StartCoroutine(TiltHead(0));
    }

    IEnumerator TiltHead(float target)
    {
        float localTilt = cameraMovement.wallrunTilt;

        if (localTilt > target)
            while (localTilt > target)
            {
                localTilt -= headRotationSpeed * Time.deltaTime;
                cameraMovement.wallrunTilt = localTilt;
                yield return null;
            }
        else
            while (localTilt < target)
            {
                localTilt += headRotationSpeed * Time.deltaTime;
                cameraMovement.wallrunTilt = localTilt;
                yield return null;
            }
    }
}
