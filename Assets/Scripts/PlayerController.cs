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
	public float speed; //  Scales the total speed
	public float tilt;
	public Boundary boundary;

	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;

	private float nextFire;

	void Update()
	{
		if (Input.GetButton ("Fire1") && Time.time > nextFire) 
		{
			nextFire = Time.time + fireRate;
			Instantiate (shot, shotSpawn.position, shotSpawn.rotation);
			audio.Play(); // plays the associated audio source - bolt sound
		}

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
