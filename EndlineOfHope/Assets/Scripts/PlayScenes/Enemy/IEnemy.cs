using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyMoveTypes {

    LINEAR,
    ACCELERATION
}

interface IEnemy{

    GameObject Player { get; }
    EnemyMoveTypes MoveType { get; }
}


