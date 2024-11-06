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

    /// <summary>
    /// <para>VerticalShake</para>
    /// It shake object's vertical position
    /// <para><paramref name="duraction"/>: Time of showing animation</para>
    /// 
    /// <paramref name="power"/>: Animation dynamic poer
    /// </summary>
    public void VShake(float power, float duraction) {

        Vector3 origin = transform.localPosition;

        if (current == null || !current.IsActive()) {

            current = transform.DOShakePosition(duraction, Vector3.up * power)
                .OnComplete(() => transform.localPosition = origin);
        }
    }

    /// <summary>
    /// <para>HorizontalShake</para>
    /// It shake object's Horizontal position
    /// <para><paramref name="duraction"/>: Time of showing animation</para>
    /// 
    /// <paramref name="power"/>: Animation dynamic poer
    /// </summary>
    public void HShake(float power, float duraction) {

        Vector3 origin = transform.localPosition;

        if (current == null || !current.IsActive()) {

            current = transform.DOShakePosition(duraction, Vector3.right * power)
                .OnComplete(() => transform.localPosition = origin);
        }
    }
}
