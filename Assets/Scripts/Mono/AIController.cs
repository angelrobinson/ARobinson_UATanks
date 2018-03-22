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

public class AIController : MonoBehaviour
{
    public enum EnemyMode { PATROL, CHASE, RUN, SHOOT, REST, INVESTIGATE, AVOIDING }
    public enum Behavior { AGGRESSIVE, NERVOUS, TIMID }

    //variables
    [Header("Main Variables")]
    private GameManager aiManager;
    public TankMotor aiMotor;
    public TankData aiData;
    public TankShoot aiShoot;
    public Transform aiTrans;
    public EnemyMode aiMode;
    public Behavior aiBehavior;

    [Header("Shooting info")]
    private float lastShot;
    public float maxDistanceToShoot = 15.0f;

    [Header("Avoiding info")]
    public bool avoid; //bool to keep track of if  tank needs to avoid or not
    private int avoidingStep; //0 = nothing in the way 1 = turn left -1 = turn right 2 = move backwards and right
    public EnemyMode defaultMode;
    public float angle = 50.0f; //angle to use for sensors
    public float sensorLength = 1.0f;

    [Header("Sensor info")]
    private Vector3 start; //set start position of sensors
    private Vector3 rtEndAngle; //set angle of sensor to the right
    private Vector3 lftEndAngle; //set angle of sensor to the left
    private RaycastHit hitInfo; //hold sensor hit info

    //front sensors
    private Ray front;
    private Ray rtFront;
    private Ray lftFront;

    [Header("Chasing & Running info")]
    Vector3 chaseRunTarget; //target position to chase
    float distanceToTarget; //distance between chaseTarget and gameObject
    private int targetIndex; //variable to hold a random number to indicate index of the players array so chastTarget can be set randomly
    public float timeToChaseRun; //amount of time to chase the target allowable to change in inspector
    private float chaseRunTime; //used in timer

    [Header("Patol Info")]
    public GameObject[] patrolPath;
    [SerializeField] private GameObject patrol;
    private bool isForward;
    private int destination;
    public float close;
    public enum Pathway { Loop, PingPong };
    public Pathway pathway;

    [Header("FOV info")]
    public float fieldOfView = .45f;
    public float viewDistance = 15f;
    public float hearDistance = 15f;

    // Use this for initialization
    void Start()
    {
        //grab the game manager so we can access that information quickly
        aiManager = GameObject.Find("Game").GetComponent<GameManager>();

        //grab info from manager enemies array
        for (int i = 0; i < aiManager.enemies.Length; i++)
        {
            if (gameObject.Equals(aiManager.enemies[i]))
            {
                aiMotor = aiManager.enemies[i].GetComponent<TankMotor>();
                aiData = aiManager.enemies[i].GetComponent<TankData>();
                aiShoot = aiManager.enemies[i].GetComponent<TankShoot>();
                aiTrans = aiManager.enemies[i].GetComponent<Transform>();

            }

            
            //randomly choose a mode for the enemies to start with
            /***NOTE: This will need to be commented out if you want to test and change states within the inspector***/
                    
            int modeIndex = Random.Range(0, 3);
            switch (modeIndex)
            {
                case 0:
                    aiBehavior = Behavior.AGGRESSIVE;
                    aiMode = defaultMode = EnemyMode.CHASE;
                    break;
                case 1:
                    aiBehavior = Behavior.NERVOUS;
                    aiMode = defaultMode = EnemyMode.PATROL;
                    break;
                case 2:
                    aiBehavior = Behavior.TIMID;
                    defaultMode = EnemyMode.RUN;
                    aiMode = EnemyMode.PATROL;
                    break;              
            }
        }

        isForward = true;
        close = aiData.frontSpeed;



        
        for (int spawn = 0; spawn < aiManager.enemySpawns.Length; spawn++)
        {
            if (aiTrans.position == aiManager.enemySpawns[spawn].transform.position)
            {
                
                patrol = aiManager.patrols[spawn];
                int childCount = patrol.transform.childCount;
                patrolPath = new GameObject[childCount];

                for (int child = 0; child < patrolPath.Length; child++)
                {
                    patrolPath[child] = patrol.transform.GetChild(child).gameObject;
                }
            }
        }
       


        //we assume that the last time that the object shot was at the time of beginning of game
        lastShot = Time.time;
        chaseRunTime = 0; //set initial chase time to zero so that when mode is changed to chase the initial target can be picked
        avoidingStep = 0;
        destination = 0;
        


    }
        

    // Update is called once per frame
    void Update()
    {
        distanceToTarget = Vector3.Distance(aiTrans.position, chaseRunTarget);
        
        //avoid = CanMove(); //use this if you want to see in inspector to debug CanMove(). if you do use this change the following iff statement to if (!avoid){...}

        if (!CanMove())
        {            
            aiMode = EnemyMode.AVOIDING;
        }

        //update timer of chaseRunTime
        chaseRunTime -= Time.deltaTime;

        //if chaseRuntime is zero or less get another random target from players array to chase
        if (chaseRunTime <= 0)
        {
            if (defaultMode == EnemyMode.CHASE || defaultMode == EnemyMode.RUN)
            {
                targetIndex = Random.Range(0, aiManager.players.Length); //random number to use as index of players array for initial target
                chaseRunTarget = aiManager.players[targetIndex].transform.position; // chase target

            }

            if (aiBehavior == Behavior.NERVOUS)
            {
                for (int i = 0; i < (timeToChaseRun/2); i++)
                {
                    aiMotor.Turn(aiData.turnSpeed);
                }
                
                
            }
            chaseRunTime = timeToChaseRun;//reset timer
        }

        if (aiData.health < aiData.maxHealth*.25)
        {
            aiMode = EnemyMode.REST;
        }

        

        switch (aiMode)
        {
            case EnemyMode.PATROL:
                
                Patrolling();
                break;
            case EnemyMode.CHASE:             
                Chasing(chaseRunTarget);
                break;            
            case EnemyMode.REST:
                Resting();
                break;
            case EnemyMode.RUN:                
                Running(chaseRunTarget);
                break;
            case EnemyMode.INVESTIGATE:
                //not implemented yet. see AIEXplinations.txt for intended behavior
                break;
            case EnemyMode.AVOIDING:
                Avoid();
                break;
            default:
                break;
        }
    }


    public void Resting()
    {
        //if health is lower than 25% of maxHealth AI finds a spot with tag "Rest" and heals up
        aiMotor.TurnTowards(GameObject.FindGameObjectWithTag("Rest").transform.position, aiData.turnSpeed);
        aiMotor.Move(aiData.frontSpeed);

        //regen health
        while (aiData.health < aiData.maxHealth)
        {
            aiData.health += 1;
        }

        //once rested up go back to default mode
        if (aiData.health == aiData.maxHealth)
        {
            aiMode = defaultMode;
        }
    }
    public void Patrolling()
    {
        //if tank is facing the destination position then move towards that position and set the next destination when we get close enough to the current destination
        if (!(aiMotor.TurnTowards(patrolPath[destination].transform.position, aiData.turnSpeed)))
        {

            aiMotor.Move(aiData.frontSpeed);

            if (Vector3.SqrMagnitude(patrolPath[destination].transform.position - aiTrans.position) < (close * close))
            {
                //if current destination is the last point in the path array check the pathway loop setting

                if (destination == patrolPath.Length - 1)
                {
                    switch (pathway)
                    {

                        case Pathway.Loop:
                            //if setting is to loop change current destination back to first index of the array
                            destination = 0;
                            break;
                        case Pathway.PingPong:
                            //if setting is PingPong then change the forward bool to false to make it go the other way and decrease the currentdestnation by 1
                            isForward = false;
                            destination--;
                            break;
                        default:
                            break;
                    }

                }
                else
                {
                    //if the forward bool is true increment the current destination
                    //if the forward bool is false decrement the current destination unless the current destination equal zero then change the bool to true
                    if (isForward)
                    {
                        destination++;
                    }
                    else
                    {
                        destination--;
                        if (destination == 0)
                        {
                            isForward = true;
                        }
                    }

                }

            }

        }
    }

    public void Running(Vector3 hostile)
    {
        //find a vector away from the chasee by first getting the vector that is between the chaser and chasee
        Vector3 betweenTanks = hostile - aiTrans.position;

        //multiply the vector that is between the two objects by -1 to flip the direction the vector is facing
        Vector3 runDirection = -1 * betweenTanks;

        //use the normalize method to make that vector a length of one meter and then multiply it by the distance that designers set for the AI to go. 
        runDirection.Normalize();
        runDirection *= distanceToTarget;

        //add the runDirection vector to the AI position vector to use as a target position when passed into the TankMotr turnTowards method and then tell it to move towards that target
        Vector3 runTarget = runDirection + aiTrans.position;
        aiMotor.TurnTowards(runTarget, aiData.turnSpeed);
        aiMotor.Move(aiData.frontSpeed);
    }

    public void Chasing(Vector3 target)
    {

        aiMotor.TurnTowards(target, aiData.turnSpeed);
        //if the target is greater than the distance of the frontSpeed variable then continue chasing
        if (distanceToTarget < aiData.frontSpeed)
        {
            if (Time.time >= lastShot + aiData.fireRate)
            {
                aiShoot.Shoot(aiData.shellForce);
                lastShot = Time.time;
            }
        }

        aiMotor.Move(aiData.frontSpeed);
        
        
    }
    public bool CanMove()
    {
        start = aiTrans.position; //set start position of sensors
        rtEndAngle = Quaternion.AngleAxis(angle, aiTrans.up) * aiTrans.forward; //set angle of sensor to the right
        lftEndAngle = Quaternion.AngleAxis(-angle, aiTrans.up) * aiTrans.forward; //set angle of sensor to the left

        //front sensors
        front = new Ray(start, aiTrans.forward);
        rtFront = new Ray(start, rtEndAngle);
        lftFront = new Ray(start, lftEndAngle);

        //check if front center sensor hit anything. if not return true
        if (Physics.Raycast(front, out hitInfo, aiData.frontSpeed))
        {
            Debug.DrawLine(start, hitInfo.point, Color.red);//show sensor in inspector as a line

            
            //if the front sensor hit something, check to see if the right & left sensors are hitting something

            //if right and front sensors are hitting something set avoiding ste to 1
            if (Physics.Raycast(rtFront, out hitInfo, aiData.frontSpeed))
            {
                Debug.DrawLine(start, hitInfo.point, Color.red);//show sensor in inspector as a line

                if (!hitInfo.collider.gameObject.CompareTag("Player"))
                {
                    avoidingStep = 1;
                }                
                else
                {
                    //if collider hit belongs to a player tank, make the player tank the chase target and get the distance between the two
                    chaseRunTarget = hitInfo.collider.gameObject.transform.position;
                    distanceToTarget = Vector3.Distance(chaseRunTarget, aiTrans.position);
                }
            }
            //if left and front sensors are hitting something set avoiding step to -1
            if (Physics.Raycast(lftFront, out hitInfo, aiData.frontSpeed))
            {
                Debug.DrawLine(start, hitInfo.point, Color.red);//show sensor in inspector as a line

                if (!hitInfo.collider.gameObject.CompareTag("Player"))
                {
                    avoidingStep = -1;
                }
                else
                {
                    //if collider hit belongs to a player tank, make the player tank the chase target and get the distance between the two
                    
                }
            }
            
            //return false because something is blocking the way and we can't move onward
            return false;
        }
        else
        {
            //if nothing in the way set avoiding set to 0 and return true because nothing is blocking the way.
            avoidingStep = 0;
            return true;
        }

        
    }
    public void Avoid()
    {
        if (avoidingStep == 1)
        {
            aiMotor.Turn(-1 * aiData.turnSpeed);
        }
        else if (avoidingStep == -1)
        {
            aiMotor.Turn(1 * aiData.turnSpeed);
        }
        else if (avoidingStep == 0)
        {
            aiMode = defaultMode;
        }
        
    }

    public bool CanSee(GameObject target)
    {
        //find agle to target
        Vector3 vectorToTarget = target.transform.position - transform.position;

        //find the angle between current object and the vector to Target
        float angleToTarget = Vector3.Angle(transform.forward, vectorToTarget);


        //if it is less than my FOV, then object is inside my FOV
        if (angleToTarget < fieldOfView)
        {


            //check for line of sight
            //Rays only sees colliders, so we need to put info of the collider that the ray hit into a variable by using RaycastHit variable
            RaycastHit hit;
            Ray ray = new Ray();
            ray.origin = transform.position; //start at my position
            ray.direction = vectorToTarget;  //look at my target

            //Physics.Raycast(ray, out hit, viewDistance); == cast a ray out in the distance of viewDistance and put the info of the object that is hit into the variable hit. if nothing is hit, hit variable is null
            //check to see what the ray hit
            if (Physics.Raycast(ray, out hit, viewDistance))
            {
                if (hit.collider.gameObject.Equals(target))
                {
                    //if I hit my target
                    return true;
                }
                else
                {
                    //if I hit something else other than my target
                    return false;
                }

            }
            //if the ray didn't hit anything at all
            return false;

        }
        else
        {
            //else
            //  i can't see it, is out of my FOV
            return false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        AudioSource noise = other.gameObject.GetComponent<AudioSource>();

        if (noise != null)
        {
            aiTrans.LookAt(noise.transform.position);

            if (CanSee(GameObject.FindGameObjectWithTag("Player")))
            {
                switch (aiBehavior)
                {
                    case Behavior.AGGRESSIVE:
                        aiShoot.Shoot(aiData.shellForce);
                        aiMode = EnemyMode.CHASE;
                        break;
                    case Behavior.NERVOUS:
                        aiShoot.Shoot(aiData.shellForce);
                        aiMode = EnemyMode.CHASE;
                        break;
                    case Behavior.TIMID:
                        aiShoot.Shoot(aiData.shellForce);
                        aiMode = EnemyMode.RUN;
                        break;
                    default:
                        break;
                }
                
            }


        }

    }


}


/********
 * 
 * 
        //automatically make the AI turn in a circle by "pushing" right rotatiion
        aiMotor.Turn(aiData.turnSpeed);

        //if current time is greater than the last time we shot plus the amount of secconds in fireRate then we can have AI shoot
        //once shot, reset the lastShot as the current time
        if (Time.time >= lastShot + aiData.fireRate)
        {
            aiShoot.Shoot(aiData.shellForce);
            lastShot = Time.time;
        }

 * 
 * *******/
