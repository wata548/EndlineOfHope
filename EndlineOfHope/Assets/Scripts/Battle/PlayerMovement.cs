using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

//==================================================| Defaul Values

    const float DEFAULT_MOVEMENT_POEWR  = 500f;
    const float TRASH_HOLD              = 0.1f;
    const float FRICTION_RATIO          = 0.5f;
    const float DEFAULT_GRABITY_POWER   = 50f;

    const int   VERTICAL_START          = 0;
    const int   HORIZONTAL_START        = 2;


//==================================================| Set on Heararchy

    [SerializeField] 
    GameObject  player = null;

//==================================================| Fields

    #region Field

    Rigidbody2D playerRigidBody = null;
    Vector2     playerSize;
    Vector2     ableMoveRange;
    Vector2     playerVelocity;
    Vector2     playerPos;

    [Range(-10, 10)] // It just test I will Delete
    [SerializeField] float timePower = 1;

    [Serializable]
    private enum Direction { 
    
        NONE = -1,
        UP,
        DOWN,
        RIGHT,
        LEFT
    }

    Dictionary<Direction, Vector2> directionForce = new Dictionary<Direction, Vector2> {

        { Direction.NONE,   Vector2.zero    },
        { Direction.UP,     Vector2.up      },
        { Direction.DOWN,   Vector2.down    },
        { Direction.RIGHT,  Vector2.right   },
        { Direction.LEFT,   Vector2.left    }
    };
    (KeyCode key, Direction direction)[] keyDirectionPairs = {

            (KeyCode.W, Direction.UP    ),
            (KeyCode.S, Direction.DOWN  ),
            (KeyCode.A, Direction.LEFT  ),
            (KeyCode.D, Direction.RIGHT )
    };
    (KeyCode key, Direction direction) jumpDirction = (KeyCode.Space, Direction.UP);
    bool[] contactWall = new bool[4];

    bool playerMove = true;

    bool ableMoveVertex     = true;
    bool ableMoveHorizon    = true;

    bool ableJump = false;

    [SerializeField]
    Direction gravityDirection = Direction.NONE;

    Vector2 gravityForce = Vector2.zero;

    #endregion

//==================================================| Method

    #region Method

    //* Return fixed position
    private Vector2 CheckPlayerOutRange(Vector2 playerPos) {

        Vector2 playFieldPos = Datas.Instance.PlayFieldPos;

        for(int i = 0, size = contactWall.Length; i < size; i++) {

            contactWall[i] = false;
        }

        //* First check up and right, Second check down and left
        int[] direction = { 1, -1 };

        for (int i = 0, size = direction.Length; i < size; i++) {

            if (direction[i] * playerPos.x >= direction[i] * playFieldPos.x + ableMoveRange.x) {

                playerPos.x = playFieldPos.x + direction[i] * ableMoveRange.x;

                contactWall[i + HORIZONTAL_START] = 
                    direction[i] * playerPos.x == direction[i] * playFieldPos.x + ableMoveRange.x;
            }

            if (direction[i] * playerPos.y >= direction[i] * playFieldPos.y + ableMoveRange.y) {

                playerPos.y = playFieldPos.y + direction[i] * ableMoveRange.y;

                contactWall[i + VERTICAL_START] = 
                    direction[i] * playerPos.y == direction[i] * playFieldPos.y + ableMoveRange.y;
            }
        }

        return playerPos;
    }

    //* Decrese velocity by friction (use lerf)
    private Vector2 DecreseVelocity(Vector2 playerVelo) {

        Vector2 absVelocity = new Vector2(playerVelo.x, playerVelo.y);

        if (absVelocity.x < TRASH_HOLD) {
            playerVelo.x = 0;
        }

        if (absVelocity.y < TRASH_HOLD) {
            playerVelo.y = 0;
        }

        switch (gravityDirection) {

            case Direction.NONE:
                playerVelo *= FRICTION_RATIO;
                break;

            case Direction.UP:
            case Direction.DOWN:
                playerVelo.x *= FRICTION_RATIO;
                break;

            default:
                playerVelo.y *= FRICTION_RATIO;
                break;

        }


        return playerVelo;
    }

    //* Get Input and return direction
    private Vector2 MovementInput() {

        Vector2 forceDelta = Vector2.zero;

        foreach(var keyDirectionPair in keyDirectionPairs) {

            if(Input.GetKey(keyDirectionPair.key)) {

                forceDelta += directionForce[keyDirectionPair.direction];
            }
        }

        if(!ableMoveVertex) {

            forceDelta.y = 0;
        }

        if(!ableMoveHorizon) {

            forceDelta.x = 0;
        }

        if(forceDelta.x != 0) {

            forceDelta.x = Convert.ToInt32(forceDelta.x) / Mathf.Abs(Convert.ToInt32(forceDelta.x));
        }
        if(forceDelta.y != 0) {

            forceDelta.y = Convert.ToInt32(forceDelta.y) / Mathf.Abs(Convert.ToInt32(forceDelta.y));
        }


        return forceDelta;
    }

    private void CalculateGravity(ref Vector2 gravityForce, Direction gravityDirection) {

        if (gravityDirection != Direction.NONE && contactWall[Convert.ToInt16(gravityDirection)] == false) {

            gravityForce += directionForce[gravityDirection] * DEFAULT_GRABITY_POWER * Time.deltaTime * timePower;
        }
        else {

            gravityForce = Vector2.zero;
        }
    }

    private void UpdataSizeData() {

        playerVelocity  = playerRigidBody.velocity;
        playerPos       = player.transform.position;
        playerSize      = player.transform.localScale / 2;

        ableMoveRange   = Datas.Instance.PlayFieldSize - playerSize;
    }

    #endregion

//==================================================| Logic

    #region Ligic

    private void Awake() {

        if(playerRigidBody == null) {

            playerRigidBody = player.GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {

        if(playerRigidBody != null && playerMove) {

            UpdataSizeData();

            Vector2 velocity = Vector2.zero;
            Vector2 forceDelta = MovementInput();
            bool input = (forceDelta != Vector2.zero);

            if(input) {

                forceDelta.Normalize();
                velocity = DEFAULT_MOVEMENT_POEWR * forceDelta * Time.deltaTime * timePower;
            }

            else {

                //* Decrese velocity
                velocity = DecreseVelocity(playerVelocity);
            }

            CalculateGravity(ref gravityForce, gravityDirection);

            switch (gravityDirection) {

                case Direction.UP:
                case Direction.DOWN:
                    playerVelocity.y = gravityForce.y;
                    playerVelocity.x = velocity.x;
                    break;

                case Direction.LEFT:
                case Direction.RIGHT:
                    playerVelocity.x = gravityForce.x;
                    playerVelocity.y = velocity.y;
                    break;

                default:
                    playerVelocity = velocity;
                    break;
            }

            //* check player out field and fix player's position 
            var fixPos = CheckPlayerOutRange(playerPos);

            if(fixPos != playerPos) {

                player.transform.position = fixPos;
            }

            playerRigidBody.velocity = playerVelocity;
            
        }   
    }

    #endregion
}
