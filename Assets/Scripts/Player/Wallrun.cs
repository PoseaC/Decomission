using UnityEngine;
using System.Collections;

public class Wallrun : MonoBehaviour
{
    public float headRotationSpeed = 20;
    public float headTiltAngle = 15;
    public float thresholdAndle = 45;

    public PlayerMovementManager inputManager;
    public CameraMovement cameraMovement;
    Coroutine activeTilt;
    Collider wall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("IgnoreWallrun"))
            return;

        Vector3 collisionPoint = other.ClosestPoint(transform.position);
        Vector3 collisionNormal = (collisionPoint - transform.position).normalized;
        Debug.Log(Vector3.Angle(transform.up, collisionNormal));

        if (Vector3.Angle(transform.up, collisionNormal) > 180 - thresholdAndle)
        {
            //when starting a wall run we turn off gravity and use a weaker artificial force to simulate it to allow for further travel on the wall
            Vector3 currentVelocityDirection = inputManager.playerBody.velocity;
            Vector3 newVelocity = new Vector3(currentVelocityDirection.x, 0, currentVelocityDirection.z);
            inputManager.playerBody.velocity = Vector3.ProjectOnPlane(newVelocity, collisionNormal);
            inputManager.playerBody.AddForce(inputManager.playerBody.velocity.normalized * inputManager.dashForce, ForceMode.VelocityChange);
            inputManager.isWallrunning = true;
            inputManager.physicsMovement.wallNormal = collisionNormal;

            //a good indicator to the player that he is wallrunning is to tilt the head in the opposite direction of the wall,
            //which is done using the right hand rule by crossing the player position and contact point position and then cheking if the resulting vector is higher or lower than the origin
            try { StopCoroutine(activeTilt); } catch { };

            if (Vector3.Cross((inputManager.playerBody.position - collisionPoint).normalized, inputManager.playerBody.transform.forward).y < 0)
                activeTilt = StartCoroutine(TiltHead(-headTiltAngle));
            else
                activeTilt = StartCoroutine(TiltHead(headTiltAngle));

            wall = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (inputManager.isWallrunning && wall == other)
        {
            StopWallRunning();
        }
    }

    void StopWallRunning()
    {
        try { StopCoroutine(activeTilt); } catch { };

        inputManager.playerBody.mass = inputManager.playerMass;
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
