using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator anim;
    private BoxCollider2D coll;

    void Start()
    {
        InitializeDoor();
    }
    private void InitializeDoor()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        anim.enabled = false;
    }
    public void OpenDoor()
    {
        anim.Play("openDoor");
        coll.enabled = false;
        StartCoroutine(DisableAnimator());
    }
    public void CloseDoor()
    {
        anim.enabled = true;
        anim.Play("closeDoor");
        coll.enabled = true;
    }
    private IEnumerator DisableAnimator()
    {
        yield return new WaitForSeconds(3f); //Change for animation duration later
        anim.enabled = false;
    }
}
