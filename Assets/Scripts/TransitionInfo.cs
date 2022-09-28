using UnityEngine;
//this class keeps track of the player's camera orientation as well as his velocity, movement state and the current song title to display between scenes
public class TransitionInfo : MonoBehaviour
{
    public static TransitionInfo instance;

    public float cameraRotationX;
    public float cameraRotationY;
    public PlayerMovementManager.MovementState movementState;
    public Vector3 velocity;
    public string songTitle;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(this);
            return;
        }
    }
}
