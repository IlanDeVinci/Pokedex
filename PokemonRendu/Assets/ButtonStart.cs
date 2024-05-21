using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonStart : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image image;
    Tween tweenColor;
    Tween tweenScale;

    [SerializeField] TweenSettings<Vector3> settings;
    public void OnPointerEnter(PointerEventData eventData)
    {
        tweenColor.Stop();
        tweenScale = Tween.Scale(transform, settings);
        tweenColor = Tween.Color(image, Color.grey, duration: 0.25f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tweenColor.Stop();
        tweenScale = Tween.Scale(transform, settings.WithDirection(toEndValue: false));
        tweenColor = Tween.Color(image, Color.white, duration: 0.5f);

    }

    private void OnDestroy()
    {
        tweenColor.Stop();
        tweenScale.Stop();

    }

}
