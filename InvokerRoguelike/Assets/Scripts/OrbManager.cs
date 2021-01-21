using System.Collections.Generic;
using UnityEngine;

public class OrbManager : MonoBehaviour
{
    private List<Orb> invokedOrbs;
    private List<Orb> availableOrbs;

    void Start()
    {
        invokedOrbs = new List<Orb>(3);
        availableOrbs = new List<Orb>();

        AddAvailableOrb(Orb.chaos);
        AddAvailableOrb(Orb.ice);
        AddAvailableOrb(Orb.fire);
    }
    private void Update()
    {
        CheckOrbCast(KeyCode.Q, availableOrbs[0]);
        CheckOrbCast(KeyCode.E, availableOrbs[1]);
        CheckOrbCast(KeyCode.R, availableOrbs[2]);

        if (Input.GetMouseButtonDown(0) && invokedOrbs.Count == 3 )
        {
            List<Orb> orbs = invokedOrbs;
            orbs.Sort();
            string orbsString = "mix";
            if (orbs [0] == orbs [1] || orbs[1] == orbs[2] || orbs[0] == orbs[2]) 
            {
               orbsString = orbs[0].ToString() + orbs[1].ToString() + orbs[2].ToString();
            }

            AbilityManager.instance.Cast(orbsString);
            Debug.Log(orbsString);
        }
    }
    private void AddAvailableOrb(Orb orbToAdd)
    {
        availableOrbs.Add(orbToAdd);
        availableOrbs.Sort();
    }
    private void CheckOrbCast(KeyCode orbKey, Orb orbToCast)
    {
        if (Input.GetKeyDown(orbKey))
        {
            invokedOrbs.Insert(0, orbToCast);
            if (invokedOrbs.Count > 3)
                invokedOrbs.RemoveAt(3);
        }
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
