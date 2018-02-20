using UnityEngine;
using System.Collections;

public class MoverMothership : MonoBehaviour 
{
    public Boundary boundary;
    private Vector3 movement, newMovement; // Need to save old movement so speed doesn't accumulate too fast
    public float speed;

    public GameObject shot; // The physical shot the enemy fires
    public GameObject ship; // Ship to be spawned
    public Transform enemySpawn; // Location to be spawned at
    public Transform shotSpawnLeft; // The gameobjects transform (~location) to hold our shot
    public Transform shotSpawnRight;
    public float fireRate; // Ships fire rate
    public float spawnRate; // Rate at which enemy ships spawn from mother ship
    public float waveRateBolt;
    public float waveRateShip;
    //private float nextFire; // when the next shot will occur
    //private float nextSpawn;
    private float nextWaveBolt;
    private float nextWaveShip;

    public GameObject backgroundMusic;
    public GameObject bossMusic;

	// Use this for initialization
	void Start () 
    {
        BossMusicOn();
        
        GetComponent<Rigidbody>().velocity = transform.right * (speed);
        movement = GetComponent<Rigidbody>().velocity; // save starting velocity
        newMovement = movement;

        // Needed to flip the ship orientation
        GetComponent<Rigidbody>().rotation = Quaternion.Euler(180, 0, 0);

        nextWaveBolt = Time.time + 1;
        nextWaveShip = Time.time + 5;

        // Unneeded?
        //nextSpawn = Time.time + 5;
        //nextFire = Time.time + 1;        
	}

    public void BossMusicOn()
    {
        // Change music
        backgroundMusic.GetComponent<AudioSource>().Stop();
        bossMusic.GetComponent<AudioSource>().Play();
    }

    public void BossMusicOff()
    {
        // Change music
        bossMusic.GetComponent<AudioSource>().Stop();
        backgroundMusic.GetComponent<AudioSource>().Play(); 
    }
	
	// Update is called once per frame
    void Update()
    {
        // Want ship to descend downwards, not just suddenly appear
        if (Time.time > nextWaveBolt)
        {
            nextWaveBolt = Time.time + waveRateBolt + Random.Range(0.0f, 0.75f);
            StartCoroutine(FireWave(5));
        }

        if (Time.time > nextWaveShip)
        {
            nextWaveShip = Time.time + waveRateShip + Random.Range(0.0f, 0.75f);
            StartCoroutine(SpawnWave(6));
            //nextSpawn = Time.time + spawnRate + Random.Range(0.0f, 0.75f);
            //Instantiate(ship, enemySpawn.position, enemySpawn.rotation);
        }

        Bounce();

        // Make ship bounded by different boundaries
        GetComponent<Rigidbody>().position = new Vector3
        (
            Mathf.Clamp(GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
        );

    }

    // Spawns a wave of ships
    IEnumerator SpawnWave(int ships)
    {
        float wait;
        for (int i = 0; i < ships; ++i)
        {
            wait = Random.Range(1.0f,1.5f);
            Instantiate(ship, enemySpawn.position, enemySpawn.rotation);
            yield return new WaitForSeconds(wait);
        }
    }

    // Spawns a wave of bolts
    IEnumerator FireWave(int shots)
    {
        float wait;
        for (int i = 0; i < shots; ++i)
        {
            //wait = Random.Range(0.1f,0.2f);
            wait = 0.2f;
            //nextFire = Time.time + fireRate + Random.Range(0.0f, 0.75f);
            Instantiate(shot, shotSpawnLeft.position, shotSpawnLeft.rotation);
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(wait);

            Instantiate(shot, shotSpawnRight.position, shotSpawnRight.rotation);
            GetComponent<AudioSource>().Play();
            //wait = Random.Range(0.1f, 0.2f);
            yield return new WaitForSeconds(wait);
        }
    }

    // Make ship bounce off corners
    private void Bounce()
    {
        // Make the ship bounce off the left and right
        if (GetComponent<Rigidbody>().position.x >= boundary.xMax)
        {
            newMovement.x = -1 * movement.x * Random.Range(0.8f, 1.3f);
            GetComponent<Rigidbody>().velocity = newMovement;
        }

        if (GetComponent<Rigidbody>().position.x <= boundary.xMin)
        {
            newMovement.x = 1 * movement.x * Random.Range(0.8f, 1.3f);
            GetComponent<Rigidbody>().velocity = newMovement;
        }
    }
}
