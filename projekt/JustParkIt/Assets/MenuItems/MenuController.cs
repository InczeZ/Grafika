using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) {
            SceneManager.LoadSceneAsync("Main Menu");
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            SceneManager.LoadSceneAsync("Level Select");
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadSceneAsync(currentSceneIndex);
        }
    }
}
