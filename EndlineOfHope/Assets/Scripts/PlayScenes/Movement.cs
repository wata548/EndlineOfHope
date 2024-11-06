using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{

    public static Vector2 Magnetic(Vector2 start, Vector2 target, float power = 1) {

        Vector2 result = start;
        result -= target;
        result = result.normalized;
        result *= power;

        return result;
    }
}
