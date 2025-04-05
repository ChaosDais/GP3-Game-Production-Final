using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DamageableCharacter : MonoBehaviour
{
    public int Health
    {
        set
        {
            if (value > 0 && value < health)
            {
                // Hit animation and sound
            }

            health = value;

            if (health > maxHealth)
            {
                health = maxHealth;
            }

            if (health <= 0 && Targetable)
            {
                Targetable = false;
                health = 0;
                //if (deathAnimation != null)
                //{
                //    deathAnimation.SetTrigger("Start");
                //}
                if (!isPlayer)
                {
                    Debug.Log(gameObject.name + " is dead.");
                    RemoveCharacter();
                }
                else
                {
                    canMove = false;
                }
            }
        }
        get
        {
            return health;
        }
    }
    public bool Targetable { get; set; }

    public bool canMove = true;
    public int maxHealth = 10;
    public int health = 10;
    public bool targetable = true;
    public bool isPlayer = false;

    [HideInInspector] public Rigidbody rb;

    public UnityEvent OnDestroyEvents;

    // Start is called before the first frame update
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = maxHealth;
    }

    public virtual void OnHit(int damage)
    {
        Debug.Log(gameObject.name + " took " + damage + " damage. " + health + " health remaining.");
        Health -= damage;
    }

    public virtual void Heal(int health)
    {
        Health += health;
    }

    public virtual void RemoveCharacter()
    {
        OnDestroyEvents.Invoke(); // Invoke any special events when destroyed
        Destroy(gameObject);
    }

    IEnumerator StunRecover(float time)
    {
        canMove = false;
        if (rb)
        {
            rb.velocity = Vector3.zero;
        }
        yield return new WaitForSeconds(time);
        canMove = true;
    }
}
