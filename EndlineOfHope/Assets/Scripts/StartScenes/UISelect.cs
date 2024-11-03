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

//==================================================| Set on Heararcy

    [SerializeField] GameObject[] buttons = new GameObject[BUTTON_COUNT];

    //* It change color
    TMP_Text[] texts = new TMP_Text[BUTTON_COUNT];
    //* It change size
    GameObject[] textsObject = new GameObject[BUTTON_COUNT];

//==================================================| Default Values

    const int   BUTTON_COUNT     = 3;
    const float SELECT_SIZE      = 1.2f;
    const float EFFECT_DURATION  = 0.2f;
    const KeyCode indexUp        = KeyCode.UpArrow;
    const KeyCode indexDown      = KeyCode.DownArrow;
    const Ease    MOVEMENT_TYPE  = Ease.OutSine;
                                 
    readonly Color DISABLE_COLOR = new Color(0.62f, 0.62f, 0.62f);
    readonly Color ENABLE_COLOR  = Color.white;

//==================================================| Fields

    ObserverData.ButtonIndex select = ObserverData.ButtonIndex.START;

    Tween currentEffect;
    Vector2 beforeScale;
    ObserverData.ButtonIndex beforeEffectTarget = ObserverData.ButtonIndex.NONE;

//==================================================| Method

    #region Method

    void CanceleffectSelect() {

        if (beforeEffectTarget == ObserverData.ButtonIndex.NONE)
            return;

        var target = textsObject[Convert.ToInt32(beforeEffectTarget)];
        var color = texts[Convert.ToInt32(beforeEffectTarget)];

        currentEffect.Kill();
        target.transform.DOScale(beforeScale, 0);
        color.color = DISABLE_COLOR;

    }

    void EffectSelect(ObserverData.ButtonIndex select) {

        if (select == ObserverData.ButtonIndex.NONE) {

            return;
        }

        CanceleffectSelect();

        GameObject target = textsObject[Convert.ToInt32(select)];
        
        beforeEffectTarget                      = select;
        beforeScale                             = target.transform.localScale;
        currentEffect                           = target.transform.DOScale(beforeScale * SELECT_SIZE, EFFECT_DURATION).SetEase(MOVEMENT_TYPE);
        texts[Convert.ToInt32(select)].color    = ENABLE_COLOR;

    }

    ObserverData.ButtonIndex CheckInput() {

        if (Input.GetKeyDown(indexDown)) {

            var next = (ObserverData.ButtonIndex)(Convert.ToInt32(select + 1) % Convert.ToInt32(ObserverData.ButtonIndex.COUNT));

            select = next;

            return select;
        }

        if (Input.GetKeyDown(indexUp)) {

            var next = Convert.ToInt32(select - 1);

            if (next == -1) {
                select = ObserverData.ButtonIndex.COUNT - 1;
            }

            else {
                select = (ObserverData.ButtonIndex)next;
            }

            return select;
        }

        return ObserverData.ButtonIndex.NONE;
    }

    #endregion

//==================================================| Logic 

    #region Logic

    private void Awake() {
        
        for(int i = 0, size = buttons.Length; i < size; i++) {

            texts[i] = buttons[i].GetComponentInChildren<TMP_Text>();
            textsObject[i] = buttons[i].transform.GetChild(0).gameObject;
        }
    }


    private void Update() {

        var select = CheckInput();
        EffectSelect(select);

        if(ObserverData.SelecteChange) {

            ObserverData.CheckEffect();
            EffectSelect(this.select = ObserverData.CurrentSelect);
        }

    }

    #endregion

}



