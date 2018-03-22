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

public class CannonData : MonoBehaviour
{
    public float cannonShelfLife = 1.5f;
    public float shellDamage; //amount of damage each cannon does
    public GameObject shooter;
    public AudioClip hit;

    

    // Use this for initialization
    void Start ()
    {
        if (shellDamage == 0)
        {
            shellDamage = 10;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        
    }

    /****Please see bottom of this class file to see explination on why I used OnCollisionEnter instead of a trigger******/

    private void OnTriggerEnter(Collider other)
    {
        //grab tankData of object that cannonball collided with
        TankData otherObjData = other.gameObject.GetComponent<TankData>();

        //check to see if the TankData otherobject is null
        if (otherObjData != null)
        {
            //if otherObjData not null  update the health on that object by taking away the amount of shell damage
            otherObjData.updateHealth(shellDamage);
            AudioSource.PlayClipAtPoint(hit, Vector3.zero);
            //update the shooting objects total damage done by adding the amount of shell damage that was done
            shooter.GetComponent<TankData>().updateDamageDone(shellDamage);

            //check to see if the healt of that object is at or below zero, if so destroy it and add a score to the shooting object
            if (otherObjData.health <= 0)
            {
                Destroy(other.gameObject);
                //shooter.GetComponent<TankData>().score += 1;
            }

            //after updating everything, destroy the cannonball
            Destroy(gameObject);
        }
        else
        {
            //cannonball will be destroyed by time with other script
        }
    }

}
