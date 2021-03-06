﻿using UnityEngine;
using System.Collections;

public class MoverEnemyShip : MonoBehaviour 
{
	public float speed; // Speed of ship
    public float newSpeed; // To see the new speed in the inspector
	public float tilt; // Ship tilt
	public Boundary boundary; // Playing area boundary
	private Vector3 movement; // the current velocity of the ship

    public GameObject shot; // The physical shot the enemy fires
    public Transform shotSpawn; // The gameobjects transform (~location) to hold our shot
    public float fireRate; // Ships fire rate
    private float nextFire; // when the next shot will occur

    // Evasive maneuver
    public bool evadeOn;
    private bool initialEvade;
    public float dodge;
    public float smoothing;
    public Vector2 startWait;
    public Vector2 maneuverTime;
    public Vector2 maneuverWait;
    public GameController gameController;

    private float currentSpeed;
    private float targetManeuver;

    // For testing
	public bool test = false;
    public float f;

	void Start()
	{
        // Gamecontroller setup
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");

        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }

        if (gameControllerObject == null)
            Debug.Log("Cannot find 'GameController' script");

        // Evade off initialiity
        evadeOn = true;
        initialEvade = true;

        // Initial values        
        startWait.x = 0.5f;
        startWait.y = 1;
        maneuverTime.x = 1;
        maneuverTime.y = 2;
        dodge = 5;
        smoothing = 7.5f;
        maneuverWait.x = 1;
        maneuverWait.y = 2;   

		// Transform.forward is the local x-axis
        // Give each ship a slightly different speed
        //newSpeed = speed + Random.Range(-1.0f, 1.0f);
        //rigidbody.velocity = transform.right * (newSpeed);
		//movement = rigidbody.velocity; // save starting velocity

        // Needed to flip the ship
        GetComponent<Rigidbody>().rotation = Quaternion.Euler(180, 0, 0);

        // Gives downward velocity
        newSpeed = speed + Random.Range(-1.0f, 1.0f);
        
        // Weird game bug - the ships go the wrong direction when spawned from the mother ship!
        if (gameController.mothershipAlive)
            GetComponent<Rigidbody>().velocity = transform.forward * (newSpeed);

        else
        {
            newSpeed = speed + 2 + Random.Range(-1.0f, 1.0f);
            GetComponent<Rigidbody>().velocity = transform.forward * (-newSpeed);            
        }

        movement = GetComponent<Rigidbody>().velocity;
        boundary.zMin = 8.5f;
        nextFire = Time.time + 1; // Wait one second before firing
	}

	void FixedUpdate()
	{       
        // Make ship fire bolts every fireRate inverse seconds 
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate + Random.Range(0.0f, 0.75f);
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);            
            GetComponent<AudioSource>().Play();
        }

        // If moving downward to evade
        if (evadeOn)
        {
            //boundary.zMin = 8.5f;
            if (initialEvade)
            {
                currentSpeed = GetComponent<Rigidbody>().velocity.z;                
                StartCoroutine(Evade());

                initialEvade= false;
            }

            float newManeuver = Mathf.MoveTowards(GetComponent<Rigidbody>().velocity.x, targetManeuver, smoothing * Time.deltaTime);
            GetComponent<Rigidbody>().velocity = new Vector3(newManeuver, 0.0f, currentSpeed);

            /*
            float delta = 0.1f;
            if ((rigidbody.position.z >= boundary.zMin - delta) && (rigidbody.position.z <= boundary.zMin + delta))
            {
                //rigidbody.velocity = transform.right * (speed + Random.Range(-1.0f, 1.0f));
                currentSpeed = (2 * (Random.Range(0, 1)) - 1) * (speed + Random.Range(-1.0f, 1.0f));
                rigidbody.velocity = new Vector3(currentSpeed, 0.0f, 0.0f);
                //rigidbody.velocity = new Vector3(2*(Random.Range(0,1)-1)*(speed + Random.Range(-1.0f, 1.0f)), 0.0f, 0.0f);
                //currentSpeed = rigidbody.velocity.z;
                //StartCoroutine(Evade());
                Bounce();
            }

            else
            {
                float newManeuver = Mathf.MoveTowards(rigidbody.velocity.x, targetManeuver, smoothing * Time.deltaTime);
                rigidbody.velocity = new Vector3(newManeuver, 0.0f, currentSpeed);
            }
             * */

            /*
            f = Random.Range(0.0f, 1.0f);
            if (f >= 0.5f)
            {
                if (rigidbody.position.z == boundary.zMin)
                {
                    rigidbody.velocity = transform.right * (speed + Random.Range(-1.0f, 1.0f));
                }
            }*/

            // Make ship bounded by boundaries
            // Use random number generator to clamp into different rows
            GetComponent<Rigidbody>().position = new Vector3
                (
                    Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax),
                    0.0f,
                    Mathf.Clamp(GetComponent<Rigidbody>().position.z, -6.5f, 16.7f)
                    );

            
            /*
            f = Random.Range(0.0f, 1.0f);
            if (f >= 0.5f)
            {
                boundary.zMin = -6.5f;
            }

            else
                boundary.zMax = 8.5f;
             * */
        }

        else
        {

            // Make the ship bounce off the left and right
            if (GetComponent<Rigidbody>().position.x >= boundary.xMax)
            {
                movement.x = -1 * movement.x * Random.Range(0.5f, 1.0f);
                GetComponent<Rigidbody>().velocity = movement;
            }

            if (GetComponent<Rigidbody>().position.x <= boundary.xMin)
            {

                movement.x = -1 * movement.x * Random.Range(0.5f, 1.0f);
                GetComponent<Rigidbody>().velocity = movement;
            }

            // Make the ship bounce off top and bottom
            if (GetComponent<Rigidbody>().position.z >= boundary.zMax)
            {
                movement.z = -1 * movement.z * Random.Range(0.5f, 1.0f);
                GetComponent<Rigidbody>().velocity = movement;
            }

            if (GetComponent<Rigidbody>().position.z <= boundary.zMin)
            {
                movement.z = -1 * movement.x * Random.Range(0.5f, 1.0f);
                GetComponent<Rigidbody>().velocity = movement;
            }

            // Make ship bounded by different boundaries
            GetComponent<Rigidbody>().position = new Vector3
                (
                    Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax),
                    0.0f,
                    Mathf.Clamp(GetComponent<Rigidbody>().position.z, -6.5f, boundary.zMax)
                    );
        }
		// Ship movement tilt
		GetComponent<Rigidbody>().rotation = Quaternion.Euler(180,0,GetComponent<Rigidbody>().velocity.x*(tilt));
        //shotSpawn.rotation = Quaternion.Euler(0, 0, 180);
	}

    // Make ship bounce off corners
    private void Bounce()
    {
        // Make the ship bounce off the left and right
        if (GetComponent<Rigidbody>().position.x >= boundary.xMax)
        {
            movement.x = -1 * movement.x * Random.Range(0.5f, 1.0f);
            GetComponent<Rigidbody>().velocity = movement;
        }

        if (GetComponent<Rigidbody>().position.x <= boundary.xMin)
        {
            movement.x = -1 * movement.x * Random.Range(0.5f, 1.0f);
            GetComponent<Rigidbody>().velocity = movement;
        }

        // Make the ship bounce off top and bottom
        if (GetComponent<Rigidbody>().position.z >= boundary.zMax)
        {
            movement.z = -1 * movement.z * Random.Range(0.5f, 1.0f);
            GetComponent<Rigidbody>().velocity = movement;
        }

        if (GetComponent<Rigidbody>().position.z <= boundary.zMin)
        {
            movement.z = -1 * movement.x * Random.Range(0.5f, 1.0f);
            GetComponent<Rigidbody>().velocity = movement;
        }
    }

    // Evasive maneuvers
    IEnumerator Evade()
    {
        yield return new WaitForSeconds(Random.Range(startWait.x, startWait.y));
        while (true)
        {
            targetManeuver = Random.Range(1, dodge) * -Mathf.Sign(transform.position.x);
            yield return new WaitForSeconds(Random.Range(maneuverTime.x, maneuverTime.y));
            targetManeuver = 0;
            yield return new WaitForSeconds(Random.Range(maneuverWait.x, maneuverWait.y));
        }
    }
}



