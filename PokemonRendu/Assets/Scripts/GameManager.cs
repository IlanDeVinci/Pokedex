using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManagerStatic
{
    public static PokemonInfoController pokemonInfoController;
    public static PokemonData firstPokemon;
    public static PokemonData secondPokemon;
    public static int firstPokemonlvl = 50;
    public static int secondPokemonlvl = 50;
    public static List<PokemonInfoController.Damage> firstPokemonDamageRelation = new();
    public static List<PokemonInfoController.Damage> secondPokemonDamageRelation = new();

}
