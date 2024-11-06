using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LinearMove : EnemyBase
{

    public override void SetUp(GameObject target, EnemyMoveTypes type = EnemyMoveTypes.LERP, float defaultPower = 1, float power = 0) {

        Start = true;

        Velocity = target.transform.position - this.transform.position;
        Velocity = Velocity.normalized;
        Velocity *= defaultPower;

        SetPower(type, power);
    }

    public override void SetUp(Vector3 target, EnemyMoveTypes type = EnemyMoveTypes.LERP, float defaultPower = 1, float power = 0) {

        Start = true;

        Velocity = target - this.transform.position;
        Velocity = Velocity.normalized;
        Velocity *= defaultPower;

        SetPower(type, power);
    }
    public GameObject player;
    private void Awake() {

        Player = player;
    }
    private void Update() {

        if(Start) {

            Enemy.velocity = Velocity;

            if (MoveType == EnemyMoveTypes.LERP) {

                Lerp();
            }

            if (MoveType == EnemyMoveTypes.ACCELERATION) {

                Acceleration();
            }
        }
    }

}
