using UnityEngine;
public class StageDirector : MonoBehaviour
{
    public static StageDirector instance;
    public bool levelChanged = false;
    public Vector3[] startPositions;
    int activeLevel = 0;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (PlayerMovementManager.instance.playerBody.position.y < -200)
            ChangeLevel();
    }

    public void ChangeLevel()
    {
        levelChanged = true;
        activeLevel = Random.Range(0, startPositions.Length);
        PlayerMovementManager.instance.playerBody.velocity = Vector3.zero;
        PlayerMovementManager.instance.playerCamera.head.rotation = Quaternion.identity;
        PlayerMovementManager.instance.SwitchStates(PlayerMovementManager.MovementState.Walking);
        PlayerMovementManager.instance.playerBody.position = startPositions[activeLevel];
    }
}
