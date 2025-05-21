using UnityEngine;

public class CheckPointScript : MonoBehaviour
{
    private DiamondScript diamondScript;

    void Start()
    {
        // Find the CheckpointManager in the scene
        diamondScript = FindObjectOfType<DiamondScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            diamondScript.CheckpointReached();
        }
    }
}
