using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquip : MonoBehaviour
{
    public event Action<Gun> OnGunEquipped;

    [SerializeField] private Transform weaponParent;
    [SerializeField] private Animator rigAnimator;

    private List<Gun> _ownedGuns = new List<Gun>();
    private int _currentGunIndex = 0;
    private Gun _currentGun;

    private void Start()
    {
        _currentGun = GetComponentInChildren<Gun>();
        
        if(_currentGun)
        {
            _ownedGuns.Add(_currentGun);
            EquipGun(_currentGun);
        }

        // rigAnimator.updateMode = AnimatorUpdateMode.Fixed;
        // rigAnimator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        // rigAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        // rigAnimator.updateMode = AnimatorUpdateMode.Normal;
    }

    public void EquipGun(Gun newGun)
    {
        if (!_ownedGuns.Contains(newGun))
        {
            _ownedGuns.Add(newGun);
        }

        if (_currentGun)
        {
            _currentGun.gameObject.SetActive(false);
        }

        _currentGun = newGun;
        _currentGunIndex = _ownedGuns.IndexOf(newGun);
        _currentGun.gameObject.SetActive(true);

        _currentGun.transform.parent = weaponParent;
        _currentGun.transform.localPosition = Vector3.zero;
        _currentGun.transform.localRotation = Quaternion.identity;
        _currentGun.rigAnimator = rigAnimator;

        rigAnimator.Play("Equip_" + _currentGun.Name, 0, 0f);

        OnGunEquipped?.Invoke(_currentGun);
    }

    public void SwitchGun()
    {
        if (_ownedGuns.Count <= 1) return;

        _currentGunIndex = (_currentGunIndex + 1) % _ownedGuns.Count;
        EquipGun(_ownedGuns[_currentGunIndex]);
    }
}
