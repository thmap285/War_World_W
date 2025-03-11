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
    [SerializeField] private Vector2[] recoilPattern;
    [SerializeField] private float recoilDuration = 0.1f;

    [Header("Bullet Projectile Settings")]
    [SerializeField] private GameObject pfBullet;
    [SerializeField] private Transform bulletSpawnPoint;

    [Header("Ammo Settings")]
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private float reloadTime = 2f;
    private int _currentAmmo;
    private bool _isReloading = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip shotSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField][Range(0, 1)] private float volume = 0.5f;

    [Header("Others")]
    [SerializeField] private GameObject pfMuzzleFlash;
    [SerializeField] private Transform tfMuzzleFlash;
    [SerializeField] private CinemachineOrbitalFollow playerCamera;
    [SerializeField] private CinemachineImpulseSource impulseSource;

    private Coroutine _recoilCoroutine;

    private void Start()
    {
        _currentAmmo = maxAmmo;
    }

    public void Shoot(Vector3 aimPos)
    {
        // Tự động reload khi đạn hết
        if (_currentAmmo <= 0 && !_isReloading) Reload();
        if (!CanShoot) return;

        _nextFireTime = Time.time + fireRate;
        _currentAmmo--;

        Vector3 direction = (aimPos - bulletSpawnPoint.position).normalized;
        if (Vector3.Distance(bulletSpawnPoint.position, aimPos) < 1.5f)
        {
            direction = bulletSpawnPoint.forward;
        }

        Quaternion spreadRotation = GetSpreadRotation();
        direction = spreadRotation * direction;

        Instantiate(pfBullet, bulletSpawnPoint.position, Quaternion.LookRotation(direction));

        Instantiate(pfMuzzleFlash, tfMuzzleFlash);
        impulseSource.GenerateImpulse();

        ApplyRecoil();
        ApplySound();
    }

    public void Reload()
    {
        if (!_isReloading)
        {
            // fxMuzzleFlash.SetActive(false);
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
        if (_recoilCoroutine != null)
            StopCoroutine(_recoilCoroutine);

        _recoilCoroutine = StartCoroutine(RecoilRoutine());
    }

    private IEnumerator RecoilRoutine()
    {
        float stepTime = recoilDuration / recoilPattern.Length; // Thời gian mỗi bước
        int index = 0;

        while (index < recoilPattern.Length)
        {
            playerCamera.HorizontalAxis.Value -= recoilPattern[index].x; // Giật ngang
            playerCamera.VerticalAxis.Value -= recoilPattern[index].y;   // Giật dọc

            index++;
            yield return new WaitForSeconds(stepTime);
        }
    }

    private void ApplySound()
    {
        if (shotSound)
        {
            AudioSource.PlayClipAtPoint(shotSound, bulletSpawnPoint.transform.position, volume);
        }
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

    #region Getters and Setters
    public bool CanShoot => Time.time >= _nextFireTime && !_isReloading && _currentAmmo > 0;
    #endregion
}
