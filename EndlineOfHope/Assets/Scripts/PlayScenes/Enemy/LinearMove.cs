using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LinearMove : EnemyBase
{

    public override void SetUp(GameObject target, EnemyMoveTypes type = EnemyMoveTypes.LERP, float power = 0) {

        start = true;

        Velocity = target.transform.position - this.transform.position;
        Velocity = Velocity.normalized;

        SetPower(type, power);
    }

    public override void SetUp(Vector3 target, EnemyMoveTypes type = EnemyMoveTypes.LERP, float power = 0) {

        start = true;

        Velocity = target - this.transform.position;
        Velocity = Velocity.normalized;

        SetPower(type, power);
    }

    private void Update() {

        if(start) {

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
