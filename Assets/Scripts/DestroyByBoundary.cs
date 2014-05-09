using UnityEngine;
using System.Collections;

public class DestroyByBoundary : MonoBehaviour 
{
	// Destroy everything that leaves the trigger
	void OnTriggerExit (Collider other) 
	{
		Destroy(other.gameObject);
	}
}
