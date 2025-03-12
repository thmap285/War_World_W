using System;
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Rig[] aimRigs;
    [SerializeField] private float aimDuration = 10f;
    [SerializeField] private Animator rigAnimator;
    [SerializeField] private CinemachineCamera aimCamera;
    [SerializeField] private CinemachineCamera followCamera;
    [SerializeField] private float aimSensitivity, normalSensitivity;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private LayerMask aimColliderLayerMask;
    [SerializeField] private Transform aimTransform;

    private StarterAssetsInputs _input;
    private ThirdPersonController _controller;
    private Gun _gun;
    private float _aimRigWeight;

    private void Awake()
    {
        _input = GetComponent<StarterAssetsInputs>();
        _controller = GetComponent<ThirdPersonController>();
        GetComponent<PlayerEquip>().OnGunEquipped += UpdateGun;

        aimCamera.Priority = -1;
        crosshair.SetActive(false);
    }

    private void Update()
    {
        HandleAimRig();
        if (!_gun) return;

        Vector3 aimPos = GetAimPoint();
        if (_input.aim)
        {
            EnterAimMode(aimPos);
            if (_input.shoot) _gun.Shoot(aimPos);
        }
        else
        {
            ExitAimMode();
        }

        if (_input.reload)
        {
            _gun.Reload();
            _input.reload = false;
        }
    }

    private void HandleAimRig()
    {
        float targetWeight = _gun ? _aimRigWeight : 0f;
        foreach (Rig rig in aimRigs)
        {
            rig.weight = Mathf.Lerp(rig.weight, targetWeight, Time.deltaTime * aimDuration);
        }
    }

    private void EnterAimMode(Vector3 aimPos)
    {
        _controller.SetRotateOnMove(false);
        _controller.SetSensitivity(aimSensitivity);
        _controller.SetSprint(false);
        
        aimCamera.Priority = 10;
        crosshair.SetActive(true);
        rigAnimator.SetBool("Holster_Weapon", false);

        Vector3 aimDirection = new Vector3(aimPos.x, transform.position.y, aimPos.z);
        transform.forward = Vector3.Lerp(transform.forward, 
                (aimDirection - transform.position).normalized, Time.deltaTime * 20f);
        
        _aimRigWeight = 1f;
    }

    private void ExitAimMode()
    {
        _controller.SetRotateOnMove(true);
        _controller.SetSensitivity(normalSensitivity);
        _controller.SetSprint(true);

        aimCamera.Priority = -1;
        crosshair.SetActive(false);
        rigAnimator.SetBool("Holster_Weapon", true);
        
        SyncCameraAxes();
        
        _aimRigWeight = 0f;
    }

    private void SyncCameraAxes()
    {
        var follow = followCamera.GetComponent<CinemachineOrbitalFollow>();
        var aim = aimCamera.GetComponent<CinemachineOrbitalFollow>();
        follow.VerticalAxis.Value = aim.VerticalAxis.Value;
        follow.HorizontalAxis.Value = aim.HorizontalAxis.Value;
    }

    public Vector3 GetAimPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimColliderLayerMask))
        {
            aimTransform.position = hit.point;
            return hit.point;
        }
        return ray.GetPoint(aimTransform.position.z);
    }

    private void UpdateGun(Gun newWeapon)
    {
        _gun = newWeapon;
    }
}
