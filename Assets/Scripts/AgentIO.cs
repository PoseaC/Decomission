using UnityEngine;

public class AgentIO : MonoBehaviour
{
    public static AgentIO instance;

    //Output
    private Vector3 movement;
    private Vector2 cameraOrientation;
    private bool sprint;
    private bool jump;
    private bool grapple;

    private void Awake()
    {
        instance = this;
    }
}
