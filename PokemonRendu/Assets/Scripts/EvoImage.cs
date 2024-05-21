using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvoObject: MonoBehaviour
{
        public string evoname;
        public int id;
        public Image image;
        public TextMeshProUGUI textMeshProUGUI;
        public GameObject evoGameObject;


        public EvoObject(string evoname, int id, Image image, TextMeshProUGUI textMeshProUGUI, GameObject evoGameObject)
        {
            this.evoname = evoname;
            this.id = id;
            this.image = image;
            this.textMeshProUGUI = textMeshProUGUI;
            this.evoGameObject = evoGameObject;
        }
        public void GetEvoPoke()
        {
            GameManagerStatic.pokemonInfoController.id = id;
            GameManagerStatic.pokemonInfoController.RefreshPokemon();
        }
}

