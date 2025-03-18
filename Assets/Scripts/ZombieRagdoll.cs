using UnityEngine;
using UnityEngine.AI;

public class ZombieRagdoll : MonoBehaviour
{
    private Collider _zombieCollider;
    private Collider[] _ragdollColliders;
    private Rigidbody[] _ragdollRigidbodies;
    private NavMeshAgent _agent;
    private Animator _animator;

    private void Start()
    {
        _zombieCollider = GetComponent<Collider>();
        _ragdollColliders = GetComponentsInChildren<Collider>();
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        ActiveRagdoll(false);
    }

    public void ActiveRagdoll(bool isActive)
    {
        foreach (var rb in _ragdollRigidbodies)
        {
            rb.isKinematic = !isActive;
            rb.useGravity = isActive;
        }

        foreach (var collider in _ragdollColliders)
        {
            collider.enabled = isActive;
            if (collider != _zombieCollider)
            {
                collider.gameObject.layer = LayerMask.NameToLayer("ZombieRagdoll");
            }
        }

        _zombieCollider.enabled = !isActive;
        _agent.enabled = !isActive;
        _animator.enabled = !isActive;

        if (isActive)
{
    if (GetComponent<Rigidbody>())
    {
        Destroy(GetComponent<Rigidbody>()); // Xóa Rigidbody chính để tránh bị đẩy đi mạnh
    }
}
    }
}
