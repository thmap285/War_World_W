using System;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Collider testCollider;
    
    private Rigidbody[] _ragdollRigibodies;
    private Collider[] _ragdollColliders;
    private NavMeshAgent _agent;
    private Animator _animator;

    private void Start()
    {
        _ragdollRigibodies = GetComponentsInChildren<Rigidbody>();
        _ragdollColliders = GetComponentsInChildren<Collider>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        DisableRagdoll();
    }

    public void EnableRagdoll()
    {
        _agent.enabled = false;
        _animator.enabled = false;

        foreach (var rigidbody in _ragdollRigibodies)
        {
            rigidbody.isKinematic = false;
        }

        foreach (var collider in _ragdollColliders)
        {
            if (collider == testCollider)
            {
                testCollider.enabled = false;
                continue;
            }
            collider.enabled = true;
            collider.gameObject.layer = LayerMask.NameToLayer("ZombieRagdoll"); // Gán layer
        }

        // Physics.IgnoreLayerCollision(LayerMask.NameToLayer("ZombieRagdoll"), LayerMask.NameToLayer("ZombieRagdoll"), true);
    }


    public void DisableRagdoll()
    {
        _agent.enabled = true;
        _animator.enabled = true;

        foreach (var rigidbody in _ragdollRigibodies)
        {
            rigidbody.isKinematic = true;
        }

        foreach (var collider in _ragdollColliders)
        {
            if(collider == testCollider) continue;
            collider.enabled = false;
        }
    }

    private void Update()
    {
        if(_agent.enabled)
            _agent.SetDestination(target.position);
    }
}
