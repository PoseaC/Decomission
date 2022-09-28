using UnityEngine;

public class PhysicsPlayerMovement : MonoBehaviour
{
    [Header("Speed Settings")]
    public float acceleration = 300; //player acceleration
    public float maxSpeed = 30; //maximum speed
    public float deceleration = 200; //how much to slow down when there is no input
    public float jumpForce = 700; //how high to isJumping
    public float startingSprintSpeed = 15;

    [Header("Other")]
    public Rigidbody playerBody; //player's rigidbody

    bool isJumping = false; //if the player can isJumping
    bool isGrounded = true;

    [HideInInspector] public Vector3 movementDirection; //movementDirection of movement
    [HideInInspector] public Vector3 wallNormal;
    PlayerMovementManager inputManager;
    private void Awake()
    {
        playerBody = GetComponent<Rigidbody>();
        inputManager = GetComponentInParent<PlayerMovementManager>();
    }
    void Start()
    {
        //to make seamless transitions between scenes the player keeps velocity
        playerBody.velocity = TransitionInfo.instance.velocity.sqrMagnitude > maxSpeed * maxSpeed ? TransitionInfo.instance.velocity.normalized * maxSpeed : TransitionInfo.instance.velocity;
    }
    void Update()
    {
        GetInput();

        if (isJumping)
            Jump();
    }
    void FixedUpdate()
    {
        Move();
    }
    void GetInput()
    {
        movementDirection = inputManager.movementDirection;
        isJumping = inputManager.isJumping;
        isGrounded = inputManager.isGrounded;
    }
    void Move()
    {
        playerBody.AddForce(acceleration * Time.fixedDeltaTime * movementDirection, ForceMode.Acceleration);

        CounterMovement();
    }
    public void Jump()
    {
        Vector3 direction = transform.up;

        if (inputManager.isWallrunning)
        {
            direction = wallNormal + transform.forward;
        }

        playerBody.velocity = new Vector3(playerBody.velocity.x, 0, playerBody.velocity.z) + direction * jumpForce;
        isJumping = false;
    }
    void CounterMovement()
    {
        //we calculate the velocity and direction of movement relative to the player
        Vector3 localVelocity = transform.InverseTransformDirection(playerBody.velocity);
        float angle = Vector3.Angle(transform.forward, movementDirection);

        //apply drag if the player exceeds the maximum speed or doesn't input movement or is trying to move in the opposite direction of the velocity
        if (movementDirection.sqrMagnitude == 0 || playerBody.velocity.sqrMagnitude > maxSpeed * maxSpeed || angle > 90)
            playerBody.AddForce(deceleration * Time.fixedDeltaTime * -playerBody.velocity.normalized, ForceMode.Acceleration);

        //to avoid sloppy feeling movement while grounded, small directional changes occur instantly 
        if (isGrounded && angle < 90 && movementDirection.sqrMagnitude != 0)
        {
            playerBody.velocity = movementDirection * new Vector3(localVelocity.x, 0, localVelocity.z).magnitude;
            playerBody.velocity += Vector3.up * localVelocity.y;
        }
    }
    private void OnEnable()
    {
        if (inputManager.movementDirection.sqrMagnitude != 0)
           playerBody.velocity = inputManager.movementDirection * startingSprintSpeed;
    }
}
