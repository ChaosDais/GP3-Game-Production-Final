using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage = 5;
    public bool playerSourced = false;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetBool("isAttacking", true);
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            anim.SetBool("isAttacking", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetType() == typeof(CapsuleCollider))
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
}
