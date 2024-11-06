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
    (KeyCode key, Direction direction) jumpDirction;

    ContactCheck contactWall;
    bool playerMove = true;
    bool ableMoveVertical = true;
    bool ableMoveHorizon = true;
    bool ableJump = false;
    Direction gravityDirection = Direction.NONE;
    Vector2 gravityForce = Vector2.zero;

    #endregion

//==================================================| Method

    #region Method

    //* Return fixed position
    private Vector2 CheckPlayerOutRange(Vector2 playerPos) {

        Vector2 playFieldPos = PlayerInnerData.Instance.PlayFieldPos;

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
    private Vector2 DecreseVelocity(Vector2 playerVelocity) {

        playerVelocity = FrictionTrashHold(playerVelocity, TRASH_HOLD);
        playerVelocity = MovementFriction(playerVelocity, gravityDirection);

        return playerVelocity;
    }
    private Vector2 MovementFriction(Vector2 velocity, Direction gravity = Direction.NONE) {

        switch (gravity) {

            case Direction.LEFT:
            case Direction.RIGHT:
                velocity.y *= FRICTION_RATIO;
                break;

            case Direction.UP:
            case Direction.DOWN:
                velocity.x *= FRICTION_RATIO;
                break;

            default:
                velocity *= FRICTION_RATIO;
                break;

        }

        return velocity;
    }
    private Vector2 FrictionTrashHold(Vector2 velocity, float trashHoldRange) {

        if (Mathf.Abs(velocity.x) < trashHoldRange) {
            velocity.x = 0;
        }

        if (Mathf.Abs(velocity.y) < trashHoldRange) {
            velocity.y = 0;
        }

        return velocity;
    }
    
    //* Get Input and return direction
    private Vector2 MovementInput() {

        Vector2 forceDelta = InputKey();

        //* check able move
        if(!ableMoveVertical) {

            forceDelta.y = 0;
        }
        if(!ableMoveHorizon) {

            forceDelta.x = 0;
        }

        //* multple key process 
        if(forceDelta.x != 0) {

            forceDelta.x = (forceDelta.x > 0 ? 1 : -1);
        }
        if(forceDelta.y != 0) {

            forceDelta.y = (forceDelta.y > 0 ? 1 : -1);
        }


        return forceDelta;
    }
    private Vector2 InputKey() {

        Vector2 inputKey = Vector2.zero;

        foreach (var keyDirectionPair in keyDirectionPairs) {

            if (Input.GetKey(keyDirectionPair.key)) {

                inputKey += directionForce[keyDirectionPair.direction];
            }
        }

        return inputKey;
    }

    private Vector2 CalculateGravity(Vector2 velocity, Vector2 gravityForce, Direction gravityDirection) {

        if (gravityDirection != Direction.NONE && !contactWall.Get(gravityDirection)) {

            gravityForce += 
                directionForce[gravityDirection] * DEFAULT_GRABITY_POWER * timePower * Time.deltaTime;
        }

        else {

            gravityForce = Vector2.zero;
        }

        switch (gravityDirection) {

            case Direction.UP:
            case Direction.DOWN:
                velocity.y = gravityForce.y;
                break;

            case Direction.LEFT:
            case Direction.RIGHT:
                velocity.x = gravityForce.x;
                break;
        }

        return velocity;
    }

    private void SetDefaultData() {

        contactWall = PlayerInnerData.Instance.ContactWall;
        keyDirectionPairs = PlayerInnerData.Instance.KeyDirectionPairs;
    }

    private void UpdataSizeData() {

        timePower               = PlayerInnerData.Instance.TimePower;
        gravityDirection        = PlayerInnerData.Instance.GravibtyDirection;
        ableJump                = PlayerInnerData.Instance.AbleJump;
        ableMoveHorizon         = PlayerInnerData.Instance.AbleMoveHorizon;
        ableMoveVertical        = PlayerInnerData.Instance.AbleMoveVertical;
        playerMove              = PlayerInnerData.Instance.PlayerMode;
        jumpDirction            = PlayerInnerData.Instance.jumpDirction;

        playerVelocity  = playerRigidBody.velocity;
        playerPos       = player.transform.position;
        playerSize      = player.transform.localScale / 2;

        ableMoveRange   = PlayerInnerData.Instance.PlayFieldSize / 2 - playerSize;
    }

    #endregion

//==================================================| Logic

    #region Ligic

    private void Awake() {

        SetDefaultData();

        if(playerRigidBody == null) {

            player          = PlayerInnerData.Instance.Player;
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

                velocity = DecreseVelocity(playerVelocity);
            }

            playerVelocity = CalculateGravity(velocity, gravityForce, gravityDirection);

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
