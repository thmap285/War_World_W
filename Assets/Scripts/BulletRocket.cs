using UnityEngine;

public class BulletRocket : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float damage = 100f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 50f;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private AudioClip flySound;
    [Range(0f, 1f)] public float flyVolume = 0.5f;
    [SerializeField] private AudioClip explosionSound;
    [Range(0f, 1f)] public float explosionVolume = 1.0f;

    private Rigidbody _rb;
    private AudioSource _audioSource;
    private bool _hasExploded = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
    }

    [System.Obsolete]
    private void Start()
    {
        _rb.velocity = transform.forward * speed;
        // transform.rotation = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(-90f,0,0);

        if (flySound)
        {
            _audioSource.clip = flySound;
            _audioSource.loop = true;
            _audioSource.volume = 0.5f;
            _audioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider Collider)
    {
        if (!_hasExploded)
        {
            Explode();
        }
    }

    private void Explode()
    {
        _hasExploded = true;
        _audioSource.Stop();

        AudioSource.PlayClipAtPoint(explosionSound, transform.position, explosionVolume);
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            ZombieHealth zombieHealth = hit.GetComponent<ZombieHealth>();
            if (zombieHealth == null) continue;
            zombieHealth.TakeDamage(damage);

            if (zombieHealth.CurrentHealth <= 0)
            {
                ZombieRagdoll zombieRagdoll = hit.GetComponentInParent<ZombieRagdoll>();
                if (zombieRagdoll != null)
                {
                    foreach (Rigidbody rb in zombieRagdoll.GetComponentsInChildren<Rigidbody>())
                    {
                        rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 3f, ForceMode.Impulse);
                    }
                }
            }

            Destroy(gameObject);
        }
    }
}