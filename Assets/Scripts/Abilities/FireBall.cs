using System;
using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float fireCooldown = 1.5f;
    public float projectileSpeed = 100f;
    public float projectileLifetime = 3f;
    public Transform spawnPoint;

    bool alreadyShooting = false;
    vThirdPersonInput input;
    Transform playerTransform;

    void Start()
    {
        input = gameObject.GetComponent<vThirdPersonInput>();
        playerTransform = gameObject.transform;
    }

    private void OnEnable()
    {
        print("Fireball Active!");
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

    IEnumerator Fireball()
    {
        alreadyShooting = true;
        Vector3 fwd = playerTransform.forward;
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody>().velocity = fwd * projectileSpeed;
        Destroy(projectile, projectileLifetime);

        yield return new WaitForSeconds(fireCooldown);
        alreadyShooting = false;
    }
}
