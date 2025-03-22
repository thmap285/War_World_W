using UnityEngine;
using UnityEngine.AI;

public class ZombieRagdoll : MonoBehaviour
{
    private Collider _zombieCollider;
    private Collider[] _ragdollColliders;
    private Rigidbody[] _ragdollRigidbodies;
    private ZombieAI _ai;
    private Animator _animator;

    private void Start()
    {
        _zombieCollider = GetComponent<Collider>();
        _ragdollColliders = GetComponentsInChildren<Collider>();
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        _ai = GetComponent<ZombieAI>();
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
            // if (collider != _zombieCollider)
            // {
            //     collider.gameObject.layer = LayerMask.NameToLayer("ZombieRagdoll");
            // }
        }

        _zombieCollider.enabled = !isActive;
        _ai.enabled = !isActive;
        _animator.enabled = !isActive;

        if (isActive)
        {
            if (GetComponent<Rigidbody>())
            {
                Destroy(GetComponent<Rigidbody>());
            }
        }
    }
}
