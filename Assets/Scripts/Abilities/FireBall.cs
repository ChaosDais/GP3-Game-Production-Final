using System;
using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.UI;

public class FireBall : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float fireCooldown = 1.5f;
    public float projectileSpeed = 100f;
    public float projectileLifetime = 3f;
    public Transform spawnPoint;
    public AudioSource fireballSound;
    public GameObject cooldownMeter;

    bool alreadyShooting = false;
    vThirdPersonInput input;
    Transform playerTransform;
    float cdCurrent;
    Slider cooldownSlider;

    void Start()
    {
        input = gameObject.GetComponent<vThirdPersonInput>();
        playerTransform = gameObject.transform;
        cooldownSlider = cooldownMeter.GetComponent<Slider>();
        cdCurrent = fireCooldown;
    }

    private void Update()
    {
        if (Input.GetKeyDown(input.abilityInput) && !alreadyShooting)
        {
            print("Cast Fireball!");
            StartCoroutine(Fireball());
        }
        else if (Input.GetKeyDown(input.abilityInput))
        {
            print("Can't cast again yet!");
        }
    }

    private void FixedUpdate()
    {
        if (alreadyShooting) cdCurrent += Time.deltaTime;

        cooldownSlider.value = cdCurrent / fireCooldown;
        cooldownMeter.SetActive(cdCurrent < fireCooldown);
    }

    IEnumerator Fireball()
    {
        if (fireballSound != null) fireballSound.Play();
        alreadyShooting = true;
        Vector3 fwd = playerTransform.forward;
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody>().velocity = fwd * projectileSpeed;
        Destroy(projectile, projectileLifetime);
        cdCurrent = 0;

        yield return new WaitForSeconds(fireCooldown);

        cdCurrent = fireCooldown;
        alreadyShooting = false;
    }
}
