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

    [Header("Ammo Settings")]
    [SerializeField] private GameObject magazine;    
    [SerializeField] private int maxAmmo = 300;
    [SerializeField] private int clipSize = 30;
    [SerializeField] private float reloadTime = 2f;

    [Header("Bullet Spread Settings")]
    [SerializeField] private bool addSpread = true;
    [SerializeField] private float spreadAngle = 5f;

    [Header("Recoil Settings")]
    [SerializeField] private bool addRecoil = true;
    [SerializeField] private Vector2[] recoilPattern;
    [SerializeField] private float recoilDuration = 0.1f;

    [Header("Bullet Projectile Settings")]
    [SerializeField] private GameObject pfBullet;
    [SerializeField] private Transform bulletSpawnPoint;

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

    [HideInInspector] public Animator rigAnimator;

    private Coroutine _recoilCoroutine;

    private void Start()
    {
        playerCamera = FindFirstObjectByType<CinemachineOrbitalFollow>();
        _currentAmmo = clipSize;
    }

    public void Shoot(Vector3 aimPos)
    {
        if (_isReloading) return;

        if (_currentAmmo <= 0)
        {
            Reload();
            return; 
        }

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
        if (!_isReloading )
        {

            StartCoroutine(ReloadRoutine());
        }
    }

    private IEnumerator ReloadRoutine()
    {
        if (maxAmmo <= 0) yield break;

        _isReloading = true;
        rigAnimator.SetTrigger("Reload_Weapon");
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = clipSize - _currentAmmo;
        int ammoToLoad = Mathf.Min(ammoNeeded, maxAmmo);

        _currentAmmo += ammoToLoad;
        maxAmmo -= ammoToLoad;

        rigAnimator.ResetTrigger("Reload_Weapon"); 
        _isReloading = false;
        Debug.Log("Reloaded! Current Ammo: " + _currentAmmo + " / " + maxAmmo);
    }

    private void ApplyRecoil()
    {
        if(!addRecoil) return;
        
        if (_recoilCoroutine != null)
            StopCoroutine(_recoilCoroutine);

        rigAnimator.Play("Weapon_Recoil_" + gunName, 1, 0.0f); 

        _recoilCoroutine = StartCoroutine(RecoilRoutine());
    }

    private IEnumerator RecoilRoutine()
    {
        float stepTime = recoilDuration / recoilPattern.Length;
        int index = 0;

        while (index < recoilPattern.Length)
        {
            playerCamera.HorizontalAxis.Value -= recoilPattern[index].x;
            playerCamera.VerticalAxis.Value -= recoilPattern[index].y;

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
        if (!addSpread || spreadAngle <= 0f)
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
    public GameObject Magazine => magazine;
    public string Name => gunName;
    public int Ammo => _currentAmmo;
    public int MaxAmmo => maxAmmo;
    public int ClipSize => clipSize;
    #endregion
}
