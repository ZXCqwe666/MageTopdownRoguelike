using UnityEngine;

[CreateAssetMenu(fileName = "stats", menuName = "new Stats")]
public class PlayerStatsData : ScriptableObject
{
    public int maxHealth;
    public float mana;
    public int speed;
    public int acceleration;
}
