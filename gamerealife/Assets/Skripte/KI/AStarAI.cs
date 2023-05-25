using UnityEngine;
using System.Collections.Generic;

public class AStarAI : MonoBehaviour
{
    public Transform target; // Zielobjekt
    public float movementSpeed = 5f; // Bewegungsgeschwindigkeit
    public float rotationSpeed = 5f; // Rotationsgeschwindigkeit
    public float avoidanceRadius = 2f; // Radius zum Vermeiden von Objekten
    public float fieldOfView = 60f; // Sichtfeld des Objekts
    public float maxAvoidanceDistance = 5f; // Maximaler Abstand, um dem Hindernis auszuweichen

    private List<Vector3> path; // Liste der Wegpunkte
    private int currentWaypointIndex = 0; // Aktueller Index des Wegpunkts

    private void Start()
    {
        // Pfad berechnen
        CalculatePath();
    }

    private void CalculatePath()
    {
        // Hier kannst du deinen eigenen Wegfindungsalgorithmus implementieren, um den Pfad zum Ziel zu berechnen.
        // Du könntest den A* Algorithmus verwenden oder andere Methoden, die dir zur Verfügung stehen.
        // Der resultierende Pfad sollte in der 'path' Liste gespeichert werden.

        // Beispiel: Ein einfacher Pfad direkt zum Ziel
        path = new List<Vector3>();
        path.Add(transform.position);
        path.Add(target.position);
    }

    private void Update()
    {
        if (path == null || path.Count == 0)
            return;

        // Wegpunkt erreicht
        if (Vector3.Distance(transform.position, path[currentWaypointIndex]) < 0.1f)
        {
            currentWaypointIndex++;

            // Ziel erreicht
            if (currentWaypointIndex >= path.Count)
            {
                Debug.Log("Ziel erreicht!");
                return;
            }
        }

        // Blickrichtung
        Vector3 targetDirection = path[currentWaypointIndex] - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

        // Kollisionsvermeidung
        if (!HasClearPath())
        {
            MoveAroundObstacle();
        }
        else
        {
            transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
        }
    }

    private bool HasClearPath()
    {
        Vector3 forward = transform.forward;
        Vector3 toTarget = (target.position - transform.position).normalized;

        float angle = Vector3.Angle(forward, toTarget);
        if (angle > fieldOfView * 0.5f)
        {
            return false;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, toTarget, out hit, maxAvoidanceDistance))
        {
            if (!hit.collider.CompareTag("Obstacle"))
            {
                return true;
            }
        }

        return false;
    }

    private void MoveAroundObstacle()
    {
        Quaternion leftRotation = Quaternion.Euler(0f, -rotationSpeed * Time.deltaTime, 0f);
        Quaternion rightRotation = Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f);

        while (!HasClearPath())
        {
            transform.rotation *= leftRotation;
        }

        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
    }
}
