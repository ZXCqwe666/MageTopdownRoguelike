using System.Collections;
using UnityEngine;

public class OrbManager : MonoBehaviour
{
    private Queue invokedOrbs;
    private Orb[] availableOrbs;

    void Start()
    {
        invokedOrbs = new Queue(3);
        availableOrbs = new Orb[3] { Orb.fire, Orb.ice, Orb.nature };
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            invokedOrbs.Enqueue(availableOrbs[0]);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            invokedOrbs.Enqueue(availableOrbs[1]);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            invokedOrbs.Enqueue(availableOrbs[2]);
    }
}
public enum Orb
{
    fire,
    ice,
    nature,
    storm,
    chaos
}
