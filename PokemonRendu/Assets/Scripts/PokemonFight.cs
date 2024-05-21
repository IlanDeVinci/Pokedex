using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using ThreeDISevenZeroR.UnityGifDecoder;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static MoveData;
using static PokeAPI;
using static PokeAPIMove;
using PrimeTween;
using Random = System.Random;

[Serializable]
public class PokemonObj : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI currenthpText;
    [SerializeField] public TextMeshProUGUI maxHpText;

    [SerializeField] public TextMeshProUGUI nameText;
    [SerializeField] public TextMeshProUGUI damageText;

    [SerializeField] public Slider HealthBar;
    [SerializeField] public Image image;
    [SerializeField] private PokemonObj PokemonOpponent;
    [SerializeField] private Gradient gradient;
    [SerializeField] public Image HPColor;
    [SerializeField] public TextMeshProUGUI levelText;
    [SerializeField] public Image statusImg;
    [SerializeField] private Sprite[] statusIcons;
    [SerializeField] private Slider LoadBar;
    [SerializeField] public Image confusionImg;

    private PokemonData pokemondata;
    public int pokenum;
    private List<Texture2D> framesFront = new List<Texture2D>();
    private List<Texture2D> framesBack = new List<Texture2D>();
    private int gifsLoaded = 0;
    public bool isReloading = true;
    private IEnumerator coroutine;
    private List<MoveData.Move> moves = new List<MoveData.Move>();

    private List<MoveData.Move> allmoves = new List<MoveData.Move>();
    private MoveData.Move moveUsed;
    private List<MoveData.Move> attackMoves = new();
    private List<MoveData.Move> statusMoves = new();
    private List<MoveData.Move> otherMoves = new();

    private Status status = Status.none;
    private int previousHp;
    private int statusTimer;
    public bool canSwap = true;
    private float speedMultiplier = 1;
    private List<SecondaryStatus> secondaryStatuses = new List<SecondaryStatus>();
    private List<int> secondaryStatusesTimer = new List<int>();
    public StatModifier[] statModifiers = {new StatModifier("atk",0), new StatModifier("def", 0), new StatModifier("atkspe", 0), new StatModifier("defspe", 0), new StatModifier("speed", 0), new StatModifier("accuracy", 0), new StatModifier("evasion", 0) };
    private int[] baseStats = new int[5];



    public class StatModifier
    {
        public string stat;
        public int modifier;

        public StatModifier(string stat, int modifier)
        {
            this.stat = stat;
            this.modifier = modifier;
        }
    }

    [SerializeField] private PokeFightStats pokeFightStats;

    public struct PokeFightStats
    {
        [SerializeField] public string pokename;
        [SerializeField] public int maxhealth;
        [SerializeField] public int atk;
        [SerializeField] public int def;
        [SerializeField] public int atkspe;
        [SerializeField] public int defspe;
        [SerializeField] public int speed;
        [SerializeField] public int level;
        [SerializeField] public List<PokemonInfoController.Damage> damagerelations;

        public int currenthealth;
        public int statpoints;

        public PokeFightStats(string pokename, int maxhealth, int atk, int def, int atkspe, int defspe, int speed, int level, List<PokemonInfoController.Damage> damagerelations, int statpoints)
        {
            this.atk = atk;
            this.maxhealth = maxhealth;
            this.def = def;
            this.atkspe = atkspe;
            this.defspe = defspe;
            this.speed = speed;
            this.level = level;
            this.damagerelations = damagerelations;
            this.statpoints = statpoints;
            this.pokename = pokename;
            this.currenthealth = maxhealth;
        }

    }

    [SerializeField]

    public enum Status
    {
        none,
        burn,
        freeze,
        poison,
        paralysis,
        sleep

    }

    public enum SecondaryStatus
    {
        confusion
    }
    public enum Type
    {
        bug,
        dark,
        dragon,
        electric,
        fairy,
        fighting,
        fire,
        flying,
        ghost,
        grass,
        ground,
        ice,
        normal,
        poison,
        psychic,
        rock,
        steel,
        water
    }

    public void Awake()
    {
        HealthBar.value = 0;
        framesBack.Clear();
        framesFront.Clear();
        gifsLoaded = 0;
        if (pokenum == 1)
        {
            pokemondata = GameManagerStatic.firstPokemon;
            int statpoint = pokemondata.stats.atk + pokemondata.stats.atkSpe + pokemondata.stats.speed + pokemondata.stats.pv + pokemondata.stats.def + pokemondata.stats.defSpe;
            pokeFightStats = new PokeFightStats(pokemondata.pokename, pokemondata.stats.pv, pokemondata.stats.atk, pokemondata.stats.def, pokemondata.stats.atkSpe, pokemondata.stats.defSpe, pokemondata.stats.speed, GameManagerStatic.firstPokemonlvl, GameManagerStatic.firstPokemonDamageRelation, statpoint);
        }
        else
        {
            pokemondata = GameManagerStatic.secondPokemon;
            int statpoint = pokemondata.stats.atk + pokemondata.stats.atkSpe + pokemondata.stats.speed + pokemondata.stats.pv + pokemondata.stats.def + pokemondata.stats.defSpe;
            pokeFightStats = new PokeFightStats(pokemondata.pokename, pokemondata.stats.pv, pokemondata.stats.atk, pokemondata.stats.def, pokemondata.stats.atkSpe, pokemondata.stats.defSpe, pokemondata.stats.speed, GameManagerStatic.secondPokemonlvl, GameManagerStatic.secondPokemonDamageRelation, statpoint);

        }
        if (pokemondata.number < 649)
        {
            StartCoroutine(GifCoroutine(pokemondata.backspriteurl, framesBack));
            StartCoroutine(GifCoroutine(pokemondata.frontspriteurl, framesFront));
        }
        baseStats[0] = pokeFightStats.atk;
        baseStats[1] = pokeFightStats.def;
        baseStats[2] = pokeFightStats.atkspe;
        baseStats[3] = pokeFightStats.defspe;
        baseStats[4] = pokeFightStats.speed;

        StartCoroutine(getMovesList());
        InitCurrentLife(pokeFightStats);
        InitStatPoints(pokeFightStats);
        Reload();
    }

    public void Reload()
    {
        if (pokeFightStats.currenthealth <= 0)
        {
            previousHp = 0;
            HealthBar.value = 0;
            canSwap = false;
        }
        isReloading = true;
        UpdateHealthBar();
        UpdateHealthText();
        UpdateStatus();
        nameText.text = pokemondata.pokename;
        levelText.text = pokeFightStats.level.ToString();
        //status.sprite = status;
        if (pokemondata.number > 649)
        {
            if (pokenum == 1)
            {
                if (pokemondata.backspriteurl != "")
                {
                    StartCoroutine(LoadImage(image, pokemondata.backspriteurl));
                }
                else
                {
                    StartCoroutine(LoadImage(image, pokemondata.frontspriteurl));
                }
            }
            else
            {

                StartCoroutine(LoadImage(image, pokemondata.frontspriteurl));

            }
        }
        else
        {
            StartCoroutine(AnimationManager());

        }
    }

    public void UpdateStatus()
    {
        switch (status)
        {
            case Status.none:
                speedMultiplier = 1f;
                statusImg.gameObject.SetActive(false);
                break;
            case Status.burn:
                speedMultiplier = 1f;
                statusImg.gameObject.SetActive(true);
                statusImg.sprite = statusIcons[0];
                break;
            case Status.freeze:
                speedMultiplier = 1f;
                statusImg.gameObject.SetActive(true);
                statusImg.sprite = statusIcons[1];
                break;
            case Status.paralysis:
                speedMultiplier = 0.25f;
                statusImg.gameObject.SetActive(true);
                statusImg.sprite = statusIcons[2];
                break;
            case Status.poison:
                speedMultiplier = 1f;
                statusImg.gameObject.SetActive(true);
                statusImg.sprite = statusIcons[3];
                break;
            case Status.sleep:
                speedMultiplier = 1f;
                statusImg.gameObject.SetActive(true);
                statusImg.sprite = statusIcons[4];
                break;

        }
    }

    private IEnumerator AnimationManager()
    {
        int num = pokenum;
        if (num == 1)
        {
            //frames = new List<Texture2D>(framesBack);
            coroutine = AnimateGif(framesBack);
        }
        else
        {
            //frames = new List<Texture2D>(framesFront);
            coroutine = AnimateGif(framesFront);

        }
        yield return new WaitUntil(() => gifsLoaded >= 2);
        StartCoroutine(coroutine);
        isReloading = false;
        yield return new WaitUntil(() => (num != pokenum));
        StopCoroutine(coroutine);
    }
    private IEnumerator GifCoroutine(string url, List<Texture2D> frames)
    {


        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();


        var gifStream = new GifStream(www.downloadHandler.data);

        while (gifStream.HasMoreData)
        {
            switch (gifStream.CurrentToken)
            {
                case GifStream.Token.Image:
                    var image = gifStream.ReadImage();
                    var frame = new Texture2D(
                        gifStream.Header.width,
                        gifStream.Header.height,
                        TextureFormat.ARGB32, false);

                    frame.SetPixels32(image.colors);
                    frame.Apply();

                    frames.Add(frame);
                    break;

                case GifStream.Token.Comment:
                    var commentText = gifStream.ReadComment();
                    Debug.Log(commentText);
                    break;

                default:
                    gifStream.SkipToken(); // Other tokens
                    break;
            }
        }
        yield return new WaitUntil(() => !gifStream.HasMoreData);
        gifsLoaded++;
    }

    private IEnumerator AnimateGif(List<Texture2D> frames)
    {
        int num = pokenum;


        while (num == pokenum)
        {

            for (int i = 0; i < frames.Count; i++)
            {
                image.rectTransform.sizeDelta = new Vector2(frames[0].width * 7, frames[0].height * 7);

                if (num == pokenum)
                {
                    if (frames.Count > 1)
                    {
                        frames[i].filterMode = FilterMode.Point;
                        Sprite newsprite = Sprite.Create(frames[i], new Rect(0, 0, frames[i].width, frames[i].height), Vector2.zero);
                        image.sprite = newsprite;
                        yield return new WaitForSeconds(0.1f);
                    }

                }
                else
                {
                    yield break;
                }


            }
        }


    }

    private void UpdateStats()
    {
        if (statModifiers[0].modifier >= 0)
        {
            pokeFightStats.atk = baseStats[0] * (statModifiers[0].modifier + 2) / 2;
        }
        else
        {
            pokeFightStats.atk = baseStats[0] * 2 / (statModifiers[0].modifier + 2);
        }
        if (statModifiers[1].modifier >= 0)
        {
            pokeFightStats.def = baseStats[1] * (statModifiers[1].modifier + 2) / 2;
        }
        else
        {
            pokeFightStats.def = baseStats[1] * 2 / (statModifiers[1].modifier + 2);
        }
        if (statModifiers[2].modifier >= 0)
        {
            pokeFightStats.atkspe = baseStats[2] * (statModifiers[2].modifier + 2) / 2;
        }
        else
        {
            pokeFightStats.atkspe = baseStats[2] * 2 / (statModifiers[2].modifier + 2);
        }
        if (statModifiers[3].modifier >= 0)
        {
            pokeFightStats.defspe = baseStats[3] * (statModifiers[3].modifier + 2) / 2;
        }
        else
        {
            pokeFightStats.defspe = baseStats[3] * 2 / (statModifiers[3].modifier + 2);
        }
        if (statModifiers[4].modifier >= 0)
        {
            pokeFightStats.speed = baseStats[4] * (statModifiers[4].modifier + 2) / 2;
        }
        else
        {
            pokeFightStats.speed = baseStats[4] * 2 / (statModifiers[4].modifier + 2);
        }
        if (statModifiers[5].modifier >= 0)
        {

        }

    }

    public void Start()
    {
        StartCoroutine(Fight());
    }
    private IEnumerator Fight()
    {
        var random = new Random();
        yield return new WaitUntil(() => moves.Count == 4);
        yield return new WaitUntil(() => PokemonOpponent.moves.Count == 4);
        previousHp = pokeFightStats.maxhealth;
        while (true)
        {
            UpdateStats();
            float speeds = (float)pokeFightStats.speed * speedMultiplier / 500;
            float delay = Mathf.Lerp(7, 4f, speeds);
            yield return new WaitForSeconds(((float)random.Next(-10, 10) * 0.01f) + delay);
            AttackOpponent();
            if (!isPokemonAlive(pokeFightStats) || !isPokemonAlive(PokemonOpponent.pokeFightStats)) { break; }

        }
        if (pokeFightStats.currenthealth <= 0)
        {
            StartCoroutine(Death());
        }
        yield return new WaitForSeconds(2.5f);
        if (pokeFightStats.currenthealth <= 0)
        {
            if (pokenum == 1)
            {
                damageText.text = $"Pokemon {pokeFightStats.pokename} has fainted.";
                PokemonOpponent.damageText.text = "";

                Debug.Log($"Pokemon {pokeFightStats.pokename} has fainted.");
            }
            else
            {
                PokemonOpponent.damageText.text = $"Pokemon {pokeFightStats.pokename} has fainted.";
                damageText.text = "";

                Debug.Log($"Pokemon {pokeFightStats.pokename} has fainted.");
            }
        }
    }

    private IEnumerator Death()
    {
        canSwap = false;
        yield return new WaitForSeconds(2);
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            image.color = new Color(1, 1, 1, (float)i);
            image.rectTransform.position = new Vector2(image.rectTransform.position.x, image.rectTransform.position.y - i / 2);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return null;
    }
    private IEnumerator Paralysis()
    {
        yield return new WaitForSeconds(1.5f);
        PokemonOpponent.damageText.text = $"{pokeFightStats.pokename} couldn't move !";
    }
    private IEnumerator Confusion()
    {
        yield return new WaitForSeconds(0.75f);
        int levelmulti = (2 * pokeFightStats.level / 5) + 2;
        int damage = 40 * levelmulti;
        damage *= pokeFightStats.atk / pokeFightStats.def;
        damage /= 50;
        damage += 2;
        var random = new Random();
        damage = damage * random.Next(85, 100);
        damage = damage / 100;
        pokeFightStats.currenthealth -= damage;
        yield return new WaitForSeconds(0.75f);
        UpdateHealthBar();
        UpdateHealthText();
        PokemonOpponent.damageText.text = $"{pokeFightStats.pokename} hurt itself in its confusion !";
        confusionImg.gameObject.SetActive(false);

    }
    private IEnumerator SendAttack()
    {
        yield return new WaitForSeconds(2);
        confusionImg.gameObject.SetActive(false);
        var random = new Random();
        moveUsed = moves[random.Next(moves.Count)];
        int levelmulti = (2 * pokeFightStats.level / 5) + 2;
        int damage = 0;
        List<string> damagecategories = new List<string> {"damage", "damage+ailment", "damage+heal", "damage+lower", "damage+raise"};
        
        if (damagecategories.Contains(moveUsed.meta.category))
        {
            damage = (int)moveUsed.power; 
            damage = damage * levelmulti;
            string moveType = moveUsed.type;

            if (moveType == pokemondata.type1 || moveType == pokemondata.type2)
            {
                damage *= 3 / 2;
            }
            Type type = (Type)Enum.Parse(typeof(Type), moveType, true);
            PokemonOpponent.TakeDamage(damage, moveUsed);
            
        }
        else if (moveUsed.meta.ailment != "none")
        {
            PokemonOpponent.TakeDamage(0, moveUsed);
        }
        else
        {
            PokemonOpponent.TakeDamage(70 * levelmulti, moveUsed);

        }
        yield return new WaitForSeconds(2);
        if (!isReloading && isPokemonAlive(pokeFightStats))
        {
            StartCoroutine(AttackAnim());

        } 

    }

    private void DoStatuses()
    {
        var random = new Random();
        if (statusTimer > 0)
        {
            statusTimer--;

            switch (status)
            {
                case Status.none:
                    StartCoroutine(SendAttack());
                    break;
                case Status.burn:
                    pokeFightStats.currenthealth -= pokeFightStats.maxhealth / 16;
                    PokemonOpponent.damageText.text = $"{pokeFightStats.pokename} is hurt by its burn.";
                    UpdateHealthBar();
                    UpdateHealthText();
                    StartCoroutine(SendAttack());
                    break;
                case Status.freeze:
                    PokemonOpponent.damageText.text = $"{pokeFightStats.pokename} is frozen solid.";

                    if (random.Next(0, 100) > 80)
                    {
                        PokemonOpponent.damageText.text = $"{pokeFightStats.pokename} is no longer frozen !";
                        status = Status.none;
                        StartCoroutine(SendAttack());
                    }
                    break;
                case Status.paralysis:
                    PokemonOpponent.damageText.text = $"{pokeFightStats.pokename} is paralyzed.";
                    if (random.Next(0, 100) > 75)
                    {
                        StartCoroutine(Paralysis());
                    }
                    else
                    {
                        StartCoroutine(SendAttack());
                    }

                    break;
                case Status.poison:
                    PokemonOpponent.damageText.text = $"{pokeFightStats.pokename} is hurt from poison.";

                    pokeFightStats.currenthealth -= pokeFightStats.maxhealth / 8;
                    UpdateHealthBar();
                    UpdateHealthText();
                    StartCoroutine(SendAttack());

                    break;
                case Status.sleep:
                    PokemonOpponent.damageText.text = $"{pokeFightStats.pokename} is fast asleep.";

                    if (random.Next(100) > 50)
                    {
                        statusTimer--;
                        if (random.Next(100) > 50)
                        {
                            statusTimer--;
                        }
                    }

                    break;

            }



        }
        else
        {
            if (status == Status.sleep)
            {
                PokemonOpponent.damageText.text = $"{pokeFightStats.pokename} wakes up !";
            }

            status = Status.none;
            UpdateStatus();
            StartCoroutine(SendAttack());
        }
    }
    public void AttackOpponent()
    {
        if (isPokemonAlive(pokeFightStats))
        {
            if (isPokemonAlive(PokemonOpponent.pokeFightStats))
            {
                var random = new Random();

                if (secondaryStatuses.Count > 0)
                {
                    if (secondaryStatuses.Contains(SecondaryStatus.confusion))
                    {
                        PokemonOpponent.damageText.text = $"{pokeFightStats.pokename} is confused.";
                        confusionImg.gameObject.SetActive(true);
                        if (random.Next(1, 100) > 66)
                        {
                            StartCoroutine(Confusion());
                        }
                        else
                        {
                            StartCoroutine(SendAttack());
                        }
                        secondaryStatusesTimer[secondaryStatuses.IndexOf(SecondaryStatus.confusion)] -= 1;
                        if (secondaryStatusesTimer[secondaryStatuses.IndexOf(SecondaryStatus.confusion)] == 0)
                        {
                            secondaryStatuses.Remove(SecondaryStatus.confusion);
                        }
                        else if (random.Next(1, 100) > 75)
                        {
                            secondaryStatusesTimer[secondaryStatuses.IndexOf(SecondaryStatus.confusion)] -= 1;
                        }
                    }
                    else
                    {
                        DoStatuses();
                    }
                }
                else
                {
                    DoStatuses();
                }


            }
        }

    }
    private IEnumerator AttackAnim()
    {
        Vector3 startpos = image.rectTransform.anchoredPosition;
        Vector3 endpos = new();
        if (pokenum == 1)
        {
            endpos = new Vector3(startpos.x + 200, startpos.y, startpos.z);

        }
        else
        {
            endpos = new Vector3(startpos.x - 200, startpos.y, startpos.z);

        }
        
        int max = 69;
        for (int i = 0; i < max; i++)
        {
            if (isReloading)
            {
                break;
            }
            image.rectTransform.anchoredPosition = Vector3.Lerp(startpos, endpos, (float)i / max);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        image.rectTransform.anchoredPosition = endpos;

        for (int i = 0; i < max; i++)
        {
            if (isReloading)
            {
                break;
            }
            image.rectTransform.anchoredPosition = Vector3.Lerp(endpos, startpos, (float)i / max);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        if (pokenum == 2)
        {
            image.rectTransform.anchoredPosition = new Vector2(450, 250);

        }
        else
        {
            image.rectTransform.anchoredPosition = new Vector2(-450, -100);

        }

        yield return null;

    }
    private void UpdateHealthBar()
    {
        StartCoroutine(HealthBarCoroutine());
    }
    private IEnumerator HealthBarCoroutine()
    {
        /*
        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            HealthBar.value = Mathf.Lerp((float)previousHp / (float)pokeFightStats.maxhealth, (float)pokeFightStats.currenthealth / (float)pokeFightStats.maxhealth, (float)i);
            HPColor.color = gradient.Evaluate(HealthBar.value);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        */
        Tween tween = Tween.Custom(startValue: (float)previousHp / (float)pokeFightStats.maxhealth, endValue: (float)pokeFightStats.currenthealth / (float)pokeFightStats.maxhealth, duration: 1, ease:Ease.Linear,onValueChange: newValue => HealthBar.value = newValue);
        if (HealthBar.value == 0)
        {
            previousHp = 0;
        }
        else
        {
            previousHp = pokeFightStats.currenthealth;
        }
        yield return tween.ToYieldInstruction();
    }
    private void UpdateHealthText()
    {
        if (pokenum == 1)
        {
            if (pokeFightStats.currenthealth > 0)
            {
                currenthpText.text = pokeFightStats.currenthealth.ToString();
                maxHpText.text = pokeFightStats.maxhealth.ToString();

            }
            else
            {
                currenthpText.text = "0";
                maxHpText.text = pokeFightStats.maxhealth.ToString();

            }
        }

    }


    IEnumerator LoadImage(Image imgIcon, string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        yield return new WaitForSeconds(0.1f);
        Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        texture.filterMode = FilterMode.Point;
        Sprite newsprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        imgIcon.sprite = newsprite;
        gifsLoaded++;
        isReloading = false;
    }

    private void InitCurrentLife(PokeFightStats poke)
    {
        Random random = new();
        int iv = random.Next(0, 31);
        pokeFightStats.maxhealth = ((2 * pokeFightStats.maxhealth + iv) * pokeFightStats.level / 100) + pokeFightStats.level + 10;
        pokeFightStats.currenthealth = pokeFightStats.maxhealth;
        iv = random.Next(0, 31);
        pokeFightStats.atk = ((2 * pokeFightStats.atk + iv) * pokeFightStats.level / 100);
        pokeFightStats.atk += iv + 5;
        iv = random.Next(0, 31);
        pokeFightStats.atkspe = ((2 * pokeFightStats.atkspe + iv) * pokeFightStats.level / 100);
        pokeFightStats.atkspe += iv + 5;
        iv = random.Next(0, 31);
        pokeFightStats.def = ((2 * pokeFightStats.def + iv) * pokeFightStats.level / 100);
        pokeFightStats.def += iv + 5;
        iv = random.Next(0, 31);
        pokeFightStats.defspe = ((2 * pokeFightStats.defspe + iv) * pokeFightStats.level / 100);
        pokeFightStats.defspe += iv + 5;
        iv = random.Next(0, 31);
        pokeFightStats.speed = ((2 * pokeFightStats.speed + iv) * pokeFightStats.level / 100);
        pokeFightStats.speed += iv + 5;
    }

    private void InitStatPoints(PokeFightStats poke)
    {
        poke.statpoints = poke.atk + poke.maxhealth + poke.def;
    }

    public bool isPokemonAlive(PokeFightStats poke)
    {
        if (poke.currenthealth > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator TakeDamageCoroutine(int damage, MoveData.Move move)
    {
        var random = new Random();
        string moveName = move.name.Replace("-", " ");
        string firstChar = moveName[0].ToString();
        moveName = firstChar.ToUpper() + moveName.Substring(1);
        damageText.text = $"{PokemonOpponent.pokeFightStats.pokename} used {moveName} !";
        Debug.Log($"{PokemonOpponent.pokeFightStats.pokename} used {moveName}");
        Type type = (Type)Enum.Parse(typeof(Type), move.type, true);
        string additional = "";
        if (damage > 0)
        {
            int opoatk = 0;
            int pokedef = 0;
            if (move.damage_class == "physical")
            {
                opoatk = PokemonOpponent.pokeFightStats.atk;
                pokedef = pokeFightStats.def;
            }
            else
            {
                opoatk = PokemonOpponent.pokeFightStats.atkspe;
                pokedef = pokeFightStats.defspe;
            }
            //if(opoatk < pokedef) { opoatk *= 5 / 4  ; }
            damage = damage * opoatk / pokedef;
            damage /= 50;
            damage += 2;

            foreach (PokemonInfoController.Damage dmg in pokeFightStats.damagerelations)
            {
                if (dmg.type.ToString() == type.ToString())
                {
                    damage = (Int32)(damage * dmg.multiplier);
                    if (dmg.multiplier > 1)
                    {
                        additional = "It's Super Effective !";
                    }
                    else if (dmg.multiplier < 1)
                    {
                        additional = "It's not very effective...";

                    }

                }
            }
            int maxcr = 24;
            if (move.meta.critrate > 0) { maxcr /= (int)move.meta.critrate; }
            if (random.Next(0, maxcr) >= maxcr - 1)
            {
                damage += damage / 2;
                additional += " Critical hit!";
            }
        }
        if (random.Next(0, 100) > move.accuracy)
        {
            damage = 0;
            additional = " It missed !";
            move.meta.ailmentchance = 0;
        }
        damage = damage * random.Next(85, 100);
        damage = damage / 100;
        previousHp = pokeFightStats.currenthealth;
        pokeFightStats.currenthealth -= damage;
        if (pokeFightStats.currenthealth < 0)
        {
            pokeFightStats.currenthealth = 0;
        }
        //Debug.Log(move.meta.ailment);
        if (move.meta.ailment != "none")
        {


            statusTimer = 3;

            switch (move.meta.ailment)
            {
                case "burn":
                    if (status == Status.none)
                    {
                        if (move.meta.ailmentchance == 0)
                        {
                            status = (Status)Enum.Parse(typeof(Status), move.meta.ailment, true);
                        }
                        if (random.Next(0, 100) <= move.meta.ailmentchance)
                        {
                            status = (Status)Enum.Parse(typeof(Status), move.meta.ailment, true);
                        }
                    }
                    break;
                case "freeze":
                    if (status == Status.none)
                    {
                        if (move.meta.ailmentchance == 0)
                        {
                            status = (Status)Enum.Parse(typeof(Status), move.meta.ailment, true);
                        }
                        if (random.Next(0, 100) <= move.meta.ailmentchance)
                        {
                            status = (Status)Enum.Parse(typeof(Status), move.meta.ailment, true);
                        }
                    }
                    break;
                case "paralysis":
                    if (status == Status.none)
                    {
                        if (move.meta.ailmentchance == 0)
                        {
                            status = (Status)Enum.Parse(typeof(Status), move.meta.ailment, true);
                        }
                        if (random.Next(0, 100) <= move.meta.ailmentchance)
                        {
                            status = (Status)Enum.Parse(typeof(Status), move.meta.ailment, true);
                        }
                    }
                    break;
                case "poison":
                    Debug.Log(status);
                    if (status == Status.none)
                    {
                        if (move.meta.ailmentchance == 0)
                        {
                            Debug.Log("poisoned");
                            status = (Status)Enum.Parse(typeof(Status), move.meta.ailment, true);
                        }
                        if (random.Next(0, 100) <= move.meta.ailmentchance)
                        {
                            status = (Status)Enum.Parse(typeof(Status), move.meta.ailment, true);
                        }
                    }
                    break;
                case "sleep":
                    if (status == Status.none)
                    {
                        if (move.meta.ailmentchance == 0)
                        {
                            status = (Status)Enum.Parse(typeof(Status), move.meta.ailment, true);
                        }
                        if (random.Next(0, 100) <= move.meta.ailmentchance)
                        {
                            status = (Status)Enum.Parse(typeof(Status), move.meta.ailment, true);
                        }
                    }
                    break;
                case "confusion":
                    if (!secondaryStatuses.Contains(SecondaryStatus.confusion))
                    {
                        if (move.meta.ailmentchance == 0)
                        {
                            secondaryStatuses.Add((SecondaryStatus)Enum.Parse(typeof(SecondaryStatus), move.meta.ailment, true));
                            secondaryStatusesTimer.Add(3);
                        }
                        if (random.Next(0, 100) <= move.meta.ailmentchance)
                        {
                            secondaryStatuses.Add((SecondaryStatus)Enum.Parse(typeof(SecondaryStatus), move.meta.ailment, true));
                            secondaryStatusesTimer.Add(3);
                        }
                    }

                    break;
                default:
                    Debug.Log("Status not yet implemented.");
                    break;
            }


        }
        //Debug.Log($"pokemon {pokeFightStats.pokename} has {pokeFightStats.currenthealth}");
        yield return new WaitForSeconds(2f);
        if(moveUsed.meta.category == "damage+raise")
        {

        }
        if (isPokemonAlive(PokemonOpponent.pokeFightStats))
        {
            UpdateStatus();
            damageText.text = $"{PokemonOpponent.pokeFightStats.pokename} hit {pokeFightStats.pokename} for {damage} damage ! {additional}";
            UpdateHealthBar();
            UpdateHealthText();
        }
        else
        {
            pokeFightStats.currenthealth = previousHp;
        }



    }
    public void TakeDamage(int damage, MoveData.Move move)
    {
        StartCoroutine(TakeDamageCoroutine(damage, move));
    }

    private IEnumerator LerpLoad(int Max)
    {
        LoadBar.gameObject.SetActive(true);

        while (allmoves.Count < Max)
        {
            canSwap = false;
            Tween tween = Tween.Custom(startValue: LoadBar.value, endValue: (float)allmoves.Count / Max, duration: 0.5f, onValueChange: newVal => LoadBar.value = newVal);
            /*
            for (float i = 0; i < 30; i++)
            {
                LoadBar.value = Mathf.Lerp(LoadBar.value, (float)allmoves.Count / Max, (float)i / 30f);
                yield return new WaitForSeconds(Time.deltaTime);

            }
            */
            yield return new WaitForSeconds(0.5f);
        }
        LoadBar.value = 1;
        canSwap = true;
        yield return new WaitForSeconds(0.3f);
        LoadBar.gameObject.SetActive(false);

    }

    private IEnumerator getMovesList()
    {
        List<string> moveurls = new();
        UnityWebRequest www = UnityWebRequest.Get($"https://pokeapi.co/api/v2/pokemon/{pokemondata.number}");
        yield return www.SendWebRequest();
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
        Root pokemon = JsonConvert.DeserializeObject<Root>(www.downloadHandler.text, settings);
        foreach (PokeAPI.Move move in pokemon.moves)
        {
            moveurls.Add(move.move.url);
        }
        int amountofweb = 0;
        yield return new WaitUntil(() => moveurls.Count >= pokemon.moves.Count);
        int movenumber = 0;
        StartCoroutine(LerpLoad(moveurls.Count));
        foreach (string url in moveurls)
        {

            movenumber++;
            amountofweb++;
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
            yield return unityWebRequest.SendWebRequest();
            amountofweb--;
            RootMove move = JsonConvert.DeserializeObject<RootMove>(unityWebRequest.downloadHandler.text, settings);
            string name = move.name;
            int? accuracy = move.accuracy;
            int? effect_chance = move.effect_chance;
            int? pp = move.pp;
            string damage_class = move.damage_class.name;
            int? priority = move.priority;
            int? power = move.power;
            string type = move.type.name;
            string statchangenames = "";
            int statchangeamount = -1;
            List<StatChanges> statChanges = new();

            foreach (StatChange statchange in move.stat_changes)
            {
                statchangenames = statchange.stat.name;
                statchangeamount = (int)statchange.change;
                statChanges.Add(new StatChanges(statchangenames, statchangeamount));
            }
            string ailment = "";
            string category = "";
            int? minhits = -1;
            int? maxhits = -1;
            int? minturns = -1;
            int? maxturns = -1;
            int? drain = -1;
            int? healing = -1;
            int? critrate = -1;
            int? ailmentchance = -1;
            int? flinchchance = -1;
            int? statchance = -1;
            MoveMeta meta = new();
            if (move != null && move.meta != null && move.meta.ailment != null && move.meta.ailment.name != null)
            {
                ailment = move.meta.ailment.name;
                category = move.meta.category.name;
                minhits = move.meta.min_hits;
                maxhits = move.meta.max_hits;
                minturns = move.meta.min_turns;
                maxturns = move.meta.max_turns;
                drain = move.meta.drain;
                healing = move.meta.healing;
                critrate = move.meta.crit_rate;
                ailmentchance = move.meta.ailment_chance;
                flinchchance = move.meta.flinch_chance;
                statchance = move.meta.stat_chance;
                meta = new MoveMeta(ailment, category, minhits, maxhits, minturns, maxturns, drain, healing, critrate, ailmentchance, flinchchance, statchance);
            }

            allmoves.Add(new MoveData.Move(name, accuracy, effect_chance, pp, damage_class, priority, power, type, statChanges, meta));

            //Debug.Log($"{name} {movenumber}");

            if (amountofweb > 10)
            {
                yield return new WaitUntil(() => amountofweb < 10);
            }
        }

        yield return StartCoroutine(SortMoves());

        yield return null;
    }

    private IEnumerator SortMoves()
    {

        otherMoves = new();
        attackMoves = new();
        statusMoves = new();
        var random = new Random();

        foreach (var move in allmoves)
        {
            if (move.meta.ailment != "none")
            {
                statusMoves.Add(move);
            }
            else if (move.power > 0 && (move.type == pokemondata.type1.ToLower() || move.type == pokemondata.type2.ToLower()))
            {
                attackMoves.Add(move);
            }
            else
            {
                otherMoves.Add(move);
            }
        }

        MoveData.Move attack1 = attackMoves[random.Next(attackMoves.Count - 1)];
        MoveData.Move attack2 = attackMoves[random.Next(attackMoves.Count - 1)];
        while (attack1 == attack2)
        {
            attack2 = attackMoves[random.Next(attackMoves.Count - 1)];

        }
        MoveData.Move status1 = statusMoves[random.Next(statusMoves.Count - 1)];
        MoveData.Move other1 = otherMoves[random.Next(otherMoves.Count - 1)];
        moves.Add(other1);
        moves.Add(attack1);
        moves.Add(attack2);
        moves.Add(status1);
        foreach (var move in moves)
        {
            Debug.Log($"{pokeFightStats.pokename} has {move.name}");
        }


        yield return null;
    }

    /*

    private void TakeDamage(int damage, Type type)
    {
        if (damage > 0)
        {
            if (weaknesses.Contains(type))
            {
                currentHp -= 2 * damage;
                DisplayDamage(2 * damage, type, 2);
            }
            else if (resistances.Contains(type))
            {
                currentHp -= damage / 2;
                DisplayDamage(damage / 2, type, 3);

            }
            else if (immune.Contains(type))
            {
                DisplayDamage(0, type, 0);
            }
            else
            {
                currentHp -= damage;
                DisplayDamage(damage, type, 1);
            }
        }
        Debug.Log($"Pokemon HP = {currentHp}");
    }


    private void DisplayDamage(int damage, Type type, int modifier)
    {

        damageText.text = $"Darkrai took {damage} damage of {type} type.";
        switch (modifier)
        {
            case 0: damageText.text = damageText.text + $" Darkrai is Immune to {type}."; break;
            case 1: break;
            case 2: damageText.text = damageText.text + $" Darkrai is weak to {type}."; break;
            case 3: damageText.text = damageText.text + $" Darkrai is resistant to {type}."; break;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        InitCurrentLife();
        InitStatPoints();
        DisplayPokemonName();
        DisplayCurrentHP();
        DisplayAtk();
        DisplayDef();
        DisplayStats();
        DisplayType();
        DisplayWeight();
        DisplayResistances();
        DisplayWeaknesses();
        StartCoroutine(TakingDamage());
        image.sprite = newSprite; //comme j'utilise une animation pour l'image ce script change l'image du type
    }

    // Update is called once per frame
    void Update()
    {
        SetName();
        UpdateHealthText();
        UpdateHealthBar();
        CheckIfPokemonAlive();
        if (Input.GetKeyDown(KeyCode.P))
        {
            currentHp = currentHp - 1;
        }
    }
    IEnumerator TakingDamage()
    {
        while (currentHp > 0)
        {
            yield return new WaitForSeconds(2.5f);
            TakeDamageRandom();
        }
    }
    */
}
