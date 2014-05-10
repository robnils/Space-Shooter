﻿using UnityEngine;
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

    // Powerup Mode
    public bool powerUpOn;
    public GameObject shotLeft;
    public GameObject shotRight;
    public Transform shotSpawnLeft;
    public Transform shotSpawnRight;
    public GameObject powerupMusic;
    public GameObject powerupSoundEffect;
    public GameObject powerupWeapon;
    public float volMax; // The time for the music to come up to full volume
    private bool firstTime;
    //public GameController gameController;
    public GameObject backgroundMusic;
	
    void Start()
    {
        powerUpOn = false;
        volMax = 0.5f;
        firstTime = true;
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

    // Sets up power up mode
    private void PowerupMode()
    {
        // Play/Stop music
        powerupMusic.audio.Play();
        backgroundMusic.audio.Stop();
        powerupSoundEffect.audio.Play();

        // Trigger the event only one
        firstTime = false;
        fireRate -= 0.05f; // Fire faster
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
