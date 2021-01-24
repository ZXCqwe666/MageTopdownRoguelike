using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    private Transform doorColliderTilemap;
    public Animator[] doorAnimatorUp;
    public Animator[] doorAnimatorSide;

    private void Start()
    {
        InitializeRoomManager();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            foreach(Animator anim in doorAnimatorUp)
                anim.Play("closeDoor");
            foreach (Animator anim in doorAnimatorSide)
                anim.Play("closeDoorSide");
            Debug.Log("entered room");
            Destroy(doorColliderTilemap);
        }
    }
    private void InitializeRoomManager()
    {
        doorColliderTilemap = transform.Find("door_colliders");
        doorAnimatorUp = new Animator[]
        {
            transform.Find("doorUp").GetComponent<Animator>(),
            transform.Find("doorDown").GetComponent<Animator>()
        };
        doorAnimatorSide = new Animator[]
        {
            transform.Find("doorLeft").GetComponent<Animator>(),
            transform.Find("doorRight").GetComponent<Animator>()
        };
    }
}
