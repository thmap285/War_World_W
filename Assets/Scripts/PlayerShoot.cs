using System;
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private CinemachineCamera aimCamera;
    [SerializeField] private float aimSensitivity, normalSensitivity;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform aimTransform;
    [SerializeField] private GameObject pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private float fireRate = 0.2f;

    private StarterAssetsInputs _startedAssetsInputs;
    private ThirdPersonController _thirdPersonController;
    private float _nextFireTime;

    private void Awake()
    {
        _startedAssetsInputs = GetComponent<StarterAssetsInputs>();
        _thirdPersonController = GetComponent<ThirdPersonController>();

        aimCamera.Priority = -1;
        crosshair.SetActive(false);
    }

    private void Update()
    {
        Vector3 aimPos = GetAimPoint();
        if (_startedAssetsInputs.aim)
        {
            EnterAimMode(aimPos);

            if (_startedAssetsInputs.shoot && Time.time >= _nextFireTime + fireRate)
            {
                _nextFireTime = Time.time;
                Shoot(aimPos);
            }
        }
        else
        {
            ExitAimMode();
        }
    }

    private void EnterAimMode(Vector3 aimPos)
    {
        _thirdPersonController.SetRotateOnMove(false);
        _thirdPersonController.SetSensitivity(aimSensitivity);

        Vector3 aimDirection = new Vector3(aimPos.x, transform.position.y, aimPos.z);
        transform.forward = Vector3.Lerp(transform.forward, (aimDirection - transform.position).normalized, Time.deltaTime * 20f);

        aimCamera.Priority = 10;
        crosshair.SetActive(true);
    }

    private void ExitAimMode()
    {
        _thirdPersonController.SetRotateOnMove(true);
        _thirdPersonController.SetSensitivity(normalSensitivity);

        aimCamera.Priority = -1;
        crosshair.SetActive(false);
    }

    private Vector3 GetAimPoint()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimColliderLayerMask))
        {
            aimTransform.position = hit.point;
            return hit.point;
        }

        return ray.GetPoint(1000);
    }

    private void Shoot(Vector3 aimPos)
    {
        Vector3 direction = (aimPos - spawnBulletPosition.position).normalized;
        Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(direction, Vector3.up));
    }
}
