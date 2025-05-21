using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;
    public Camera camera3;    
    private int currentCameraIndex;

    void Start()
    {
        // Kezdéskor csak az első kamera legyen aktív
        currentCameraIndex = 0;
        camera1.gameObject.SetActive(true);
        camera2.gameObject.SetActive(false);
        camera3.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if(currentCameraIndex == 2){
                currentCameraIndex = 0;
            }
            else {
                currentCameraIndex++;
            }

        }
        SwitchCamera(currentCameraIndex);
    }

    void SwitchCamera(int cameraIndex)
    {
        // Kapcsoljuk ki az összes kamerát
        camera1.gameObject.SetActive(false);
        camera2.gameObject.SetActive(false);
        camera3.gameObject.SetActive(false);
        
        // Kapcsoljuk be a kiválasztott kamerát
        switch (cameraIndex)
        {
            case 0:
                camera1.gameObject.SetActive(true);
                break;
            case 1:
                camera2.gameObject.SetActive(true);
                break;
            case 2:
                camera3.gameObject.SetActive(true);
                break;
        }

        currentCameraIndex = cameraIndex;
    }
}
