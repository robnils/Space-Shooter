using UnityEngine;
using System.Collections;

// Used to spawn hazards
public class GameController : MonoBehaviour 
{
    /* Problems
     * Ships destroy asteroids and each other and interact with the asteroids
     * 
     * */
	public GameObject hazard;
	public GameObject enemyShip;
	public Vector3 spawnValues;
    public Vector3 spawnValuesEnemy;

    public MoverEnemyShip moverEnemyShip;

    public float instructionTime;

    // Game data
    private float shipSpeed; // Original peed of enemy ships
    private float shipFireRate; // Original ship fire rate

    public float hazardRate;
    private bool restart;
    private bool gameOver;
    private int numberOfEnemyShips; 
	public int hazardCount; // Keeps track of number of asteroids
	public float spawnWait; // Time between spawns
	public float startWait; // Initial time
	public float waveWait;	// Time between waves
	private int score;
    private int highScore;
	private int waveCount; // Keeps track of number of waves
    
    // Display
    public GUIText scoreText;
	public GUIText gameOverText;
	public GUIText restartText;
	public GUIText waveText;
    public GUIText highScoreText;
    public GUIText resetHighScoresText;
    public GUIText newHighScoreText;
    public GUIText InstructionsText;

    // test
    public GUIText test;

	void Start()
	{
        Screen.showCursor = false; // Disable mouse cursor

        // Basic initialisations
		score = 0;
        highScore = 0;
		waveCount = 1;
        numberOfEnemyShips = 2;
		gameOver = false;
		restart = false;
		restartText.text = "";
		gameOverText.text = "";
        newHighScoreText.text = "";
        shipSpeed = moverEnemyShip.speed; // To stop the speed increasing between sessions
        shipFireRate = moverEnemyShip.fireRate;
        //InstructionsText.color = waveText.color;

        // Import high score        
        LoadHighScore();
        highScoreText.text = "High Score: " + highScore;
		waveText.text = "";
		UpdateScore();

        // Lets user know how to reset high scores
        DisplayResetHighScores();

        // Start game
		StartCoroutine (SpawnWaves ());;

        test.text = "";
	}

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("High Score", highScore);
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("High Score");
    }

	public void GameOver()
	{
        waveText.text = "";

        //Reset enemy ships speed and fire rate
        moverEnemyShip.speed = shipSpeed;
        moverEnemyShip.fireRate = shipFireRate;

        // Maybe destroy all objects?
        // ****

        if (score > highScore)
        { 
            // Print NEW HIGH SCORE!
            highScore = score;
            UpdateHighScore();
            SaveHighScore();// Save high score

            newHighScoreText.color = new Color(209, 0, 255, 255);
            newHighScoreText.text = "New High Score!";
            WaitSecs(5.0f);
        }
        gameOverText.color = new Color(238, 0, 0, 255);
		gameOverText.text = "Game Over";        
		gameOver = true;
	}

    IEnumerator WaitSecs(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

	void UpdateScore()
	{
		scoreText.text = "Score: " + score;
	}

    void UpdateHighScore()
    {
        highScoreText.text = "High Score: " + highScore.ToString();
    }

    // Resets high score and saves it
    void ResetHighScore()
    {
        highScore = 0;
        UpdateHighScore();
        SaveHighScore();
    }

    // Block of code that uses the restartText to display resetHighScores instructions
    void DisplayResetHighScores()
    {
        resetHighScoresText.color = new Color(0, 234, 255, 255);
        resetHighScoresText.text = "Press 'P' to reset high scores";
    }

    // Wait for time seconds
    void Wait(int time)
    {
        float elapsedTime = 0;
        int counter = 0;
        if (counter < time)
        {
            if (elapsedTime >= 1)
            {
                elapsedTime = 0; //reset it zero again
                counter++;
            }

            else
            {
                elapsedTime += Time.deltaTime;
            }
        }
    }

	// For use when a hazard is destroyed
	public void AddScore(int newScoreValue)
	{
		score += newScoreValue;
		UpdateScore ();
	}

    // For restarting the game
	void Update()
	{      
        if(Input.GetKeyDown(KeyCode.P))
        {
            ResetHighScore();
        }

		if (restart) 
		{
			if(Input.GetKeyDown(KeyCode.R))
			{
				Application.LoadLevel(Application.loadedLevel); // currently loaded level
			}
		}
	}

	IEnumerator SpawnWaves()
	{          
        InstructionsText.text = "left ctrl to fire \narrow keys to move";
        yield return new WaitForSeconds(instructionTime);
        InstructionsText.text = "";
        waveText.text = "Get ready!";
        yield return new WaitForSeconds(3);

        int waitTime = (int)(startWait - (instructionTime + 3));

        for (int i = waitTime; i != 0; --i)
        {
            waveText.text = i.ToString();
            yield return new WaitForSeconds(1);
        }

		while(true)
		{
			waveText.text = "Wave " + waveCount.ToString();
			yield return new WaitForSeconds (4);
			waveText.text = "";
            resetHighScoresText.text = ""; // Hide so "restarttext" can be displayed when needed

			for (int i = 0; i < hazardCount; i++) 
			{
				// Spawn enemy ship every second wave
                // NOTE: Make a "Mother Ship" that's big and evades attacks?
				if(waveCount % 1 == 0)
				{
                    for (int j = 0; j < numberOfEnemyShips; j++)
                    {
                        // Aside from the first wave, increase difficult (ship speed & fire rate) each wave
                        if (waveCount != 1)
                        {
                            moverEnemyShip.speed += 0.1f;
                            moverEnemyShip.fireRate -= 0.01f;
                        }

                        // Spawn the ship in one of three invisible rows along the top
                        // and a random horizontal position  along the x axis
                        int row = Random.Range(0,2)*2; // The vertical row (z axis) they lie on
                        float range = Random.Range(-spawnValuesEnemy.x, spawnValuesEnemy.x); // random position along x axis
                                                
                        Vector3 spawnPositionEnemy = new Vector3(range, spawnValuesEnemy.y, (spawnValuesEnemy.z+row));
                        Quaternion spawnRotationEnemy = Quaternion.identity;
                        Instantiate(enemyShip, spawnPositionEnemy, spawnRotationEnemy);
                        
                        /* if (spawn on location as other enemy)
                         * delete, spawn new location 
                         * maybe while loop?
                         * */
                    }
                    numberOfEnemyShips++;
                    yield return new WaitForSeconds(spawnWait);
                    break;					
				}

                // Otherwise spawn asteroids
				else
				{
					Vector3 spawnPosition = new Vector3 (Random.Range (-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
					Quaternion spawnRotation = Quaternion.identity;
					Instantiate (hazard, spawnPosition, spawnRotation);
					yield return new WaitForSeconds (spawnWait);
				}
			}

			waveCount++;
            hazardCount = hazardCount + 3; // add more hazards each wave
			yield return new WaitForSeconds (waveWait);

			if(gameOver)
			{
				restartText.text = "Press 'R' to Restart";
				restart = true;
				break; 
			}
		}
	}
}
