using UnityEngine;
using System.Collections;

public class DestroyByBoundary : MonoBehaviour 
{
    public GameController gameController;

	// Destroy everything that leaves the trigger
	void OnTriggerExit (Collider other) 
	{
        // Game controller object set up
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");

        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }

        if (gameControllerObject == null)
            Debug.Log("Cannot find 'GameController' script");

        // Destroy leaving object
		Destroy(other.gameObject);

        // Subtract from total list of enemies
        --gameController.totalNumberOfEnemies;
        gameController.test.text = gameController.totalNumberOfEnemies.ToString();
	}
}
