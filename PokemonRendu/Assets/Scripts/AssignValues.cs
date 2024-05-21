using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AssignValues : MonoBehaviour
{
    [SerializeField] PokemonObj pokemon1;
    [SerializeField] PokemonObj pokemon2;
    [SerializeField] private float reloadtime = 0.5f;
    [SerializeField] Image fade;
    private float lastreload = 0;
    private void Awake()
    {
        pokemon1.pokenum = 1;
        pokemon2.pokenum = 2;
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }
    public void GobBack()
    {
        SceneManager.LoadSceneAsync("PokeInfo");
    }
        
    public void SwapPoke()
    {
        if(lastreload + reloadtime < Time.time)
        {
            if (pokemon1.canSwap && pokemon2.canSwap)
            {
                lastreload = Time.time;
                StartCoroutine(PokeSwap());
            }
        }
       

    }

    private IEnumerator FadeIn()
    {
        fade.gameObject.SetActive(true);
        for(float i = 1; i >=0; i -= Time.deltaTime/2) 
        {
            fade.color = new Color(0, 0, 0, i);
            yield return null;
        }
        fade.gameObject.SetActive(false);
    }

    private IEnumerator PokeSwap()
    {
        Image image = pokemon1.image;
        TextMeshProUGUI currenthpText = pokemon1.currenthpText;
        TextMeshProUGUI maxHpText = pokemon1.maxHpText;
        TextMeshProUGUI nameText = pokemon1.nameText;
        TextMeshProUGUI damageText = pokemon1.damageText;
        Slider HealthBar = pokemon1.HealthBar;
        Image HPColor = pokemon1.HPColor;
        int pokenum = pokemon1.pokenum;
        TextMeshProUGUI level = pokemon1.levelText;
        Image status = pokemon1.statusImg;

        pokemon1.image = pokemon2.image;
        pokemon1.HPColor = pokemon2.HPColor;
        pokemon1.currenthpText = pokemon2.currenthpText;
        pokemon1.maxHpText = pokemon2.maxHpText;
        pokemon1.nameText = pokemon2.nameText;
        pokemon1.damageText = pokemon2.damageText;
        pokemon1.HealthBar = pokemon2.HealthBar;
        pokemon1.pokenum = pokemon2.pokenum;
        pokemon1.levelText = pokemon2.levelText;
        pokemon1.statusImg = pokemon2.statusImg;

        pokemon2.image = image;
        pokemon2.levelText = level; 
        pokemon2.HPColor = HPColor;
        pokemon2.currenthpText = currenthpText;
        pokemon2.maxHpText = maxHpText;
        pokemon2.nameText = nameText;
        pokemon2.damageText = damageText;
        pokemon2.HealthBar = HealthBar;
        pokemon2.pokenum = pokenum;
        pokemon2.statusImg = status;
        pokemon1.Reload();
        pokemon2.Reload();
        yield return null;
    }

}
