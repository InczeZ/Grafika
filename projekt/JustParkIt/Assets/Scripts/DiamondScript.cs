using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DiamondScript : MonoBehaviour
{
    public GameObject[] checkpoints;
    public Rigidbody carRigidbody;  // Reference to the car's Rigidbody
    public GameObject resultTable;  // Reference to the result table UI
    public TextMeshProUGUI resultText;  // Reference to the Text component to display the time
    public GameObject Star1;
    public GameObject Star2;
    public GameObject Star3;
    private int currentCheckpointIndex = 0;
    private float startTime;
    public WheelController wheelController;

    void Start()
    {        
        startTime = Time.time;  // Start the timer
        resultTable.SetActive(false);  // Ensure the result table is initially inactive

        wheelController = carRigidbody.GetComponent<WheelController>();
    }

    public void CheckpointReached()
    {
        if (currentCheckpointIndex < checkpoints.Length)
        {
            checkpoints[currentCheckpointIndex].SetActive(false);
            currentCheckpointIndex++;

            if (currentCheckpointIndex < checkpoints.Length)
            {
                checkpoints[currentCheckpointIndex].SetActive(true);
            }
            else
            {
                Debug.Log("All checkpoints reached!");
                EndRace();
            }
        }
    }

    private void EndRace()
    {
        carRigidbody.velocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;

        wheelController.enabled = false;

        float playerTime = Time.time - startTime;

        int minutes = Mathf.FloorToInt(playerTime / 60F);
        int seconds = Mathf.FloorToInt(playerTime % 60F);

        resultText.text = $"{minutes:00}:{seconds:00}";
        resultTable.SetActive(true);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int x, y, z;
        if (currentSceneIndex == 2) {
            x = 60;
            y = 50;
            z = 45;
        } else if (currentSceneIndex == 3) {
            x = 65;
            y = 55;
            z = 50;
        } else {
            x = 70;
            y = 60;
            z = 55;
        }

        if(playerTime <= x)
            Star1.SetActive(true);
        if(playerTime <= y)
            Star2.SetActive(true);
        if(playerTime <= z)
            Star3.SetActive(true);

    }
}
