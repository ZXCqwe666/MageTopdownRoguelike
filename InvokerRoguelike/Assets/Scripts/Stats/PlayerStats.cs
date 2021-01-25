using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;

    public PlayerStatsData[] defaultPlayerStatsData;
    public PlayerStatsData[] currentPlayerStatsData;

    private void Awake()
    {
        instance = this;
    }
}
