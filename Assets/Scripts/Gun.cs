using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    [SerializeField] private string gunName;
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    private float _nextFireTime;

    [Header("Ammo Settings")]
    [SerializeField] private GameObject goMagazine;
    [SerializeField] private int maxMagazines = 10;
    [SerializeField] private int sizeOfMagazine = 30;
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
    private int _currentAmmoInMagazine;
    private int _totalAmmo;
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

    private void Start()
    {
        playerCamera = FindFirstObjectByType<CinemachineOrbitalFollow>();
        _currentAmmoInMagazine = sizeOfMagazine;
        _totalAmmo = maxMagazines * sizeOfMagazine;
    }

    public void Shoot(Vector3 aimPos)
    {
        if (!CanShoot) return;

        _nextFireTime = Time.time + fireRate;
        _currentAmmoInMagazine--;

        Vector3 direction = GetBulletDirection(aimPos);
        Instantiate(pfBullet, bulletSpawnPoint.position, Quaternion.LookRotation(direction));

        ApplyEffects();
        ApplyRecoil();
        ApplySound(shotSound);

        AutoReload();
    }

    private Vector3 GetBulletDirection(Vector3 aimPos)
    {
        Vector3 direction = (aimPos - bulletSpawnPoint.position).normalized;

        if (Vector3.Distance(bulletSpawnPoint.position, aimPos) < 1.5f)
            direction = bulletSpawnPoint.forward;

        return GetSpreadRotation() * direction;
    }

    private void ApplyEffects()
    {
        if (pfMuzzleFlash && tfMuzzleFlash)
            Instantiate(pfMuzzleFlash, tfMuzzleFlash.position, tfMuzzleFlash.rotation, tfMuzzleFlash);

        impulseSource?.GenerateImpulse();
    }

    private void AutoReload()
    {
        if (_currentAmmoInMagazine <= 0)
        {
            Reload();
        }
    }

    public void Reload()
    {
        if (_isReloading || _currentAmmoInMagazine == sizeOfMagazine || _totalAmmo <= 0) return;
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        if (_totalAmmo <= 0) yield break;

        _isReloading = true;
        rigAnimator.SetTrigger("Reload_Weapon");
        ApplySound(reloadSound);

        float waitTime = rigAnimator ? Mathf.Max(reloadTime, rigAnimator.GetCurrentAnimatorStateInfo(0).length) : reloadTime;
        yield return new WaitForSeconds(waitTime);

        rigAnimator.ResetTrigger("Reload_Weapon");
        int ammoToLoad = Mathf.Min(sizeOfMagazine - _currentAmmoInMagazine, _totalAmmo);
        _currentAmmoInMagazine += ammoToLoad;
        _totalAmmo -= ammoToLoad;

        _isReloading = false;
    }

    private void ApplyRecoil()
    {
        if (!addRecoil || playerCamera == null || recoilPattern == null || recoilPattern.Length == 0) return;

        ApplyAnimation("Weapon_Recoil_" + gunName);
        StartCoroutine(RecoilRoutine());
    }

    private IEnumerator RecoilRoutine()
    {
        float stepTime = recoilDuration / recoilPattern.Length;
        foreach (var recoil in recoilPattern)
        {
            playerCamera.HorizontalAxis.Value -= recoil.x;
            playerCamera.VerticalAxis.Value -= recoil.y;
            yield return new WaitForSeconds(stepTime);
        }
    }

    private Quaternion GetSpreadRotation()
    {
        if (!addSpread || spreadAngle <= 0f) return Quaternion.identity;

        Vector3 randomPoint = Random.insideUnitSphere * Mathf.Tan(spreadAngle * Mathf.Deg2Rad);
        return Quaternion.FromToRotation(Vector3.forward, Vector3.forward + randomPoint);
    }

    public void AddAmmo(int amount)
    {
        if (amount > 0)
            _totalAmmo = Mathf.Min(_totalAmmo + amount, maxMagazines * sizeOfMagazine);
    }

    private void ApplySound(AudioClip clip)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, transform.position, volume);
    }

    private void ApplyAnimation(string animationName)
    {
        if (rigAnimator != null && !string.IsNullOrEmpty(animationName))
            rigAnimator.Play(animationName, 1, 0.0f);
    }

    public bool CanShoot => Time.time >= _nextFireTime && !_isReloading && _currentAmmoInMagazine > 0;
    public GameObject MagazineGO => goMagazine;
    public bool IsReloading => _isReloading;
    public string Name => gunName;
    public int Ammo => _currentAmmoInMagazine;
    public int TotalAmmo => _totalAmmo;
    public int SizeOfMagazine => sizeOfMagazine;
    public int MaxMagazines => maxMagazines;
}
