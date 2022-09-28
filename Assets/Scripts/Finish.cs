using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    bool alreadyFinished = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !alreadyFinished)
        {
            alreadyFinished = true;
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            levelManager.stopTimer = true;
            levelManager.uiAnimator.Play("Base Layer.TimerStop", 0, 0);
            StartCoroutine(levelManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex)); //currently reloading the active scene when finished just for testing purposes
        }
    }
}
