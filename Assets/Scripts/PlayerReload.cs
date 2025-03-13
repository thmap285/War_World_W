using System;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerReload : MonoBehaviour
{
    [SerializeField] private GunAnimationEvent gunAnimationEvent;
    [SerializeField] private Transform leftHand;

    private StarterAssetsInputs _input;
    private Gun _gun;
    private GameObject _magazineHand;

    private void Start()
    {
        gunAnimationEvent.AnimationEvent.AddListener(OnAnimationEvent);
        _input = GetComponent<StarterAssetsInputs>();
        GetComponent<PlayerEquip>().OnGunEquipped += UpdateGun;
    }

    private void Update()
    {
        if (!_gun) return;

        if (_input.reload)
        {
            if (_gun.MaxAmmo > 0 && _gun.Ammo < _gun.ClipSize)
            {
                _gun.Reload();
            }
            _input.reload = false;
        }
    }

    private void OnAnimationEvent(string eventName)
    {
        switch (eventName)
        {
            case "Detach_Magazine":
                Detach_Magazine();
                break;
            case "Drop_Magazine":
                Drop_Magazine();
                break;
            case "Refill_Magazine":
                Refill_Magazine();
                break;
            case "Attach_Magazine":
                Attach_Magazine();
                break;
        }
    }

    private void Detach_Magazine()
    {
        _magazineHand = Instantiate(_gun.Magazine, leftHand, true);
        _gun.Magazine.SetActive(false);
    }

    private void Drop_Magazine()
    {
        GameObject magazine = Instantiate(_magazineHand, _magazineHand.transform.position, _magazineHand.transform.rotation);
        magazine.AddComponent<Rigidbody>();
        magazine.AddComponent<BoxCollider>();
        _magazineHand.SetActive(false);

        Destroy(magazine, 10f);
    }

    private void Refill_Magazine()
    {
        _magazineHand.SetActive(true);
    }

    private void Attach_Magazine()
    {
        _gun.Magazine.SetActive(true);
        Destroy(_magazineHand);
    }

    private void UpdateGun(Gun newWeapon)
    {
        _gun = newWeapon;
    }
}
