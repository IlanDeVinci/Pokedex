using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;
using UnityEngine.UI;
public class BackgroundSpin : MonoBehaviour
{
    private Tween tween;
    // Start is called before the first frame update
    void Start()
    {

        Quaternion quaternion1 = Quaternion.Euler(0,0,180);
        Quaternion quaternion2 = Quaternion.Euler(0, 0, 360);
        Sequence.Create(cycles: -1, cycleMode: CycleMode.Restart)
            .Chain(Tween.Rotation(target: transform, startValue: Quaternion.identity, duration: 10, ease: Ease.Linear, endValue: quaternion1, cycles: 1))
            .Chain(Tween.Rotation(target: transform, startValue: quaternion1, duration: 10, ease: Ease.Linear, endValue: quaternion2, cycles: 1));
        //tween = Tween.Rotation(target:transform, startValue:Quaternion.identity, duration: 1, ease:Ease.Linear, endValue:quaternion1, cycles:-1, cycleMode:CycleMode.Restart);
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        tween.Stop();
    }
}
