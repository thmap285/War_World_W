using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private string gunName;
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    private float _nextFireTime;


    [Header("Bullet Spread Settings")]
    [SerializeField] private bool addBulletSpread = true;
    [SerializeField] private float spreadAngle = 5f;

    [Header("Recoil Settings")]
    [SerializeField] private float verticalRecoil;
    [SerializeField] private float horizontalRecoil;


    [Header("Bullet Projectile Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;


    [Header("Ammo Settings")]
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private float reloadTime = 2f;
    private int _currentAmmo;
    private bool _isReloading = false;


    [Header("Others")]
    [SerializeField] private GameObject mulzzeFlashEffect;
    [SerializeField] private CinemachineOrbitalFollow playerCamera;
    [SerializeField] private CinemachineImpulseSource impulseSource;

    private Vector3 _originalPosition;

    private void Start()
    {
        _currentAmmo = maxAmmo;
        mulzzeFlashEffect.SetActive(false);
    }

    public void Shoot(Vector3 aimPos)
    {
        if (_isReloading) return;
        if (_currentAmmo <= 0 && !_isReloading) Reload();

        if (Time.time >= _nextFireTime)
        {
            mulzzeFlashEffect.SetActive(true);
            _nextFireTime = Time.time + fireRate;
            _currentAmmo--;

            Vector3 direction = (aimPos - bulletSpawnPoint.position).normalized;
            Quaternion spreadRotation = GetSpreadRotation();
            direction = spreadRotation * direction;

            Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.LookRotation(direction));

            ApplyRecoil();
        }
    }

    public void Reload()
    {
        if (!_isReloading)
        {
            mulzzeFlashEffect.SetActive(false);
            StartCoroutine(ReloadRoutine());
        }
    }

    private IEnumerator ReloadRoutine()
    {
        _isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        _currentAmmo = maxAmmo;
        _isReloading = false;
        Debug.Log("Reloaded!");
    }

    private void ApplyRecoil()
    {
        var randomPosition = new Vector2(Random.Range(-horizontalRecoil, horizontalRecoil), 
                                         Random.Range(-verticalRecoil, verticalRecoil));
        
        playerCamera.HorizontalAxis.Value -= randomPosition.x;
        playerCamera.VerticalAxis.Value -= randomPosition.y;
        impulseSource.GenerateImpulse();
    }

    private Quaternion GetSpreadRotation()
    {
        if (!addBulletSpread || spreadAngle <= 0f)
            return Quaternion.identity;

        // Lấy một điểm ngẫu nhiên trên mặt cầu đơn vị
        Vector3 randomPoint = Random.onUnitSphere;

        // Điều chỉnh độ lớn của vector tán xạ theo spreadAngle
        randomPoint *= Mathf.Tan(spreadAngle * Mathf.Deg2Rad); // Đưa về đơn vị góc

        // Xoay hướng bắn ban đầu theo tán xạ
        return Quaternion.FromToRotation(Vector3.forward, Vector3.forward + randomPoint);
    }

    public void StopShooting()
    {
        mulzzeFlashEffect.SetActive(false);
    }
}
