using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private List<Room> roomVariants;
    private Dictionary<Vector2, Room> spawnedRooms;
    public GameObject TestTile;
    public int roomCount;
    void Start()
    {
        Random.InitState(Random.Range(0, int.MaxValue));
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
        GenerateMap(roomCount);
    }
    private void GenerateMap(int roomCount)
    {
        spawnedRooms = new Dictionary<Vector2, Room>
        {
            { Vector2.zero, roomVariants[14] }
        };
        Instantiate(TestTile, Vector2.zero, Quaternion.identity);

        while (roomCount > 0)
        {
            IEnumerable<Room> roomQuery = spawnedRooms.Values.Where(room => room.up == true || room.down == true || room.left == true || room.right == true);
            List<Room> nonClosedRooms = roomQuery.ToList();
            if (nonClosedRooms.Count == 0) { Debug.Log("all rooms are closed"); return; } // ПРОБЛЕМКА что комнаты выбирает с 1 проходом на старте генерации

            Room randomRoom = nonClosedRooms[Random.Range(0, nonClosedRooms.Count)];
            Vector2 randomRoomPosition = randomRoom.position; // PICK RANDOM OPEN-ENDED ROOM

            List<Vector2> ValidDirections = new List<Vector2>();
            if (randomRoom.up) ValidDirections.Add(Vector2.up);
            if (randomRoom.down) ValidDirections.Add(Vector2.down);
            if (randomRoom.left) ValidDirections.Add(Vector2.left);
            if (randomRoom.right) ValidDirections.Add(Vector2.right);
            Vector2 addedRoomPosition = randomRoomPosition + ValidDirections[Random.Range(0, ValidDirections.Count)]; // CALCULATED POSITION OF ROOM WE ARE adding

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

            Room chosenRoomToAdd = validRoomsToAdd[Random.Range(0, validRoomsToAdd.Count)];
            chosenRoomToAdd.position = addedRoomPosition;

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
            // CLOSED ALL OPENINGS ON OLD AND NEW ROOMS

            spawnedRooms.Add(addedRoomPosition, chosenRoomToAdd);
            Instantiate(TestTile, addedRoomPosition, Quaternion.identity);

            roomCount--;
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
        up = _up;
        down = _down;
        left = _left;
        right = _right;
    }
}
