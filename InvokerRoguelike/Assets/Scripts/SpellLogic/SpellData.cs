using UnityEngine;

[CreateAssetMenu(fileName = "spell", menuName = "new Spell")]
public class SpellData : ScriptableObject
{
    public int damage;
    public int manaCost;
    public int coolDown;
    public string spellName;

    public SpellType spellType;

    public GameObject spellPrefab;
}

public enum SpellType 
{
    directional,
    selfCast,
    mouseCast
}