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
                    Debug.Log("Player died!");
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
    public bool dropsAbility = false;

    [HideInInspector] public Rigidbody rb;
    private Animator animator;

    public UnityEvent OnDestroyEvents;

    #region Ability Drop Variables
    public GameObject abilityDrop; // Determines which ability this character drops when defeated

    readonly float dropForce = 1; // Force when arcane soul pops out from character
    readonly float spawnOffset = 0.1f; // Randomization of where loot spawns from character
    #endregion

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        Targetable = targetable;
        health = maxHealth;
    }

    public virtual void OnHit(int damage)
    {
        Health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. " + health + " health remaining.");
    }

    public virtual void Heal(int health)
    {
        Health += health;
    }

    public virtual void RemoveCharacter()
    {
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        OnDestroyEvents.Invoke(); // Invoke any special events when destroyed

        // Set death animation
        if (animator != null)
        {
            animator.SetBool("IsDead", true);
        }

        if (dropsAbility)
        {
            Vector3 spawn = new(transform.position.x + Random.Range(-spawnOffset, spawnOffset),
                transform.position.y + Random.Range(0, spawnOffset),
                transform.position.z + Random.Range(-spawnOffset, spawnOffset));
            GameObject arcaneSoul = Instantiate(abilityDrop, spawn, Quaternion.identity); // Spawns arcane soul
            Debug.Log("Created " + arcaneSoul.name);

            arcaneSoul.GetComponent<Rigidbody>().AddExplosionForce(dropForce, transform.position - transform.up, 5); // Applies pop force
        }

        // Wait for animation
        yield return new WaitForSeconds(2f);

        // Destroy the game object
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
