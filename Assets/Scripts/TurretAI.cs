using UnityEngine;

public enum FireMode
{
    Sequential,    // Bắn từng nòng theo thứ tự
    Simultaneous   // Bắn tất cả nòng cùng lúc
}

public enum TargetPriority
{
    Closest,  // Ưu tiên kẻ địch gần nhất
    LowestHealth // Ưu tiên kẻ địch có máu thấp nhất
}

public class TurretAI : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private Transform turretHead;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private FireMode fireMode = FireMode.Sequential;
    [SerializeField] private TargetPriority targetPriority = TargetPriority.Closest; // Chế độ mặc định
    [SerializeField] private LayerMask zombieLayer;


    private Transform _target;
    private float _nextFireTime = 0f;
    private int _currentFirePointIndex = 0;

    void Update()
    {
        FindTarget();
        RotateHeadTowardsTarget();

        if (_target != null && Time.time >= _nextFireTime)
        {
            Shoot();
            _nextFireTime = Time.time + fireRate;
        }
    }

    void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange, zombieLayer);

        float bestValue = Mathf.Infinity;
        _target = null;

        foreach (Collider col in colliders)
        {
            float valueToCompare = Mathf.Infinity;

            if (targetPriority == TargetPriority.Closest)
            {
                valueToCompare = Vector3.Distance(transform.position, col.transform.position);
            }
            else if (targetPriority == TargetPriority.LowestHealth)
            {
                if (col.TryGetComponent(out ZombieHealth zombieHealth))
                {
                    valueToCompare = zombieHealth.CurrentHealth;
                }
            }

            if (valueToCompare < bestValue)
            {
                bestValue = valueToCompare;
                _target = col.transform;
            }
        }
    }
    
    void RotateHeadTowardsTarget()
    {
        if (_target == null) return;

        Vector3 direction = (_target.position - turretHead.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        turretHead.rotation = Quaternion.Slerp(turretHead.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void Shoot()
    {
        if (firePoints.Length == 0) return;

        switch (fireMode)
        {
            case FireMode.Sequential:
                Instantiate(bulletPrefab, firePoints[_currentFirePointIndex].position, firePoints[_currentFirePointIndex].rotation);
                _currentFirePointIndex = (_currentFirePointIndex + 1) % firePoints.Length;
                break;

            case FireMode.Simultaneous:
                foreach (Transform firePoint in firePoints)
                {
                    Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                }
                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _target.position);
    }
}
