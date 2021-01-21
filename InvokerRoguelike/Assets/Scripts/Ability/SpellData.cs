using UnityEngine;

[CreateAssetMenu(fileName = "spell", menuName = "new Spell")]
public class SpellData : ScriptableObject
{
    public int damage;
    public int manaCost;
    public int coolDown;
    public string id;
    public string abilityName;

    public GameObject spellPrefab;

}
