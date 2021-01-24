using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using Unity.Mathematics;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    private Dictionary<Vector2, Room> spawnedRooms;
    private List<Room> roomVariants;
    private Tilemap levelWallTilemap, levelFloorTilemap;
    public LevelData[] levelDatas;

    private const float roomGenerationStepTime = 0.05f;
    private const float boxFillStepTime = 0.025f;

    void Start()
    {
        InitializeDungeonGenerator();
        StartCoroutine(GenerateMap(levelDatas[0]));
    }
    private IEnumerator GenerateMap(LevelData levelData)
    {
        ResetDungeon(levelData);
        int roomsToSpawn = Random.Range(levelData.spawnAmount.x, levelData.spawnAmount.y);

        while (roomsToSpawn > 0)
        {
            #region RoomGenerationCycle

            IEnumerable<Room> roomQuery = spawnedRooms.Values.Where(room => room.up == true || room.down == true || room.left == true || room.right == true);
            List<Room> nonClosedRooms = roomQuery.ToList();

            if (nonClosedRooms.Count == 0) // 1 in 1000 reset
            {
                Debug.Log("all rooms are closed error");
                yield return GenerateMap(levelData); 
                break;
            }

            Room randomRoom = nonClosedRooms[Random.Range(0, nonClosedRooms.Count)];
            Vector2 randomRoomPosition = randomRoom.position;

            List<Vector2> ValidDirections = new List<Vector2>();
            if (randomRoom.up) ValidDirections.Add(Vector2.up);
            if (randomRoom.down) ValidDirections.Add(Vector2.down);
            if (randomRoom.left) ValidDirections.Add(Vector2.left);
            if (randomRoom.right) ValidDirections.Add(Vector2.right);
            Vector2 addPosition = randomRoomPosition + ValidDirections[Random.Range(0, ValidDirections.Count)];

            bool[] hasRoomNear = new bool[4]
            {
                spawnedRooms.ContainsKey(addPosition + Vector2.up),
                spawnedRooms.ContainsKey(addPosition + Vector2.down),
                spawnedRooms.ContainsKey(addPosition + Vector2.left),
                spawnedRooms.ContainsKey(addPosition + Vector2.right)
            };
            bool[] requiredPaths = new bool[4]
            {
                hasRoomNear[0] && spawnedRooms[addPosition + Vector2.up].down,
                hasRoomNear[1] && spawnedRooms[addPosition + Vector2.down].up,
                hasRoomNear[2] && spawnedRooms[addPosition + Vector2.left].right,
                hasRoomNear[3] && spawnedRooms[addPosition + Vector2.right].left
            };
            bool[] blockedPaths = new bool[4]
            {
                hasRoomNear[0] && spawnedRooms[addPosition + Vector2.up].down == false,
                hasRoomNear[1] && spawnedRooms[addPosition + Vector2.down].up == false,
                hasRoomNear[2] && spawnedRooms[addPosition + Vector2.left].right == false,
                hasRoomNear[3] && spawnedRooms[addPosition + Vector2.right].left == false
            };

            List<Room> validRoomsToAdd = new List<Room>();

            int iterationStart = 4;
            if (nonClosedRooms.Count > 4)
                iterationStart = 0;

            FindValidRooms(iterationStart, ref validRoomsToAdd, requiredPaths, blockedPaths);
            if (validRoomsToAdd.Count == 0)
                FindValidRooms(0, ref validRoomsToAdd, requiredPaths, blockedPaths);

            Room roomToAdd = validRoomsToAdd[Random.Range(0, validRoomsToAdd.Count)];
            roomToAdd.position = addPosition;
            SpawnRoom(roomToAdd, levelData, Random.Range(1, levelData.roomCount + 1).ToString(), true);

            if (requiredPaths[0])
            { roomToAdd.up = false; Room copy = spawnedRooms[addPosition + Vector2.up]; copy.down = false; spawnedRooms[addPosition + Vector2.up] = copy; }
            if (requiredPaths[1])
            { roomToAdd.down = false; Room copy = spawnedRooms[addPosition + Vector2.down]; copy.up = false; spawnedRooms[addPosition + Vector2.down] = copy; }
            if (requiredPaths[2]) 
            { roomToAdd.left = false; Room copy = spawnedRooms[addPosition + Vector2.left]; copy.right = false; spawnedRooms[addPosition + Vector2.left] = copy; }
            if (requiredPaths[3])
            { roomToAdd.right = false; Room copy = spawnedRooms[addPosition + Vector2.right]; copy.left = false; spawnedRooms[addPosition + Vector2.right] = copy; }

            spawnedRooms.Add(roomToAdd.position, roomToAdd);
            roomsToSpawn--;
            yield return new WaitForSeconds(roomGenerationStepTime);

            #endregion
        }
        if (roomsToSpawn == 0)
        {
            CloseOpenedRooms(levelData);
            levelWallTilemap.GetComponent<CompositeCollider2D>().GenerateGeometry();
            StartCoroutine(FillSurroundingArea(levelData));
        }
    }
    private void FindValidRooms(int iterationStart, ref List<Room> validRoomsToAdd, bool[] requiredPaths, bool[] blockedPaths)
    {
        for (int i = iterationStart; i < roomVariants.Count; i++)
        {
            if (!((requiredPaths[0] == true && roomVariants[i].up == false) || (requiredPaths[1] == true && roomVariants[i].down == false) ||
                (requiredPaths[2] == true && roomVariants[i].left == false) || (requiredPaths[3] == true && roomVariants[i].right == false) ||
                (blockedPaths[0] == true && roomVariants[i].up == true) || (blockedPaths[1] == true && roomVariants[i].down == true) ||
                (blockedPaths[2] == true && roomVariants[i].left == true) || (blockedPaths[3] == true && roomVariants[i].right == true)))
                validRoomsToAdd.Add(roomVariants[i]);
        }
    }
    private void ResetDungeon(LevelData levelData)
    {
        levelWallTilemap.ClearAllTiles();
        levelFloorTilemap.ClearAllTiles();
        levelWallTilemap.GetComponent<CompositeCollider2D>().GenerateGeometry();

        transform.position = new Vector3(-levelData.roomSize.x / 2f, -levelData.roomSize.y / 2f, 0f);
        spawnedRooms = new Dictionary<Vector2, Room> { { Vector2.zero, roomVariants[0] } };
        SpawnRoom(spawnedRooms[Vector2.zero], levelData, "startRoom", false);
    }
    private void SpawnRoom(Room chosenRoom, LevelData _levelData, string roomName, bool normalRoom)
    {
        GameObject loadedRoom = Resources.Load<GameObject>("rooms/" + _levelData.levelName + "/" + roomName);
        Transform roomTransform = loadedRoom.transform;
        Tilemap wallTilemap = roomTransform.Find("RoomGrid").transform.Find("wall_tilemap").GetComponent<Tilemap>();
        Tilemap floorTilemap = roomTransform.Find("RoomGrid").transform.Find("floor_tilemap").GetComponent<Tilemap>();

        if(normalRoom)
        {
            GameObject roomManager = Resources.Load<GameObject>("rooms/" + _levelData.levelName + "/" + roomName + "_manager");
            Vector3 roomManagerPosition = new Vector3(chosenRoom.position.x * _levelData.roomSize.x, chosenRoom.position.y * _levelData.roomSize.y, 0f);
            Vector3 offsetPosition = new Vector3(-_levelData.roomSize.x / 2f, -_levelData.roomSize.y / 2f, 0f);
            Instantiate(roomManager, roomManagerPosition + offsetPosition, Quaternion.identity, transform);
        }

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
    private IEnumerator FillSurroundingArea(LevelData _levelData)
    {
        BoundsInt bounds = levelWallTilemap.cellBounds;
        int2 roomAmount = new int2(bounds.size.x / _levelData.roomSize.x + 2, bounds.size.y / _levelData.roomSize.y + 2);

        int startX = bounds.xMin - _levelData.roomSize.x;
        int startY = bounds.yMin - _levelData.roomSize.y;
        int endX = bounds.xMax + _levelData.roomSize.x;
        int endY = bounds.yMax + _levelData.roomSize.y;

        levelWallTilemap.SetTile(new Vector3Int(startX, startY - 1, 0), _levelData.wallTopTile);   
        levelWallTilemap.SetTile(new Vector3Int(endX, endY + 1, 0), _levelData.wallTopTile);

        for(int y = 0; y < roomAmount.y; y++)
        {
            for (int x = 0; x < roomAmount.x; x++)
            {
                int bStartX = startX + x * _levelData.roomSize.x;
                int bStartY = startY + y * _levelData.roomSize.y;
                int bEndX = bStartX + _levelData.roomSize.x - 1; // inclusive
                int bEndY = bStartY + _levelData.roomSize.y - 1; // inclusive
                levelWallTilemap.BoxFill(new Vector3Int(bStartX, bStartY, 0), _levelData.wallTopTile, bStartX, bStartY, bEndX, bEndY);
            }
            yield return new WaitForSeconds(boxFillStepTime * roomAmount.x);
        }
    }
    private void InitializeDungeonGenerator()
    {
        roomVariants = new List<Room>()
        {
            new Room(true,false,false,false), new Room(false,true,false,false), new Room(false,false,true,false), new Room(false,false,false,true), // 1openings
            new Room(true,false,true,false), new Room(true,false,false,true), new Room(false,true,true,false), new Room(false,true,false,true), // 2openings
            new Room(true,true,false,false), new Room(false,false,true,true), // 2openings (straight)
            new Room(true,true,true,false), new Room(false,true,true,true), new Room(true,false,true,true), new Room(true,true,false,true), // 3openings
            new Room(true,true,true,true) // 4openings
        };
        levelWallTilemap = transform.Find("LevelGrid").transform.Find("wall_tilemap").GetComponent<Tilemap>();
        levelFloorTilemap = transform.Find("LevelGrid").transform.Find("floor_tilemap").GetComponent<Tilemap>();
        Random.InitState(Random.Range(0, int.MaxValue));
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
