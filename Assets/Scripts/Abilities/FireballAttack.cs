using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballAttack : MonoBehaviour
{
    public int damage = 3;
    public GameObject explosionPrefab;

    [HideInInspector] public Collider col;

    void Start()
    {
        col = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        DamageableCharacter damageable = collision.gameObject.GetComponent<DamageableCharacter>();
        if(damageable && !collision.gameObject.CompareTag("Player"))
        {
            damageable.OnHit(damage);
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
