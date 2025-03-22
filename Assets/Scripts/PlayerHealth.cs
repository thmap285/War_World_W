using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float _currentHealth;

    private void Start()
    {
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
        Debug.Log("Player Dead");
        PauseMenu.Instance.gameObject.SetActive(true);
    }

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => maxHealth;
}
