using UnityEngine;

public class Ability : MonoBehaviour
{
    public SpellData data;
    public LayerMask spell;

    /*public void Cast() 
    {
        if (PlayerStats.mana > data.manaCost) 
        {
            PlayerStats.mana -= data.manaCost
            Instantiate(data.spellPrefab, castPoint, rotation);
        }
    }*/
}
