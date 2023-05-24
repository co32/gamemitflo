using UnityEngine;

public class Auto : MonoBehaviour
{
    public float maxSpeed = 10f;         // Maximale Geschwindigkeit des Autos
    public float maxReverseSpeed = 5f;   // Maximale Rückwärtsgeschwindigkeit des Autos
    public float acceleration = 5f;      // Beschleunigung des Autos
    public float deceleration = 5f;      // Verzögerung des Autos
    public float turnSpeed = 200f;       // Lenkgeschwindigkeit des Autos

    private float currentSpeed = 0f;     // Aktuelle Geschwindigkeit des Autos

    private void Update()
    {
        float moveInput = Input.GetAxis("Vertical");    // Vorwärts-/Rückwärts-Eingabe
        float turnInput = Input.GetAxis("Horizontal");  // Lenk-Eingabe

        // Vorwärts/Rückwärts fahren
        if (moveInput > 0)
        {
            // Vorwärts beschleunigen
            currentSpeed += acceleration * Time.deltaTime;

            // Begrenze die Geschwindigkeit auf die maximale Vorwärtsgeschwindigkeit
            currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
        }
        else if (moveInput < 0)
        {
            // Rückwärts beschleunigen
            currentSpeed -= acceleration * Time.deltaTime;

            // Begrenze die Geschwindigkeit auf die maximale Rückwärtsgeschwindigkeit
            currentSpeed = Mathf.Clamp(currentSpeed, -maxReverseSpeed, 0f);
        }
        else
        {
            // Keine Eingabe, das Auto verlangsamen
            if (currentSpeed > 0)
            {
                currentSpeed -= deceleration * Time.deltaTime;

                // Begrenze die Geschwindigkeit auf 0 (keine Bewegung)
                currentSpeed = Mathf.Max(currentSpeed, 0f);
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += deceleration * Time.deltaTime;

                // Begrenze die Geschwindigkeit auf 0 (keine Bewegung)
                currentSpeed = Mathf.Min(currentSpeed, 0f);
            }
        }

        // Lenken nur, wenn die Geschwindigkeit nicht null ist
        if (currentSpeed != 0f)
        {
            // Berechne die Lenkgeschwindigkeit basierend auf der Geschwindigkeit des Autos
            float turnFactor = 1f - Mathf.Abs(currentSpeed / maxSpeed);
            float currentTurnSpeed = turnSpeed * turnFactor;

            transform.Rotate(Vector3.up, turnInput * currentTurnSpeed * Time.deltaTime);
        }

        // Bewege das Auto in die aktuelle Richtung und Geschwindigkeit
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // Kollision mit Hindernis, das Auto stoppen
            currentSpeed = 0f;
        }
    }
}
