using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Rigidbody))]
public class Damageable : MonoBehaviour
{
    [SerializeField] private float maxHealth;

    [SerializeField] private UnityEvent onHealthZero;

    private float currentHealth;

    private bool isDead;

    private void Start()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
        {
            //if we're dead, stop early.
            return;
        }
        Debug.Log($"The agent {name} took {amount} damage! Git Gud, scrub.");

        currentHealth -= amount;

        if(currentHealth <= 0)
        {
            HealthZero();
        }
    }

    public void RestoreHealth(float amount)
    {
        isDead = false;
        Debug.Log($"The agent {name} healed {amount} damage! You did Git Gud.");

        currentHealth += amount;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    private void HealthZero()
    {
        isDead = true;
        currentHealth = 0;
        Debug.Log($"The agent {name} has died!");
        onHealthZero.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckForDamage(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckForDamage(other.gameObject);
        
    }

    private void OnParticleCollision(GameObject other)
    {
        CheckForDamage(other);
    }

    private void CheckForDamage(GameObject possibleSource)
    {
        //TryGetComponent will return true or false, if true, it will also "out" the component it found
        if (possibleSource.gameObject.TryGetComponent<DamageSource>(out DamageSource damageSource))
        {
            //if the tags match..
            if (possibleSource.gameObject.CompareTag(tag))
            {
                //do nothing.
                return;
            }

            //if we get here, tags don't match. so we should take damage.
            TakeDamage(damageSource.GetDamage());
        }
    }
    /// <summary>
    /// Returns the percent of health this Damageable has, between 0-1
    /// </summary>
    /// <returns></returns>
    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }
}
