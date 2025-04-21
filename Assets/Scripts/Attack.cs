using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Invector.vCharacterController;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage = 5;
    public bool playerSourced = false;
    public bool temporary = false;

    [HideInInspector] public Collider col;

    public virtual void Start()
    {
        col = GetComponent<Collider>();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
            DamageableCharacter damageable = other.gameObject.GetComponent<DamageableCharacter>();
            if (damageable && other.gameObject.CompareTag("Player") && !playerSourced)
            {
                damageable.OnHit(damage);
            }
            else if (damageable && playerSourced && !other.gameObject.CompareTag("Player"))
            {
                damageable.OnHit(damage);
            }
    }
}
