using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveData
{

    public class Move
    {
        public string name;
        public int? accuracy;
        public int? effect_chance;
        public int? pp;
        public string damage_class;
        public int? priority;
        public int? power;
        public string type;
        public List<StatChanges> statchanges;
        public MoveMeta meta;

        public Move(string name, int? accuracy, int? effect_chance, int? pp, string damage_class, int? priority, int? power, string type, List<StatChanges> statchanges, MoveMeta meta)
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
        public struct StatChanges
    {
        public string name;
        public int amount;
        public StatChanges(string name, int amount)
        {
            this.name = name; this.amount = amount;
        }
    }

    public struct MoveMeta
    {
        public string ailment;
        public string category;
        public int? minhits;
        public int? maxhits;
        public int? minturns;
        public int? maxturns;
        public int? drain;
        public int? healing;
        public int? critrate;
        public int? ailmentchance;
        public int? flinchchance;
        public int? statchance;

        public MoveMeta(string ailment, string category, int? minhits, int? maxhits, int? minturns, int? maxturns, int? drain, int? healing, int? critrate, int? ailmentchance, int? flinchchance, int? statchance)
        {
            this.ailment = ailment;
            this.category = category;
            this.minhits = minhits;
            this.maxhits = maxhits;
            this.minturns = minturns;
            this.maxturns = maxturns;
            this.drain = drain;
            this.healing = healing;
            this.critrate = critrate;
            this.ailmentchance = ailmentchance;
            this.flinchchance = flinchchance;
            this.statchance = statchance;
        }
    }


}
