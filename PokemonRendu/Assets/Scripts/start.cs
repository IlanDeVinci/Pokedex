using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PrimeTween;
using UnityEngine.UI;

public class start : MonoBehaviour
{
    [SerializeField] private Image fade;
    // Start is called before the
    // first frame update

    public void StartGame()
    {
        StartCoroutine(StartFade());
    }
    private IEnumerator StartFade()
    {
        Tween tween = Tween.Alpha(fade, 1, 1);
        yield return new WaitUntil(() => !tween.isAlive);
        SceneManager.LoadSceneAsync("PokeInfo");

    }
}
