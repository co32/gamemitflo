using UnityEngine;

public class CarAI : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float accelerationTime = 2f;
    public float brakeForce = 50f;
    public float turnSpeed = 5f;
    public Transform[] waypoints;
    public LayerMask speedLimitLayers;
    public GameObject trafficLight;
    private int currentWaypointIndex = 0;

    private Rigidbody rb;
    private bool braking = false;
    private float currentSpeed = 0f;
    private float currentAcceleration = 0f;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        // Check if there are waypoints available
        if (waypoints.Length == 0)
            return;

        // Calculate distance to the current waypoint
        Vector3 targetDirection = waypoints[currentWaypointIndex].position - transform.position;
        float distanceToWaypoint = targetDirection.magnitude;

        // Move towards the current waypoint
        if (distanceToWaypoint < 1f && !braking)
        {
            // Reached the current waypoint, move to the next one
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
        else if (!braking)
        {
            // Rotate towards the target direction
            targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);

            // Accelerate to the maximum speed
            currentAcceleration += Time.fixedDeltaTime / accelerationTime;
            currentSpeed = Mathf.Lerp(0f, maxSpeed, currentAcceleration);

            // Apply speed limit based on layers
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, speedLimitLayers))
            {
                SpeedLimit speedLimitComponent = hit.transform.GetComponent<SpeedLimit>();
                if (speedLimitComponent != null)
                {
                    currentSpeed = Mathf.Min(currentSpeed, speedLimitComponent.GetSpeedLimit());
                }
            }


            // Check the state of the traffic light
            if (trafficLight != null && trafficLight.GetComponent<TrafficLight>().IsRed() && IsInTrigger())
            {
                currentSpeed = 0f;
            }

            rb.velocity = transform.forward * currentSpeed;
        }
        else if (braking && !IsInTrigger())
        {
            // Resume driving if braking and not in the trigger
            braking = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the triggered object is an obstacle or the traffic light trigger
        if (other.CompareTag("Obstacle") || other.CompareTag("TrafficLightTrigger"))
        {
            // Brake if an obstacle or the traffic light trigger enters the trigger
            rb.velocity = Vector3.zero;
            currentSpeed = 0f;
            currentAcceleration = 0f;
            braking = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the triggered object is an obstacle or the traffic light trigger
        if (other.CompareTag("Obstacle") || other.CompareTag("TrafficLightTrigger"))
        {
            // Resume driving if an obstacle or the traffic light trigger exits the trigger
            braking = false;
        }
    }

    private bool IsInTrigger()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1f);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("TrafficLightTrigger"))
            {
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        // Update the target position and rotation
        targetPosition = transform.position + rb.velocity * Time.deltaTime;
        targetRotation = Quaternion.LookRotation(rb.velocity);
    }
}
