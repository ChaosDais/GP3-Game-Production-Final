using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Invector.vCharacterController;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage = 5;
    public bool playerSourced = false;

    Collider col;
    vThirdPersonInput input;

    void Start()
    {
        col = GetComponent<BoxCollider>();
        input = GameObject.FindWithTag("Player").GetComponent<vThirdPersonInput>();
    }

    void Update()
    {
        
        if (Input.GetKeyDown(input.attackInput))
        {
            Debug.Log("Attack!");
            col.enabled = true;
        }
        else if (Input.GetKeyUp(input.attackInput))
        {
            Debug.Log("Stopped attacking.");
            col.enabled = false;
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
