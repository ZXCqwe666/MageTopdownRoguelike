using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private TilemapCollider2D tileCollider;
    private List<Door> doors;

    private List<Transform> spawnPoints;
    public int[] enemyIdsToSpawn;
    private int enemyCount;

    private void Start()
    {
        InitializeRoomManager();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PlayerController playerController))
        {
            foreach (Door door in doors)
            {
                door.CloseDoor();
            }
            tileCollider.enabled = false;
            SpawnEnemies();
        }
    }
    private void SpawnEnemies()
    {
        for (int i = 0; i < enemyIdsToSpawn.Length; i++)
        {
            GameObject enemy = Instantiate(Resources.Load<GameObject>("enemies/" + enemyIdsToSpawn[i].ToString()), spawnPoints[i].position, Quaternion.identity);
            enemy.GetComponent<Health>().enemyDied += DecreaseEnemyCount;
            enemyCount++;
        }
    }
    public void DecreaseEnemyCount()
    {
        enemyCount --;
        if (enemyCount == 0)
            foreach (Door door in doors)
            {
                door.OpenDoor();
            }
    }
    private void InitializeRoomManager()
    {
        enemyCount = 0;
        tileCollider = GetComponent<TilemapCollider2D>();

        Transform spawnPointsHolder = transform.Find("spawnPointsHolder"); 
        spawnPoints = new List<Transform>();

        if(spawnPointsHolder.childCount != enemyIdsToSpawn.Length)
        {
            Debug.LogError("Spawn points doesnt match enemies amount"); //Error debug, delete in final version 
        }
        for(int i = 0; i < spawnPointsHolder.childCount; i++)
        {
            spawnPoints.Add(spawnPointsHolder.GetChild(i));
        }
    }
    public void SetDoors(bool up, bool down, bool left, bool right, LevelData levelData)
    {
        Vector3 position = transform.position;
        doors = new List<Door>();

        if (up)
            InstantiateDoor(levelData.levelName, "doorHorizontal", new Vector3(levelData.roomSize.x / 2, levelData.roomSize.y - 1.5f, 0) + position);
        if (down)
            InstantiateDoor(levelData.levelName, "doorHorizontal", new Vector3(levelData.roomSize.x / 2, 0.5f, 0) + position);
        if (left)
            InstantiateDoor(levelData.levelName, "doorSide", new Vector3(0.5f, levelData.roomSize.y / 2 - 1f, 0) + position );
        if (right)
            InstantiateDoor(levelData.levelName, "doorSide", new Vector3(levelData.roomSize.x - 2 + 1.5f, levelData.roomSize.y / 2 - 1f, 0) + position);
    }
    private void InstantiateDoor(string levelName, string objectName, Vector3 position )
    {
        GameObject door = Instantiate(Resources.Load<GameObject>("rooms/" + levelName + "/doors/" + objectName), position, Quaternion.identity, transform);
        doors.Add(door.GetComponent<Door>());
    }
}
