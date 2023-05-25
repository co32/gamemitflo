using UnityEngine;

public class WayointGenerator : MonoBehaviour
{
    public int numberOfWaypoints = 10;
    public float minDistance = 1f;
    public float maxDistance = 10f;

    public GameObject waypointPrefab;
    public Transform waypointsParent;

    void Start()
    {
        GenerateWaypoints();
    }

    void GenerateWaypoints()
    {
        for (int i = 0; i < numberOfWaypoints; i++)
        {
            Vector3 randomPosition = GetRandomPointOnRoad();
            GameObject waypoint = Instantiate(waypointPrefab, randomPosition, Quaternion.identity, waypointsParent);
            waypoint.name = "Waypoint " + (i + 1);
        }
    }

    Vector3 GetRandomPointOnRoad()
    {
        RaycastHit hit;
        Vector3 randomPoint;

        do
        {
            randomPoint = transform.position + Random.insideUnitSphere * Random.Range(minDistance, maxDistance);
            randomPoint.y = transform.position.y; // Set the Y-coordinate to the same as the generator's position

        } while (!Physics.Raycast(randomPoint, -transform.up, out hit));

        return hit.point;
    }
}
