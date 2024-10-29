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

    const int   BUTTON_INDEX    = 3;
    const float SELECT_SIZE     = 1.2f;
    const float EFFECT_DURATION = 0.2f;

    enum ButtonIndex {

        NONE = -1,
        START,
        SETTING,
        EXIT,
        COUNT

    };
    [SerializeField] GameObject[] buttons = new GameObject[BUTTON_INDEX];
    TMP_Text[] texts = new TMP_Text[BUTTON_INDEX];
    GameObject[] textsObject = new GameObject[BUTTON_INDEX];

    const KeyCode indexUp       = KeyCode.UpArrow;
    const KeyCode indexDown     = KeyCode.DownArrow;
    const Ease    MOVEMENT_TYPE = Ease.OutSine;

    ButtonIndex select = ButtonIndex.START;

    Tween currentEffect;
    Vector2 beforeScale;
    ButtonIndex beforeEffectTarget = ButtonIndex.NONE;

    void UneffectSelect() {

        if (beforeEffectTarget == ButtonIndex.NONE)
            return;

        var target = textsObject[Convert.ToInt32(beforeEffectTarget)];
        var color = texts[Convert.ToInt32(beforeEffectTarget)];

        currentEffect.Kill();
        target.transform.DOScale(beforeScale, 0);
        color.color = Color.white;

    }

    void EffectSelect(ButtonIndex select) {

        if (select == ButtonIndex.NONE) {

            return;
        }

        UneffectSelect();

        GameObject target = textsObject[Convert.ToInt32(select)];
        
        beforeEffectTarget = select;
        beforeScale = target.transform.localScale;
        currentEffect = target.transform.DOScale(beforeScale * SELECT_SIZE, EFFECT_DURATION).SetEase(MOVEMENT_TYPE);
        texts[Convert.ToInt32(select)].color = Color.red;

    }

    ButtonIndex CheckInput() {

        if (Input.GetKeyDown(indexDown)) {

            var temp = (ButtonIndex)(Convert.ToInt32(select + 1) % Convert.ToInt32(ButtonIndex.COUNT));

            select = temp;

            return select;
        }

        if (Input.GetKeyDown(indexUp)) {

            var temp = Convert.ToInt32(select - 1);

            if (temp == -1) {
                select = ButtonIndex.COUNT - 1;
            }

            else {
                select = (ButtonIndex)temp;
            }

            return select;
        }

        return ButtonIndex.NONE;
    }

    private void Awake() {
        
        for(int i = 0, size = buttons.Length; i < size; i++) {

            texts[i] = buttons[i].GetComponentInChildren<TMP_Text>();
            textsObject[i] = buttons[i].transform.GetChild(0).gameObject;
        }
    }

    private void Update()
    {
        var select = CheckInput();
        EffectSelect(select);
    }
}
