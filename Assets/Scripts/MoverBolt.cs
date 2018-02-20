using UnityEngine;
using System.Collections;

public class MoverBolt : MonoBehaviour 
{
    public float speed;
	// Use this for initialization
	void Start () 
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed * (Random.Range(0.5f, 1.0f));
        //rigidbody.velocity = new Vector3(0, 0, speed * (Random.Range(0.5f, 1.0f)));
	}
}
