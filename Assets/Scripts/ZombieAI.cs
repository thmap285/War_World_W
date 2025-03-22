using System;
using System.Collections;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private BoxCollider attackHitbox;
    [SerializeField] private AudioClip screamSound; 

    private NavMeshAgent _agent;
    private Animator _animator;
    private PlayerHealth _playerHealth;
    private float _lastAttackTime;
    private bool _isAttacking;
    private bool _isStarted = false;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerHealth = player.GetComponent<PlayerHealth>();
            target = player.transform;
        }
        
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        if(gameObject.name == "ZombieBoss")
        {
            StartCoroutine(Test());
        }

        attackHitbox.enabled = false;
        _agent.speed = speed;
        _agent.stoppingDistance = attackRange;
    }

    private IEnumerator Test()
    {
        float length = _animator.GetCurrentAnimatorClipInfo(0).Length;
        AudioSource.PlayClipAtPoint(screamSound, transform.position, 0.5f);
        yield return new WaitForSeconds(length);
        _isStarted = true;
        Debug.Log("Test");
    }

    private void Update()
    {
        if(_isStarted) return;

        if (_playerHealth.CurrentHealth <= 0) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > attackRange)
        {
            Chase();
        }
        else if (Time.time - _lastAttackTime >= attackCooldown)
        {
            Attack();
        }
    }

    private void Chase()
    {
        if (_isAttacking) return;

        _agent.isStopped = false;
        _agent.SetDestination(target.position);
    }

    private void Attack()
    {
        _isAttacking = true;
        _agent.isStopped = true;
        _animator.SetTrigger("Attack");
        _lastAttackTime = Time.time;
    }

    public void EnableHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.enabled = true;
        }
    }

    public void DisableHitbox()
    {
        if (attackHitbox != null)
        {
            attackHitbox.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && attackHitbox.enabled)
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
                Debug.Log("Zombie tấn công trúng người chơi!");
            }
        }
    }

    // Gọi từ Animation Event khi kết thúc Attack
    public void EndAttack()
    {
        Debug.Log("EndAttack được gọi!");
        _isAttacking = false;
        Chase();
    }
}
