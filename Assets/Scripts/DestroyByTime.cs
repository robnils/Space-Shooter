using UnityEngine;
using System.Collections;

// Destroys the explosion object after lifetime seconds
public class DestroyByTime : MonoBehaviour 
{
	public float lifetime;
    public GameController gameController;
	// Use this for initialization
	void Start () 
	{
        // Game controller object set up
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");

        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }

        if (gameControllerObject == null)
            Debug.Log("Cannot find 'GameController' script");

        // Destroy object 
		Destroy (gameObject, lifetime);        

        // Subtract from total list of enemies
        --gameController.totalNumberOfEnemies;
        gameController.test.text = gameController.totalNumberOfEnemies.ToString();
	}
	

}
