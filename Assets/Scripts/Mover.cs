using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour 
{
	public float speed; // Speed of bolt

	void Start()
	{
		// Transform.forward is the local z-axis        
        // Velocity is pointed downward - asteroid falls with speed "speed"
        // which is then scaled randomly between speed and speed*(0.5)
		rigidbody.velocity = transform.forward * speed * (Random.Range(0.5f,1.0f));

        // Currently fixed speed as the asteroids interact and destroy another
        //rigidbody.velocity = transform.forward * speed;
	}

}
