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
    public override void OnActionReceived(ActionBuffers actions)
    {
        AgentIO.output.Movement = new Vector3(actions.ContinuousActions[0], 0, actions.ContinuousActions[1]);
        AgentIO.output.CameraOrientation = Quaternion.Euler(actions.ContinuousActions[2], actions.ContinuousActions[3], 0);
        AgentIO.output.Jump = actions.DiscreteActions[0] == 1;
        AgentIO.output.Sprint = actions.DiscreteActions[1] == 1;
        AgentIO.output.Grapple = actions.DiscreteActions[2];
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

        foreach(var platform in AgentIO.input.PlatformPositionsDistances)
        {
            sensor.AddObservation(platform.Key);
            sensor.AddObservation(platform.Value);
        }
    }

    public override void OnEpisodeBegin()
    {
        StageDirector.instance.ChangeLevel();
    }

    private void Update()
    {
        if (PlayerMovementManager.instance.playerBody.position.y < -200)
        {
            SetReward(-1);
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            LevelManager levelManager = FindObjectOfType<LevelManager>();

            SetReward(1 / (levelManager.minutes * 60 + levelManager.seconds));
            levelManager.minutes = 0;
            levelManager.seconds = 0;
            EndEpisode();
        }
    }
}
