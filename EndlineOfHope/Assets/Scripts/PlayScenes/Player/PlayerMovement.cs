using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

//==================================================| Default Values

    const float DEFAULT_MOVEMENT_POEWR  = 5f;
    const float TRASH_HOLD              = 0.1f;
    const float FRICTION_RATIO          = 0.5f;
    const float DEFAULT_GRABITY_POWER   = 50f;

    const int   VERTICAL_START          = 0b1;
    const int   HORIZONTAL_START        = 0b001;

    //==================================================| Fields

    #region Field

    GameObject player;

    Rigidbody2D playerRigidBody = null;
    Vector2     playerSize;
    Vector2     ableMoveRange;
    Vector2     playerVelocity;
    Vector2     playerPos;

    float timePower = 1;

    Dictionary<Direction, Vector2> directionForce = new Dictionary<Direction, Vector2> {

        { Direction.NONE,   Vector2.zero    },
        { Direction.UP,     Vector2.up      },
        { Direction.DOWN,   Vector2.down    },
        { Direction.RIGHT,  Vector2.right   },
        { Direction.LEFT,   Vector2.left    }
    };
    (KeyCode key, Direction direction)[] keyDirectionPairs;
    (KeyCode key, Direction direction) jumpDirction = (KeyCode.Space, Direction.UP);

    ContactCheck contactWall = new();

    bool playerMove = true;

    bool ableMoveVertical     = true;
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

        contactWall.Clear();

        //* First check up and right, Second check down and left
        int[] direction = { 1, -1 };

        for (int i = 0, size = direction.Length; i < size; i++) {

            if (direction[i] * playerPos.x >= direction[i] * playFieldPos.x + ableMoveRange.x) {

                playerPos.x = playFieldPos.x + direction[i] * ableMoveRange.x;

                contactWall.Set((Direction)(HORIZONTAL_START << i), true);
            }

            if (direction[i] * playerPos.y >= direction[i] * playFieldPos.y + ableMoveRange.y) {

                playerPos.y = playFieldPos.y + direction[i] * ableMoveRange.y;

                contactWall.Set((Direction)(VERTICAL_START << i), true);
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

        if(!ableMoveVertical) {

            forceDelta.y = 0;
        }

        if(!ableMoveHorizon) {

            forceDelta.x = 0;
        }

        if(forceDelta.x != 0) {

            forceDelta.x = (forceDelta.x > 0 ? 1 : -1);
        }
        if(forceDelta.y != 0) {

            forceDelta.y = (forceDelta.y > 0 ? 1 : -1);
        }


        return forceDelta;
    }

    private void CalculateGravity(ref Vector2 gravityForce, Direction gravityDirection) {

        if (gravityDirection != Direction.NONE && !contactWall.Get(gravityDirection)) {

            gravityForce += directionForce[gravityDirection] * DEFAULT_GRABITY_POWER * timePower * Time.deltaTime;
        }
        else {

            gravityForce = Vector2.zero;
        }
    }

    private void UpdataSizeData() {

        timePower               = PlayerInnerData.Instance.TimePower;
        gravityDirection        = PlayerInnerData.Instance.GravibtyDirection;
        ableJump                = PlayerInnerData.Instance.AbleJump;
        ableMoveHorizon         = PlayerInnerData.Instance.AbleMoveHorizon;
        ableMoveVertical        = PlayerInnerData.Instance.AbleMoveVertical;
        playerMove              = PlayerInnerData.Instance.PlayerMode;
        player                  = PlayerInnerData.Instance.Player;

        playerVelocity  = playerRigidBody.velocity;
        playerPos       = player.transform.position;
        playerSize      = player.transform.localScale / 2;

        ableMoveRange   = Datas.Instance.PlayFieldSize - playerSize;
    }

    #endregion

//==================================================| Logic

    #region Ligic

    private void Awake() {

        keyDirectionPairs = PlayerInnerData.Instance.KeyDirectionPairs;

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
                velocity = DEFAULT_MOVEMENT_POEWR * forceDelta * timePower;
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
