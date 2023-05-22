using UnityEngine;

public class AI : MonoBehaviour
{
    public float movementSpeed = 5f; // Geschwindigkeit der Bewegung
    public float directionChangeTime = 10f; // Zeit, nach der die Richtung ge�ndert wird
    public float pauseTime = 2f; // Zeit, in der die KI pausiert
    public float maxDistanceFromLayer = 10f; // Maximaler Abstand vom Layer
    public LayerMask obstacleLayer; // Layer der Hindernisse

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

    private void Start()
    {
        // Zuf�llige Startausrichtung festlegen
        currentDirection = (Direction)Random.Range(0, 4);
        directionTimer = directionChangeTime;
        shouldChangeDirection = false;
        isPaused = false;
        pauseTimer = pauseTime;
    }

    private void Update()
    {
        if (isPaused)
        {
            // Pausenzeit aktualisieren
            pauseTimer -= Time.deltaTime;

            // Wenn die Pausenzeit abgelaufen ist, die KI fortsetzen
            if (pauseTimer <= 0f)
            {
                isPaused = false;
                pauseTimer = pauseTime;
                shouldChangeDirection = true; // Richtung �ndern, um die Bewegung fortzusetzen
            }
        }
        else
        {
            // Bewegung in der aktuellen Richtung mit Geschwindigkeit
            Move();

            // Timer aktualisieren
            directionTimer -= Time.deltaTime;

            // Richtung �ndern, wenn der Timer abgelaufen ist oder es einen Abbiegebefehl gibt
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

        // Abh�ngig von der aktuellen Richtung in die gew�nschte Richtung bewegen
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

        // �berpr�fen, ob die neue Position eine Kollision mit einem Objekt verursacht oder au�erhalb des Layerbereichs liegt
        if (!CheckCollision(newPosition) && IsWithinLayer(newPosition))
        {
            transform.position = newPosition;
        }
        else
        {
            // Wenn eine Kollision auftritt oder au�erhalb des Layerbereichs liegt, die Richtung �ndern
            shouldChangeDirection = true;
        }
    }


    private void ChangeDirection()
    {
        // Zuf�llige Richtungs�nderung
        currentDirection = (Direction)Random.Range(0, 4);
    }

    private bool CheckCollision(Vector3 position)
    {
        // �berpr�fen, ob eine Kollision mit einem Objekt auftritt
        Collider[] colliders = Physics.OverlapSphere(position, 0.5f);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject)  // Das Kollisionscheck-Objekt ausschlie�en
            {
                return true;
            }
        }

        return false;
    }

    private bool IsWithinLayer(Vector3 position)
    {
        // �berpr�fen, ob die Position sich innerhalb des Layers oder innerhalb des maximalen Abstands vom Layer befindet
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity, obstacleLayer))
        {
            return true;
        }

        return Vector3.Distance(position, transform.position) < maxDistanceFromLayer;
    }
}
