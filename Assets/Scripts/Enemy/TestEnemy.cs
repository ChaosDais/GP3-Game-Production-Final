using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : DamageableCharacter
{
    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        DamageableCharacter damageable = collision.gameObject.GetComponent<DamageableCharacter>();
        if (damageable && collision.gameObject.CompareTag("Player"))
        {
            damageable.OnHit(damage);
        }
    }
}
