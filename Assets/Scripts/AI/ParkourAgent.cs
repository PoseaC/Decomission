using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ParkourAgent : Agent
{
    //continuous actions:
    //  0 - move X
    //  1 - move z
    //  2 - camera x
    //  3 - camera y

    //discrete actions:
    //  0 - jump: 0, 1
    //  1 - sprint: 0, 1
    //  2 - grapple: 0, 1, 2

    float timer = 0;
    public float trainingSessionLength = 30;
    public override void OnActionReceived(ActionBuffers actions)
    {
        AgentIO.output.Movement = new Vector3(actions.ContinuousActions[0], 0, actions.ContinuousActions[1]);
        AgentIO.output.CameraOrientation = Quaternion.Euler(actions.ContinuousActions[2], actions.ContinuousActions[3], 0);
        AgentIO.output.Jump = actions.ContinuousActions[4] > 0;
        AgentIO.output.Sprint = actions.ContinuousActions[5] > 0;
        //AgentIO.output.Grapple = actions.ContinuousActions[6] < -0.25 ? 0 : actions.ContinuousActions[6] > 0.25 ? 1 : 2;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(AgentIO.input.AgentPosition);
        sensor.AddObservation(AgentIO.input.CameraRotation);
        sensor.AddObservation(AgentIO.input.CanGrapple);
        sensor.AddObservation(AgentIO.input.CanJump);
        sensor.AddObservation(AgentIO.input.FinishTime);
        sensor.AddObservation(AgentIO.input.Grounded);
        sensor.AddObservation(AgentIO.input.TargetDistance);
        sensor.AddObservation(AgentIO.input.TargetPosition);
    }

    public override void OnEpisodeBegin()
    {
        StageDirector.instance.ChangeLevel();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        AgentIO.input.TargetDistance = Vector3.Distance(AgentIO.input.TargetPosition, AgentIO.input.AgentPosition);

        if (AgentIO.output.Jump && AgentIO.output.Movement.magnitude != 0)
            SetReward(5);

        if (timer > trainingSessionLength)
        {
            timer = 0;
            SetReward(10 / AgentIO.input.TargetDistance);
            EndEpisode();
        }

        SetReward(Vector3.Dot(AgentIO.output.Movement, (AgentIO.input.TargetPosition - AgentIO.input.AgentPosition).normalized) * 50 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            SetReward(100f + 50f / AgentIO.input.FinishTime);
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Barrier"))
        {
            SetReward(-100f);
            EndEpisode();
        }
    }
}
