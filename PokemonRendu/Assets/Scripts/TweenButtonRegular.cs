using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TweenButtonRegular : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Tween tweenScale;

    private void Awake()
    {
        PrimeTweenConfig.warnTweenOnDisabledTarget = false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        tweenScale = Tween.Scale(transform, 1.1f, 1, ease: Ease.OutElastic);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tweenScale = Tween.Scale(transform, 1, 1, ease: Ease.OutElastic);

    }

    private void OnDestroy()
    {
        tweenScale.Stop();

    }

}