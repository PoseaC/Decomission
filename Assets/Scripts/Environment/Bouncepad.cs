using UnityEngine;

public class Bouncepad : MonoBehaviour
{
    public float bounceStrength = 3000;
    PlayerMovementManager inputManager;
    private void Start()
    {
        inputManager = FindObjectOfType<PlayerMovementManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (inputManager.movementState.Equals(PlayerMovementManager.MovementState.Walking))
            inputManager.SwitchStates(PlayerMovementManager.MovementState.Running);

        other.GetComponent<Rigidbody>().AddForce(bounceStrength * transform.up, ForceMode.VelocityChange);
    }
}
