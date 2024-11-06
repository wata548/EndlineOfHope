using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum Direction {

    NONE = -1,
    UP = 0b1000,
    DOWN = 0b0100,
    RIGHT = 0b0010,
    LEFT = 0b0001
}

public class ContactCheck {

    public byte check = 0;

    public void Set(Direction direction, bool value) {

        if (direction == Direction.NONE) {

            return;
        }
        byte direct = Convert.ToByte(direction);

        if ((check & direct) != 0) {

            if (!value) {

                check ^= direct;
            }
        }

        else {
            if (value) {
                check |= direct;
            }
        }

    }

    public bool Get(Direction direction) {

        return (check & Convert.ToByte(direction)) != 0;
    }

    public void Clear() {
        check = 0;
    }
}

public class PlayerInnerData: MonoBehaviour
{
    public static PlayerInnerData Instance { get; private set; } = null;

    const float INVINCIBLE_FRAME = 1.2f;
    float currentInvincibleFrame = 0;
    bool invincible;

    public GameObject   Player              { get; private set; }
    public bool         PlayerMode          { get; private set; } = true;
    public bool         AbleMoveVertical    { get; private set; } = true;
    public bool         AbleMoveHorizon     { get; private set; } = true;
    public bool         AbleJump            { get; private set; } = false;
    public Direction    GravibtyDirection   { get; private set; } = Direction.NONE;
    public float        TimePower           { get; private set; } = 1;

    public (KeyCode key, Direction direction)[] KeyDirectionPairs { get; private set; } = 
    {
        (KeyCode.W, Direction.UP    ),
        (KeyCode.S, Direction.DOWN  ),
        (KeyCode.A, Direction.LEFT  ),
        (KeyCode.D, Direction.RIGHT ),
        (KeyCode.UpArrow, Direction.UP    ),
        (KeyCode.DownArrow, Direction.DOWN  ),
        (KeyCode.LeftArrow, Direction.LEFT  ),
        (KeyCode.RightArrow, Direction.RIGHT )
    };

    [SerializeField] GameObject player;
 
    public void Invincible() {

        if(invincible) {
            return;
        }

        invincible = true;
        currentInvincibleFrame = INVINCIBLE_FRAME;

    }

    private void Awake() {
        
        if(Instance == null) {

            Instance = this;
        }
        Player = player;
    }

    private void Update() {
        
        if(invincible) {

            currentInvincibleFrame -= Time.deltaTime;

            if (currentInvincibleFrame < 0) {

                currentInvincibleFrame = 0;
                invincible = false;
            }

        }
    }
}
