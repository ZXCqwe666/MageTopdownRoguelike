﻿using System.Collections;
using UnityEngine;
public class PlayerHealth : MonoBehaviour
{
    private const float flashDuration = 0.1f;
    private int hp;
    public int maxHp;
    //public Action characterDied; // maybe death will be handled here too

    private SpriteRenderer rend;
    private Material defaultMaterial, whiteFlashMatherial;

    private void Start()
    {
        InitializeHealth();
        rend = GetComponent<SpriteRenderer>();
        defaultMaterial = rend.material;
        whiteFlashMatherial = Resources.Load<Material>("whiteFlash");
    }
    public virtual void InitializeHealth()
    {
        maxHp = PlayerStats.instance.defaultPlayerStatsData[0].maxHealth;
    }
    public void TakeDamage(int _amount)
    {
        StartCoroutine(WhiteFlashEffect());
        hp -= _amount;
        if (hp <= 0)
            Destroy(gameObject); // DEATH LOGIC
    }
    public void Heal(int _amount)
    {
        hp += _amount;
        if (hp > maxHp)
            hp = maxHp;
    }
    private IEnumerator WhiteFlashEffect()
    {
        rend.material = whiteFlashMatherial;
        yield return new WaitForSeconds(flashDuration);
        rend.material = defaultMaterial;
    }
}
