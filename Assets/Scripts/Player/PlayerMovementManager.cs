using UnityEngine;
using UnityEngine.UI;

//I use a player movement manager that handles the player input to use a physics based rigidbody that allows for mastery with the momentum of movement
//while still having snappy and responsible movement at low speeds using a kinematic movement based script 
public class PlayerMovementManager : MonoBehaviour
{
    public enum MovementState { Walking, Running } //walking signals we are using kinematic movement, running signals momentum based movement

    public float dashForce = 15;
    public float dashCooldown = 1;
    float currentDashCooldown = 1;
    public float playerMass = 40;
    public static PlayerMovementManager instance;
    public Rigidbody playerBody;
    public KinematicPlayerMovement kinematicMovement;
    public PhysicsPlayerMovement physicsMovement;
    public ParticleSystem speedParticle; //a particle system that plays while running to better sell the speed to the player
    public float coyoteTime = .15f; //after walking off an edge the player can still jump for a little while, which forgives little reflex mistakes that the player might not even notice
    public CameraMovement playerCamera;
    public bool isAgent = true;

    public Transform groundCheck; //the point where the ground detection occurs
    public LayerMask whatIsGround; //what is considered ground
    public Animator handsAnimator; //reference to the animator to update what is playing based on input, being an fps game most player animations are with the hands
    public AudioSource feet; //source where the stepping and jumping sounds play

    [HideInInspector] public float switchTreshold; //how slow the player must move to switch back to the walking script
    [HideInInspector] public float speed; //visual effect speed
    [HideInInspector] public MovementState movementState = MovementState.Walking; //what is the movement state at any given time
    [HideInInspector] public bool isGrounded = false;
    [HideInInspector] public bool isJumping = false;
    [HideInInspector] public bool hasDoubleJump = false;
    [HideInInspector] public bool isWallrunning = false;
    [HideInInspector] public Vector3 movementDirection;

    CharacterController characterController;
    LevelManager levelManager;
    Image doubleJumpIndicator;
    bool checkForGround = true;
    private void Awake()
    {
        instance = this;
        switchTreshold = physicsMovement.startingSprintSpeed - 1;
        characterController = GetComponentInChildren<CharacterController>();
        levelManager = FindObjectOfType<LevelManager>();
        doubleJumpIndicator = levelManager.doubleJumpIndicator;
        levelManager.dashIndicator.maxValue = levelManager.dashIndicator.value = currentDashCooldown = dashCooldown;
    }

    void Update()
    {
        if (currentDashCooldown > 0)
            currentDashCooldown -= Time.deltaTime;

        levelManager.dashIndicator.value = dashCooldown - currentDashCooldown;

        GetInput();
        doubleJumpIndicator.enabled = hasDoubleJump;
    }

    void GetInput()
    {
        if (isAgent)
        {
            movementDirection = AgentIO.output.Movement;
        }
        else
        {
            movementDirection = (Input.GetAxis("Horizontal") * playerBody.transform.right +
                                Input.GetAxis("Vertical") * playerBody.transform.forward).normalized;
        }

        if (Physics.CheckSphere(groundCheck.position, .2f, whatIsGround) && checkForGround)
        {
            isGrounded = true;
        }
        else if (isGrounded)
        {
            Invoke("NotGrounded", coyoteTime);
        }

        if (isWallrunning || isGrounded)
            hasDoubleJump = true;

        if (isGrounded)
        {
            if ((Input.GetButtonDown("Running") || (isAgent && AgentIO.output.Sprint)))
            {
                SwitchStates(MovementState.Running);
            }
            else if (playerBody.velocity.sqrMagnitude < switchTreshold * switchTreshold)
            {
                Invoke("Walk", 0.25f);
            }
        }

        if (Input.GetButtonDown("Crouch") && currentDashCooldown <= 0)
        {
            playerBody.AddForce(playerBody.transform.forward * dashForce, ForceMode.VelocityChange);
            currentDashCooldown = dashCooldown;
        }

        if (Input.GetButton("Crouch"))
        {
            handsAnimator.SetBool("Crouching", true);
            playerBody.transform.localScale = new Vector3(1, 0.5f, 1);
        }
        else
        {
            handsAnimator.SetBool("Crouching", false);
            playerBody.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void LateUpdate()
    {
        if ((Input.GetButtonDown("Jump") || (isAgent && AgentIO.output.Jump)) && (isGrounded || isWallrunning || hasDoubleJump))
        {
            if (!isGrounded && !isWallrunning)
            {
                hasDoubleJump = false;
            }

            isJumping = true;
            isGrounded = false;

            //due to the ground detection occuring every frame while the jump input occurs only in one, after jumping I stop the ground detection for a short while
            checkForGround = false;
            Invoke("CheckGround", coyoteTime); 

            AudioManager.instance.PlaySound("jump", feet);
        }
        else
            isJumping = false;

        //using the sqrMagnitude of the player velocity to calculate the speed for the visual and audio effects saves a square root call
        speed = Mathf.Clamp(playerBody.velocity.sqrMagnitude / (physicsMovement.startingSprintSpeed * physicsMovement.startingSprintSpeed), .5f, 3);
        handsAnimator.SetBool("Grounded", isGrounded);
        handsAnimator.SetFloat("Speed", speed);
        speedParticle.startSpeed = speed * 5;
        //AudioManager.filter.cutoffFrequency = levelManager.isPaused ? 1500 : speed * 4000;

        AgentIO.input.AgentPosition = playerBody.transform.position;
        AgentIO.input.CanJump = hasDoubleJump || isGrounded;
        AgentIO.input.Grounded = isGrounded;
    }

    public void SwitchStates(MovementState state)
    {
        if (state.Equals(MovementState.Running))
        {
            movementState = MovementState.Running;
            characterController.enabled = false;
            playerBody.isKinematic = false;
            kinematicMovement.enabled = false;
            physicsMovement.enabled = true;
            speedParticle.Play();
        }
        else
        {
            movementState = MovementState.Walking;
            physicsMovement.enabled = false;
            playerBody.isKinematic = true;
            characterController.enabled = true;
            kinematicMovement.enabled = true;
            speedParticle.Stop();
        }
    }

    void CheckGround()
    {
        checkForGround = true;
    }

    void NotGrounded()
    {
        isGrounded = false;
        SwitchStates(MovementState.Running);
    }

    void Walk()
    {
        if (playerBody.velocity.sqrMagnitude < switchTreshold * switchTreshold)
        {
            SwitchStates(MovementState.Walking);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheck.position, .2f);
    }
}
