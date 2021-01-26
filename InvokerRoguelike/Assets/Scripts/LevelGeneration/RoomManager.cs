using UnityEngine.Tilemaps;
using UnityEngine;
using System.Collections.Generic;

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
        if(collision.CompareTag("Player"))
        { 
            tileCollider.enabled = false;
            foreach(Door door in doors)
            {
                door.CloseDoor();
            }
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

        #if UNITY_EDITOR
        if(spawnPointsHolder.childCount != enemyIdsToSpawn.Length)
        {
            Debug.LogError("Spawn points doesnt match enemies amount"); //Error debug, delete in final version 
        }
        #endif
        for(int i = 0; i < spawnPointsHolder.childCount; i++)
        {
            spawnPoints.Add(spawnPointsHolder.GetChild(i));
        }
    }
    public void SetDoors(bool up, bool down, bool left, bool right, LevelData levelData)
    {
        doors = new List<Door>();
        Vector3 position = transform.position;
        if (up)
            InstantiateDoor(levelData.levelName, "doorHorizontal", new Vector3(levelData.roomSize.x / 2, levelData.roomSize.y - 2.5f, 0) + position); // 1 0.5
        if (down)
            InstantiateDoor(levelData.levelName, "doorHorizontal", new Vector3(levelData.roomSize.x / 2, -0.5f, 0) + position);//1 -0.5
        if (left)
            InstantiateDoor(levelData.levelName, "doorSide", new Vector3(0.5f, levelData.roomSize.y / 2 - 2f, 0) + position );//1 -1.5
        if (right)
            InstantiateDoor(levelData.levelName, "doorSide", new Vector3(levelData.roomSize.x - 2 + 1.5f, levelData.roomSize.y / 2 - 2f, 0) + position);//X +1,5 Y-1
    }

    private void InstantiateDoor(string levelName, string objectName, Vector3 position )
    {
        doors.Add(Instantiate(Resources.Load<GameObject>("rooms/" + levelName + "/doors/" + objectName), position, Quaternion.identity, transform).GetComponent<Door>());
    }
}
