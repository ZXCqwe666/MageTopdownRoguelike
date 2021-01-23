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

    private int2 roomSize = new int2(10, 10);
    private const int roomVariantsCount = 3;
    public int roomsCount;

    private Tilemap levelWallTilemap;
    public TileBase wallTile, wallTop;


    void Start()
    {
        levelWallTilemap = transform.Find("LevelGrid").transform.Find("wall_tilemap").GetComponent<Tilemap>();
        UnityEngine.Random.InitState(UnityEngine.Random.Range(0, int.MaxValue));
        roomVariants = new List<Room>()
        {
            new Room(true,false,false,false),
            new Room(false,true,false,false),
            new Room(false,false,true,false),
            new Room(false,false,false,true),

            new Room(true,false,true,false),
            new Room(true,false,false,true),
            new Room(false,true,true,false),
            new Room(false,true,false,true),
            new Room(true,true,false,false),
            new Room(false,false,true,true),

            new Room(true,true,true,false),
            new Room(false,true,true,true),
            new Room(true,false,true,true),
            new Room(true,true,false,true),

            new Room(true,true,true,true)
        };
        StartCoroutine(GenerateMap(roomsCount));
    }
    private IEnumerator GenerateMap(int roomCount)
    {
        spawnedRooms = new Dictionary<Vector2, Room>
        {
            { Vector2.zero, roomVariants[14] }
        };
        SpawnRoom(spawnedRooms[Vector2.zero]);

        while (roomCount > 0)
        {
            IEnumerable<Room> roomQuery = spawnedRooms.Values.Where(room => room.up == true || room.down == true || room.left == true || room.right == true);
            List<Room> nonClosedRooms = roomQuery.ToList();
            if (nonClosedRooms.Count == 0) { Debug.LogError("all rooms are closed"); }

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
            foreach (Room room in roomVariants)
            {
                bool isRoomValid = true;
                if ((requiredPaths[0] == true && room.up == false) || (requiredPaths[1] == true && room.down == false)
                    || (requiredPaths[2] == true && room.left == false) || (requiredPaths[3] == true && room.right == false))
                    isRoomValid = false;
                if ((blockedPaths[0] == true && room.up == true) || (blockedPaths[1] == true && room.down == true)
                    || (blockedPaths[2] == true && room.left == true) || (blockedPaths[3] == true && room.right == true))
                    isRoomValid = false;
                if (isRoomValid) validRoomsToAdd.Add(room);
            }
            if (validRoomsToAdd.Count == 0) Debug.Log("valid rooms are not found");

            Room chosenRoomToAdd = validRoomsToAdd[UnityEngine.Random.Range(0, validRoomsToAdd.Count)];
            chosenRoomToAdd.position = addedRoomPosition;
            SpawnRoom(chosenRoomToAdd);

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
            roomCount--;
            yield return new WaitForSeconds(0.1f);
        }
        CloseOpenedRooms();
    }
    private void SpawnRoom(Room chosenRoom)
    {
        Transform roomTransform = Resources.Load<GameObject>("castle" + UnityEngine.Random.Range(1, roomVariantsCount + 1).ToString()).transform;
        Transform tilemapTransform = roomTransform.Find("RoomGrid").transform.Find("wall_tilemap");
        Tilemap wallTilemap = tilemapTransform.GetComponent<Tilemap>();
        int2 offset = new int2(roomSize.x * (int)chosenRoom.position.x, roomSize.y * (int)chosenRoom.position.y);

        for (int y = 0; y < roomSize.y; y++)
        {
            for (int x = 0; x < roomSize.x; x++)
                levelWallTilemap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), wallTilemap.GetTile(new Vector3Int(x, y, 0)));
        }
        if (chosenRoom.up) CutWays(false, 1, wallTile, offset, new int2(roomSize.x / 2 - 1, roomSize.y - 3));
        if (chosenRoom.down) CutWays(false, 0, wallTile, offset, new int2(roomSize.x / 2 - 1, 0));
        if (chosenRoom.left) CutWays(true, 0, wallTile, offset, new int2(0, roomSize.y / 2 - 1));
        if (chosenRoom.right) CutWays(true, 0, wallTile, offset, new int2(roomSize.x - 2, roomSize.y / 2 - 1));
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
            levelWallTilemap.SetTile(new Vector3Int(start.x + offset.x, start.y +1 + offset.y, 0), wall);
            levelWallTilemap.SetTile(new Vector3Int(start.x + 1 + offset.x, start.y + 1 + offset.y, 0), wall);
        }
    }
    private void CloseOpenedRooms()
    {
        IEnumerable<Room> roomQuery = spawnedRooms.Values.Where(room => room.up == true || room.down == true || room.left == true || room.right == true);
        List<Room> nonClosedRooms = roomQuery.ToList();

        foreach(Room room in nonClosedRooms)
        {
            int2 offset = new int2(roomSize.x * (int)room.position.x, roomSize.y * (int)room.position.y);
            if (room.up) FillWays(true,  wallTile, wallTop, offset, new int2(roomSize.x / 2 - 1, roomSize.y - 2));
            if (room.down) FillWays(false,  wallTile, wallTop, offset, new int2(roomSize.x / 2 - 1, 0));
            if (room.left) FillWays(false,  wallTile, wallTop, offset, new int2(0, roomSize.y / 2 - 1));
            if (room.right) FillWays(false, wallTile, wallTop, offset, new int2(roomSize.x - 2, roomSize.y / 2 - 1));
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