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
        LEFT,
        RIGHT
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

    bool playerMove = true;

    bool ableMoveVertex     = true;
    bool ableMoveHorizon    = true;

    bool ableJump = false;

    [SerializeField]
    Direction grabityDirection = Direction.NONE;

    Vector2 grabityForce = Vector2.zero;

    #endregion

//==================================================| Method

    #region Method

    //* Return fixed position
    private (Vector2 position, Vector2 velocity) CheckPlayerOutRange(Vector2 playerPos, Vector2 velocity) {

        Vector2 playFieldPos = Datas.Instance.PlayFieldPos;

        //* First check up and right, Second check down and left
        int[] directions = { 1, -1 };

        foreach (var direction in directions) {

            if (direction * playerPos.x > direction * playFieldPos.x + ableMoveRange.x) {

                playerPos.x = playFieldPos.x + direction * ableMoveRange.x;
            }

            if (direction * playerPos.y > direction * playFieldPos.y + ableMoveRange.y) {

                playerPos.y = playFieldPos.y + direction * ableMoveRange.y;
            }
        }

        return (playerPos, velocity);
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

        switch (grabityDirection) {

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


        if(Input.GetKeyDown(KeyCode.R)) {

            grabityForce = Vector2.zero;
        }

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

            grabityForce += directionForce[grabityDirection] * DEFAULT_GRABITY_POWER * Time.deltaTime * timePower;

            switch (grabityDirection) {

                case Direction.UP:
                case Direction.DOWN:
                    playerVelocity.y = grabityForce.y;
                    playerVelocity.x = velocity.x;
                    break;

                case Direction.LEFT:
                case Direction.RIGHT:
                    playerVelocity.x = grabityForce.x;
                    playerVelocity.y = velocity.y;
                    break;

                default:
                    playerVelocity = velocity;
                    break;
            }

            //* check player out field and fix player's position 
            var fixInfo = CheckPlayerOutRange(playerPos, playerVelocity);

            if(fixInfo.position != playerPos) {

                player.transform.position = fixInfo.position;
            }

            playerRigidBody.velocity = playerVelocity;
            
        }   
    }

    #endregion
}
