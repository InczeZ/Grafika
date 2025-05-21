using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WheelScript : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    public float jumpForce = 50f;
    public float moveSpeed = 10f;
    private bool canJump = false;
    // Start is called before the first frame update
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ellenőrizzük, hogy a játékos érinti-e a specifikus collidert
        if (collision.gameObject.CompareTag("JumpSurface"))
        {
            canJump = true;
        }
        else if(collision.gameObject.CompareTag("LVL1")) {
            SceneManager.LoadSceneAsync("Level 1");
        }
        else if(collision.gameObject.CompareTag("LVL2")) {
            SceneManager.LoadSceneAsync("Level 2");
        }
        else if(collision.gameObject.CompareTag("LVL3")) {
            SceneManager.LoadSceneAsync("Level 3");
        }
        else if(collision.gameObject.CompareTag("Exit")) {
            SceneManager.LoadSceneAsync("Main Menu");
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Ha a játékos elhagyja a specifikus collidert
        if (collision.gameObject.CompareTag("JumpSurface"))
        {
            canJump = false;

        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Folyamatosan ellenőrizzük, hogy a játékos érinti-e a specifikus collidert
        if (collision.gameObject.CompareTag("JumpSurface"))
        {
            canJump = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newVelocity = myRigidbody.velocity;

        // Ugrás
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            newVelocity.y = jumpForce;
        }

        // Balra mozgás
        if (Input.GetKey(KeyCode.A))
        {
            newVelocity.x = -moveSpeed;
        }

        // Jobbra mozgás
        if (Input.GetKey(KeyCode.D))
        {
            newVelocity.x = moveSpeed;
        }

        // Az új sebesség alkalmazása
        myRigidbody.velocity = newVelocity;
        
    }
}
