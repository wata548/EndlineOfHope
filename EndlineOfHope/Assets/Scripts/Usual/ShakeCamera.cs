using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera: MonoBehaviour
{
    public static ShakeCamera Instance { get; private set; } = null;

    Tween current = null;

    private void Awake() {
        
        if(Instance == null) {
            Instance = this;
        }
    }

    public void Shake(float power, float duraction) {

        Vector3 origin = transform.localPosition;

        if ( current == null || !current.IsActive()) {

            current = transform.DOShakePosition(duraction, power)
                .OnComplete(() => transform.localPosition = origin);
        }
    } 
}
