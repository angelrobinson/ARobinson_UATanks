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

//ensure that there is a TankData script attached for each component that we attach a TankMotor to
[RequireComponent(typeof(TankData))]
public class TankMotor : MonoBehaviour
{
    //private variables
    private CharacterController character;
    

    //public variables
    public Transform trans; //current object transform

    //called before start function
    private void Awake()
    {
        if (trans == null)
        {
            //assign the tranform in unity to the variable transform
            trans = gameObject.GetComponent<Transform>();
        }
        
    }

    // Use this for initialization
    void Start ()
    {
        //assign the character controller in unity to the variable character
        character = gameObject.GetComponent<CharacterController>();

        
            
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    //move controller forward or backward
    public void Move(float speed)
    {
        //create speed vector variable and initialize to the forward transform for forward motion
        //multiply the forward motion by the speed we passed into the method (positive = forward, negative = backward)
        //this will allow us to adjust the length of space the controller move
        Vector3 forwardSpeed = trans.forward * speed;
              

        //move the controller with the speed information we have created
        character.SimpleMove(forwardSpeed);
    }

    //rotate controller left or right
    public void Turn(float speed)
    {
        //create rotation vector variable and initialize to rotate on the Y axis
        //multiply the rotation by the speed that we passed into the method (Positive = right rotation, Negative = left rotation)
        //in order to rotate the controller each second we need to multiply the rotation by Time.deltaTime 
        //this allows us to adjust the direction the controller is facing
        Vector3 rotationSpeed = Vector3.up * speed * Time.deltaTime;

        //rotate the controller with the ratation speed information we have created so it will face the way we want it to
        trans.Rotate(rotationSpeed, Space.Self);
    }

    public bool TurnTowards(Vector3 destination, float rotateSpeed)
    {

        //find the vector equivalent to the difference of destination vector and current vector
        Vector3 diffToDestination = destination - trans.position;

        //rotate the tank until it is looking at the destination vector
        Quaternion lookat = Quaternion.LookRotation(diffToDestination);

        if (trans.rotation != lookat)
        {
            //slow down the ratation speed buy multiplying it by the amount of time has passed since last frame drawn
            float maxRotate = rotateSpeed * Time.deltaTime;

            //set the rotation of the tank 
            trans.rotation = Quaternion.RotateTowards(trans.rotation, lookat, maxRotate);


            return true;
        }
        else
        {
            return false;
        }


    }
}
