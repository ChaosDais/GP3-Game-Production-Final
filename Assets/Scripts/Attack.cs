using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Invector.vCharacterController;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage = 5;
    public bool playerSourced = false;

    [HideInInspector] public Collider col;
    vThirdPersonInput input;

    void Start()
    {
        col = GetComponent<BoxCollider>();
        input = GameObject.FindWithTag("Player").GetComponent<vThirdPersonInput>();
    }

    private void OnTriggerEnter(Collider other)
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
