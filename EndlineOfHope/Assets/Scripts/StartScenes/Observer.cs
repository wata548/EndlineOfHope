using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObserverData: MonoBehaviour {

    public static bool SelecteChange { get; protected set; } = false;
    public static ButtonIndex CurrentSelect { get; protected set; } = ButtonIndex.NONE;

    [Serializable]
    public enum ButtonIndex {

        NONE = -1,
        START,
        SETTING,
        EXIT,
        COUNT
    };

    public static void CheckEffect() {

        SelecteChange = false;
    }
}

public class Observer : ObserverData, IPointerEnterHandler {

    [SerializeField] ButtonIndex buttonType = ButtonIndex.START;
    
    public void OnPointerEnter(PointerEventData evect) {

        if(CurrentSelect != buttonType) {

            CurrentSelect   = buttonType;
            SelecteChange    = true;
        }
    }
    
}
