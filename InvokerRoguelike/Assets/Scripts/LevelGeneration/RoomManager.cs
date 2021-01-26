using UnityEngine.Tilemaps;
using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    private TilemapCollider2D tileCollider;
    public Door[] doors;

    public List<Transform> spawnPoints;
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
}
