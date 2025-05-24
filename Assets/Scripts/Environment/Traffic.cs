using UnityEngine;
using System.Collections.Generic;

public class Traffic : MonoBehaviour
{
    public float spawnPoolSize = 10;
    public float minSpeed = 15;
    public float maxSpeed = 30;
    public Car[] cars;
    public float spawnsPerSecond = 3;
    float lastSpawnElapsed = 0;
    float laneSpeed;

    [HideInInspector] public Queue<Car> availableCars;

    private void Awake()
    {
        laneSpeed = Random.Range(minSpeed, maxSpeed);
        availableCars = new Queue<Car>();

        for (int i = 0; i < spawnPoolSize; i++)
        {
            Car car = Instantiate(cars[Random.Range(0, cars.Length)], transform.position, transform.rotation);
            car.speed = laneSpeed;
            car.lane = this;
            car.gameObject.SetActive(false);
            availableCars.Enqueue(car);
        }
    }

    void FixedUpdate()
    {
        lastSpawnElapsed += Time.deltaTime;

        if (lastSpawnElapsed > 1 / spawnsPerSecond && availableCars.Count > 0)
        {
            lastSpawnElapsed = 0;
            Car spawn = availableCars.Dequeue();

            if (spawn.isActiveAndEnabled)
                return;

            spawn.transform.position = transform.position;
            spawn.gameObject.SetActive(true);
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 gizmoCenter = transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(gizmoCenter, Vector3.one * 5);
    }
}
