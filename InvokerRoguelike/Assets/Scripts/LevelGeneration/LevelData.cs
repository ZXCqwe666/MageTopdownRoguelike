using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu (fileName = "levelData", menuName = "levelData")]
public class LevelData : ScriptableObject
{
    public TileBase wallTile, wallTopTile;
    public int roomCount;
    public string levelName;
    public int2 roomSize;
    public int2 spawnAmount;
}
