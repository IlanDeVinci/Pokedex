using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using static PokeAPI;
using static PokeAPISpecies;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Database/Pokemon", order = 0)]
public class PokemonDatabase : ScriptableObject
{
    public List<PokemonData> datas = new();

    public void InitData()
    {
        datas.Clear();
    }

}
