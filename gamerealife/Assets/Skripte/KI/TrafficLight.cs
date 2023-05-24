using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public Renderer redLight;
    public Renderer yellowLight;
    public Renderer greenLight;

    private bool isRed = true;
    private bool isYellow = false;
    private bool isGreen = false;

    public float redDuration = 3f;
    public float yellowDuration = 2f;
    public float greenDuration = 4f;

    private float timer = 0f;

    private void Start()
    {
        // Initialize the traffic light
        ChangeToRed();
    }

    private void Update()
    {
        // Update the traffic light state based on the timer
        timer += Time.deltaTime;

        if (isRed && timer >= redDuration)
        {
            ChangeToGreen();
            timer = 0f;
        }
        else if (isGreen && timer >= greenDuration)
        {
            ChangeToYellow();
            timer = 0f;
        }
        else if (isYellow && timer >= yellowDuration)
        {
            ChangeToRed();
            timer = 0f;
        }
    }

    public bool IsRed()
    {
        return isRed;
    }

    public bool IsYellow()
    {
        return isYellow;
    }

    public bool IsGreen()
    {
        return isGreen;
    }

    public void ChangeToRed()
    {
        isRed = true;
        isYellow = false;
        isGreen = false;

        // Update light colors
        redLight.material.color = Color.red;
        yellowLight.material.color = Color.grey;
        greenLight.material.color = Color.grey;
    }

    public void ChangeToYellow()
    {
        isRed = false;
        isYellow = true;
        isGreen = false;

        // Update light colors
        redLight.material.color = Color.grey;
        yellowLight.material.color = Color.yellow;
        greenLight.material.color = Color.grey;
    }

    public void ChangeToGreen()
    {
        isRed = false;
        isYellow = false;
        isGreen = true;

        // Update light colors
        redLight.material.color = Color.grey;
        yellowLight.material.color = Color.grey;
        greenLight.material.color = Color.green;
    }
}
