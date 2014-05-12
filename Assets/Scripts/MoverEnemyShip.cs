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

    // Evasive maneuver
    public bool evadeOn;
    private bool initialEvade;
    public float dodge;
    public float smoothing;
    public Vector2 startWait;
    public Vector2 maneuverTime;
    public Vector2 maneuverWait;

    private float currentSpeed;
    private float targetManeuver;

    // For testing
	public bool test = false;


	void Start()
	{
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
        
        // Speed keeps reassigning itself in inspector to 0
        // Turns out I was using enemyShipbk in gamecontroller
        //fireRate = 1.0f;
        //speed = 3.0f;        
        

		// Transform.forward is the local x-axis
        // Give each ship a slightly different speed
        rigidbody.velocity = transform.right * (speed + Random.Range(-1.0f, 1.0f));
		movement = rigidbody.velocity; // save starting velocity
        rigidbody.rotation = Quaternion.Euler(180, 0, 0);

        nextFire = 1; // Wait one second before firing
        
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

        // If moving downward to evade
        if (evadeOn)
        {
            if (initialEvade)
            {
                currentSpeed = rigidbody.velocity.z;
                StartCoroutine(Evade());

                initialEvade = false;
            }

            float newManeuver = Mathf.MoveTowards(rigidbody.velocity.x, targetManeuver, smoothing * Time.deltaTime);
            rigidbody.velocity = new Vector3(newManeuver, 0.0f, currentSpeed);

            // Make ship bounded by boundaries
            rigidbody.position = new Vector3
                (
                    Mathf.Clamp(rigidbody.position.x, boundary.xMin, boundary.xMax),
                    0.0f,
                    Mathf.Clamp(rigidbody.position.z, boundary.zMin, boundary.zMax)
                    );
        }

        else
        {

            // Make the ship bounce off the left and right
            if (rigidbody.position.x >= boundary.xMax)
            {
                movement.x = -1 * movement.x * Random.Range(0.5f, 1.0f);
                rigidbody.velocity = movement;
            }

            if (rigidbody.position.x <= boundary.xMin)
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

            // Make ship bounded by different boundaries
            rigidbody.position = new Vector3
                (
                    Mathf.Clamp(rigidbody.position.x, boundary.xMin, boundary.xMax),
                    0.0f,
                    Mathf.Clamp(rigidbody.position.z, -6.5f, boundary.zMax)
                    );
        }
		// Ship movement tilt
		rigidbody.rotation = Quaternion.Euler(180,0,rigidbody.velocity.x*(tilt));
        //shotSpawn.rotation = Quaternion.Euler(0, 0, 180);
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



