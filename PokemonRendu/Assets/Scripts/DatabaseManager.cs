using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static PokeAPI;
using static PokeAPISpecies;

public class DatabaseManager : MonoBehaviour
{
    [SerializeField] private PokemonInfoController controller;
    public PokemonDatabase database;
    public PokemonData GetData(int id) => database.datas[id];

    private PokemonData.Stats stats;
    public int id;
    private int currentActive = 0;
    public void Awake()
    {
        //  database.InitData();
        if (database.datas.Count == 0)
        {
            CreateData(30);
        }
        //  StartCoroutine(SortDB());
        //CreateData(id);
    }

    IEnumerator MassCreateData(int amount)
    {
        for (int i = 0; i < amount / 50; i++)
        {
            yield return StartCoroutine(GetXPokemon(i * 50));
            yield return new WaitForSeconds(1);
        }
    }
    public void CreateData(int amount)
    {

        StartCoroutine(GetXPokemon(amount));
    }

    IEnumerator SortDB()
    {
        database.datas.Sort((x, y) => x.number.CompareTo(y.number));
        yield return null;
    }

    IEnumerator SetLoadBar(int amount)
    {
        controller.LoadBar(amount);
        yield return null;
    }

    IEnumerator GetXPokemon(int amount)
    {
        int stopper = 0;
        for (int i = 1; i < amount + 1; i++)
        {
            if (i > database.datas.Count)
            {
                StartCoroutine(GetRequest(i, $"https://pokeapi.co/api/v2/pokemon/{i}"));
                StartCoroutine(SetLoadBar(amount));

                stopper++;
                
                if (currentActive > 50)
                {
                    yield return new WaitUntil(()=>(currentActive<50));
                    stopper = 0;
                }

            }
        }
        yield return StartCoroutine(SortDB());
    }
    IEnumerator GetRequest(int id, string uri)
    {
        currentActive++;
        string pokename = "Loading";
        float weight = 0;
        float size = 0;
        string talent1 = "";
        string talent2 = "";
        string description = "No pokedex entry available.";
        string category = "No genus.";
        string type1 = "";
        string type2 = "";
        string evolutionurl = "";
        string frontsprite = "";
        string backsprite = "";

        int pv = 0;
        int atk = 0;
        int def = 0;
        int atkSpe = 0;
        int defSpe = 0;
        int speed = 0;
        List<PokemonInfoController.Damage> damagerelations = new();
        database.datas.Add(new PokemonData(pokename, weight, size, talent1, talent2, description, category, type1, type2, id,
                                  stats = new PokemonData.Stats(pv, atk, def, atkSpe, defSpe, speed), evolutionurl, frontsprite, backsprite,damagerelations

        ));

        UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        {
            yield
            return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(String.Format("Something went wrong: {0}", webRequest.error));
                    break;
                case UnityWebRequest.Result.Success:
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
                    Root pokemon = JsonConvert.DeserializeObject<Root>(webRequest.downloadHandler.text, settings);
                    pokename = pokemon.species.name;
                    pokename = (char.ToUpper(pokename[0]) + pokename.Substring(1));
                    size = pokemon.height;
                    weight = pokemon.weight;
                    if (id < 650)
                    {
                        frontsprite = pokemon.sprites.versions.generationv.blackwhite.animated.front_default;

                        backsprite = pokemon.sprites.versions.generationv.blackwhite.animated.back_default;

                    }
                    else
                    {
                        frontsprite = pokemon.sprites.front_default;
                        backsprite = pokemon.sprites.back_default;
                    }

                    talent1 = pokemon.abilities[0].ability.name;
                    talent1 = (char.ToUpper(talent1[0]) + talent1.Substring(1));
                    if (pokemon.abilities.Count > 1)
                    {
                        
                        talent2 = pokemon.abilities[1].ability.name;
                        talent2 = (char.ToUpper(talent2[0]) + talent2.Substring(1));
                        if(talent1 == talent2)
                        {
                            talent2 = "";
                        }
                    }
                    type1 = pokemon.types[0].type.name;
                    List<String> typeList = new List<String>();
                    typeList.Add(type1);
                    if (pokemon.types.Count > 1)
                    {
                        type2 = pokemon.types[1].type.name;
                        typeList.Add(type2);
                    }
                    pv = pokemon.stats[0].base_stat;
                    atk = pokemon.stats[1].base_stat;
                    def = pokemon.stats[2].base_stat;
                    atkSpe = pokemon.stats[3].base_stat;
                    defSpe = pokemon.stats[4].base_stat;
                    speed = pokemon.stats[5].base_stat;

                    using (UnityWebRequest flavor = UnityWebRequest.Get($"https://pokeapi.co/api/v2/pokemon-species/{id}"))
                    {
                        yield
                        return flavor.SendWebRequest();

                        switch (flavor.result)
                        {
                            case UnityWebRequest.Result.ConnectionError:
                            case UnityWebRequest.Result.DataProcessingError:
                                Debug.Log(String.Format("Something went wrong :  {0}", webRequest.error));
                                break;
                            case UnityWebRequest.Result.Success:
                                currentActive--;
                                PokeSpecies species = JsonConvert.DeserializeObject<PokeSpecies>(flavor.downloadHandler.text, settings);
                                string flavortext = "";
                                species.flavor_text_entries.ForEach((entry) =>
                                {
                                    if (entry.language.name == "en")
                                    {
                                        flavortext = entry.flavor_text;
                                    }
                                });
                                if(flavortext != "")
                                {
                                    flavortext = flavortext.Replace("\n", " ");
                                    flavortext = flavortext.Replace("\f", " ");
                                    flavortext = flavortext.Replace("—", "-");
                                    description = flavortext;
                                }

                                species.genera.ForEach((entry) =>
                                {
                                    if (entry.language.name == "en")
                                    {
                                        category = entry.genus;
                                    }
                                });
                                evolutionurl = species.evolution_chain.url;
                                database.datas[id - 1] = new PokemonData(
                                  pokename, weight, size, talent1, talent2, description, category, type1, type2, id,
                                  stats = new PokemonData.Stats(pv, atk, def, atkSpe, defSpe, speed), evolutionurl, frontsprite, backsprite,damagerelations

                                );
                                break;
                        }
                    }
                    break;

            }

        }
    }

}