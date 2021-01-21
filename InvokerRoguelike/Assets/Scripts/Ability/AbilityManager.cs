using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager instance;

    public SpellData[] Spells;
    private Dictionary<string, SpellInfo> SpellInfos;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Initialize();
    }
    public void Cast(string _spell)
    { 
        if (SpellInfos[_spell].lastCastTime + SpellInfos[_spell].spellData.coolDown < Time.time) 
        {
            SpellInfos[_spell].SetTime();
            Debug.Log("Casted" + _spell);
        }
    }
    private void Initialize()
    {
        SpellInfos = new Dictionary<string, SpellInfo>() 
        {
            {"firefirefire", new SpellInfo(Spells[0])}, {"firefireice", new SpellInfo(Spells[1])}, {"firefirenature", new SpellInfo(Spells[2])}, {"firefirestorm", new SpellInfo(Spells[3])}, {"firefirechaos", new SpellInfo(Spells[4])},
            {"iceiceice", new SpellInfo(Spells[5])}, {"fireiceice", new SpellInfo(Spells[6])}, {"iceicenature", new SpellInfo(Spells[7])}, {"iceicestorm", new SpellInfo(Spells[8])}, {"iceicechaos", new SpellInfo(Spells[9])},
            {"naturenaturenature", new SpellInfo(Spells[10])}, {"firenaturenature", new SpellInfo(Spells[11])}, {"icenaturenature", new SpellInfo(Spells[12])}, {"naturenaturestorm", new SpellInfo(Spells[13])}, {"naturenaturechaos", new SpellInfo(Spells[14])},
            {"stormstormstorm", new SpellInfo(Spells[15])}, {"firestormstorm", new SpellInfo(Spells[16])}, {"icestormstorm", new SpellInfo(Spells[17])}, {"naturestormstorm", new SpellInfo(Spells[18])}, {"stormstormchaos", new SpellInfo(Spells[19])},
            {"chaoschaoschaos", new SpellInfo(Spells[20])}, {"firechaoschaos", new SpellInfo(Spells[21])}, {"icechaoschaos", new SpellInfo(Spells[22])}, {"naturechaoschaos", new SpellInfo(Spells[23])}, {"stormchaoschaos", new SpellInfo(Spells[24])},
            {"mix", new SpellInfo(Spells[25])}
        };
    }
    public struct SpellInfo
    {
        public float lastCastTime;
        public SpellData spellData;
        public SpellInfo(SpellData _spellData)
        {
            spellData = _spellData;
            lastCastTime = -_spellData.coolDown;
        }
        public void SetTime()
        {
            lastCastTime = Time.time;
        }
    }
}
