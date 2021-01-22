using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager instance;

    public SpellData[] Spells;
    private Dictionary<string, SpellData> SpellDatas;
    private Dictionary<string, float> SpellCoolDowns;
   

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
        SpellData spellData = SpellDatas[_spell];
        if (SpellCoolDowns[_spell] + spellData.coolDown < Time.time) 
        {
            Debug.Log("Casted" + _spell);
            SpellCoolDowns[_spell] = Time.time;
            if (spellData.spellType == SpellType.mouseCast) 
            {
                Instantiate(spellData.spellPrefab, CameraController.instance.GetMousePosition(), Quaternion.identity, transform);
            }
            else if (spellData.spellType == SpellType.directional) 
            {
                Instantiate(spellData.spellPrefab, CameraController.instance.GetPlayerPosition(), CameraController.instance.GetMouseDirection(), transform);
            }
            else if (spellData.spellType == SpellType.selfCast)
            {
                Instantiate(spellData.spellPrefab, Vector3.zero, Quaternion.identity, CameraController.instance.player);
            }
        }
    }
    private void Initialize()
    {
        SpellDatas = new Dictionary<string, SpellData>() 
        {
            {"firefirefire", Spells[0]}, {"firefireice", Spells[1]}, {"firefirenature", Spells[2]}, {"firefirestorm", Spells[3]}, {"firefirechaos", Spells[4]},
            {"iceiceice", Spells[5]}, {"fireiceice", Spells[6]}, {"iceicenature", Spells[7]}, {"iceicestorm", Spells[8]}, {"iceicechaos", Spells[9]},
            {"naturenaturenature", Spells[10]}, {"firenaturenature", Spells[11]}, {"icenaturenature", Spells[12]}, {"naturenaturestorm", Spells[13]}, {"naturenaturechaos", Spells[14]},
            {"stormstormstorm", Spells[15]}, {"firestormstorm", Spells[16]}, {"icestormstorm", Spells[17]}, {"naturestormstorm", Spells[18]}, {"stormstormchaos", Spells[19]},
            {"chaoschaoschaos", Spells[20]}, {"firechaoschaos", Spells[21]}, {"icechaoschaos", Spells[22]}, {"naturechaoschaos", Spells[23]}, {"stormchaoschaos", Spells[24]},
            {"mix", Spells[25]}
        };
        SpellCoolDowns = new Dictionary<string, float>()
        {
            {"firefirefire", -999f}, {"firefireice", -999f}, {"firefirenature", -999f}, {"firefirestorm", -999f}, {"firefirechaos", -999f},
            {"iceiceice", -999f}, {"fireiceice", -999f}, {"iceicenature", -999f}, {"iceicestorm", -999f}, {"iceicechaos", -999f},
            {"naturenaturenature", -999f}, {"firenaturenature", -999f}, {"icenaturenature", -999f}, {"naturenaturestorm", -999f}, {"naturenaturechaos", -999f},
            {"stormstormstorm", -999f}, {"firestormstorm", -999f}, {"icestormstorm", -999f}, {"naturestormstorm", -999f}, {"stormstormchaos", -999f},
            {"chaoschaoschaos", -999f}, {"firechaoschaos", -999f}, {"icechaoschaos", -999f}, {"naturechaoschaos", -999f}, {"stormchaoschaos", -999f},
            {"mix", -999f}
        };
    }
}
