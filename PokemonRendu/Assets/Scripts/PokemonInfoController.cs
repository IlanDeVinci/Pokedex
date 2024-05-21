using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static PokeAPI;
using static PokeAPIEvolution;
using static PokeAPISpecies;
using static PokeAPITypes;
using Random = System.Random;
using PrimeTween;
using System.Text.RegularExpressions;
using Unity.VisualScripting;

public class PokemonInfoController : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private Image fade;
    [SerializeField] private TextMeshProUGUI textName;
    [SerializeField] private TextMeshProUGUI size;
    [SerializeField] private TextMeshProUGUI weight;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI category;
    [SerializeField] private TextMeshProUGUI abilities;
    [SerializeField] private Image type1;
    [SerializeField] private Image type2;
    [SerializeField] private DatabaseManager databaseManager;
    [SerializeField] private Sprite[] typeImages;
    [SerializeField] private Sprite empty;
    [SerializeField] private AudioSource audiosource;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider atkSlider;
    [SerializeField] private Slider defSlider;
    [SerializeField] private Slider atkspeSlider;
    [SerializeField] private Slider defspeSlider;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private TextMeshProUGUI hpSliderText;
    [SerializeField] private TextMeshProUGUI atkSliderText;
    [SerializeField] private TextMeshProUGUI defSliderText;
    [SerializeField] private TextMeshProUGUI atkspeSliderText;
    [SerializeField] private TextMeshProUGUI defspeSliderText;
    [SerializeField] private TextMeshProUGUI speedSliderText;
    [SerializeField] private Image hpSliderIMG;
    [SerializeField] private Image atkSliderIMG;
    [SerializeField] private Image defSliderIMG;
    [SerializeField] private Image atkSpeSliderIMG;
    [SerializeField] private Image defSpeSliderIMG;
    [SerializeField] private Image speedSliderIMG;
    [SerializeField] private Gradient gradient;
    [SerializeField] private TextMeshProUGUI totalStats;
    [SerializeField] private Image[] weaknessesImage;
    [SerializeField] private Image[] resistancesImage;
    [SerializeField] private TextMeshProUGUI[] weaknesstext;
    [SerializeField] private TextMeshProUGUI[] resistancetext;
    [SerializeField] private GameObject weakPanel;
    [SerializeField] private GameObject weakPanelBg;
    [SerializeField] private GameObject evoPanel;
    [SerializeField] private GameObject evoBasePanel;
    [SerializeField] private GameObject evoMidPanel;
    [SerializeField] private GameObject evoMaxPanel;
    [SerializeField] private GameObject evoPanelBg;
    [SerializeField] private GameObject evoObjPrefab;
    [SerializeField] private GameObject searchBarGameObject;
    [SerializeField] private TMP_InputField searchBar;
    [SerializeField] private GameObject[] searchpanels;
    [SerializeField] private TextMeshProUGUI[] searchResults;
    [SerializeField] private GameObject searchBarGameObjectPoke1;
    [SerializeField] private TMP_InputField searchBarPoke1;
    [SerializeField] private GameObject[] searchpanelsPoke1;
    [SerializeField] private TextMeshProUGUI[] searchResultsPoke1;
    [SerializeField] private GameObject searchBarGameObjectPoke2;
    [SerializeField] private TMP_InputField searchBarPoke2;
    [SerializeField] private GameObject[] searchpanelsPoke2;
    [SerializeField] private TextMeshProUGUI[] searchResultsPoke2;
    [SerializeField] private GameObject FightPanel;
    [SerializeField] private GameObject FightPanelBg;
    [SerializeField] private TextMeshProUGUI Poke1Name;
    [SerializeField] private TextMeshProUGUI Poke2Name;
    [SerializeField] private Image Poke1Image;
    [SerializeField] private Image Poke2Image;
    [SerializeField] private Slider LoadSlider;
    [SerializeField] private GameObject SettingsPanelBg;
    [SerializeField] private GameObject SettingsPanel;
    [SerializeField] private TMP_InputField loadPokeInput;
    [SerializeField] private Slider Poke1Level;
    [SerializeField] private Slider Poke2Level;
    [SerializeField] private TextMeshProUGUI lvl1text;
    [SerializeField] private TextMeshProUGUI lvl2text;

    List<GameObject> evoObjectsBase = new();
    List<GameObject> evoObjectsMid = new();
    List<GameObject> evoObjectsMax = new();
    List<Damage> damages = new();
    List<Damage> weaknesses = new();
    List<Damage> resistances = new();
    List<Damage> immunities = new();
    private bool everythingloaded = true;
    private bool weakToggled = false;
    private bool evoToggled = false;
    private bool searchToggled = false;
    private bool fightToggled = false;
    private bool settingsToggled = false;
    private Tween hpTween;
    private Tween atkTween;
    private Tween defTween;
    private Tween atkSpeTween;
    private Tween defSpeTween;
    private Tween speedTween;


    private int poke1id = 0;
    private int poke2id = 0;
    private AudioClip myClip;
    public int id;
    public Root pokemon;
    public PokeSpecies pokespecies;
    private PokemonData data;
    private bool isWeaknessLoaded = false;
    private bool isEvolutionLoaded = false;
    List<Damage> result1 = new List<Damage>();
    List<Damage> result2 = new List<Damage>();
    private string inputNumload = "1";

    List<Type> types = new List<Type>();

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

    [Serializable]
    public class Damage
    {
        public Type type;
        public float multiplier;

        public Damage()
        {
        }

        public Damage(Type type, float multiplier)
        {
            this.type = type;
            this.multiplier = multiplier;
        }
    }

    public void onSliderMove()
    {
        GameManagerStatic.firstPokemonlvl = (int)Poke1Level.value;
        GameManagerStatic.secondPokemonlvl = (int)Poke2Level.value;
        lvl1text.text = Poke1Level.value.ToString();
        lvl2text.text = Poke2Level.value.ToString();
    }

    void Awake()
    {
        Poke1Level.value = 50;
        Poke2Level.value = 50;
        lvl1text.text = "50";
        lvl2text.text = "50";

        GameManagerStatic.pokemonInfoController = this;
        StartCoroutine(GetPokemon());
    }


    public void LoadBar(int number)
    {
        StartCoroutine(LerpLoad(number));
        //LoadSlider.value = (float)databaseManager.database.datas.Count / number;
    }

    private IEnumerator LerpLoad(int number)
    {
        Tween tween;
        while (databaseManager.database.datas.Count < number)
        {
            tween = Tween.Custom(LoadSlider.value, (float)databaseManager.database.datas.Count / number, duration: 1f,
                onValueChange: newValue => LoadSlider.value = newValue);
            yield return tween.ToYieldInstruction();
            tween.Stop();
        }

        /*
        int max = 100;
        for (int i = 0; i<max; i++)
        {
            LoadSlider.value = Mathf.Lerp(LoadSlider.value, (float)databaseManager.database.datas.Count / number, (float)i/max);
            yield return new WaitForSeconds(0.01f);
        }
        */
    }

    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("PokeFight");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void ToggleFight()
    {
        if (fightToggled)
        {
            fightToggled = false;
            searchBarGameObjectPoke1.SetActive(false);
            searchBarGameObjectPoke2.SetActive(false);
            FightPanel.SetActive(false);
            FightPanelBg.SetActive(false);
            searchBarPoke1.text = "";
            searchBarPoke2.text = "";
            foreach (GameObject panel in searchpanelsPoke1)
            {
                panel.SetActive(false);
            }

            foreach (GameObject panel in searchpanelsPoke2)
            {
                panel.SetActive(false);
            }
        }
        else
        {
            StartCoroutine(SmoothAlpha(FightPanel.GetComponent<CanvasGroup>()));
            fightToggled = true;
            searchBarGameObjectPoke1.SetActive(true);
            searchBarGameObjectPoke2.SetActive(true);
            FightPanel.SetActive(true);
            FightPanelBg.SetActive(true);
        }
    }

    public void LoadPokeInput()
    {
        id = Int32.Parse(inputNumload);
        StartCoroutine(GetPokemon());
    }

    public void LoadInput()
    {
        inputNumload = Regex.Replace(loadPokeInput.text, @"[^0-9a-zA-Z:,]+", "");
        loadPokeInput.text = inputNumload;
        if (inputNumload != "")
        {
            int loadnum = Int32.Parse(inputNumload);
            if (loadnum > 1025)
            {
                loadPokeInput.text = "1025";
                loadnum = 1025;
            }

            inputNumload = loadnum.ToString();
        }
        else
        {
            inputNumload = "1";
        }
    }

    public void FightSearchBarInput1()
    {
        foreach (GameObject panel in searchpanelsPoke1)
        {
            panel.SetActive(false);
        }

        string searchtext = searchBarPoke1.text.ToLower();
        if (string.IsNullOrEmpty(searchtext)) return;
        List<string> list = new List<string>();
        foreach (PokemonData pokemon in databaseManager.database.datas)
        {
            if (pokemon.pokename.ToLower().Contains(searchtext) || pokemon.number.ToString().Contains(searchtext))
            {
                list.Add($"{pokemon.number} {pokemon.pokename}");
            }
        }

        int index = 0;

        foreach (GameObject panel in searchpanelsPoke1)
        {
            if (list.Count >= index + 1)
            {
                searchResultsPoke1[index].text = list[index];
                panel.SetActive(true);
                index++;
            }
        }

        searchBarGameObjectPoke1.SetActive(true);
    }


    public void FightSearchBarInput2()
    {
        foreach (GameObject panel in searchpanelsPoke2)
        {
            panel.SetActive(false);
        }

        string searchtext = searchBarPoke2.text.ToLower();
        if (string.IsNullOrEmpty(searchtext)) return;
        List<string> list = new List<string>();
        foreach (PokemonData pokemon in databaseManager.database.datas)
        {
            if (pokemon.pokename.ToLower().Contains(searchtext) || pokemon.number.ToString().Contains(searchtext))
            {
                list.Add($"{pokemon.number} {pokemon.pokename}");
            }
        }

        int index = 0;

        foreach (GameObject panel in searchpanelsPoke2)
        {
            if (list.Count >= index + 1)
            {
                searchResultsPoke2[index].text = list[index];
                panel.SetActive(true);
                index++;
            }
        }

        searchBarGameObjectPoke2.SetActive(true);
    }

    public void LerpSlider(float start, int end, Slider slider)
    {
        StartCoroutine(LerpSliderCoroutine(start, end, slider));
    }

    public IEnumerator LerpSliderCoroutine(float start, int end, Slider slider)
    {
        Tween tween = Tween.Custom(start, end, duration: 0.5f, onValueChange: newValue => slider.value = newValue);
        /*
        for (int i = 0; i<30; i++)
        {
            slider.value = Mathf.Lerp(start, end, (float)i / 30);

            yield return new WaitForSeconds(0.01f);
        }
        */
        yield return tween.ToYieldInstruction();
        tween.Stop();
    }

    public void RefreshPokemon()
    {
        ToggleEvolution();
        StartCoroutine(GetPokemon());
    }

    public void ToggleSearch()
    {
        if (searchToggled)
        {
            searchToggled = false;
            searchBarGameObject.SetActive(false);
            searchBar.text = "";
            foreach (GameObject panel in searchpanels)
            {
                panel.SetActive(false);
            }
        }
        else
        {
            searchToggled = true;
            searchBarGameObject.SetActive(true);
        }
    }

    public void SearchRes(int i)
    {
        int newid = Int32.Parse(searchResults[i].text.Split(' ')[0]);
        id = newid;
        searchBarGameObject.SetActive(false);
        searchToggled = false;
        searchBar.text = "";
        StartCoroutine(GetPokemon());
    }

    public void SearchRes1(int i)
    {
        int newid = Int32.Parse(searchResultsPoke1[i].text.Split(' ')[0]);
        searchBarPoke1.text = "";
        Poke1Name.text = databaseManager.database.datas[newid - 1].pokename;
        StartCoroutine(LoadImage(Poke1Image, newid));
        poke1id = newid;
        StartCoroutine(GetDamageRelations1());
    }

    private IEnumerator GetDamageRelations2()
    {
        result2 = new();
        List<Type> typeList = new List<Type>();
        Type parsedtype1 = (Type)Enum.Parse(typeof(Type), databaseManager.database.datas[poke2id - 1].type1, true);
        typeList.Add(parsedtype1);
        if (databaseManager.database.datas[poke2id - 1].type2 != "")
        {
            Type parsedtype2 = (Type)Enum.Parse(typeof(Type), databaseManager.database.datas[poke2id - 1].type2, true);

            typeList.Add(parsedtype2);
        }

        yield return StartCoroutine(FindWeaknesses(typeList, result2));
        GameManagerStatic.secondPokemonDamageRelation = result2;
    }

    public void RandomChooseFight()
    {
        var random = new Random();
        int id1 = random.Next(1, databaseManager.database.datas.Count);
        int id2 = random.Next(1, databaseManager.database.datas.Count);
        searchBarPoke2.text = "";
        searchBarPoke1.text = "";
        Poke2Name.text = databaseManager.database.datas[id2 - 1].pokename;
        Poke1Name.text = databaseManager.database.datas[id1 - 1].pokename;
        StartCoroutine(LoadImage(Poke1Image, id1));
        StartCoroutine(LoadImage(Poke2Image, id2));
        poke1id = id1;
        poke2id = id2;
        StartCoroutine(GetDamageRelations1());
        StartCoroutine(GetDamageRelations2());
    }

    public void SearchRes2(int i)
    {
        int newid = Int32.Parse(searchResultsPoke2[i].text.Split(' ')[0]);
        searchBarPoke2.text = "";
        Poke2Name.text = databaseManager.database.datas[newid - 1].pokename;
        StartCoroutine(LoadImage(Poke2Image, newid));
        poke2id = newid;
        StartCoroutine(GetDamageRelations2());
    }


    public void SearchBarInput()
    {
        foreach (GameObject panel in searchpanels)
        {
            panel.SetActive(false);
        }

        string searchtext = searchBar.text.ToLower();
        if (string.IsNullOrEmpty(searchtext)) return;
        List<string> list = new List<string>();
        foreach (PokemonData pokemon in databaseManager.database.datas)
        {
            if (pokemon.pokename.ToLower().Contains(searchtext) || pokemon.number.ToString().Contains(searchtext))
            {
                list.Add($"{pokemon.number} {pokemon.pokename}");
            }
        }

        int index = 0;

        foreach (GameObject panel in searchpanels)
        {
            if (list.Count >= index + 1)
            {
                searchResults[index].text = list[index];
                panel.SetActive(true);
                index++;
            }
        }

        searchBarGameObject.SetActive(true);
    }

    private IEnumerator FightStart()
    {
        GameManagerStatic.firstPokemon = databaseManager.database.datas[poke1id - 1];
        GameManagerStatic.secondPokemon = databaseManager.database.datas[poke2id - 1];
        yield return new WaitUntil(() => GameManagerStatic.firstPokemonDamageRelation.Count > 17);
        yield return new WaitUntil(() => GameManagerStatic.secondPokemonDamageRelation.Count > 17);
        StartCoroutine(LoadAsyncScene());
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        fade.gameObject.SetActive(true);
        for (float i = 1; i >= 0; i -= Time.deltaTime / 2)
        {
            fade.color = new UnityEngine.Color(0, 0, 0, i);
            yield return null;
        }

        fade.gameObject.SetActive(false);
    }

    public void StartFight()
    {
        StartCoroutine(FightStart());
    }

    public void ToggleEvolution()
    {
        if (evoToggled)
        {
            evoToggled = false;
            evoPanel.SetActive(false);
            evoPanelBg.SetActive(false);
        }
        else
        {
            StartCoroutine(SmoothAlpha(evoPanel.GetComponent<CanvasGroup>()));
            evoToggled = true;
            evoPanel.SetActive(true);
            evoPanelBg.SetActive(true);
        }
    }

    public void ToggleSettings()
    {
        if (settingsToggled)
        {
            settingsToggled = false;
            SettingsPanel.SetActive(false);
            SettingsPanelBg.SetActive(false);
        }
        else
        {
            StartCoroutine(SmoothAlpha(SettingsPanel.GetComponent<CanvasGroup>()));

            settingsToggled = true;
            SettingsPanel.SetActive(true);
            SettingsPanelBg.SetActive(true);
        }
    }

    public void ToggleWeakness()
    {
        if (weakToggled)
        {
            weakToggled = false;
            weakPanel.SetActive(false);
            weakPanelBg.SetActive(false);
        }
        else
        {
            StartCoroutine(SmoothAlpha(weakPanel.GetComponent<CanvasGroup>()));
            weakToggled = true;
            weakPanel.SetActive(true);
            weakPanelBg.SetActive(true);
        }
    }

    private IEnumerator SmoothAlpha(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        //var t = 0f;
        Tween tween = Tween.Alpha(canvasGroup, 1, 1);
        /*
        while (t <= 0.5f)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, Mathf.SmoothStep(0, 1, t*2));
            yield return new WaitForSeconds(Time.deltaTime);
        }
        */
        yield return tween.ToYieldInstruction();
        tween.Stop();
    }

    public void GetIDPokemon()
    {
        id = UnityEngine.Random.Range(1, databaseManager.database.datas.Count + 1);
        if (everythingloaded)
        {
            StartCoroutine(GetPokemon());
        }
    }

    public void NextPokemon()
    {
        if (everythingloaded)
        {
            if (id < 1025)
            {
                id++;
            }

            StartCoroutine(GetPokemon());
        }
    }

    public void PreviousPokemon()
    {
        if (everythingloaded)
        {
            if (id > 1)
            {
                id--;
            }

            StartCoroutine(GetPokemon());
        }
    }

    IEnumerator GetPokemon()
    {
        isEvolutionLoaded = false;
        isWeaknessLoaded = false;
        searchBarGameObject.SetActive(false);

        if (id >= databaseManager.database.datas.Count && everythingloaded)
        {
            if (id < 1021)
            {
                databaseManager.CreateData(id + 3);
            }
            else if (id <= 1025)
            {
                databaseManager.CreateData(id);
            }
        }

        everythingloaded = false;
        yield return new WaitUntil(() => databaseManager.database.datas.Count >= id);
        yield return new WaitUntil(() => (databaseManager.database.datas[id - 1].pokename != "Loading"));
        data = databaseManager.GetData(id - 1);
        textName.text = $"{id.ToString("000")} {data.pokename}";
        size.text = $"Size = {data.size / 10} m";
        weight.text = $"Weight = {data.weight / 10} kg";
        description.text = data.description;
        category.text = data.category;
        if (data.talent2 != "")
        {
            abilities.text = $"{data.talent1}\n{data.talent2}";
        }
        else
        {
            abilities.text = data.talent1;
        }

        Type parsedtype1, parsedtype2;
        parsedtype1 = (Type)Enum.Parse(typeof(Type), data.type1, true);
        types = new();
        types.Add(parsedtype1);
        int index1 = Array.IndexOf(Enum.GetValues(parsedtype1.GetType()), parsedtype1);
        type1.sprite = typeImages[index1];
        if (data.type2 != "")
        {
            parsedtype2 = (Type)Enum.Parse(typeof(Type), data.type2, true);
            types.Add(parsedtype2);
            int index2 = Array.IndexOf(Enum.GetValues(parsedtype2.GetType()), parsedtype2);
            type2.sprite = typeImages[index2];
        }
        else
        {
            type2.sprite = empty;
        }

        LerpSlider(hpSlider.value, data.stats.pv + 20, hpSlider);
        //hpSlider.value = data.stats.pv + 20;
        hpTween.Stop();
        hpTween = Tween.Color(hpSliderIMG, startValue: hpSliderIMG.color,
            endValue: gradient.Evaluate((data.stats.pv + 20) / hpSlider.maxValue), duration: 0.5f);
        //hpSliderIMG.color = gradient.Evaluate((data.stats.pv + 20) / hpSlider.maxValue);
        LerpSlider(atkSlider.value, data.stats.atk + 20, atkSlider);
        //atkSlider.value = data.stats.atk + 20;
        atkTween.Stop();
        atkTween = Tween.Color(atkSliderIMG, startValue: atkSliderIMG.color,
            endValue: gradient.Evaluate((data.stats.atk + 20) / atkSlider.maxValue), duration: 0.5f);
        //atkSliderIMG.color = gradient.Evaluate((data.stats.atk + 20) / atkSlider.maxValue);
        LerpSlider(defSlider.value, data.stats.def + 20, defSlider);
        //defSlider.value = data.stats.def + 20;
        defTween.Stop();
        defTween = Tween.Color(defSliderIMG, startValue: defSliderIMG.color,
            endValue: gradient.Evaluate((data.stats.def + 20) / defSlider.maxValue), duration: 0.5f);
        //defSliderIMG.color = gradient.Evaluate((data.stats.def + 20) / defSlider.maxValue);
        LerpSlider(atkspeSlider.value, data.stats.atkSpe + 20, atkspeSlider);
        //atkspeSlider.value = data.stats.atkSpe + 20;
        atkSpeTween.Stop();
        atkSpeTween = Tween.Color(atkSpeSliderIMG, startValue: atkSpeSliderIMG.color,
            endValue: gradient.Evaluate((data.stats.atkSpe + 20) / atkspeSlider.maxValue), duration: 0.5f);
        //atkSpeSliderIMG.color = gradient.Evaluate((data.stats.atkSpe + 20) / atkspeSlider.maxValue);
        LerpSlider(defspeSlider.value, data.stats.defSpe + 20, defspeSlider);
        //defspeSlider.value = data.stats.defSpe + 20;
        defSpeTween.Stop();
        defSpeTween = Tween.Color(defSpeSliderIMG, startValue: defSpeSliderIMG.color,
            endValue: gradient.Evaluate((data.stats.defSpe + 20) / defspeSlider.maxValue), duration: 0.5f);
        //defSpeSliderIMG.color = gradient.Evaluate((data.stats.defSpe + 20 )/ defspeSlider.maxValue);
        LerpSlider(speedSlider.value, data.stats.speed + 20, speedSlider);
        //speedSlider.value = data.stats.speed + 20;
        speedTween.Stop();
        speedTween = Tween.Color(speedSliderIMG, startValue: speedSliderIMG.color,
            endValue: gradient.Evaluate((data.stats.speed + 20) / speedSlider.maxValue), duration: 0.5f);
        //speedSliderIMG.color = gradient.Evaluate((data.stats.speed + 20) / speedSlider.maxValue);
        hpSliderText.text = data.stats.pv.ToString();
        atkSliderText.text = data.stats.atk.ToString();
        defSliderText.text = data.stats.def.ToString();
        atkspeSliderText.text = data.stats.atkSpe.ToString();
        defspeSliderText.text = data.stats.defSpe.ToString();
        speedSliderText.text = data.stats.speed.ToString();
        totalStats.text =
            ($"TOTAL = {data.stats.pv + data.stats.atk + data.stats.def + data.stats.atkSpe + data.stats.defSpe + data.stats.speed}")
            .ToString();
        StartCoroutine(LoadImage(imgIcon, id));
        StartCoroutine(GetAudio());
        //StartCoroutine(FindWeaknesses(types));
        //StartCoroutine(GetEvolutionTree(data));
        everythingloaded = true;
    }

    public void EvolutionTreeLoad()
    {
        if (!isEvolutionLoaded)
        {
            StartCoroutine(GetEvolutionTree(data));
        }
    }

    public void ClickOnEvo(EvoObject evo)
    {
        if (evo.id < databaseManager.database.datas.Count)
        {
            id = evo.id;
            StartCoroutine(GetPokemon());
        }
    }

    private IEnumerator GetDamageRelations1()
    {
        result1 = new();
        List<Type> typeList = new();
        Type parsedtype1 = (Type)Enum.Parse(typeof(Type), databaseManager.database.datas[poke1id - 1].type1, true);
        typeList.Add(parsedtype1);
        if (databaseManager.database.datas[poke1id - 1].type2 != "")
        {
            Type parsedtype2 = (Type)Enum.Parse(typeof(Type), databaseManager.database.datas[poke1id - 1].type2, true);

            typeList.Add(parsedtype2);
        }

        yield return StartCoroutine(FindWeaknesses(typeList, result1));
        GameManagerStatic.firstPokemonDamageRelation = result1;
    }

    private IEnumerator WeaknessesDisplay(List<Type> typelist, List<Damage> damage)
    {
        yield return StartCoroutine(FindWeaknesses(typelist, damage));
        yield return StartCoroutine(DisplayWeaknesses());
    }

    private IEnumerator DisplayWeaknesses()
    {
        int index1 = 0;
        weaknesses.Sort((x, y) => -x.multiplier.CompareTo(y.multiplier));

        foreach (Damage type in weaknesses)
        {
            int imgIndex = Array.IndexOf(Enum.GetValues(type.type.GetType()), type.type);
            if (index1 < weaknessesImage.Length + 1)
            {
                weaknessesImage[index1].sprite = typeImages[imgIndex];
                weaknesstext[index1].text = $"{type.multiplier}x";
                index1++;
            }
        }

        int index2 = 0;
        resistances.Sort((x, y) => -x.multiplier.CompareTo(y.multiplier));
        foreach (Damage type in resistances)
        {
            int imgIndex = Array.IndexOf(Enum.GetValues(type.type.GetType()), type.type);
            if (index2 < resistancesImage.Length + 1)
            {
                resistancesImage[index2].sprite = typeImages[imgIndex];
                resistancetext[index2].text = $"{type.multiplier}x";
                index2++;
            }
        }

        isWeaknessLoaded = true;
        yield return null;
    }

    public void WeaknessesLoad()
    {
        if (!isWeaknessLoaded)
        {
            foreach (Image sprite in weaknessesImage)
            {
                sprite.sprite = empty;
            }

            foreach (Image sprite in resistancesImage)
            {
                sprite.sprite = empty;
            }

            foreach (TextMeshProUGUI textm in weaknesstext)
            {
                textm.text = "";
            }

            foreach (TextMeshProUGUI textm in resistancetext)
            {
                textm.text = "";
            }

            StartCoroutine(FindWeaknesses(types, damages));
        }
    }

    IEnumerator GetEvolutionTree(PokemonData data)
    {
        foreach (var evoobject in evoObjectsBase)
        {
            Destroy(evoobject);
        }

        foreach (var evoobject in evoObjectsMid)
        {
            Destroy(evoobject);
        }

        foreach (var evoobject in evoObjectsMax)
        {
            Destroy(evoobject);
        }

        UnityWebRequest www = UnityWebRequest.Get(data.evolutionurl);
        yield return www.SendWebRequest();

        if (www.downloadHandler.text != null)
        {
            Evolution evolutionChain = JsonConvert.DeserializeObject<Evolution>(www.downloadHandler.text);
            GameObject evo0 = Instantiate(evoObjPrefab, evoBasePanel.transform);
            Image evo0img = evo0.GetComponentInChildren<Image>();
            TextMeshProUGUI evo0text = evo0.GetComponentInChildren<TextMeshProUGUI>();
            string newid = evolutionChain.chain.species.url.Remove(0, 42);
            newid = newid.Replace("/", "");
            int newidint = Int32.Parse(newid);
            StartCoroutine(LoadImage(evo0img, newidint));
            evo0text.text = evolutionChain.chain.species.name;
            evo0text.text = (char.ToUpper(evo0text.text[0]) + evo0text.text.Substring(1));
            evo0.GetComponentInChildren<EvoObject>().id = newidint;
            evoObjectsBase.Add(evo0);

            foreach (var evolution in evolutionChain.chain.evolves_to)
            {
                GameObject evo1 = Instantiate(evoObjPrefab, evoMidPanel.transform);
                Image evo1img = evo1.GetComponentInChildren<Image>();
                TextMeshProUGUI evo1text = evo1.GetComponentInChildren<TextMeshProUGUI>();
                string newid1 = evolution.species.url.Remove(0, 42);
                newid1 = newid1.Replace("/", "");
                int newidint1 = Int32.Parse(newid1);
                StartCoroutine(LoadImage(evo1img, newidint1));
                evo1text.text = evolution.species.name;
                evo1text.text = (char.ToUpper(evo1text.text[0]) + evo1text.text.Substring(1));
                evo1.GetComponentInChildren<EvoObject>().id = newidint1;
                evoObjectsMid.Add(evo1);

                foreach (var evo in evolution.evolves_to)
                {
                    GameObject evo2 = Instantiate(evoObjPrefab, evoMaxPanel.transform);
                    Image evo2img = evo2.GetComponentInChildren<Image>();
                    TextMeshProUGUI evo2text = evo2.GetComponentInChildren<TextMeshProUGUI>();
                    string newid2 = evo.species.url.Remove(0, 42);
                    newid2 = newid2.Replace("/", "");
                    int newidint2 = Int32.Parse(newid2);
                    StartCoroutine(LoadImage(evo2img, newidint2));
                    evo2text.text = evo.species.name;
                    evo2text.text = (char.ToUpper(evo2text.text[0]) + evo2text.text.Substring(1));
                    evo2.GetComponentInChildren<EvoObject>().id = newidint2;
                    evoObjectsMax.Add(evo2);
                }
            }
        }

        isEvolutionLoaded = true;
        yield return null;
    }

    private IEnumerator FindWeaknesses(List<Type> typelist, List<Damage> alldamage)
    {
        weaknesses = new();
        resistances = new();
        immunities = new();
        alldamage.Clear();
        List<Type> weaknesses1 = new();
        List<Type> weaknesses2 = new();
        List<Type> resistances1 = new();
        List<Type> resistances2 = new();
        List<Type> immunities1 = new();
        List<Type> immunities2 = new();
        yield return GetTypes(typelist[0], weaknesses1, resistances1, immunities1);
        if (typelist.Count > 1)
        {
            yield return GetTypes(typelist[1], weaknesses2, resistances2, immunities2);
        }

        for (int i = 0; i < Enum.GetNames(typeof(Type)).Length; i++)
        {
            alldamage.Add(new Damage((Type)i, 1));
        }

        alldamage.ForEach(type =>
        {
            if (weaknesses1.Contains(type.type))
            {
                type.multiplier *= 2;
            }

            ;

            if (resistances1.Contains(type.type))
            {
                type.multiplier *= 0.5f;
            }

            if (immunities1.Contains(type.type))
            {
                type.multiplier = 0;
            }

            if (typelist.Count > 1)
            {
                if (weaknesses2.Contains(type.type))
                {
                    type.multiplier *= 2;
                }

                if (resistances2.Contains(type.type))
                {
                    type.multiplier *= 0.5f;
                }

                if (immunities2.Contains(type.type))
                {
                    type.multiplier = 0;
                }
            }
        });
        foreach (Damage type in alldamage)
        {
            if (type.multiplier > 1)
            {
                weaknesses.Add(type);
            }

            if (type.multiplier < 1)
            {
                resistances.Add(type);
            }

            if (type.multiplier == 0)
            {
                immunities.Add(type);
            }
        }

        int index1 = 0;
        weaknesses.Sort((x, y) => -x.multiplier.CompareTo(y.multiplier));

        foreach (Damage type in weaknesses)
        {
            int imgIndex = Array.IndexOf(Enum.GetValues(type.type.GetType()), type.type);
            if (index1 < weaknessesImage.Length)
            {
                weaknessesImage[index1].sprite = typeImages[imgIndex];
                weaknesstext[index1].text = $"{type.multiplier}x";
                index1++;
            }
        }

        int index2 = 0;
        resistances.Sort((x, y) => -x.multiplier.CompareTo(y.multiplier));
        foreach (Damage type in resistances)
        {
            int imgIndex = Array.IndexOf(Enum.GetValues(type.type.GetType()), type.type);
            if (index2 < resistancesImage.Length)
            {
                resistancesImage[index2].sprite = typeImages[imgIndex];
                resistancetext[index2].text = $"{type.multiplier}x";
                index2++;
            }
        }

        isWeaknessLoaded = true;
        yield return null;
    }


    IEnumerator GetTypes(Type type, List<Type> weaknesses, List<Type> resistances, List<Type> immunities)
    {
        UnityWebRequest www = UnityWebRequest.Get($"https://pokeapi.co/api/v2/type/{type}");
        yield return www.SendWebRequest();
        APIType typeRelations = JsonConvert.DeserializeObject<APIType>(www.downloadHandler.text);

        typeRelations.damage_relations.double_damage_from.ForEach(element =>
        {
            weaknesses.Add((Type)Enum.Parse(typeof(Type), element.name, true));
        });
        typeRelations.damage_relations.half_damage_from.ForEach(element =>
        {
            resistances.Add((Type)Enum.Parse(typeof(Type), element.name, true));
        });
        typeRelations.damage_relations.no_damage_from.ForEach(element =>
        {
            immunities.Add((Type)Enum.Parse(typeof(Type), element.name, true));
        });
    }

    IEnumerator LoadImage(Image imgIcon, int id)
    {
        UnityWebRequest www =
            UnityWebRequestTexture.GetTexture(
                $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{id}.png");
        yield return www.SendWebRequest();
        Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        texture.filterMode = FilterMode.Point;
        Sprite newsprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        imgIcon.sprite = newsprite;
    }

    IEnumerator GetAudio()
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(
            $"https://raw.githubusercontent.com/PokeAPI/cries/main/cries/pokemon/latest/{id}.ogg", AudioType.OGGVORBIS);
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                myClip = DownloadHandlerAudioClip.GetContent(www);
                audiosource.clip = myClip;
                audiosource.Play();
            }
        }
    }
}