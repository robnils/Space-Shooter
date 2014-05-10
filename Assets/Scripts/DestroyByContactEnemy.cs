using UnityEngine;
using System.Collections;

public class DestroyByContactEnemy : MonoBehaviour 
{
	public GameObject explosionPlayer;
	public GameObject explosionEnemy;
	public int scoreValueEnemy;
	private GameController gameController;
    public MoverEnemyShip moverEnemyShip;
    public GameObject explosionSoundEffect;
    public PlayerController playerController;
    // Sounds
    /*
    public AudioSource[] sounds;
    public AudioSource weapon;
    public AudioSource explosion;
    */
    

	void Start()
	{
        // Gamecontroller setup
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		
		if (gameControllerObject != null) 
		{
			gameController = gameControllerObject.GetComponent<GameController>();
		}
		
		if (gameControllerObject == null)
			Debug.Log ("Cannot find 'GameController' script");

        // Player controller object setup
        GameObject playerControllerObject = GameObject.FindWithTag("Player");

        if (playerControllerObject != null)
        {
            playerController = playerControllerObject.GetComponent<PlayerController>();
        }

        if (playerControllerObject == null)
            Debug.Log("Cannot find 'PlayerController' script");

        // Sounds
        /*
        sounds = GetComponents<AudioSource>();
        weapon = sounds[0];
        explosion = sounds[1]; // At [0] is weapon sound
         */
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.tag == "Boundary")
			return;

        if (other.tag == "Asteroid")
        {
            //gameController.test.text = "Asteroid collision in ContactEnemy";
            return;
        }

        if (other.tag == "BoltEnemy")
            return;

        if (other.tag == "EnemyShip")
        {
            //gameController.test.text = "Enemyship collision in ContactEnemy";
            return;
        }

		// If ship collides with player
        // Currently set up so this doesn't happen
		if (other.tag == "Player") 
		{
            // Display explosions and destroy ship and player
			Instantiate (explosionPlayer, transform.position, transform.rotation);
            Instantiate(explosionEnemy, other.transform.position, other.transform.rotation);
            //Destroy(other.gameObject);
            Destroy(gameObject);

            Debug.Log("Ship apparently collided with enemy ship - bug!");

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

        else if (other.tag == "Bolt")
        {
            // Destroy ship
            Instantiate(explosionEnemy, other.transform.position, other.transform.rotation);

            // Add score
            gameController.AddScore (scoreValueEnemy);
            explosionSoundEffect.audio.Play();
            Debug.Log("Is it working?");

            // Destroy the bolt and the ship
            Destroy(other.gameObject); 
            Destroy(gameObject);

            // Subtract from total list of enemies
            --gameController.totalNumberOfEnemies;

            gameController.test.text = gameController.totalNumberOfEnemies.ToString();
        }
        // Try else return to fix asteroid colliding asteroid problem?
        // -- Update, think making the collider a trigger fixed it
	}

}
