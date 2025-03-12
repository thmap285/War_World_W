using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] private Gun pfGun;
    [SerializeField] private float rotationSpeed = 50f;
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        if (_rb)
        {
            _rb.isKinematic = true;
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime), Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerEquip equip = other.GetComponent<PlayerEquip>();
        if (equip)
        {
            Gun gun = Instantiate(pfGun);
            equip.EquipGun(gun);
            
            Destroy(gameObject);
        }
    }
}
