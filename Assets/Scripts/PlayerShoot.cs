using System;
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private Rig aimRig;
    [SerializeField] private CinemachineCamera aimCamera;
    [SerializeField] private CinemachineCamera followCamera;
    [SerializeField] private float aimSensitivity, normalSensitivity;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform aimTransform;
    [SerializeField] private Gun gun;

    private StarterAssetsInputs _startedAssetsInputs;
    private ThirdPersonController _thirdPersonController;
    private Animator _animator;
    private float _aimRigWeight;

    private void Awake()
    {
        _startedAssetsInputs = GetComponent<StarterAssetsInputs>();
        _thirdPersonController = GetComponent<ThirdPersonController>();
        _animator = GetComponent<Animator>();

        aimCamera.Priority = -1;
        crosshair.SetActive(false);
    }

    private void Update()
    {
        Vector3 aimPos = GetAimPoint();
        if (_startedAssetsInputs.aim)
        {
            EnterAimMode(aimPos);

            // chỉ cho bắn khi đang nhắm
            if (_startedAssetsInputs.shoot)
            {
                gun.Shoot(aimPos);
            }
        }
        else
        {
            ExitAimMode();
        }

        if (_startedAssetsInputs.reload)
        {
            gun.Reload();
            _startedAssetsInputs.reload = false;
        }
        
        aimRig.weight = Mathf.Lerp(aimRig.weight, _aimRigWeight, Time.deltaTime * 10f);
    }

    private void EnterAimMode(Vector3 aimPos)
    {
        _thirdPersonController.SetRotateOnMove(false);
        _thirdPersonController.SetSensitivity(aimSensitivity);
        _thirdPersonController.SetSprint(false);
        _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1, Time.deltaTime * 10f));

        Vector3 aimDirection = new Vector3(aimPos.x, transform.position.y, aimPos.z);
        transform.forward = Vector3.Lerp(transform.forward, (aimDirection - transform.position).normalized, Time.deltaTime * 20f);

        aimCamera.Priority = 10;
        crosshair.SetActive(true);

        _aimRigWeight = 1f;
    }

    private void ExitAimMode()
    {
        _thirdPersonController.SetRotateOnMove(true);
        _thirdPersonController.SetSensitivity(normalSensitivity);
        _thirdPersonController.SetSprint(true);
        _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0, Time.deltaTime * 10f));

        aimCamera.Priority = -1;
        crosshair.SetActive(false);

        followCamera.GetComponent<CinemachineOrbitalFollow>().VerticalAxis.Value = 
        aimCamera.GetComponent<CinemachineOrbitalFollow>().VerticalAxis.Value;
        followCamera.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Value = 
        aimCamera.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Value;

        _aimRigWeight = 0f;
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

        return ray.GetPoint(1000f);
    }
}
