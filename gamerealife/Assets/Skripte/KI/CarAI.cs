using UnityEngine;

public class CarAI : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 5f;
    public float brakeDistance = 2f;
    public float rotationSpeed = 5f;
    public float inactiveTime = 10f;

    private int currentWaypointIndex;
    private bool isBraking;
    private bool isInactive;
    private float inactiveTimer;

    private void Start()
    {
        currentWaypointIndex = 0;
        isBraking = false;
        isInactive = false;
        inactiveTimer = 0f;
    }

    private void Update()
    {
        if (isInactive)
        {
            inactiveTimer += Time.deltaTime;
            if (inactiveTimer >= inactiveTime)
            {
                isInactive = false;
                inactiveTimer = 0f;
            }
            else
            {
                return;
            }
        }

        if (waypoints.Length == 0)
        {
            Debug.LogError("No waypoints assigned!");
            return;
        }

        Transform currentWaypoint = waypoints[currentWaypointIndex];

        // Bewegung zur aktuellen Zielposition (Waypoint)
        Vector3 targetDirection = currentWaypoint.position - transform.position;
        targetDirection.y = 0f;
        targetDirection.Normalize();

        if (Vector3.Distance(transform.position, currentWaypoint.position) < brakeDistance)
        {
            isBraking = true;
        }
        else
        {
            isBraking = false;
        }

        if (!isBraking)
        {
            // Bewegung vorwärts
            transform.position += targetDirection * speed * Time.deltaTime;

            // Drehung zur Zielrichtung
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Waypoint"))
        {
            other.gameObject.SetActive(false);
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }

            isInactive = true;
        }
    }
}
