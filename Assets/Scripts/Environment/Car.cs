using UnityEngine;

public class Car : MonoBehaviour
{
    [HideInInspector] public float speed = 5;
    [HideInInspector] public Traffic lane;
    float verticalAmplitude = 1;

    private void Start()
    {
        verticalAmplitude = Random.Range(2, 5);
    }

    private void FixedUpdate()
    {
        float verticalBop = Mathf.Sin(Time.time * Mathf.PI) * verticalAmplitude * Time.fixedDeltaTime;
        transform.position += Vector3.up * verticalBop + transform.forward * speed * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("TrafficLaneEnd"))
        {
            lane.availableCars.Enqueue(this);
            gameObject.SetActive(false);
        }
    }
}
