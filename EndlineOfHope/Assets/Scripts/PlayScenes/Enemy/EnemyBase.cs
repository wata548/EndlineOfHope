using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyMoveTypes {

    LINEAR,
    ACCELERATION,
    LERP
}

//maek Disappear ifs ,<- use delegate or action or func and enums
//deamege ifs <- it is same 

[Flags]
public enum Condition { 

    OBJECT, 
    PLAYER,
    OUT_FIELD,
    FUNCTION
}


[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase: MonoBehaviour {

    const int INF = 987654321;

    protected static List<(GameObject, EnemyBase)> Collector { get; set; } = new();

    protected bool             Start       { get; set; } = false;

    protected Rigidbody2D      Enemy       { get; set; }
    protected float            Deamage     { get; set; }
    protected GameObject       Player      { get; set; }
    protected EnemyMoveTypes   MoveType    { get; set; }
    protected float            AccelRatio  { get; set; }
    protected float            LerpPower   { get; set; }

    protected bool             Maintain    { get; set; }

    protected Vector2          Velocity    { get; set; } = Vector2.zero;

    protected Condition DemeageCondition    { get; set; }
    protected delegate bool deamage(Vector2 pos);
    protected Condition DisappearCondition  { get; set; } = Condition.PLAYER;
    protected delegate bool Disappear(Vector2 pos);

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

    public abstract void SetUp(GameObject target, EnemyMoveTypes type, float defaultPower, float power);
    public abstract void SetUp(Vector3 target, EnemyMoveTypes type, float defaultPower, float power);

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

    private void OnCollisionEnter2D(Collision2D collision) {


        if(Convert.ToInt32(DisappearCondition & Condition.PLAYER) != 0) {

            this.gameObject.SetActive(false);
            Collector.Add((this.gameObject, this));

            ShakeCamera.Instance.Shake(0.2f, 0.2f);
        }
    }
}


