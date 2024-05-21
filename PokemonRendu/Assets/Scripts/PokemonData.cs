using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PokemonData
{
    [Serializable]
    public struct Stats
    {
        public int pv;
        public int atk;
        public int def;
        public int atkSpe;
        public int defSpe;
        public int speed;

        public Stats(int pv, int atk, int def, int atkSpe, int defSpe, int speed)
        {
            this.pv=pv; this.atk = atk; this.def = def; this.atkSpe = atkSpe; this.defSpe = defSpe; this.speed = speed; 
        }
    }

    public struct Moves
    {
        List<Move> moves;

        public Moves(List<Move> moves)
        {
            this.moves = moves;
        }
    }
    public struct StatChange
    {
        public string name;
        public int amount;
        public StatChange(string name, int amount)
        {
            this.name=name; this.amount=amount;
        }
    }

        public struct MoveMeta
    {
        public string ailment;
        public string category;
        public int? minhits;
        public int? maxhits;
        public int? maxturns;
        public int drain;
        public int healing;
        public int critrate;
        public int ailmentchance;
        public int flinchchance;
        public int statchance;

        public MoveMeta(string ailment, string category, int? minhits, int? maxhits, int? maxturns, int drain, int healing, int critrate, int ailmentchance, int flinchchance, int statchance)
        {
            this.ailment = ailment;
            this.category = category;
            this.minhits = minhits;
            this.maxhits = maxhits;
            this.maxturns = maxturns;
            this.drain = drain;
            this.healing = healing;
            this.critrate = critrate;
            this.ailmentchance = ailmentchance;
            this.flinchchance = flinchchance;
            this.statchance = statchance;
        }
    }

        public struct Move
    {
        public string name;
        public int accuracy;
        public int? effect_chance;
        public int pp;
        public string damage_class;
        public int priority;
        public int power;
        public string type;
        public StatChange[] statchanges;
        public MoveMeta meta;

        public Move(string name, int accuracy, int? effect_chance, int pp, string damage_class, int priority, int power, string type, StatChange[] statchanges, MoveMeta meta)
        {
            this.name = name;
            this.accuracy = accuracy;
            this.effect_chance = effect_chance;
            this.pp = pp;
            this.damage_class = damage_class;
            this.priority = priority;
            this.power = power;
            this.type = type;
            this.statchanges = statchanges;
            this.meta = meta;
        }

    }

    public Stats stats;
    public string pokename;
    public float weight;
    public float size;
    public string talent1;
    public string talent2;
    public string description;
    public string category;
    public string type1;
    public string type2;
    public int number;
    public string evolutionurl;
    public string frontspriteurl;
    public string backspriteurl;
    public List<PokemonInfoController.Damage> damagerelations;
    public Moves moves;
    public PokemonData()
    {

    }
    public PokemonData(string pokename, float weight, float size, string talent1, string talent2, string description, string category, string type1, string type2, int number, Stats stats, string evolutionurl, string frontspriteurl, string backspriteurl, List<PokemonInfoController.Damage> damagerelations)
    {
        this.pokename = pokename;
        this.weight = weight;
        this.size = size;
        this.talent1 = talent1;
        this.talent2 = talent2;
        this.description = description;
        this.category = category;
        this.type1 = type1;
        this.type2 = type2;
        this.number = number;
        this.stats = stats;
        this.evolutionurl = evolutionurl;
        this.frontspriteurl = frontspriteurl;
        this.backspriteurl = backspriteurl;
        this.damagerelations = damagerelations;
    }
}
