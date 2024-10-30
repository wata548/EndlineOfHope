using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISelect : MonoBehaviour {

    const int   BUTTON_INDEX    = 3;
    const float SELECT_SIZE     = 1.2f;
    const float EFFECT_DURATION = 0.2f;

    [SerializeField] GameObject[] buttons = new GameObject[BUTTON_INDEX];
    TMP_Text[] texts = new TMP_Text[BUTTON_INDEX];
    GameObject[] textsObject = new GameObject[BUTTON_INDEX];

    const KeyCode indexUp       = KeyCode.UpArrow;
    const KeyCode indexDown     = KeyCode.DownArrow;
    const Ease    MOVEMENT_TYPE = Ease.OutSine;

    ObserverData.ButtonIndex select = ObserverData.ButtonIndex.START;

    Tween currentEffect;
    Vector2 beforeScale;
    ObserverData.ButtonIndex beforeEffectTarget = ObserverData.ButtonIndex.NONE;

    void UneffectSelect() {

        if (beforeEffectTarget == ObserverData.ButtonIndex.NONE)
            return;

        var target = textsObject[Convert.ToInt32(beforeEffectTarget)];
        var color = texts[Convert.ToInt32(beforeEffectTarget)];

        currentEffect.Kill();
        target.transform.DOScale(beforeScale, 0);
        color.color = Color.white;

    }

    void EffectSelect(ObserverData.ButtonIndex select) {

        if (select == ObserverData.ButtonIndex.NONE) {

            return;
        }

        UneffectSelect();

        GameObject target = textsObject[Convert.ToInt32(select)];
        
        beforeEffectTarget = select;
        beforeScale = target.transform.localScale;
        currentEffect = target.transform.DOScale(beforeScale * SELECT_SIZE, EFFECT_DURATION).SetEase(MOVEMENT_TYPE);
        texts[Convert.ToInt32(select)].color = Color.red;

    }

    ObserverData.ButtonIndex CheckInput() {

        if (Input.GetKeyDown(indexDown)) {

            var temp = (ObserverData.ButtonIndex)(Convert.ToInt32(select + 1) % Convert.ToInt32(ObserverData.ButtonIndex.COUNT));

            select = temp;

            return select;
        }

        if (Input.GetKeyDown(indexUp)) {

            var temp = Convert.ToInt32(select - 1);

            if (temp == -1) {
                select = ObserverData.ButtonIndex.COUNT - 1;
            }

            else {
                select = (ObserverData.ButtonIndex)temp;
            }

            return select;
        }

        return ObserverData.ButtonIndex.NONE;
    }

    private void Awake() {
        
        for(int i = 0, size = buttons.Length; i < size; i++) {

            texts[i] = buttons[i].GetComponentInChildren<TMP_Text>();
            textsObject[i] = buttons[i].transform.GetChild(0).gameObject;
        }
    }

    private void Update() {
        var select = CheckInput();
        EffectSelect(select);

        if(ObserverData.ChangeSelect) {
            ObserverData.CheckEffect();
            EffectSelect(this.select = ObserverData.CurrentSelect);
        }

    }

}
