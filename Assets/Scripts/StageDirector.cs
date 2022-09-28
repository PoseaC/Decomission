using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

//this class is used to play and intro and outro animation for each stage by moving each object into place at the start and out of sight at the end of the level
public class StageDirector : MonoBehaviour
{
    public float changeSpeed = 750; //how fast the objects get into position
    public float delayBetweenObjects = .2f; //how long to wait before starting to move each object
    public float startOffset = 1000; //how far down the object starts

    Transform[] children;
    Transform player;
    float lowestLimit;
    LevelManager levelManager;
    void Awake()
    {
        children = transform.GetComponentsInChildren<Transform>().Skip(1).ToArray(); //get all the children of this object without the first one which would return the object itself
        player = FindObjectOfType<PhysicsPlayerMovement>().transform;
        levelManager = FindObjectOfType<LevelManager>();

        //move all objects into the starting position and determine the lowest point of the stage to know when the player has fallen out
        //using a trigger to determine the boundries of the stage could lead to a skilled player building so much speed that it avoids it entirely and the level never restarts
        foreach (Transform child in children)
        {
            if (child.localPosition.y < lowestLimit)
                lowestLimit = child.position.y;
            child.localPosition -= Vector3.up * startOffset;
        }

        lowestLimit -= (FindObjectOfType<Grapple>().grappleDistance + 50);
        StartCoroutine(SetStage());
    }

    private void FixedUpdate()
    {
        if (player.position.y < lowestLimit && !levelManager.alreadyLoading)
        {
            StartCoroutine(levelManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex));
        }
    }

    public IEnumerator SetStage()
    {
        foreach (Transform child in children)
        {
            bool last = child == children.Last();
            Vector3 endPosition = child.localPosition + Vector3.up * startOffset;
            StartCoroutine(SetObject(child, endPosition, last));
            yield return new WaitForSeconds(delayBetweenObjects);
        }
    }

    IEnumerator SetObject(Transform child, Vector3 endPosition, bool last)
    {
        while (child.localPosition != endPosition)
        {
            child.localPosition = Vector3.MoveTowards(child.localPosition, endPosition, changeSpeed * Time.deltaTime);
            yield return null;
        }
        if (last)
            levelManager.canLoadScene = true;
    }
}
