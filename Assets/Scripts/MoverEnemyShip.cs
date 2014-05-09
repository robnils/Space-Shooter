using UnityEngine;
using System.Collections;

public class MoverEnemyShip : MonoBehaviour 
{
	public float speed; // Speed of ship
	public float tilt; // Ship tilt
	public Boundary boundary; // Playing area boundary
	private Vector3 movement; // the current velocity of the ship

    public GameObject shot; // The physical shot the enemy fires
    public Transform shotSpawn; // The gameobjects transform (~location) to hold our shot
    public float fireRate; // Ships fire rate
    private float nextFire; // when the next shot will occur

	public bool test = false;

    //Sounds
    /*
    public AudioSource[] sounds;
    public AudioSource weapon;
    public AudioSource explosion;
     */

	void Start()
	{
		// Transform.forward is the local x-axis
        // Give each ship a slightly different speed
        rigidbody.velocity = transform.right * (speed + Random.Range(-1.0f, 1.0f));
		movement = rigidbody.velocity; // save starting velocity
        rigidbody.rotation = Quaternion.Euler(180, 0, 0);

        // Sounds
        nextFire = 1; // Wait one second before firing
        /*
        sounds = GetComponents<AudioSource>();
        weapon = sounds[0];
        explosion = sounds[1];
         */
	}

	void FixedUpdate()
	{
        // Make ship fire bolts every fireRate inverse seconds 
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate + Random.Range(0.0f, 0.75f);
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);            
            audio.Play();
        }

		// Make the ship bounce off the left and right
		if(rigidbody.position.x >= boundary.xMax)
		{
            movement.x = -1 * movement.x * Random.Range(0.5f, 1.0f); 
			rigidbody.velocity = movement;
		}

		if(rigidbody.position.x <= boundary.xMin)
		{

            movement.x = -1 * movement.x * Random.Range(0.5f, 1.0f);
			rigidbody.velocity = movement;
		}

        // Make the ship bounce off top and bottom
        if (rigidbody.position.z >= boundary.zMax)
        {
            movement.z = -1 * movement.z * Random.Range(0.5f, 1.0f);
            rigidbody.velocity = movement;
        }

        if (rigidbody.position.z <= boundary.zMin)
        {

            movement.z = -1 * movement.x * Random.Range(0.5f, 1.0f);
            rigidbody.velocity = movement;
        }

        // Make ship bounded by boundaries
		rigidbody.position = new Vector3 
			(
				Mathf.Clamp(rigidbody.position.x, boundary.xMin, boundary.xMax),
				0.0f, 
				Mathf.Clamp(rigidbody.position.z, boundary.zMin, boundary.zMax)
				);

		// Ship movement tilt
		rigidbody.rotation = Quaternion.Euler(180,0,rigidbody.velocity.x*(-tilt));
        //shotSpawn.rotation = Quaternion.Euler(0, 0, 180);
	}
}
