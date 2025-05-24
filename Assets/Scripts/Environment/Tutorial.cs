using UnityEngine;
using TMPro;
using System.Collections;
//rather than using multiple objects for different tutorials, I use one with a set of tutorials and positions and move it where it needs to be
public class Tutorial : MonoBehaviour
{
    public string[] tutorials;
    public Vector3[] positions;
    public float speed;
    public TextMeshPro display;

    int tutorialIndex = 0;
    Coroutine activeTutorial;

    private void Start()
    {
        display.text = tutorials[tutorialIndex];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            try { StopCoroutine(activeTutorial); } catch { }
            activeTutorial = StartCoroutine(NextTutorial());
        }
    }

    IEnumerator NextTutorial()
    {
        Vector3 currentTransitionPosition;

        try
        {
            currentTransitionPosition = positions[tutorialIndex] - Vector3.up * 10;
        }
        catch
        {
            yield break;
        }

        while (transform.localPosition.y > currentTransitionPosition.y)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, currentTransitionPosition, speed);
            yield return null;
        }

        tutorialIndex += 1; 

        try 
        {
            Vector3 nextTransitionPosition = positions[tutorialIndex] - Vector3.up * 10;
            transform.localPosition = nextTransitionPosition;
            display.text = tutorials[tutorialIndex];
        } 
        catch 
        { 
            yield break; 
        }

        while (transform.localPosition.y < positions[tutorialIndex].y)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, positions[tutorialIndex], speed);
            yield return null;
        }
    }
}
