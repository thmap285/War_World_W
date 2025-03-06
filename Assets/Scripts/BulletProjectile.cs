using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 100f;
    [ SerializeField] private float damage = 10f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private GameObject hitEnemyEffect;
    [SerializeField] private GameObject hitWallEffect;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    [System.Obsolete]
    private void Start()
    {
        rb.velocity = transform.forward * speed;
        transform.rotation = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(-90f,0,0);

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // if(other.gameObject.CompareTag("Bullet") || other.gameObject.CompareTag("Player")) return;

        if(other.gameObject.CompareTag("Enemy"))
        {
            var hitEffect = Instantiate(hitEnemyEffect, transform.position, 
                            Camera.main.transform.rotation);
            var hitEffectLifeTime = hitEffect.GetComponent<ParticleSystem>().main.duration;
            
            Destroy(hitEffect, hitEffectLifeTime);
        }
        else if(other.gameObject.CompareTag("Wall"))
        {
            var hitEffect = Instantiate(hitWallEffect, transform.position, Camera.main.transform.rotation);
            var hitEffectLifeTime = hitEffect.GetComponent<ParticleSystem>().main.duration;

            Destroy(hitEffect, hitEffectLifeTime);
        }

        Destroy(gameObject);
    }
}
