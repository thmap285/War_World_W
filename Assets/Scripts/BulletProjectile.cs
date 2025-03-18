using System;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 100f;
    [ SerializeField] private float baseDamage = 10f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private GameObject hitEnemyEffect;
    [SerializeField] private GameObject hitWallEffect;

    private Rigidbody _rb;
    private Vector3 _spawnPosition;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    [System.Obsolete]
    private void Start()
    {
        _spawnPosition = transform.position;
        _rb.velocity = transform.forward * speed;
        transform.rotation = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(-90f,0,0);

        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Bullet") || other.gameObject.CompareTag("Player")) return;

        float distanceTraveled = Vector3.Distance(_spawnPosition, transform.position);
        float damage = CalculateDamage(distanceTraveled);
        Debug.Log(damage);

        var zombieHealth = other.gameObject.GetComponent<ZombieHealth>();
        if(other.gameObject.CompareTag("Zombie"))
        {
            zombieHealth.TakeDamage(damage);
            
            GameObject hitEffect = Instantiate(hitEnemyEffect, transform.position, 
                            Camera.main.transform.rotation);

            float hitEffectLifeTime = hitEffect.GetComponent<ParticleSystem>().main.duration;
            Destroy(hitEffect, hitEffectLifeTime);
        }
        else if(other.gameObject.CompareTag("Wall"))
        {
            Transform wall = other.gameObject.transform;
            Vector3 direction = (wall.transform.position - transform.position).normalized;

            GameObject hitEffect = Instantiate(hitWallEffect, transform.position, 
                    Quaternion.LookRotation(direction, transform.up));
            hitEffect.transform.SetParent(wall);

            float hitEffectLifeTime = hitEffect.GetComponent<ParticleSystem>().main.duration;
            Destroy(hitEffect, hitEffectLifeTime);
        }

        Destroy(gameObject);
    }

    private float CalculateDamage(float distance)
    {
        float minDamage = baseDamage * 0.3f;
        if (distance >= maxDistance) return minDamage;
        return Mathf.Lerp(baseDamage, minDamage, distance / maxDistance);
    }
}
