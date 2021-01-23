using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using Unity.Mathematics;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private Dictionary<Vector2, Room> spawnedRooms;
    private List<Room> roomVariants;
    private Tilemap levelWallTilemap, levelFloorTilemap;
    public LevelData[] levelDatas;

    void Start()
    {
        roomVariants = new List<Room>()
        {
            new Room(true,false,false,false), new Room(false,true,false,false), new Room(false,false,true,false), new Room(false,false,false,true), // 1openings
            new Room(true,false,true,false), new Room(true,false,false,true), new Room(false,true,true,false), new Room(false,true,false,true), // 2openings
            new Room(true,true,false,false), new Room(false,false,true,true), // 2openings (straight)
            new Room(true,true,true,false), new Room(false,true,true,true), new Room(true,false,true,true), new Room(true,true,false,true), // 3openings
            new Room(true,true,true,true) // 4openings
        };
        InitializeDungeonGenerator();
        StartCoroutine(GenerateMap(levelDatas[0]));
    }
    private IEnumerator GenerateMap(LevelData levelData)
    {
        ResetDungeon();
        SpawnRoom(spawnedRooms[Vector2.zero], levelData);

        int _roomsToSpawn = UnityEngine.Random.Range(levelData.roomToSpawnAmount.x, levelData.roomToSpawnAmount.y);

        while (_roomsToSpawn > 0)
        {
            IEnumerable<Room> roomQuery = spawnedRooms.Values.Where(room => room.up == true || room.down == true || room.left == true || room.right == true);
            List<Room> nonClosedRooms = roomQuery.ToList();

            if (nonClosedRooms.Count == 0)
            {
                ResetDungeon();
                Debug.LogError("error occured");
                StartCoroutine(GenerateMap(levelData));
                break;
            }

            Room randomRoom = nonClosedRooms[UnityEngine.Random.Range(0, nonClosedRooms.Count)];
            Vector2 randomRoomPosition = randomRoom.position;

            List<Vector2> ValidDirections = new List<Vector2>();
            if (randomRoom.up) ValidDirections.Add(Vector2.up);
            if (randomRoom.down) ValidDirections.Add(Vector2.down);
            if (randomRoom.left) ValidDirections.Add(Vector2.left);
            if (randomRoom.right) ValidDirections.Add(Vector2.right);
            Vector2 addedRoomPosition = randomRoomPosition + ValidDirections[UnityEngine.Random.Range(0, ValidDirections.Count)];

            bool[] hasRoomNear = new bool[4]
            {
                spawnedRooms.ContainsKey(addedRoomPosition + Vector2.up),
                spawnedRooms.ContainsKey(addedRoomPosition + Vector2.down),
                spawnedRooms.ContainsKey(addedRoomPosition + Vector2.left),
                spawnedRooms.ContainsKey(addedRoomPosition + Vector2.right)
            };
            bool[] requiredPaths = new bool[4]
            {
                hasRoomNear[0] && spawnedRooms[addedRoomPosition + Vector2.up].down,
                hasRoomNear[1] && spawnedRooms[addedRoomPosition + Vector2.down].up,
                hasRoomNear[2] && spawnedRooms[addedRoomPosition + Vector2.left].right,
                hasRoomNear[3] && spawnedRooms[addedRoomPosition + Vector2.right].left
            };
            bool[] blockedPaths = new bool[4]
            {
                hasRoomNear[0] && spawnedRooms[addedRoomPosition + Vector2.up].down == false,
                hasRoomNear[1] && spawnedRooms[addedRoomPosition + Vector2.down].up == false,
                hasRoomNear[2] && spawnedRooms[addedRoomPosition + Vector2.left].right == false,
                hasRoomNear[3] && spawnedRooms[addedRoomPosition + Vector2.right].left == false
            };

            List<Room> validRoomsToAdd = new List<Room>();

            int iterationStart = 4;
            if (nonClosedRooms.Count > 4)
            {
                iterationStart = 0;
            }
            for (int i = iterationStart; i < roomVariants.Count; i++)
            {
                bool isRoomValid = true;
                if ((requiredPaths[0] == true && roomVariants[i].up == false) || (requiredPaths[1] == true && roomVariants[i].down == false)
                    || (requiredPaths[2] == true && roomVariants[i].left == false) || (requiredPaths[3] == true && roomVariants[i].right == false))
                    isRoomValid = false;
                if ((blockedPaths[0] == true && roomVariants[i].up == true) || (blockedPaths[1] == true && roomVariants[i].down == true)
                    || (blockedPaths[2] == true && roomVariants[i].left == true) || (blockedPaths[3] == true && roomVariants[i].right == true))
                    isRoomValid = false;
                if (isRoomValid) validRoomsToAdd.Add(roomVariants[i]);
            }
            if (validRoomsToAdd.Count == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    bool isRoomValid = true;
                    if ((requiredPaths[0] == true && roomVariants[i].up == false) || (requiredPaths[1] == true && roomVariants[i].down == false)
                        || (requiredPaths[2] == true && roomVariants[i].left == false) || (requiredPaths[3] == true && roomVariants[i].right == false))
                        isRoomValid = false;
                    if ((blockedPaths[0] == true && roomVariants[i].up == true) || (blockedPaths[1] == true && roomVariants[i].down == true)
                        || (blockedPaths[2] == true && roomVariants[i].left == true) || (blockedPaths[3] == true && roomVariants[i].right == true))
                        isRoomValid = false;
                    if (isRoomValid) validRoomsToAdd.Add(roomVariants[i]);
                }
            }

            Room chosenRoomToAdd = validRoomsToAdd[UnityEngine.Random.Range(0, validRoomsToAdd.Count)];
            chosenRoomToAdd.position = addedRoomPosition;
            SpawnRoom(chosenRoomToAdd, levelData);

            if (requiredPaths[0])
            {
                chosenRoomToAdd.up = false; Room copy = spawnedRooms[addedRoomPosition + Vector2.up];
                copy.down = false; spawnedRooms[addedRoomPosition + Vector2.up] = copy;
            }
            if (requiredPaths[1])
            {
                chosenRoomToAdd.down = false; Room copy = spawnedRooms[addedRoomPosition + Vector2.down];
                copy.up = false; spawnedRooms[addedRoomPosition + Vector2.down] = copy;
            }
            if (requiredPaths[2])
            {
                chosenRoomToAdd.left = false; Room copy = spawnedRooms[addedRoomPosition + Vector2.left];
                copy.right = false; spawnedRooms[addedRoomPosition + Vector2.left] = copy;
            }
            if (requiredPaths[3])
            {
                chosenRoomToAdd.right = false; Room copy = spawnedRooms[addedRoomPosition + Vector2.right];
                copy.left = false; spawnedRooms[addedRoomPosition + Vector2.right] = copy;
            }
            spawnedRooms.Add(addedRoomPosition, chosenRoomToAdd);
            _roomsToSpawn--;
            yield return new WaitForSeconds(0.005f);
        }
        if(_roomsToSpawn == 0)
        {
            CloseOpenedRooms(levelData);
            levelWallTilemap.GetComponent<CompositeCollider2D>().GenerateGeometry();
        }
    }
    private void ResetDungeon()
    {
        levelWallTilemap.ClearAllTiles();
        levelFloorTilemap.ClearAllTiles();
        levelWallTilemap.GetComponent<CompositeCollider2D>().GenerateGeometry();
        spawnedRooms = new Dictionary<Vector2, Room>
        { { Vector2.zero, roomVariants[14] } };
    }
    private void SpawnRoom(Room chosenRoom, LevelData _levelData)
    {
        Transform roomTransform = Resources.Load<GameObject>(_levelData.levelName + "/castle" + UnityEngine.Random.Range(1, _levelData.availableRoomAssets + 1).ToString()).transform; 
        Tilemap wallTilemap = roomTransform.Find("RoomGrid").transform.Find("wall_tilemap").GetComponent<Tilemap>();
        Tilemap floorTilemap = roomTransform.Find("RoomGrid").transform.Find("floor_tilemap").GetComponent<Tilemap>();

        int2 offset = new int2(_levelData.roomSize.x * (int)chosenRoom.position.x, _levelData.roomSize.y * (int)chosenRoom.position.y);

        for (int y = 0; y < _levelData.roomSize.y; y++)
        {
            for (int x = 0; x < _levelData.roomSize.x; x++)
            {
                levelWallTilemap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), wallTilemap.GetTile(new Vector3Int(x, y, 0)));
                levelFloorTilemap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), floorTilemap.GetTile(new Vector3Int(x, y, 0)));
            }
        }
        if (chosenRoom.up) CutWays(false, 1, _levelData.wallTile, offset, new int2(_levelData.roomSize.x / 2 - 1, _levelData.roomSize.y - 3));
        if (chosenRoom.down) CutWays(false, 0, _levelData.wallTile, offset, new int2(_levelData.roomSize.x / 2 - 1, 0));
        if (chosenRoom.left) CutWays(true, 0, _levelData.wallTile, offset, new int2(0, _levelData.roomSize.y / 2 - 1));
        if (chosenRoom.right) CutWays(true, 0, _levelData.wallTile, offset, new int2(_levelData.roomSize.x - 2, _levelData.roomSize.y / 2 - 1));
    }
    private void CloseOpenedRooms(LevelData _levelData)
    {
        IEnumerable<Room> roomQuery = spawnedRooms.Values.Where(room => room.up == true || room.down == true || room.left == true || room.right == true);
        List<Room> nonClosedRooms = roomQuery.ToList();

        foreach(Room room in nonClosedRooms)
        {
            int2 offset = new int2(_levelData.roomSize.x * (int)room.position.x, _levelData.roomSize.y * (int)room.position.y);
            if (room.up) FillWays(true, _levelData.wallTile, _levelData.wallTopTile, offset, new int2(_levelData.roomSize.x / 2 - 1, _levelData.roomSize.y - 2));
            if (room.down) FillWays(false, _levelData.wallTile, _levelData.wallTopTile, offset, new int2(_levelData.roomSize.x / 2 - 1, 0));
            if (room.left) FillWays(false, _levelData.wallTile, _levelData.wallTopTile, offset, new int2(0, _levelData.roomSize.y / 2 - 1));
            if (room.right) FillWays(false, _levelData.wallTile, _levelData.wallTopTile, offset, new int2(_levelData.roomSize.x - 2, _levelData.roomSize.y / 2 - 1));
        }
    }
    private void CutWays(bool isSide, int addedIterations, TileBase wall, int2 offset, int2 start)
    {
        for (int y = start.y; y < start.y + 2 + addedIterations; y++)
        {
            for (int x = start.x; x < start.x + 2; x++)
                levelWallTilemap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), null);
        }
        if (isSide)
        {
            levelWallTilemap.SetTile(new Vector3Int(start.x + offset.x, start.y + 1 + offset.y, 0), wall);
            levelWallTilemap.SetTile(new Vector3Int(start.x + 1 + offset.x, start.y + 1 + offset.y, 0), wall);
        }
    }
    private void FillWays(bool isUp, TileBase wall, TileBase wallTop, int2 offset, int2 start)
    {
        for (int y = start.y; y < start.y + 2; y++)
        {
            for (int x = start.x; x < start.x + 2; x++)
                levelWallTilemap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), wallTop);
        }
        if (isUp)
        {
            levelWallTilemap.SetTile(new Vector3Int(start.x + offset.x, start.y -1 + offset.y, 0), wall);
            levelWallTilemap.SetTile(new Vector3Int(start.x + 1 + offset.x, start.y -1 + offset.y, 0), wall);
        }
    }
    private void InitializeDungeonGenerator()
    {
        levelWallTilemap = transform.Find("LevelGrid").transform.Find("wall_tilemap").GetComponent<Tilemap>();
        levelFloorTilemap = transform.Find("LevelGrid").transform.Find("floor_tilemap").GetComponent<Tilemap>();
        UnityEngine.Random.InitState(UnityEngine.Random.Range(0, int.MaxValue));
    }
}
public struct Room
{
    public Vector2 position;
    public bool up, down, left, right;
    public Room(bool _up, bool _down, bool _left, bool _right)
    {
        position = Vector2.zero;
        up = _up; down = _down;
        left = _left; right = _right;
    }
}