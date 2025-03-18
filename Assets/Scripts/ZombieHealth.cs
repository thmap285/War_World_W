using System;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private int points = 100;

    private ZombieRagdoll _zombieRagdoll;
    private float _currentHealth;

    private void Start()
    {
        _zombieRagdoll = GetComponent<ZombieRagdoll>();
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
        _zombieRagdoll.ActiveRagdoll(true);

        PointsManager.Instance.AddPoints(points);
        WaveManager.Instance.ZombieDied();

        Destroy(gameObject, 6f);
    }

    public float CurrentHealth => _currentHealth;
}
