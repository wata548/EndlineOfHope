using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UISelect : MonoBehaviour {

    enum Buttons {

        NONE = -1,
        START,
        SETTING,
        EXIT,
        COUNT

    };
    [SerializeField] GameObject[] buttons = new GameObject[3];
    TMP_Text[] texts = new TMP_Text[3];

    const float SELECT_SIZE = 1.2f;
    const float EFFECT_DURATION = 0.2f;

    const KeyCode indexUp       = KeyCode.UpArrow;
    const KeyCode indexDown     = KeyCode.DownArrow;
    const Ease    MOVEMENT_TYPE = Ease.OutSine;

    Buttons select = Buttons.START;

    Tween currentEffect;
    Buttons beforeEffectTarget = Buttons.NONE;

    void UneffectSelect() {

        if (beforeEffectTarget == Buttons.NONE)
            return;

        var target = buttons[Convert.ToInt32(beforeEffectTarget)];
        var color = texts[Convert.ToInt32(beforeEffectTarget)];

        currentEffect.Kill();
        target.transform.DOScale(1 / SELECT_SIZE, 0);
        color.color = Color.white;

    }

    void EffectSelect(Buttons select) {

        if (select == Buttons.NONE) {

            return;
        }

        GameObject target = buttons[Convert.ToInt32(select)];

        UneffectSelect();

        beforeEffectTarget = select;

        currentEffect = target.transform.DOScale(SELECT_SIZE, EFFECT_DURATION).SetEase(MOVEMENT_TYPE);

        texts[Convert.ToInt32(select)].color = Color.red;

    }

    Buttons CheckInput() {
        if (Input.GetKeyDown(indexDown)) {

            var temp = (Buttons)(Convert.ToInt32(select + 1) % Convert.ToInt32(Buttons.COUNT));

            select = temp;

            return select;
        }

        if (Input.GetKeyDown(indexUp)) {

            currentEffect.Kill();

            var temp = Convert.ToInt32(select - 1);

            if (temp == -1) {
                select = Buttons.COUNT - 1;
            }

            else {
                select = (Buttons)temp;
            }

            return select;
        }

        return Buttons.NONE;
    }

    private void Awake() {
        
        for(int i = 0, size = buttons.Length; i < size; i++) {

            texts[i] = buttons[i].GetComponent<TMP_Text>();
        }
    }

    private void Update()
    {
        var select = CheckInput();
        EffectSelect(select);
    }
}
