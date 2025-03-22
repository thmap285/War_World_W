using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Text ammoText, maxAmmoText, gunNameText, hpText;
    [SerializeField] private Image hpBar;

    private Gun _gun;
    private PlayerHealth _playerHealth;

    private void Start()
    {
        _playerHealth = GetComponent<PlayerHealth>();
        GetComponent<PlayerEquip>().OnGunEquipped += UpdateGun;

        SetCrosshair(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            _playerHealth.TakeDamage(10);
        }
        hpBar.fillAmount = _playerHealth.CurrentHealth / _playerHealth.MaxHealth;
        hpText.text = $"{_playerHealth.CurrentHealth / _playerHealth.MaxHealth * 100:0}%";

        if (_gun)
        {
            Refresh();
        }
        else
        {
            ammoText.text = "0";
            maxAmmoText.text = "0";
            gunNameText.text = " ";
        }

    }

    private void Refresh()
    {
        ammoText.text = _gun.Ammo.ToString();
        maxAmmoText.text = _gun.TotalAmmo.ToString();
        gunNameText.text = _gun.Name;
    }

    public void SetCrosshair(bool isActive)
    {
        crosshair.SetActive(isActive);
    }

    private void UpdateGun(Gun newWeapon)
    {
        _gun = newWeapon;
        Refresh();
    }
}
