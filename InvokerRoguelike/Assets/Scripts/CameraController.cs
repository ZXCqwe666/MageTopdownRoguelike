﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    private Camera mainCam;
    private Transform player;//, gun, gunHolder;
    private SpriteRenderer playerSprite, gunSprite;
    private Vector3 mouseViewportPos, cameraTargetPosition, velocity;
    private Vector3 RoundShakeOffset, DirectionalShakeOffset;
    private readonly float smoothTime = 0.05f, offsetStrength = 2.1f;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        mainCam = Camera.main;
        player = FindObjectOfType<PlayerController>().transform;
        //gun = FindObjectOfType<Gun>().transform;
        //gunHolder = player.GetChild(0);
        playerSprite = player.GetComponent<SpriteRenderer>();
        //gunSprite = gun.GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        transform.position = CalculateMouseOffset() + RoundShakeOffset + DirectionalShakeOffset;
        //UpdatePlayerAndGunRotation();
     }   
    private Vector3 CalculateMouseOffset()
    {
        mouseViewportPos = mainCam.ScreenToViewportPoint(Input.mousePosition) * 2f - Vector3.one;
        cameraTargetPosition = player.position + mouseViewportPos * offsetStrength;
        cameraTargetPosition.z = -10f;
        return Vector3.SmoothDamp(transform.position, cameraTargetPosition, ref velocity, smoothTime);
    }
    /*private void UpdatePlayerAndGunRotation()
    {
        if (mouseViewportPos.x > 0f)
        {
            playerSprite.flipX = false;
            gunSprite.flipY = false;
        }
        else
        {
            playerSprite.flipX = true;
            gunSprite.flipY = true;
        }
        Vector2 aimDirection = (mainCam.ScreenToWorldPoint(Input.mousePosition) - player.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        gunHolder.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
    }*/
    public void StartShakeRound(float duration, float magnitude)
    {
        StartCoroutine(ShakeRound(duration, magnitude));
    }
    public void StartShakeDirectional(Vector3 direction, float duration, float magnitude)
    {
        StartCoroutine(ShakeDirectional(direction, duration, magnitude));
    }
    IEnumerator ShakeRound(float duration, float shakeMagnitude)
    {
        while (duration > 0)
        {
            RoundShakeOffset = Random.insideUnitSphere * shakeMagnitude;
            duration -= 0.02f;
            yield return new WaitForSeconds(0.02f);
        }
        RoundShakeOffset = Vector3.zero;
    }
    IEnumerator ShakeDirectional(Vector3 direction, float duration, float shakeMagnitude)
    {
        DirectionalShakeOffset = direction * shakeMagnitude;
        yield return new WaitForSeconds(duration);
        DirectionalShakeOffset = Vector3.zero;
    }
}
