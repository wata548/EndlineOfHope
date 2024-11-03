using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffect { 

    NONE,
    BURNED,
    POISON
}

public class PlayersInfo
{

    const int RECORD_PROKE = 5;

    public string       Name            { get; private set; } = "\0";

    public int          MaximumHp       { get; private set; } = 0;
    public int          Hp              { get; private set; } = 0;
    public int          MaximumMp       { get; private set; } = 0;
    public int          Mp              { get; private set; } = 0;
    public int          AttackPower     { get; private set; } = 0;

    public StatusEffect Effect          { get; private set; } = StatusEffect.NONE;
    public float        EffectPower     { get; private set; } = 0;
    public int          EffectDuraction { get; private set; } = 0;

    public int          Shild           { get; private set; } = 0;
    public int          shildDuraction  { get; private set; } = 0;

    public int          SomeProvoke     { get; private set; } = 0;

    List<int> provokeHistory = new();

    PlayersInfo() {

    }

    public int Attack() {

        if (provokeHistory.Count >= RECORD_PROKE) {

            SomeProvoke -= provokeHistory[0];
            provokeHistory.RemoveAt(0);
        }

        SomeProvoke += AttackPower;
        provokeHistory.Add(AttackPower);

        return AttackPower;
    }

    public int Attacked(int power) {

        if(Shild > 0) {

            if (Shild > power) {
                Shild -= power;

                return Hp;
            }

            else {

                power -= Shild;
                Shild = 0;
            }
        }

        Hp -= power;

        return Hp;
    }

    public void MakeShild(int duraction, int power) {

        Shild           = power;
        shildDuraction  = duraction;
    }

    public void ReinforceShild(int duraction = 0, int power = 0) {

        Shild          += power;
        shildDuraction += duraction;
    }

    public void GetStatusEffect(StatusEffect effect, float power, int duraction) {

        Effect          = effect;
        EffectPower     = power;
        EffectDuraction = duraction;
    }

    public void UseItem() {

    }
}
