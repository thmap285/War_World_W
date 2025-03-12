using System;
using UnityEngine;

public class PlayerEquip : MonoBehaviour
{
    public event Action<Gun> OnGunEquipped;

    [SerializeField] private Transform weaponParent;
    [SerializeField] private Transform weaponRightGrip;
    [SerializeField] private Transform weaponLeftGrip;
    [SerializeField] private Animator rigAnimator;

    private Gun _gun;

    private void Start()
    {
        _gun = GetComponentInChildren<Gun>();
        
        EquipGun(_gun);

        // rigAnimator.updateMode = AnimatorUpdateMode.Fixed;
        // rigAnimator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        // rigAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        // rigAnimator.updateMode = AnimatorUpdateMode.Normal;
    }

    public void EquipGun(Gun newGun)
    {
        if (_gun)
        {
            Destroy(_gun.gameObject);
        }

        _gun = newGun;

        if (_gun)
        {
            _gun.transform.parent = weaponParent;
            _gun.transform.localPosition = Vector3.zero;
            _gun.transform.localRotation = Quaternion.identity;

            rigAnimator.Play("Holster_" + _gun.Name, 0, 0f);
        }

        OnGunEquipped?.Invoke(_gun);
    }
}
