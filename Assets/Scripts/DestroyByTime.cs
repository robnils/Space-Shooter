using UnityEngine;
using System.Collections;

// Destroys the explosion object after lifetime seconds
public class DestroyByTime : MonoBehaviour 
{
	public float lifetime;
	// Use this for initialization
	void Start () 
	{
		Destroy (gameObject, lifetime);
	}
	

}
