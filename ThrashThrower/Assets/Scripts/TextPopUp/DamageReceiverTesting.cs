using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiverTesting : MonoBehaviour, IDamagable
{
    private float maxHealth = 10000f;
    private float currentHealth = 0;
    private void Awake()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0) Destroy(gameObject);
    }
}
