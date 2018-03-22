Enemy states:
PATROL, CHASE, SHOOT, REST, RUN, INVESTIGATE, AVOIDING

PATROL: An enemy with this state patrols between a set number of  points. Along with this main behavior there is a secondary mode for the patrol to either LOOP or PING PONG when it gets to the last destination.

CHASE: An enemy in this state will find a target within the players array of the game manager and chase after it for a time. After that time has expired, it will randomly choose another player and chase after it.

REST: An enemy in this state will not move and reginerate health

RUN: An an enemy in this state will run towards a specific spot so they can rest

INVESTIGATE: This is not implemented in Module 2 but the later intention is to turn towards a sound and check if they can see anything in that direction. if it is a player then it will choose to chase, shoot or run from it.


Music:
AuraReverb:used for tank sound of movement for player (recorded by myself)
PowerUp: used for powerup pick up (recorded by myself)
explosion_player:  used for when shooting (taken from the Unity asteroids tutorial package)
Battlefield Loop: used for in game background music (downloaded from asset store - Dynamic Music Pack by John Leonard French)
Tension Full Track: used for game menu music (downloaded from asset store - Dynamic Music Pack by John Leonard French)
explosion_asteroid:  used for when enemy tank dies (taken from the Unity asteroids tutorial package)
MenuClick: used for button clicks (recorded by myself)
ShotHit: used for when cannonball hits something (recorded by myself)