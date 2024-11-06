using DG.Tweening;
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

    OBJECT      = 0b1000, 
    PLAYER      = 0b0100,
    OUT_FIELD   = 0b0010,
    FUNCTION    = 0b0001
}

public class CheckAndAnimation {

    public Func<Vector2, bool> Check = null;
    public Func<GameObject, Sequence> Before = null;
    public Func<GameObject, Sequence> After = null;
}

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase: MonoBehaviour {

    const int INF = 987654321;

    const float CAMERA_SHAKE_POWER = 0.2f;
    const float CAMERA_SHAKE_DURACTION = 0.2f;

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

    protected Condition DemeageCondition    { get; set; } = Condition.OBJECT;
    public CheckAndAnimation damage = new();

    protected Condition DisappearCondition  { get; set; } = Condition.PLAYER;
    public CheckAndAnimation disappear = new();

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

        bool playerCollision = collision.gameObject == PlayerInnerData.Instance.Player;

        bool disappearCheck =
            (ConditionCheck(DisappearCondition, Condition.PLAYER) && playerCollision) ||
            (ConditionCheck(DisappearCondition, Condition.OBJECT));

        bool deameageCheck =
            (ConditionCheck(DemeageCondition, Condition.PLAYER) && playerCollision) ||
            (ConditionCheck(DemeageCondition, Condition.OBJECT));

        if (disappearCheck) {

            DisappearProcess();
        }
        if (deameageCheck) {

            DamageProcess();
        }

    }
    bool ConditionCheck(Condition current, Condition target) {

        return Convert.ToInt32(current & target) != 0;
    }

    void DisappearProcess() {

        var nullCheck = disappear.Before?.Invoke(this.gameObject);

        if (nullCheck == null) {

            DisappearAfterProcess();
        }
        else {

            nullCheck.OnComplete(DisappearAfterProcess);
        }
    }
    void DisappearAfterProcess() {

        this.gameObject.SetActive(false);
        Collector.Add((this.gameObject, this));

        disappear.After?.Invoke(this.gameObject);
    }

    void DamageProcess() {

        var nullCheck = damage.Before?.Invoke(this.gameObject);

        if (nullCheck == null) {

            DamageAfterProcess();
        }
        else {

            nullCheck.OnComplete(DamageAfterProcess);
        }
    }
    void DamageAfterProcess() {

        if(PlayerInnerData.Instance.Invincible.StartInvincible()) {

            ShakeCamera.Instance.Shake(CAMERA_SHAKE_POWER, CAMERA_SHAKE_DURACTION)?.
                OnComplete(() => damage.After?.Invoke(this.gameObject));
        }
    }
}


