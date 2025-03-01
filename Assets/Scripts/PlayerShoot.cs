using System;
using StarterAssets;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject aimCamera;
    [SerializeField] private float aimSensitivity, normalSensitivity;

    private StarterAssetsInputs _startedAssetsInputs;
    private ThirdPersonController _thirdPersonController;

    private void Awake()
    {
        _startedAssetsInputs = GetComponent<StarterAssetsInputs>();
        _thirdPersonController = GetComponent<ThirdPersonController>();

        aimCamera.SetActive(false);
    }

    private void Update()
    {
        if (_startedAssetsInputs.aim)
        {
            EnterAimMode();
        }
        else
        {
            ExitAimMode();
        }
    }

    private void EnterAimMode()
    {
        _thirdPersonController.SetSensitivity(aimSensitivity);
        aimCamera.SetActive(true);
    }

    private void ExitAimMode()
    {
        _thirdPersonController.SetSensitivity(normalSensitivity);
        aimCamera.SetActive(false);
    }
}
