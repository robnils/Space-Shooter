using UnityEngine;
using System.Collections;

public class DestroyByContactMothership : MonoBehaviour 
{
    public GameObject explosionPlayer;
    public GameObject explosionEnemy;
    private GameController gameController;
    public PlayerController playerController;
    public MoverMothership moverMothership;
    public MothershipHealth mothershipHealth;
    public AudioSource[] gameMusic;

    private float currentHealth; // Boss current health
    public float maxHealth; // Boss max health
    public int scoreValueHitpoint; // Gain points for each hit on the boss
    public int weaponDamge; // Damage player bolts to enemy mothership
    public bool mothershipAlive;

    // Bar display
    private float barDisplay; //current progress
    public Vector2 pos = new Vector2(20, 40);
    public Vector2 size = new Vector2(60, 20);
    public Texture2D emptyTex;
    public Texture2D fullTex;

	// Use this for initialization
	void Start () 
    {
        // Initialise bar display and health
        barDisplay = 1.0f;
        currentHealth = maxHealth; 

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


        gameController.mothershipAlive = true;
        gameMusic[1].GetComponent<AudioSource>().Stop();
        playerController.powerupMusic.GetComponent<AudioSource>().Stop();
	}
    
    
    void OnGUI()
    {
        //barDisplay = 1.0f;
        //draw the background:
        GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), emptyTex);

        //draw the filled-in part:
        GUI.BeginGroup(new Rect(0, 0, size.x * barDisplay, size.y));
        GUI.Box(new Rect(0, 0, size.x, size.y), fullTex);
        GUI.EndGroup();
        GUI.EndGroup();
    }

    void Update()
    {
        //barDisplay = 1.0f - Time.time * 0.05f;
        barDisplay = currentHealth / maxHealth;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary")
            return;

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
            Instantiate(explosionPlayer, transform.position, transform.rotation);
            Instantiate(explosionEnemy, other.transform.position, other.transform.rotation);
            //Destroy(other.gameObject);
            Destroy(gameObject);

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
            // Deduct hit points
            currentHealth -= weaponDamge;
            //barDisplay -= Time.time * 0.05f;
            //mothershipHealth.barDisplay -= 10.0f;

            // Add score
            gameController.AddScore(scoreValueHitpoint);

            // Destroy the bolt and the ship
            Destroy(other.gameObject);
            Instantiate(explosionEnemy, other.transform.position, other.transform.rotation);

            if (currentHealth == 0)
            {
                moverMothership.BossMusicOff(); // doesnt work
                gameMusic[0].GetComponent<AudioSource>().Stop();

                // Add life to player
                playerController.AddLife();
                //StartCoroutine(gameController.AddLifeText()); // doesn't work from in here for some reason

                gameController.mothershipAlive = false;
                Destroy(gameObject);
                StartCoroutine(DelayedExplosions(3, 1.5f));
            }
        }
    }

    // Spawn "number" of explosions with "delay" between them
    IEnumerator DelayedExplosions(int number, float delay)
    {
        for (int i = 0; i < number; ++i)
        {
            Vector3 newPosition = new Vector3(transform.position.x + Random.Range(-3.0f, 3.0f),
                transform.position.y + Random.Range(-3.0f, 3.0f),
                transform.position.z + Random.Range(-3.0f, 3.0f));
            Instantiate(explosionEnemy, newPosition, transform.rotation);
            yield return new WaitForSeconds(delay);
        }
    }
}

