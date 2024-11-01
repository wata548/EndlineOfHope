using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

//==================================================| Defaul Values

    #region DefaultValue

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


    const float DEFAULT_MOVEMENT_POEWR  = 50f;
    const float TRASH_HOLD              = 0.1f;
    const float FRICTION_RATIO          = 0.5f;

    readonly Vector2 MAXIMUM_SPEED      = new Vector2(5, 5);

    #endregion

//==================================================| Set on Heararchy

    [SerializeField] 
    GameObject  player          = null;

//==================================================| Fields

    #region Field

    Rigidbody2D playerRigidBody = null;
    Vector2     playerSize;
    Vector2     ableMoveRange;
    Vector2     playerVelocity;

    [Range(-10, 10)] // It just test I will Delete
    [SerializeField] float timePower = 1;

    (KeyCode key, Direction direction)[] keyDirectionPairs = {

            (KeyCode.W, Direction.UP    ),
            (KeyCode.S, Direction.DOWN  ),
            (KeyCode.A, Direction.LEFT  ),
            (KeyCode.D, Direction.RIGHT )
    };

    bool playerMove = true;

    bool ableMoveVertex     = true;
    bool ableMoveHorizon    = true;

    bool ableJump = false;
    (KeyCode key, Direction direction) jumpDirction = (KeyCode.Space, Direction.UP);

    [SerializeField]
    Direction grabityDirection = Direction.NONE;

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
                
                if(direction * playerVelocity.x > 0) {

                    playerVelocity.x = 0;
                }
            }

            if (direction * playerPos.y > direction * playFieldPos.y + ableMoveRange.y) {

                playerPos.y = playFieldPos.y + direction * ableMoveRange.y;

                if (direction * playerVelocity.y > 0) {

                    playerVelocity.y = 0;
                }
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

        return forceDelta;
    }

    #endregion

    float ScalarNomalize(float scalar) {

        return scalar > 0 ? 1 : -1;
    }

    //==================================================| Logic

    #region Logic

    private void Awake() {

        if(playerRigidBody == null) {

            playerRigidBody = player.GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {

        if(playerRigidBody != null && playerMove) {

            Vector2 forceDelta = MovementInput();
            bool input = (forceDelta != Vector2.zero);

            if(input) {

                forceDelta.Normalize();

                Vector2 delta = DEFAULT_MOVEMENT_POEWR * forceDelta * Time.deltaTime * timePower;
                Vector2 estimate = playerRigidBody.velocity + delta;

                if (MAXIMUM_SPEED.x - Mathf.Abs(estimate.x) < Mathf.Abs(delta.x)) {

                    delta.x = ScalarNomalize(delta.x) * (MAXIMUM_SPEED.x - Mathf.Abs(estimate.x));
                }

                if (MAXIMUM_SPEED.y - Mathf.Abs(estimate.y) < Mathf.Abs(delta.y)) {

                    delta.y = ScalarNomalize(delta.y) * (MAXIMUM_SPEED.y - Mathf.Abs(estimate.y));
                }


                Debug.Log($"{playerRigidBody.velocity} , {delta}");
                playerRigidBody.AddForce(delta, ForceMode2D.Impulse);
            }

            else {

                //* Decrese velocity
                playerVelocity = playerRigidBody.velocity;

                playerRigidBody.velocity = DecreseVelocity(playerVelocity);
            }
/*
            playerSize = player.transform.localScale / 2;
            ableMoveRange = Datas.Instance.PlayFieldSize - playerSize;

            playerVelocity = playerRigidBody.velocity + directionForce[grabityDirection] * 5;

            //* check player out field and fix player's position 
            Vector2 playerPos = player.transform.position;
            var fixInfo = CheckPlayerOutRange(playerPos, playerVelocity);

            if(fixInfo.position != playerPos) {

                player.transform.position   = fixInfo.position;
            }
            playerRigidBody.velocity    = fixInfo.velocity;
            */
        }   
    }

    #endregion

}
