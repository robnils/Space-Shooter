using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour 
{
	public float speed; // Speed of bolt
    public bool diagonal; // asteroids fall diagonally if true
    public GameController gameController;

	void Start()
	{
        // Move diagonally
        if (diagonal)
        {
            // If asteroid spawns on the left half, move it right
            if(rigidbody.transform.position.x < 0)
                rigidbody.velocity = new Vector3(-speed * (Random.Range(0.7f, 1.0f)), 0, speed * (Random.Range(0.7f, 1.0f)));

            // If asteroid spawns on the right half, move it left
            if (rigidbody.transform.position.x > 0)
                rigidbody.velocity = new Vector3(speed * (Random.Range(0.7f, 1.0f)), 0, speed * (Random.Range(0.7f, 1.0f)));
        }

        // Fall straight down
        else
        {
            //rigidbody.velocity = transform.forward * speed * (Random.Range(0.5f, 1.0f));
            rigidbody.velocity = new Vector3(0, 0, speed * (Random.Range(0.7f, 1.0f)));
        }

        
	}

}
