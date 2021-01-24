using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    private TilemapCollider2D tileCollider;
    public Door[] doors;

    private void Start()
    {
        InitializeRoomManager();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("entered room");
            tileCollider.enabled = false;
            foreach(Door door in doors)
            {
                door.CloseDoor();
                StartCoroutine(Test());
            }
        }
    }
    private void InitializeRoomManager()
    {
        tileCollider = GetComponent<TilemapCollider2D>();
    }
    private IEnumerator Test()
    {
        yield return new WaitForSeconds(5f);
        foreach (Door door in doors)
        {
            door.OpenDoor();
        }
    }
}
