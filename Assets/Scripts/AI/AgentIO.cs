using UnityEngine;

public class AgentIO : MonoBehaviour
{
    public class Output
    {
        private Vector3 movement;
        private Quaternion cameraOrientation;
        private bool sprint;
        private bool jump;
        private int grapple;

        public Vector3 Movement { get => movement; set => movement = value; }
        public Quaternion CameraOrientation { get => cameraOrientation; set => cameraOrientation = value; }
        public bool Sprint { get => sprint; set => sprint = value; }
        public bool Jump { get => jump; set => jump = value; }
        public int Grapple { get => grapple; set => grapple = value; }
    }

    public class Input
    {
        private Quaternion cameraRotation;
        private Vector3 agentPosition;
        private Vector3 targetPosition;
        private float targetDistance;
        private float finishTime;
        private bool grounded;
        private bool canJump;
        private bool canGrapple;

        public Quaternion CameraRotation { get => cameraRotation; set => cameraRotation = value; }
        public Vector3 AgentPosition { get => agentPosition; set => agentPosition = value; }
        public Vector3 TargetPosition { get => targetPosition; set => targetPosition = value; }
        public float TargetDistance { get => targetDistance; set => targetDistance = value; }
        public float FinishTime { get => finishTime; set => finishTime = value; }
        public bool Grounded { get => grounded; set => grounded = value; }
        public bool CanJump { get => canJump; set => canJump = value; }
        public bool CanGrapple { get => canGrapple; set => canGrapple = value; }
    }

    public static Input input = new Input();
    public static Output output = new Output();
}
