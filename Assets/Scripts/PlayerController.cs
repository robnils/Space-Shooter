using UnityEngine;
using System.Collections;



// Class containing the game boundary
[System.Serializable] // Needed so the inspector can "see" our class
public class Boundary
{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour 
{
    // Normal mode
    public float speed; //  Scales the total speed
    public float tilt;
    public Boundary boundary;
	public GameObject shot;   
	public Transform shotSpawn;
    public float fireRate;
    private float nextFire;

    public int lives;
    public int maxLives;
    private Vector3 lifePosition;
    private int newLivesCount; // Keeps track of the lives the player has gained

    // Powerup Mode
    public bool powerUpOn;
    public GameObject player;
    public GameObject shotLeft;
    public GameObject shotRight;
    public Transform shotSpawnLeft;
    public Transform shotSpawnRight;
    public GameObject powerupMusic;
    public GameObject powerupSoundEffect;
    public GameObject powerupWeapon;

    public GameController gameController;

    public GameObject livesObject;
    private GameObject[] livesObjectArray;

    public GUIText poweredUpText;
    private bool firstTime;
    public GameObject backgroundMusic;
    private Transform t;
	
    void Start()
    {
        powerUpOn = false;
        firstTime = true;
        lives = 3;
        newLivesCount = 0;
        //maxLives = 5;

        // Game controller object set up
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");

        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }

        if (gameControllerObject == null)
            Debug.Log("Cannot find 'GameController' script");
        
    }
	public void Update()
	{
        // Turns on power mode
        /*
        if (powerUpOn)
        {
            powerupMusic.audio.Play();
            firstTime = false;
            //PowerupMode();
            //StartCoroutine(PowerupModeEnum());
        }*/

		if (Input.GetButton ("Fire1") && Time.time > nextFire) 
		{
            if (!powerUpOn)
            {
                nextFire = Time.time + fireRate;
                Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
                audio.Play(); // plays the associated audio source - bolt sound
            }

            else
            {
                // Powerup Mode on
                if (firstTime)
                {
                    PowerupMode();
                }

                nextFire = Time.time + fireRate;
                PowerUpMultipleBolts();
            }
		}
	}

    // Setup up lives
    public void SetUpLives()
    {
        livesObjectArray = new GameObject[lives];
        for (int i = 0; i < lives; i++)
        {
            lifePosition = new Vector3(12.2f - i*1.3f, 0.0f, -4.0f);            
            livesObjectArray[i] = (GameObject)Instantiate(livesObject, lifePosition, Quaternion.identity);            
        }
    }

    // What to do when a player loses his life
    public void LoseLife()
    {
        --lives;       
        Destroy(livesObjectArray[lives]);  
        transform.position = new Vector3(0.0f, 0.0f, 0.0f); // Reset position

        // Restart the wave
        // StartCoroutine(gameController.SpawnWaves());     // doesnt really work
    }

    // Add a life
    public void AddLife()
    {
        // If user hasn't reached max lives and addLife is true
        // Should play a sound effect as well
        if (lives != maxLives)
        {
            // Clear lives array
            for (int i = 0; i < livesObjectArray.Length; ++i)
            {
                Destroy(livesObjectArray[i]);
            }

            ++lives;
            livesObjectArray = new GameObject[lives];

            // Displays lives in rows of 3
            int row = 0;
            int column = 0;
            for (int i = 0; i < lives; i++)
            {
                
                // Ugly, need to fix (but works)
                if (i >= 3)
                {
                    // First instance, reset column alignment
                    if(i == 3)
                        column = 0;
                    //++row;
                    // idea is to substitue 1 and 0 with row and loop over that, but the lives
                    // end up too high up                     

                    lifePosition = new Vector3(12.2f - column * 1.3f, 0.0f, -4.0f + 1 * 1.55f);
                    livesObjectArray[i] = (GameObject)Instantiate(livesObject, lifePosition, Quaternion.identity);
                }

                else
                {
                    lifePosition = new Vector3(12.2f - column * 1.3f, 0.0f, -4.0f + 0 * 1.55f);
                    livesObjectArray[i] = (GameObject)Instantiate(livesObject, lifePosition, Quaternion.identity);
                }
                ++column;
            }
        }
    }

    // Sets up power up mode
    private void PowerupMode()
    {
        // Play/Stop music
        powerupMusic.audio.Play();
        backgroundMusic.audio.Stop();
        powerupSoundEffect.audio.Play();
        StartCoroutine(PoweredUpText());

        // Trigger the event only one
        firstTime = false;
        fireRate -= 0.05f; // Fire faster
    }

    IEnumerator PoweredUpText()
    {
        poweredUpText.text = "Powered Up!";

        // loop over colours
        // change distance
        //yield return new WaitForSeconds(3.0f);
        //Transform t = new Transform();
        t = poweredUpText.transform; // Store text position

        float delta= 0.01f;
        int sign = 1;
        int rand = 1;
        /*
        for (float i = 0.0f; i < 3.0f; i += 0.1f)
        {
            // Creates random number, effectively either +1 or -1
            rand = Random.Range(-1, 1);
            //if (rand == 0)
              //  rand = -1;
            /*
            if (sign == 1)
                sign = -1;

            else
                sign = 1;
            
            // Play with colours
           
            //poweredUpText.color = new Color(104.0f - 20.0f * i, 255.0f - 20.0f * i, 20.0f * i, 255.0f);
            //poweredUpText.text = i.ToString();
            //poweredUpText.color = new Color(104, 255, 0, 255);
            // poweredUpText.text = j.ToString();
            // Create a bounded box and let it go crazy there
            // Move position up or down a bit randomly
            //poweredUpText.transform.position = new Vector3(t.position.x + delta*sign, t.position.y + delta*sign, 0);

            yield return new WaitForSeconds(0.1f);
        }
        */

        yield return new WaitForSeconds(3.0f);       
        poweredUpText.enabled = false;
    }

    private IEnumerator PowerupModeEnum()
    {
        yield return new WaitForSeconds(1.0f);
        powerupMusic.audio.Play();
    }
    

    
    // Sets up power up mode
    /*
    private IEnumerator PowerupMode()
    {
        powerupSoundEffect.audio.Play();

        // Turn down the music, start playing it 
        powerupMusic.audio.volume = 0.0f;
        powerupMusic.audio.Play();

        // Takes one seconds to "volume up"
        for (int i = 0; i < 10; ++i)
        {
            powerupMusic.audio.volume += 0.05f; // beause we want a maximum of 0.5 for our music                 
            yield return new WaitForSeconds(0.1f);
        }

        //Fancy loop
        //for (float i = 1.0f; i <= volMax; ++i)
        //{
          //  powerupMusic.audio.volume += 1.0f / volMax;
            //yield return new WaitForSeconds(5.0f);
        //}
    }*/

    // Activates multiple firing 
    private void PowerUpMultipleBolts()
    {        
        Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        powerupWeapon.audio.Play();
        Instantiate(shotLeft, shotSpawnLeft.position, shotSpawnLeft.rotation);
        powerupWeapon.audio.Play();
        Instantiate(shotRight, shotSpawnRight.position, shotSpawnRight.rotation);
        powerupWeapon.audio.Play();
    }

	void FixedUpdate()
	{       
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		// 2D plane so y cmmpt = 0
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		rigidbody.velocity = speed*movement;

        //Mouse movement   
        // If mouse moves...
        
        if (Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse X") < 0
            || Input.GetAxis("Mouse Y") > 0 || Input.GetAxis("Mouse Y") < 0)
        {
            // Create new position vectors at those new mouse positions
            moveHorizontal = Input.GetAxis("Mouse X");
            moveVertical = Input.GetAxis("Mouse Y");

            movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            rigidbody.velocity = speed * movement * 1.5f; // 1.5f to increase mouse sensitivity
        }      

		// Creates a new position vector each frame that is "clamped" 
		// border-wise by xMin, xMax, zMin, zMax
		rigidbody.position = new Vector3 
		(
				Mathf.Clamp(rigidbody.position.x, boundary.xMin, boundary.xMax),
				0.0f, 
				Mathf.Clamp(rigidbody.position.z, boundary.zMin, boundary.zMax)
		);

		// For the tilt when moving along horizontal axis
		// Negative so it doesn't tilt toward the player
		rigidbody.rotation = Quaternion.Euler(0,0,rigidbody.velocity.x*(-tilt));
	}
}
