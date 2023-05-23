using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject uiSystem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            uiSystem.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            uiSystem.SetActive(false);
        }
    }
}
