using UnityEngine;
using System.Collections;


/* PROBLEMS:
 * DONE
 * Add current wave - done!
 * Add a life system - done!
 * Add more asteroids - done! 
 * add a pause button - done! 
 * make asteroids harder, faster - make ships move slower - done!
 * diagonal asteroids - done!
 * points add lives!
 * 
 * TO DO
 * add shield
 * Enemy ship explosion sound effect fix
 * Consider mesh renderer for asteroids
 * make powerup a temporary bonus - nerf it
 * weapon ideas: full laser beam
 * asteroids from the sides/bottom 
 * Fix wave issue - new wave should only spawn when old one is dead
 * Consider mixed waves for later levels (asteroids and ships)
 * Evasive maneuver for ships
 * Different asteroids
 * every 10 waves, tell the player theyre doing great (acknowledge/reward somehow)
 * button to disable music/sound effects
 * scrolling background
 * back to main menu button
 * consider screen.showcursor lockcursor
 * enemy ships should fly up and down and also attack players
 * every x waves combine asteroids and ships
 * Make ships randomly "stop" in different imaginary rows (1-3), and then randomly attack
 * is powered down sound effect working correctly?    
 * make mothership fire different things at different speeds
 * can use mothership "game logic" to controll waves? 
 * 10 waves - mother ship, shots missiles, spawns tons of small ships
 */

// Used to spawn hazards
public class GameController : MonoBehaviour 
{
	public GameObject hazard;
	public GameObject enemyShip;
    public GameObject motherShip;
	public Vector3 spawnValues;
    public Vector3 spawnValuesEnemy;

    public MoverEnemyShip moverEnemyShip;
    public PlayerController playerController;
    public DestroyByContactMothership destroyByContactMothership;
    public Mover mover;

    public float instructionTime;
    private bool newGame;

    // Game data
    public bool mothershipAlive; // Test to see if mothership is alive or not
    private bool paused;
    private float shipSpeed; // Original peed of enemy ships
    private float shipFireRate; // Original ship fire rate
    private float hazardSpeed; // Original asteroid speed
    public GameObject[] asteroidsObjects;

    public int totalNumberOfEnemies; // keeps track of number of enemies

    float timeScale; // Game timescale - used for (un)pausing
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
    private int highestWave;
	private int waveCount; // Keeps track of number of waves
    
    // Display
    public GUIText scoreText;
    public GUIText fullScreenText;
	public GUIText gameOverText;
	public GUIText restartText;
	public GUIText waveText;
    public GUIText highScoreText;
    public GUIText highestWaveText;
    public GUIText resetHighScoresText;
    public GUIText newHighScoreText;
    public GUIText instructionsText;
    public GUIText pausedText;
    public GUIText currentWaveText;
    public GUIText poweredUpText;
    public GUIText poweredDownText;
    public GUIText newHighestWaveText;
    public GameObject pausedObject;
    public GameObject livesObject; // Lives icon in the corner

    // Change resolution
    private float originalWidth = 600.0f;
    private float originalHeight = 900.0f;
    private Vector3 scale;

    // Powerup
    public GameObject powerupMusic;
    public GameObject powerupSoundEffect;
    public GameObject backgroundMusic;

    // test
    public GUIText test;


    void OnApplicationQuit()
    {
        ResetSpeeds();
    }

    void OnGUI()
    {
        if (paused)
        {
            Screen.showCursor = true;
            if (GUI.Button(new Rect((Screen.width / 2) - 75, 250, 150, 80), "Back to main menu"))
            {
                Application.LoadLevel("menu");
            }

            else if (GUI.Button(new Rect((Screen.width / 2) - 75, 350, 150, 80), "Resume game"))
            {
                PauseGame();
            }
        }
        /*
        scale.x = Screen.width / originalWidth; // calculate hor scale
        scale.y = Screen.height / originalHeight; // calculate vert scale
        scale.z = 1;
        var svMat = GUI.matrix; // save current matrix
        // substitute matrix - only scale is altered from standard
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
        // draw your GUI controls here:
        //GUI.Box(Rect(10, 10, 200, 50), "Box");
        //GUI.Button(Rect(400, 180, 230, 50), "Button");
        //...
        // restore matrix before returning
        GUI.matrix = svMat; // restore matrix*/
    }

	void Start()
	{       
        // Basic initialisations
        Initialise();

        // Set up lives
        SetUpLives();

        // Store initial values
        StoreSpeeds();

        // Import high score and wave
        LoadHighScore();
        LoadHighestWave();
        highScoreText.text = "High Score: " + highScore;		
        UpdateHighestWave();
		UpdateScore();

        // Lets user know how to reset high scores
        DisplayResetHighScores();

        // Start game
		StartCoroutine (SpawnWaves ());;

        // Play background music
        backgroundMusic.audio.Play();
    }

    // Initialise number oflives
    public void SetUpLives()
    {
        playerController.SetUpLives();
        /*
        for (int i = 0; i < playerController.lives; i++)
        {
            Vector3 lifePosition = new Vector3(12.2f - i*1.3f, 0.0f, -4.0f);
            Instantiate(livesObject, lifePosition, Quaternion.identity);
        }*/
    }

    // Basic initialisations, to keep Start() tidy
    private void Initialise()
    {
        // Disable mouse cursor
        Screen.showCursor = false; 

        // Basic initialisations
        score = 0;
        highScore = 0;
        highestWave = 0;
        waveCount = 1;
        numberOfEnemyShips = 3;
        gameOver = false;
        restart = false;
        newGame = true;
        mover.diagonal = false;  // For diagonal asteroids, set to true

        // Text intialisations
        restartText.text = "";
        gameOverText.text = "";
        waveText.text = "";
        newHighScoreText.text = "";
        test.text = "";
        pausedText.text = "";
        newHighestWaveText.text = "";
        poweredUpText.enabled = false;
        poweredDownText.enabled = false;
        
        //FullScreenText();
        Screen.fullScreen = true;
        UpdateCurrentWaveText();

        // Tests
        test.enabled = false;

        // Pausing
        paused = false;
        timeScale = Time.timeScale; // Store current time scale
    }

    // Enter fullscreen text
    private void FullScreenText()
    {
        if (!Screen.fullScreen)
            fullScreenText.text = "Press to Enter to enter fullscreen";
        else
            fullScreenText.text = "";
    }

    // Save highest score
    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("High Score", highScore);
    }

    private void SaveHighestWave()
    {
        PlayerPrefs.SetInt("Highest Wave", highestWave);
    }

    private void LoadHighestWave()
    {
        highestWave = PlayerPrefs.GetInt("Highest Wave");
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("High Score");
    }

    // Store initial speeds
    // These functions are needed as the speed before games is remembered
    private void StoreSpeeds()
    {
        shipSpeed = moverEnemyShip.speed; 
        shipFireRate = moverEnemyShip.fireRate;
        hazardSpeed = mover.speed;
    }

    // Reinitialises speeds and fire rates
    private void ResetSpeeds()
    {
        moverEnemyShip.speed = shipSpeed;
        moverEnemyShip.fireRate = shipFireRate;
        mover.speed = hazardSpeed;
    }

	public void GameOver()
	{
        waveText.text = "";
   
        // Restart
        restartText.text = "Press 'R' to Restart";
        restart = true;            
        
        ResetSpeeds(); // Reset speeds of ships and asteroids
        // Maybe destroy all objects?
        // ****

        if (score > highScore)
        { 
            highScore = score;            
            UpdateHighScore();           
            SaveHighScore();// Save high score
           
            newHighScoreText.color = new Color(209, 0, 255, 255);
            newHighScoreText.text = "New High Score!";
            WaitSecs(5.0f);
        }

        if(waveCount > highestWave)
        {
            highestWave = waveCount;
            UpdateHighestWave();
            SaveHighestWave();

            newHighestWaveText.color = new Color(209, 0, 255, 255);
            newHighestWaveText.text = "New Highest Wave!";
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

	private void UpdateScore()
	{
		scoreText.text = "Score: " + score;
	}

    private void UpdateHighScore()
    {
        highScoreText.text = "High Score: " + highScore.ToString();
    }

    private void UpdateHighestWave()
    {
        highestWaveText.text = "Highest Wave: " + highestWave.ToString();
    }

    // Resets high score and saves it
    private void ResetHighScore()
    {
        highScore = 0;
        UpdateHighScore();
        SaveHighScore();
    }

    // Block of code that uses the restartText to display resetHighScores instructions
    private void DisplayResetHighScores()
    {
        resetHighScoresText.color = new Color(0, 234, 255, 255);
        resetHighScoresText.text = "Press 'L' to reset high score";
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

        // Add life every 10 000 points
        if (score % 10000 == 0)
            playerController.AddLife();
     
	}

    // For updating the game
	void Update()
	{      
        // Reset high score
        if(Input.GetKeyDown(KeyCode.L))
        {
            ResetHighScore();
        }

        // Fullscreeen text
       FullScreenText();        

        // Go full screen or out of it
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (Screen.fullScreen)
                Screen.fullScreen = false;

            if (!Screen.fullScreen)
                Screen.fullScreen = true;
        }

        // Pause game
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PauseGame();
        }

        // Restart game 
		if (restart) 
		{
			if(Input.GetKeyDown(KeyCode.R))
			{
				Application.LoadLevel(Application.loadedLevel); // currently loaded level
			}
		}
        
        if (playerController.powerupWaveCount == 3)
            playerController.powerUpOn = false;

        
        // test case
        /*
        if (score % 100 == 0 && score != 0)
        {
            playerController.powerUpOn = true;
        }*/
        
        // Turn on powerup mode every 5000 points
        if (score % 3000 == 0 && score != 0)
        {
            playerController.powerUpOn = true;
        }     
 
        if(waveCount % 10 == 0)
        {
            playerController.powerUpOn = true;
        }
	}

    private void PauseGame()
    {      
        // If not paused, pause
        if (!paused)
        {
            pausedObject.audio.Play();
            //audio.Stop(); // Pause background music
            backgroundMusic.audio.volume = 0.1f;
            pausedText.text = "Paused";
            paused = true;
            Time.timeScale = 0;
        }

        // Otherwise unpause
        else if (paused)
        {
            pausedObject.audio.Play();
            //audio.Play(); // Resume background music
            backgroundMusic.audio.volume = 0.5f;
            pausedText.text = "";
            paused = false;
            Time.timeScale = timeScale;
        }
    }

    // Stuff to do once at the start, like display instructions and so on
    private void StartingText()
    {

    }

    // Update Current Wave text()
    private void UpdateCurrentWaveText()
    {
        currentWaveText.text = "Current Wave: " + waveCount;
    }

	public IEnumerator SpawnWaves()
	{
        totalNumberOfEnemies = 0;
        newGame = false;
        // If the user just started a game, display instructions
        if (newGame)
        {
            instructionsText.text = "left ctrl (left mouse button) to fire \narrow keys (mouse) to move\n'space' to pause";
            yield return new WaitForSeconds(instructionTime);
            instructionsText.text = "";
            waveText.text = "Survive as long \nas you can!";
            yield return new WaitForSeconds(3);

            int waitTime = (int)(startWait - (instructionTime + 3));

            for (int i = waitTime; i != 0; --i)
            {
                waveText.text = i.ToString();
                yield return new WaitForSeconds(1);
            }
        }

        newGame = false;
        
        // Spawn waves while gameOver is false
		while(!gameOver)
		{          
            if (playerController.powerUpOn)
                playerController.powerupWaveCount++;  // keeps track of how long powerupmode is active

			waveText.text = "Wave " + waveCount.ToString();
            UpdateCurrentWaveText();
			yield return new WaitForSeconds (4);
			waveText.text = "";
            resetHighScoresText.text = ""; // Hide so "restarttext" can be displayed when needed

            // Mother ship every 10 waves
            if (waveCount % 1 == 0)
            {
                Vector3 spawnPositionMothership = new Vector3(0.0f, 0.0f, 12.0f);               
                Quaternion spawnRotationMothership = Quaternion.identity;
                Instantiate(motherShip, spawnPositionMothership, spawnRotationMothership);
                mothershipAlive = true;

                // While fighting boss, wait
                while (true)
                {
                    yield return new WaitForSeconds(1.0f);

                    if (!mothershipAlive)
                    {
                        //yield return new WaitForSeconds(3.0f);
                        break;
                    }
                }
            }

            else
            {
                for (int i = 0; i < hazardCount; i++)
                {
                    // Spawn enemy ship every second wave
                    // NOTE: Make a "Mother Ship" that's big and evades attacks?
                    if (waveCount % 2 == 0)
                    {
                        SpawnEnemyShips(i);

                        test.text = totalNumberOfEnemies.ToString();
                        break;
                    }

                    else if (waveCount % 5 == 0)
                    {
                        SpawnAsteroids(i);
                        yield return new WaitForSeconds(spawnWait / 2.0f);
                        SpawnEnemyShips(i);
                    }


                    // Otherwise spawn asteroids
                    else
                    {
                        SpawnAsteroids(i);

                        test.text = totalNumberOfEnemies.ToString();

                        yield return new WaitForSeconds(spawnWait);
                    }
                }
            }
            
			waveCount++;
            hazardCount = hazardCount + 4; // add more hazards each wave            

            /*
            bool enemiesDeadTest = true;
            while (test)
            {
                test.text = totalNumberOfEnemies.ToString();
                yield return new WaitForSeconds(0.01f);
                if (totalNumberOfEnemies == 0)
                {
                    enemiesDeadTest = false;
                    break;
                }
            }
            */
            waveWait += 0.1f; // Increase time between waves each wave            
			yield return new WaitForSeconds (waveWait);
            

		}
	}

    private void SpawnEnemyShips(int i)
    {        
        if (i == 0)
        {
            // Aside from the first wave, increase difficult (ship speed & fire rate) each wave
            // Note: we're in a loop so we only want the speed to increase once per wave
            if (waveCount != 1)
            {
                numberOfEnemyShips++;

                if (moverEnemyShip.speed <= 15)
                    moverEnemyShip.speed += 0.2f;

                if (moverEnemyShip.fireRate >= 0.4)
                    moverEnemyShip.fireRate -= 0.02f;
            }


            for (int j = 0; j < numberOfEnemyShips; j++)
            {
                // Spawn the ship in one of three invisible rows along the top
                // and a random horizontal position  along the x axis
                //int row = Random.Range(0,4); // The vertical row (z axis) they lie on
                float rangeZ = Random.Range(0.0f, 5.2f);
                float range = Random.Range(-spawnValuesEnemy.x, spawnValuesEnemy.x); // random position along x axis

                Vector3 spawnPositionEnemy = new Vector3(range, spawnValuesEnemy.y, (16.6f));                        
                //Vector3 spawnPositionEnemy = new Vector3(range, spawnValuesEnemy.y, (spawnValuesEnemy.z + rangeZ));
                Quaternion spawnRotationEnemy = Quaternion.identity;
                Instantiate(enemyShip, spawnPositionEnemy, spawnRotationEnemy);


                totalNumberOfEnemies++;
            }
        }
    }

    private void SpawnAsteroids(int i)
    {
        // Aside from first wave, increase asteroid difficulty
        // note the minus sign: speed is negative to incorporate direction opposite to bolts
        // Note: we're in a loop so we only want the speed to increase once per wave
        if (waveCount != 1 && i == 0)
        {
            if (mover.speed <= -30)
                mover.speed -= 0.5f;

            if (spawnWait > 0.3f)
                spawnWait -= 0.02f;
        }

        // For waves larger than 10 make some asteroids diagonally
        if (waveCount >= 10)
        {
            if (Random.Range(0, 2) == 0)
                mover.diagonal = true;

            else
                mover.diagonal = false;
        }

        float range = Random.Range(-spawnValuesEnemy.x, spawnValuesEnemy.x);
        GameObject asteroid = asteroidsObjects[Random.Range(0, asteroidsObjects.Length)];
        Vector3 spawnPosition = new Vector3(range, spawnValues.y, spawnValues.z);
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(asteroid, spawnPosition, spawnRotation);
        //Instantiate(hazard, spawnPosition, spawnRotation);

        totalNumberOfEnemies++;
    }
}

