using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Grapple : MonoBehaviour
{
    [Header("Pazzaz")]
    public LineRenderer lineRenderer;
    public Animator hands;
    Image cursor;
    public Color active; //color of cursor when hovering over something that could be grappled
    public Color inactive;
    public float katanaSpeed;

    [Header("Grapple Details")]
    public PlayerMovementManager inputManager;
    public LayerMask whatIsGrappable; //what can be grappled
    public float grappleDistance = 50; //how far away can the grapple go

    [Header("Points of Reference")]
    public Transform mainCamera;
    public Transform grappleHand;
    public Transform grapplePoint;
    public Transform katana;

    [Header("Sine Wave Rope Movement")]
    public float amplitude;
    public float waveSpeed;
    public int points;

    Vector3 katanaStartPosition;
    Quaternion katanaStartRotation;
    bool recalling = false;

    Coroutine throwRoutine;
    AudioSource source;
    LevelManager levelManager;
    private void Start()
    {
        katanaStartPosition = katana.localPosition;
        katanaStartRotation = katana.localRotation;
        source = GetComponent<AudioSource>();
        levelManager = FindObjectOfType<LevelManager>();
        cursor = levelManager.cursor;
    }
    private void Update()
    {
        //could use a sphere cast in the future to forgive small aiming errors
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out RaycastHit hit, grappleDistance, whatIsGrappable))
        {
            cursor.color = active;
            if (Input.GetMouseButtonDown(0) && !recalling)
                throwRoutine = StartCoroutine(DrawSineWave(hit.point, true));
        }
        else
        {
            cursor.color = inactive;
        }

        if (Input.GetMouseButtonUp(0))
        {
            try { StopCoroutine(throwRoutine); } catch { }

            recalling = true;
            StartCoroutine(DrawSineWave(grappleHand.position, false));
        }
    }
    //moving the line after the player has moved so the visual doesn't lag behind
    private void LateUpdate()
    {
        lineRenderer.SetPosition(0, grappleHand.position);
    }

    //this is the pazzaz function, to wiggle the grapple rope and make swinging more funky
    //endPosition is where the katana should end up after being thrown and the bool throwing is to use the same function for the recall
    IEnumerator DrawSineWave(Vector3 endPosition, bool throwing)
    {
        float localAmplitude = amplitude;
        bool transitionFinished = false;

        if (throwing)
        {
            //play the animation and wait for it to finish before starting to move the katana
            //should be using hands.GetCurrentAnimatorClipInfo(0)[0].clip.length for the waiting period but for some reason this returns a full second rather than the expected length, needs further investigation
            hands.SetBool("Grappling", true);
            hands.Play("Base Layer.GrappleThrow", 0, 0);
            yield return new WaitForSeconds(.4f);

            katana.rotation = mainCamera.rotation;
            grapplePoint.gameObject.SetActive(true);
            katana.parent = null;
            katana.GetChild(0).gameObject.layer = 0;
        }

        lineRenderer.positionCount = points;
        float localKatanaSpeed = Mathf.Clamp(katanaSpeed * Vector3.Distance(katana.position, endPosition) / grappleDistance, .5f, katanaSpeed);
        AudioManager.instance.PlaySound("rope", source);

        while (localAmplitude > .05f)
        {
            if (levelManager.isPaused)
            {
                if (source.isPlaying)
                    source.Pause();

                goto skip;
            }
            else
            {
                source.UnPause();
            }

            //if the katana is recalled the end position is constantly updated because the player is moving
            if (!throwing)
                endPosition = grappleHand.position;

            katana.position = grapplePoint.position = Vector3.MoveTowards(katana.position, endPosition, localKatanaSpeed);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, katana.position - katana.forward * .75f);

            //to wiggle the grapple rope we use a sinus function to offset the points of the line renderer

            //first we calculate the direction and distance between each point in the rope
            float distance = Vector3.Distance(grappleHand.position, katana.position);
            float distanceBetweenPoints = distance / points;
            Vector3 direction = (katana.position - grappleHand.position).normalized;

            //second we iterate through all the points and calculate the sine value of it's distance from the hand
            for (int i = 1; i < lineRenderer.positionCount - 1; i++)
            {
                float sine = localAmplitude * Mathf.Sin(i * distanceBetweenPoints + waveSpeed * Time.timeSinceLevelLoad);

                //after that we position the point relative to the last one and offset it vertically depending on the sine
                Vector3 position = lineRenderer.GetPosition(i - 1) + direction * distanceBetweenPoints + mainCamera.up * sine;
                lineRenderer.SetPosition(i, position);
            }

            //when the katana reached it's destination we can start collapsing the sine wave by diminishing it's amplitude
            if ((katana.position - endPosition).sqrMagnitude < .5f)
            {
                //when being recalled we want to reset the katana to it's starting state
                if (transitionFinished && !throwing)
                {
                    hands.SetBool("Grappling", false);
                    katana.parent = grappleHand;
                    katana.localRotation = katanaStartRotation;
                    katana.localPosition = katanaStartPosition;
                    grapplePoint.gameObject.SetActive(false);
                    katana.GetChild(0).gameObject.layer = 6;
                    recalling = false;
                }
                transitionFinished = true;
                inputManager.SwitchStates(PlayerMovementManager.MovementState.Running);
                localAmplitude -= waveSpeed * Time.deltaTime;
            }
            
            skip: yield return null;
        }
        AudioManager.instance.PlaySound("katana", source);
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, katana.position - katana.forward * .75f);
    }
}
