using UnityEngine;
using System.Collections.Generic;

public class AI : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float directionChangeTime = 10f;
    public float pauseTime = 2f;
    public float maxDistanceFromLayer = 10f;
    public LayerMask obstacleLayer;
    public LayerMask targetLayer;
    public List<Transform> waypoints;

    private enum Direction
    {
        North,
        East,
        South,
        West
    }

    private Direction currentDirection;
    private float directionTimer;
    private bool shouldChangeDirection;
    private bool isPaused;
    private float pauseTimer;
    private int currentWaypointIndex;
    private bool isTargetLayerReached;

    private void Start()
    {
        currentDirection = (Direction)Random.Range(0, 4);
        directionTimer = directionChangeTime;
        shouldChangeDirection = false;
        isPaused = false;
        pauseTimer = pauseTime;
        currentWaypointIndex = 0;
        isTargetLayerReached = false;
    }

    private void Update()
    {
        if (isPaused)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;
                pauseTimer = pauseTime;
                shouldChangeDirection = true;
            }
        }
        else
        {
            Move();
            directionTimer -= Time.deltaTime;
            if (directionTimer <= 0f || shouldChangeDirection)
            {
                shouldChangeDirection = false;
                directionTimer = directionChangeTime;
                ChangeDirection();
            }
        }
    }

    private void Move()
    {
        Vector3 newPosition = transform.position;

        // Check if there are waypoints to navigate
        if (waypoints.Count > 0)
        {
            // Get the current waypoint position
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;

            // Calculate the direction to the target waypoint
            Vector3 targetDirection = (targetPosition - transform.position).normalized;

            // Move towards the target waypoint
            newPosition += targetDirection * movementSpeed * Time.deltaTime;

            // Check if the AI has reached the current waypoint
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                // Pause at the waypoint before moving to the next one
                isPaused = true;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count; // Move to the next waypoint
            }
        }
        else
        {
            // Move in the current direction
            switch (currentDirection)
            {
                case Direction.North:
                    newPosition += transform.forward * movementSpeed * Time.deltaTime;
                    break;
                case Direction.East:
                    newPosition += transform.right * movementSpeed * Time.deltaTime;
                    break;
                case Direction.South:
                    newPosition -= transform.forward * movementSpeed * Time.deltaTime;
                    break;
                case Direction.West:
                    newPosition -= transform.right * movementSpeed * Time.deltaTime;
                    break;
            }
        }

        if (!CheckCollision(newPosition) && IsWithinLayer(newPosition))
        {
            transform.position = newPosition;
        }
        else
        {
            shouldChangeDirection = true;

            // Try to find a new direction towards the layer if maximum distance from the layer is reached
            if (!isTargetLayerReached && Vector3.Distance(transform.position, GetClosestPointOnLayer(transform.position)) >= maxDistanceFromLayer)
            {
                Vector3 newTargetDirection = (GetClosestPointOnLayer(transform.position) - transform.position).normalized;
                currentDirection = GetDirectionFromVector(newTargetDirection);
                isTargetLayerReached = true;
            }
        }
    }

    private void ChangeDirection()
    {
        currentDirection = (Direction)Random.Range(0, 4);
        isTargetLayerReached = false;
    }

    private bool CheckCollision(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.5f);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsWithinLayer(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity, obstacleLayer))
        {
            return true;
        }
        return Vector3.Distance(position, GetClosestPointOnLayer(position)) < maxDistanceFromLayer;
    }

    private Vector3 GetClosestPointOnLayer(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity, targetLayer))
        {
            return hit.point;
        }
        return position;
    }

    private Direction GetDirectionFromVector(Vector3 direction)
    {
        float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        if (angle < 0f)
        {
            angle += 360f;
        }

        if (angle >= 45f && angle < 135f)
        {
            return Direction.East;
        }
        else if (angle >= 135f && angle < 225f)
        {
            return Direction.South;
        }
        else if (angle >= 225f && angle < 315f)
        {
            return Direction.West;
        }
        else
        {
            return Direction.North;
        }
    }
}