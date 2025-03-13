using System;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private int points = 100;

    private ZombieAI _zombieAI;
    private float _currentHealth;

    private void Start()
    {
        _zombieAI = GetComponent<ZombieAI>();
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        _zombieAI.EnableRagdoll();
        
        PointsManager.Instance.AddPoints(points);
        FindFirstObjectByType<WaveManager>().ZombieDied();

        Destroy(gameObject, 6f);
    }
}
