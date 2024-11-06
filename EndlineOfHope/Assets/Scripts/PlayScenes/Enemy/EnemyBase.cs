using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyMoveTypes {

    LINEAR,
    ACCELERATION,
    LERP
}

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase: MonoBehaviour {

    const int INF = 987654321;

    protected bool             start       { get; set; } = false;

    protected Rigidbody2D      Enemy       { get; set; }
    protected float            Deamage     { get; set; }
    protected GameObject       Player      { get; set; }
    protected EnemyMoveTypes   MoveType    { get; set; }
    protected float            AccelRatio  { get; set; }
    protected float            LerpPower   { get; set; }
    protected bool             Maintain    { get; set; }

    protected Vector2          Velocity    { get; set; } = Vector2.zero;

    public void SendDeamage() {

        PlayerInnerData.Instance.Invincible.StartInvincible();
    }

    public void Lerp() {

        if(MoveType == EnemyMoveTypes.LERP) {

            float power = LerpPower * Time.deltaTime;
            Velocity = new Vector2(Velocity.x + power, Velocity.y + power);
        }
    }

    public void Acceleration() {

        if (MoveType == EnemyMoveTypes.ACCELERATION) {

            Velocity += Velocity * AccelRatio * Time.deltaTime;
        }
    }

    public abstract void SetUp(GameObject target, EnemyMoveTypes type, float power);
    public abstract void SetUp(Vector3 target, EnemyMoveTypes type, float power);

    protected void SetPower(EnemyMoveTypes type, float power) {

        MoveType = type;

        if (type == EnemyMoveTypes.ACCELERATION) {

            AccelRatio = power;
        }

        if (type == EnemyMoveTypes.LERP) {

            LerpPower = power;
        }
    }

    private void Awake() {

        Enemy = GetComponent<Rigidbody2D>();
        Enemy.mass = INF;
    }
}


