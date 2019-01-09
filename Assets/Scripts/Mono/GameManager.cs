/*
Name: Angela Robinson
Class: GPE205/GAM205 Gameplay Programming Concepts
Date: 3/18/2018
Assignment: UATanks
Milestone 1:
    1. player tank able to move, shoot and destroy (stationary) enemy tanks
        -shells use rigidbody physics and are destroyed when it hits something (floor, tank, or time)
        -There should be a certain amount of seconds between shots (fireRate)
        -AddForce needed for shells when shot
        -Damage variable to set the amount of damage per shell
    2. Floor and obstacles are in the gamefield
    3. player uses WASD for movement and space for shooting
    5. Tanks have a health status that when depleted, the tank is destroyed
    6. Each tank keeps track of their own score
    7. Variables viewable in inspector: frontSpeed, backSpeed, turnSpeed, shellForce, damageDone, fireRate, maxHealth, score
        -TankData Script Component to hold all Tank info for designers (player and enemy alike)
        -AIController Script Component used for enemy tanks to send messages to other components
    8. GameManager singleton should be used to keep track of important game info and manages flow of game

Milestone 2: (along with the above requirements)
    1. Game includes at least 4 AI
    2. AI personalities detailed in text file
        a. at least one exhibits chase behavior
        b. at least one exhibits flee behavior
        c. at least one exhibits patrol behavior
        d. at least one exhibits hearing and vision (FOV lecture/assignments from Game104)
        e. all behaviors utilizes object avoidance
    3. AI and Player tanks utilize the same motor, shooter & data components
    4. AI correctly utilized state machine logic
    5. Game Manager Singleton allows easy access to player object and enemy tank objects.
    6. bug free, comments in code, project organization, code logic and structure, exceeds minimum requirements
    
Milestone 3: (along with the above requirements)
    1. Map generated through instantiating tiles
        a. the tile array must be exposed to designers
        b. at least three tiles to choose from
        c. generation of map is random if "Random" boolean is selected
        d. generation of map is seeded to the current date if "Map of the Day" is selected
        e. generation of map can be seeded to a specific number set by designers
        f. Map size adjustable by designers
    2. AI and players spawn in random tiles and are not active objects when level is loaded
    3. Powerups:
        a. can be picked up by both AI and players
        b. expire after a set time after being picked up OR can be permanent set by a bool
    4. Game Manager singleton allows easy access to level object, powerups, player object and an array of enemy tank objects
    5. bug free, comments in code, project organization, code logic and structure, exceeds minimum requirements
    
Milestone 4: (along with the above requirements) 
    1. Start screen
        a. start game, Options, & Quict Game buttons work
    2.Options screen exists and has settings for:
        a. SFX Volume 
        b. Music Volume
        c. One/Two Player mode
        d. Map of the DAy/Random Map modes
    3.Game music plays and is controlled by Music Volum Option
    4.Game Sounds play (Tank Fire, Tank death, bullet hit, powerup sounds, buttons on menus) and ar controlled by SFX volumne setting
    5.player lives, scores, and high score show in game UI, and UI functions correctly in both single player and 2 player modes
    6. 2 Player mode utilizes separae controls and split screen cameras
    7. Options and high scores persist between game sessions
    8. Game ends when both players lives are <0. 
        a.Game over condition works correctly in 2-player mode. (when one player dies, the ther continues until their lives are used
    9. Game Tracks score for each player
    10. Game manager Singleton exists, allows easy access to key object, and controls main game logic, including game state (menu, options, play, etc) and game-wide variables (high score, game mode, etc.)
            



*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    //create variable of instance
    public static GameManager instance;
    

    [Header("Map Generation")]
    public GameObject[] mapRooms; //array to hold prefabs
    public int rows, collumns, seed; //map size & seed number
    private float width, height; //room size
    private Room[,] mapGrid; // two dimensional array to hold the finished map
    public bool isDayMap;
    public bool isRandom = true;


    [Header("Game Initialization")]
    public GameObject prefabCannon; //hold prefab for cannonballs to be shot
    public GameObject[] playerSpawns; //hold coords of player spawns on the map
    public GameObject[] enemySpawns; //hold coords of enemy spawns on the map
    public GameObject[] powerUpSpawns; //hold coords of powerup Spawns on the map
    public int numOfPlayers;
    public int numOfEnemies;
    public bool multiplayer;
    public int highScore;
    public string highScoreName;
    //public GameSettings optionSettings;
    public GameObject[] players;
    private TankData[] playerData;
    public GameObject[] enemies;
    public GameObject[] patrols;
    public int[] scores;
    public AudioSource asource;
    public AudioMixer msource;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup fxGroup;
    public AudioClip menuClip;
    public AudioClip backgroundCLip;
    public AudioClip shootClip;
    public AudioClip menuClick;

    [Header("UI Information")]
    private GameObject startPanel;
    private GameObject optionsPanel;
    private GameObject gameOverPanel;
    private GameObject playPanel;
    private GameObject twoPlayer;
    public Toggle tPlayer;
    private GameObject mapOfDay;
    public Toggle dayMap;
    private GameObject mapRandom;
    public Toggle randomMap;
    public Slider music;
    public Slider sfx;
    public Text p1Lives;
    public Text p1Score;
    public Text p2Lives;
    public Text p2Score;
    public Text p2LivesText;
    public Text p2ScoreText;
    public Text player1Score;
    public Text player2Score;
    public Text highestScore;


    //runs before start function
    private void Awake()
    {
        //make sure there is only one copy of instance
        //if a second copy of instance tries to open, destroy it
        if (instance == null)
        {
            instance = this;
            
        }
        else
        {
            Debug.LogError("DANGER ANGIE ROBINSON:  There can only be one Game Manager");
            Destroy(gameObject);
        }

        //gameInit();
    }

    

    // Use this for initialization
    void Start ()
    {
        
        GUIInit();
        

    }

    // Update is called once per frame
    void Update ()
    {

        

        if (numOfPlayers == 2)
        {
            p2Lives.text = playerData[1].lives.ToString();
            p2Score.text = playerData[1].score.ToString();
        }

        //if the startPanel is not showing
        //this stops null reference spam at beginning of the game.
        if (!startPanel.activeSelf)
        {
            p1Lives.text = playerData[0].lives.ToString();
            p1Score.text = playerData[0].score.ToString();

            for (int i = 0; i < playerData.Length; i++)
            {
                //check health and update lives
                if (playerData[i].health <= 0)
                {
                    playerData[i].lives -= 1;
                }

                //check lives: if out of lives save score to scores array to compare
                //if not out of lives reset health
                if (playerData[i].lives <= 0)
                {
                    scores[i + 1] = playerData[i].score;

                }
                else
                {
                    playerData[i].health = playerData[i].maxHealth;
                }
            }

            
            
            if (scores.Length == 3)
            {
                if (playerData[0].lives == 0 && playerData[1].lives == 0)
                {
                    gameOverPanel.SetActive(true);
                }
                GameOver();
            }
            else if (playerData[0].lives == 0)
            {
                gameOverPanel.SetActive(true);
                GameOver();
            }

           
        }

        //if escape key is pressed open the main menu to allow for options adjustment or quit game
        //if the panel is alreay up, close it and options panel if needed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            if (startPanel.activeSelf)
            {
                if (optionsPanel.activeSelf)
                {
                    optionsPanel.SetActive(false);
                }
                startPanel.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                startPanel.SetActive(true);
                Time.timeScale = 0;
                
                
            }
        }
        
	}

    public void menuClickSound()
    {
        AudioSource.PlayClipAtPoint(menuClick, Vector3.zero);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public void GameOver()
    {
        //check if high score has been broken
        for (int i = 0; i < scores.Length; i++)
        {
            if (i > 0)
            {
                if (scores[i] > scores[0])
                {
                    scores[0] = scores[i];
                }
            }
        }
        //gameOverPanel.SetActive(true);
        highestScore.text = "High Score: " + scores[0].ToString();
        PlayerPrefs.SetInt("HighestScore", scores[0]);

        if (scores.Length == 3)
        {
            player1Score.text = "Player 1 score: " + scores[1].ToString();
            player2Score.text = "Player 2 score: " + scores[2].ToString();            
        }
        else
        {
            player1Score.text = scores[1].ToString();
            player2Score.text = "";
        }
    }

    public void SaveOptions()
    {
        PlayerPrefs.SetFloat("MusicVol", music.value);
        PlayerPrefs.SetFloat("SFXVol", sfx.value);
        PlayerPrefs.SetString("MultiPlayer", multiplayer.ToString());
        PlayerPrefs.SetString("DayMap", isDayMap.ToString());
        PlayerPrefs.SetString("RandomMap", isRandom.ToString());
        //PlayerPrefs.SetInt("HighestScore", highScore);
        //PlayerPrefs.SetString("High Score Score Name", highScoreName);
        PlayerPrefs.Save();

       
    }

    public void LoadOptions()
    {
        music.value = PlayerPrefs.GetFloat("Music Vol");
        sfx.value = PlayerPrefs.GetFloat("SFXVol");
        string mplayer = PlayerPrefs.GetString("MultiPlayer");
        string dMap = PlayerPrefs.GetString("DayMap");
        string rMap = PlayerPrefs.GetString("RandomMap");
        highScore = PlayerPrefs.GetInt("HighestScore");

        if (mplayer == "True")
        {
            tPlayer.isOn = true;
            multiplayer = true;
        }
        else
        {
            tPlayer.isOn = false;
            multiplayer = false;
        }

        if (dMap == "True")
        {
            isDayMap = true;
            dayMap.isOn = true;
        }
        else
        {
            isDayMap = false;
            dayMap.isOn = false;
        }

        if (rMap == "True")
        {
            isRandom = true;
            randomMap.isOn = true;
        }
        else
        {
            isRandom = false;
            randomMap.isOn = false;
        }
    }
    public void isMultiPlayer()
    {

        //check to see if two player is checked
        if (tPlayer.isOn)
        {
            multiplayer = true;
        }
        else
        {
            multiplayer = false;
        }
    }

    public void onMusicVolumeChange()
    {
        asource.volume = music.value;
        //msource.SetFloat("MusicVolume", music.value);
        
        
    }

    public void onSFXChange()
    {
       // asource.volume = sfx.value;
       
    }

    public void StartGame()
    {
        //when start game button is clicked disable the main menu panels and change music clip
        startPanel.SetActive(false);
        optionsPanel.SetActive(false);
        asource.clip = backgroundCLip;
        
        GenerateMap();
        GameInit();
    }

    public void ranDayMap()
    {
        if (dayMap.isOn)
        {
            isDayMap = true;
        }
        else
        {
            isDayMap = false;
        }

        if (randomMap.isOn)
        {
            isRandom = true;
        }
        else
        {
            isRandom = false;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleOptions()
    {
        if (optionsPanel.activeSelf == true)
        {
            optionsPanel.SetActive(false);
        }
        else
        {
            optionsPanel.SetActive(true);
        }
    }
    public int makeSeedFromDate(DateTime date)
    {
        return date.Year + date.Month + date.Day + date.Hour + date.Minute + date.Second + date.Millisecond;
    }

    public void GenerateMap()
    {

        //set the width and height of the rooms based on the prefab we made
        width = height = 50.0f;

        //if designer hasn't inserted the prefabs into the array in inspector, get them from the resources folder
        if (mapRooms.Length == 0)
        {
            mapRooms = Resources.LoadAll<GameObject>("Rooms");
        }

        if (seed == 0)
        {
            //if seed is zero and if we want Random map of the day set the seed to Datetime.Now or if just map of the day set seed to DateTime.Today. otherwise randomly select seed
            if (isRandom && isDayMap)
            {
                seed = makeSeedFromDate(DateTime.Now);
            }
            else if (isDayMap)
            {
                seed = makeSeedFromDate(DateTime.Today);
            }
            else if (isRandom)
            {
                seed = new System.Random().Next();
            }
            else
            {
                seed = 761979;
            }
        }

        //if designer hasn't put in a grid size set default map size to 4 by 4
        if (rows == 0)
        {
            rows = 4;

        }

        if (collumns == 0)
        {
            collumns = 4;
        }

        //create new blank grid
        mapGrid = new Room[rows, collumns];

        //set seed
        if (seed != 0)
        {
            UnityEngine.Random.InitState(seed); //set the seed of our map            
        }

        //the outer loop is used to control which row we are on. Once the inner loop finishes for the collumns, it is then when this loop goes to the next iteration
        for (int row = 0; row < rows; row++)
        {
            //for each collumn in the current row we use an inner loop to determine the position of the rooms for the map
            for (int col = 0; col < collumns; col++)
            {
                //determine position of room
                //to do this we take the size of the room: width and height and multiply them by the number we are on in the loop to determine the position of the next tile.  Since this is a top down view the width is on the X axis and the height is on the Z axis.
                float xPos = width * col;
                float zPos = height * row;

                //once we get the starting width and height, we input those into a vector so we can show where exactly to position the room
                Vector3 newPos = new Vector3(xPos, 0.0f, zPos);

                //place the room at the appropriate location
                GameObject tempRoom = Instantiate(RandomRoom(), newPos, Quaternion.identity) as GameObject;

                //set the parent of this new room (I'm not entirely sure why we need to do this but the lecture is doing it. I'm hoping that as I work through this I will see why)
                tempRoom.transform.parent = this.transform;

                //name the room i.e. GridRoom 1,1;
                tempRoom.name = "GridRoom " + row + "," + col;

                //create a Room variable so that we can access the script and open doors before saving the room into our mapGrid
                Room gridRoom = tempRoom.GetComponent<Room>();

                gridRoom.SendMessage("Start");
                //open the doors

                //check the north and south doors
                if (row == 0 && col == 0)
                {
                    //if we are in bottom row and first collumn we open the north east door only
                    gridRoom.doorNorthEast.SetActive(false);
                }
                else if (row == 0 && col == collumns - 1)
                {
                    //if we are in bottom row and last collumn we open the north west door only
                    gridRoom.doorNorthWest.SetActive(false);
                }
                else if (row == 0)
                {
                    gridRoom.doorNorthEast.SetActive(false);
                    gridRoom.doorNorthWest.SetActive(false);
                }
                else if (row == rows - 1 && col == 0)
                {
                    //if we are on top row and first collumn we open the south east door only
                    gridRoom.doorSouthEast.SetActive(false);

                }
                else if (row == rows - 1 && col == collumns - 1)
                {
                    //if we are on top row and last collumn we open the south west door only
                    gridRoom.doorSouthWest.SetActive(false);
                }
                else if (row == rows - 1)
                {
                    gridRoom.doorSouthWest.SetActive(false);
                    gridRoom.doorSouthEast.SetActive(false);
                }
                else
                {
                    //otherwise we are in the middle and need to open all north and south doors
                    gridRoom.doorNorthEast.SetActive(false);
                    gridRoom.doorNorthWest.SetActive(false);
                    gridRoom.doorSouthWest.SetActive(false);
                    gridRoom.doorSouthEast.SetActive(false);

                }

                //check the east and west doors
                if (col == 0)
                {
                    //if we are on the first collumn we open east door only
                    gridRoom.doorEast.SetActive(false);
                }
                else if (col == collumns - 1)
                {
                    //if we are on the last collumn we open west door only
                    gridRoom.doorWest.SetActive(false);
                }
                else
                {
                    //otherwise we are in the middle and we need to open both doors
                    gridRoom.doorEast.SetActive(false);
                    gridRoom.doorWest.SetActive(false);
                }



                //save the room into the grid for safe keeping
                mapGrid[row, col] = gridRoom;
            }
        }
    }
    public GameObject RandomRoom()
    {
        //pick a random room out of our prefabs array mapRooms
        return mapRooms[UnityEngine.Random.Range(0, mapRooms.Length)];
    }

    void GameInit()
    {
        asource.Play();

        if (highScore == 0)
        {
            highScore = PlayerPrefs.GetInt("HighestScore");
        }
        
        //find the possible spawn points for players, enemies and powerups
        playerSpawns = GameObject.FindGameObjectsWithTag("Respawn");

        //rename the spawns so that they are easy to find in inspector
        for (int i = 0; i < playerSpawns.Length; i++)
        {
            playerSpawns[i].name = "PlayerSpawn" + i;

        }

        enemySpawns = GameObject.FindGameObjectsWithTag("Spawn");

        //rename the spawns so that they are easy to find in inspector
        for (int i = 0; i < enemySpawns.Length; i++)
        {
            enemySpawns[i].name = "EnemySpawn" + i;
        }

        powerUpSpawns = GameObject.FindGameObjectsWithTag("PowerUpSpawn");

        //rename the spawns so that they are easy to find in inspector
        for (int i = 0; i < powerUpSpawns.Length; i++)
        {
            powerUpSpawns[i].name = "PowerUpSpawn" + i;
        }

        patrols = GameObject.FindGameObjectsWithTag("Patrol");
        //rename the spawns so that they are easy to find in inspector
        for (int i = 0; i < patrols.Length; i++)
        {
            patrols[i].name = "Patrol" + i;


        }

        //pull in the prefabCannon from resources folder
        prefabCannon = Resources.Load("CannonBall", typeof(GameObject)) as GameObject;
             
        //if multiplayer is true set number of players to 2 otherwise, there should always be one player
        if (multiplayer)
        {
            numOfPlayers = 2;
        }
        else
        {
            numOfPlayers = 1;
        }

        //set size of scores array
        scores = new int[numOfPlayers+1];
        scores[0] = highScore;

        //set size of playerData array
        playerData = new TankData[numOfPlayers];

        //set size of players array
        players = new GameObject[numOfPlayers];

        for (int i = 0; i < numOfPlayers; i++)
        {
            GameObject tempPlayer = Resources.Load("Tank/Tank") as GameObject;//get a copy of prefab tank
            tempPlayer.GetComponent<InputController>().enabled = true;
            tempPlayer.GetComponent<AIController>().enabled = false; //take off the AI Controller script

            //set split screen
            if (multiplayer)
            {
                if (i == 0)
                {
                    //player one is on left side of screen
                    tempPlayer.GetComponentInChildren<Camera>().rect = new Rect(-0.5f, 0, 1, 1);
                    //set input schemas to Arrows
                    tempPlayer.GetComponent<InputController>().input = InputController.Schemes.ARROWS;
                }
                else
                {
                    //player two is on right side of screen
                    tempPlayer.GetComponentInChildren<Camera>().rect = new Rect(0.5f, 0, 1, 1);
                    //set input schemas to wasd
                    tempPlayer.GetComponent<InputController>().input = InputController.Schemes.WASD;
                }

            }
            else
            {
                //full screen for single player and default movement is Arrows
                tempPlayer.GetComponentInChildren<Camera>().rect = new Rect(0, 0, 1, 1);
                tempPlayer.GetComponent<InputController>().input = InputController.Schemes.ARROWS;
            }
            
            tempPlayer.GetComponentInChildren<Camera>().enabled = true;
            //get random spawn location
            int spawnIndex = UnityEngine.Random.Range(0, playerSpawns.Length);
            Vector3 spawnPos = playerSpawns[spawnIndex].GetComponent<Transform>().position;


            players[i] = Instantiate(tempPlayer, spawnPos, Quaternion.identity);
            players[i].name = "Player " + (1 + i); //name player

            

        }

        //set default info for each of the players
        for (int i = 0; i < players.Length; i++)
        {
            playerData[i] = players[i].GetComponent<TankData>();

            //set data info of enemies rounding and making floats to 2 decimal places
            //since there is not mathf.truncate, I have to multiple the random float by 100 to move the decimal two places to the right
            //once multiplied by 100, then round the number to the neareast int
            //then devide by 100 in order to get the decmial back to the left 2 spaces
            playerData[i].frontSpeed = Mathf.Round(UnityEngine.Random.Range(3f, 6f) * 100.0f) / 100.0f;
            playerData[i].fireRate = Mathf.Clamp01(UnityEngine.Random.Range(0f, 1f) * 100.0f) / 100.0f; //i wanted to clamp this between 0.0 - 1.0 instead of rounding it

            playerData[i].backSpeed = Mathf.Round(UnityEngine.Random.Range(1f, 3f) * 100.0f) / 100.0f;
            playerData[i].turnSpeed = Mathf.Round(UnityEngine.Random.Range(90f, 150f) * 100.0f) / 100.0f;

            //get a random number between 50-150 in steps of 10
            playerData[i].maxHealth = Mathf.Floor((UnityEngine.Random.Range(50, 151) / 10) * 10);

            //get a random number between 100-1000 and step it by 100
            playerData[i].shellForce = Mathf.Floor((UnityEngine.Random.Range(100, 1001) / 10) * 10);
            if (playerData[i].lives == 0)
            {
                playerData[i].lives = 3;
            }


        }



        //there must always be at least one enemy populated for each row within the map.  
        if (numOfEnemies == 0)
        {
            numOfEnemies = rows;
        }

        //set size of enemies array
        enemies = new GameObject[numOfEnemies];

        for (int i = 0; i < numOfEnemies; i++)
        {
            GameObject tempEnemy = Resources.Load("Tank/Tank") as GameObject;//get a copy of prefab tank

            tempEnemy.GetComponent<AIController>().enabled = true;
            tempEnemy.GetComponent<InputController>().enabled = false; //take off the AI Controller script
            tempEnemy.GetComponentInChildren<Camera>().enabled = false;//diable camera for enemy

            //get random spawn location
            //TODO: need to figure out card deck style where if on position already picked,you can't pick it again
            int spawnIndex = UnityEngine.Random.Range(0, enemySpawns.Length);
            Transform spawnPos = enemySpawns[spawnIndex].GetComponent<Transform>();


            enemies[i] = Instantiate(tempEnemy, spawnPos.position, Quaternion.identity);
            enemies[i].name = "Enemy " + (1 + i); //name player




        }

        //set default info for each of the enemies
        for (int i = 0; i < enemies.Length; i++)
        {
            TankData data = enemies[i].GetComponent<TankData>();


            //set data info of enemies rounding and making floats to 2 decimal places
            //since there is not mathf.truncate, I have to multiple the random float by 100 to move the decimal two places to the right
            //once multiplied by 100, then round the number to the neareast int
            //then devide by 100 in order to get the decmial back to the left 2 spaces
            data.frontSpeed = Mathf.Round(UnityEngine.Random.Range(3f, 6f) * 100.0f) / 100.0f;
            data.fireRate = Mathf.Round(UnityEngine.Random.Range(0f, 1f) * 100.0f) / 100.0f;

            data.backSpeed = Mathf.Round(UnityEngine.Random.Range(1f, 3f) * 100.0f) / 100.0f;
            data.turnSpeed = Mathf.Round(UnityEngine.Random.Range(90f, 150f) * 100.0f) / 100.0f;

            //get a random number between 50-150 in steps of 10
            data.maxHealth = Mathf.Floor((UnityEngine.Random.Range(50, 151) / 10) * 10);

            //get a random number between 100-1000 and step it by 10
            data.shellForce = Mathf.Floor((UnityEngine.Random.Range(100, 1001) / 10) * 10);

        }

        playPanel.SetActive(true); //show the playing panel

        if (numOfPlayers < 2)
        {
            p2Lives.text = "";
            p2Score.text = "";
            p2LivesText.text = "";
            p2ScoreText.text = "";
        }
    }

    void GUIInit()
    {
        //assign UI Panels to variables
        startPanel = GameObject.Find("StartPanel");
        optionsPanel = GameObject.Find("OptionsPanel");
        gameOverPanel = GameObject.Find("GameOverPanel");
        playPanel = GameObject.Find("PlayingPanel");

        //multiplayer options references
        twoPlayer = GameObject.Find("TwoPlayer");
        tPlayer = twoPlayer.GetComponent<Toggle>();

        //map options references
        mapOfDay = GameObject.Find("DayMap");
        dayMap = mapOfDay.GetComponent<Toggle>();
        

        mapRandom = GameObject.Find("RandomMap");
        randomMap = mapRandom.GetComponent<Toggle>();

        
        //get audioSource
        asource = gameObject.GetComponent<AudioSource>();
        msource = Resources.Load("Audio/GameAudioMixer") as AudioMixer;

        menuClip = Resources.Load("Audio/Tension Full Track") as AudioClip;
        backgroundCLip = Resources.Load("Audio/Battlefield Loop") as AudioClip;
        shootClip = Resources.Load("Audio/explosion_player") as AudioClip;
        asource.loop = true;
        asource.clip = menuClip;
        asource.Play();

        dayMap.isOn = false;
        randomMap.isOn = false;
        optionsPanel.SetActive(false); //set default status of options panel to not show
        gameOverPanel.SetActive(false); //set default status of gameover panel to not show
        playPanel.SetActive(false); //set default status of playing panel to not show
    }
}

