using UnityEngine;
using System.Collections;

public class DestroyByContactEnemyBolt : MonoBehaviour 
{

    public GameObject explosionPlayer;
    public GameObject explosionEnemy;
    private GameController gameController;
    public PlayerController playerController;

    void Start()
    {
        // Game controller setup
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");

        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }

        if (gameControllerObject == null)
            Debug.Log("Cannot find 'GameController' script");

        // Player controller object setup
        GameObject playerControllerObject = GameObject.FindWithTag("Player");

        if (playerControllerObject != null)
        {
            playerController = playerControllerObject.GetComponent<PlayerController>();
        }

        if (playerControllerObject == null)
            Debug.Log("Cannot find 'PlayerController' script");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary")
            return;

        if (other.tag == "Asteroid")
            return;

        if (other.tag == "EnemyShip")
            return;

        if (other.tag == "Bolt")
            return;

        // If bolt collides with player
        if (other.tag == "Player")
        {
            // Player explosion
            Instantiate(explosionPlayer, transform.position, transform.rotation);

            // Destroy bolt and player objects
            //Destroy(other.gameObject);
            Destroy(gameObject);

            Debug.Log("bolt hit player,# of lives is: " + playerController.lives);

            // End game if out of lives
            if (playerController.lives == 0)
            {
                Destroy(other.gameObject);
                gameController.GameOver();
            }

            // Otherwise deduct a life
            else
                playerController.LoseLife(); // Subtract a life
        }
        
        else
            Destroy(gameObject);
    }
}
