using UnityEngine;

public class Bouncepad : MonoBehaviour
{
    public float bounceStrength = 3000;
    PlayerMovementManager inputManager;
    private void Start()
    {
        inputManager = FindObjectOfType<PlayerMovementManager>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (inputManager.movementState.Equals(PlayerMovementManager.MovementState.Walking))
            inputManager.SwitchStates(PlayerMovementManager.MovementState.Running);

        collision.collider.attachedRigidbody.AddForce(-bounceStrength * collision.GetContact(0).normal, ForceMode.Impulse);
    }
}
