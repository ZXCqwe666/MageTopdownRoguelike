using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator anim;
    private BoxCollider2D coll;

    void Start()
    {
        InitializeDoor();
        OpenDoor();
    }
    private void InitializeDoor()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
    }
    public void OpenDoor()
    {
        anim.Play("openDoor");
        coll.enabled = false;
    }
    public void CloseDoor()
    {
        anim.Play("closeDoor");
        coll.enabled = true;
    }
}
