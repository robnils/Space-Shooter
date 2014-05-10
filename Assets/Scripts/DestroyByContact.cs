using UnityEngine;
using System.Collections;

public class DestroyByContact : MonoBehaviour 
{
	public GameObject explosion;
	public GameObject explosionPlayer;
	public int scoreValue;
	private GameController gameController;
    public PlayerController playerController;

	void Start()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");

		if (gameControllerObject != null) 
		{
			gameController = gameControllerObject.GetComponent<GameController>();
		}

		if (gameControllerObject == null)
			Debug.Log ("Cannot find 'GameController' script");
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.tag == "Boundary")
			return;

        if (other.tag == "EnemyShip")
        {
            //gameController.test.text = "Enemyship collision in Contact";
            return;
        }

        if (other.tag == "BoltEnemy")
            return;

        if (other.tag == "Asteroid")
            return;

        if (other.tag == "Player")
        {          
            // Display explosions and destroy player and asteroid
            Instantiate(explosionPlayer, transform.position, transform.rotation);
            Instantiate(explosion, other.transform.position, other.transform.rotation);
            Destroy(gameObject);
            Destroy(other.gameObject);

            // End game if out of lives
            if (playerController.lives == 0)
                gameController.GameOver();

            // Otherwise deduct a life
            else
                --playerController.lives; // Subtract a life
            
        }

        else if (other.tag == "Bolt")
        {
            // Destroy asteroid
            Instantiate(explosion, other.transform.position, other.transform.rotation);

            // Add score
            gameController.AddScore(scoreValue);
            
            Destroy(other.gameObject); // Destroy colliding object
            Destroy(gameObject); // Destroy asteroid
        }
	}


}
