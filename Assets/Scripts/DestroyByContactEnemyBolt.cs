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
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");

        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }

        if (gameControllerObject == null)
            Debug.Log("Cannot find 'GameController' script");
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

        // If ship collides with player
        if (other.tag == "Player")
        {
            // Player explosion
            Instantiate(explosionPlayer, transform.position, transform.rotation);

            // Destroy bolt and player objects
            Destroy(other.gameObject);
            Destroy(gameObject);

            // End game if out of lives
            if (playerController.lives == 0)
                gameController.GameOver();

            // Otherwise deduct a life
            else
                --playerController.lives; // Subtract a life
        }
        
        else
            Destroy(gameObject);
    }
}
