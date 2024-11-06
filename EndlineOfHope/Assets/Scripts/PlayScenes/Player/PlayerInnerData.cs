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

public class Invincible {

    const float INVINCIBLE_FRAME = 1.2f;
    float currentFrame = 0;
    bool invincible;

    public void StartInvincible() {

        if (invincible) {
            return;
        }

        invincible = true;
        currentFrame = INVINCIBLE_FRAME;

    }

    public void Updata() {

        if(invincible) {

            currentFrame -= Time.deltaTime;

            if(currentFrame <= 0) {

                currentFrame = 0;
                invincible = false;
            }
        }
    }
}

public class PlayerInnerData : MonoBehaviour {
    public static PlayerInnerData Instance { get; private set; } = null;

    public Invincible Invincible { get; private set; } = new();
    [SerializeField] GameObject player;

    #region PlayFieldInfo
    [SerializeField] GameObject playField;
    public Vector2 PlayFieldSize { get; private set; }
    public Vector2 PlayFieldPos { get; private set; }
    #endregion

    #region ControleInfo
    public GameObject Player { get; private set; }
    public ContactCheck ContactWall { get; private set; } = new();

    public float TimePower { get; private set; } = 1;

    public bool PlayerMode { get; private set; } = true;
    public bool AbleMoveVertical { get; private set; } = true;
    public bool AbleMoveHorizon { get; private set; } = true;

    public bool AbleJump { get; private set; } = false;
    public Direction GravibtyDirection { get; private set; } = Direction.NONE;
    #endregion  

    #region Key
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
    public (KeyCode key, Direction direction) JumpDirction { get; private set; } = (KeyCode.Space, Direction.UP);
    #endregion

    private void UpdataFieldSize() {

        PlayFieldSize = playField.transform.localScale;
        PlayFieldPos = playField.transform.position;
    }

    private void Awake() {
        
        if(Instance == null) {

            Instance = this;
        }
        Player = player;
    }

    private void Update() {

        UpdataFieldSize();

        Invincible.Updata();

    }
}
