using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunPickup : MonoBehaviour
{
    [SerializeField] private Gun pfGun;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private int requiredPoints = 1000;
    [SerializeField] private TextMeshProUGUI pointsText;

    private Rigidbody _rb;
    private bool _isPlayerInRange = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        if (_rb)
        {
            _rb.isKinematic = true;
        }

        pointsText.text = requiredPoints.ToString() + " Points";
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime), Space.World);

        Vector3 direction = pointsText.transform.position - Camera.main.transform.position;
        direction.y = 0;
        pointsText.transform.rotation = Quaternion.LookRotation(direction);

        if (_isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            PickupGun();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInRange = false;
        }
    }


    private void PickupGun()
    {
        if (PointsManager.Instance != null && PointsManager.Instance.HasEnoughPoints(requiredPoints))
        {
            PlayerEquip equip = FindFirstObjectByType<PlayerEquip>();
            if (equip)
            {
                Gun gun = Instantiate(pfGun);
                equip.EquipGun(gun);

                PointsManager.Instance.MinusPoints(requiredPoints);

                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("Không đủ điểm để mua súng!");
        }
    }
}
