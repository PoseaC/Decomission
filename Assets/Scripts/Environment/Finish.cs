using UnityEngine;

public class Finish : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            AgentIO.input.FinishTime = levelManager.minutes * 60 + levelManager.seconds;
            levelManager.minutes = 0;
            levelManager.seconds = 0;
            StageDirector.instance.ChangeLevel();
        }
    }
}
