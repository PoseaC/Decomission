using UnityEngine;

public class KinematicPlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Attributes")]
    public float gravity = 10f; //how fast the player falls
    public float speed = 5f; //movement speed

    Vector3 velocity; //downwards velocity, direction of gravity better said
    PlayerMovementManager inputManager;
    private void Start()
    {
        inputManager = GetComponentInParent<PlayerMovementManager>();
    }
    void Update()
    {
        //get the input from the player
        Vector3 movementDirection = inputManager.movementDirection;
        bool isGrounded = inputManager.isGrounded;
        bool isJumping = inputManager.isJumping;
        inputManager.handsAnimator.SetBool("Walking", movementDirection.sqrMagnitude != 0);

        if (isGrounded && velocity.y < -2f)
        {
            velocity.y = -2f;
        }
        controller.Move(movementDirection * speed * Time.deltaTime);

        //when jumping switch to the momentum based movement
        if (isJumping)
        {
            inputManager.SwitchStates(PlayerMovementManager.MovementState.Running);
            inputManager.physicsMovement.Jump();
        }

        //apply gravity
        velocity.y -= gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
