using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunPickup : MonoBehaviour
{
    [SerializeField] private Gun pfGun;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private int pointsEquip = 1000;
    [SerializeField] private int pointsReload = 300;
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

        pointsText.text = pointsEquip.ToString() + " Points";
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
        if (PointsManager.Instance.HasEnoughPoints(pointsEquip))
        {
            PlayerEquip equip = FindFirstObjectByType<PlayerEquip>();
            if (equip)
            {
                Gun existingGun = equip.OwnedGuns.Find(gun => gun.Name == pfGun.Name);

                if (existingGun)
                {
                    if (existingGun.TotalAmmo >= existingGun.SizeOfMagazine * existingGun.MaxMagazines)
                    {
                        NotificationManager.Instance.ShowNotification("Bạn đã có vũ khí này!", 1);
                        return;
                    }

                    if (PointsManager.Instance.HasEnoughPoints(pointsReload))
                    {
                        existingGun.AddAmmo(existingGun.SizeOfMagazine * 2); // Thêm 2 băng đạn
                        PointsManager.Instance.MinusPoints(pointsReload);
                        NotificationManager.Instance.ShowNotification("Bạn đã nạp đạn thành công!", 0);
                    }
                    else
                    {
                        NotificationManager.Instance.ShowNotification("Không đủ điểm để nạp đạn!", 1);
                    }
                }
                else
                {
                    Gun gun = Instantiate(pfGun);
                    equip.EquipGun(gun);
                    PointsManager.Instance.MinusPoints(pointsEquip);
                    NotificationManager.Instance.ShowNotification("Bạn đã mua súng " + gun.Name + " thành công!", 0);
                    pointsText.text = pointsReload.ToString() + " Points";
                }
            }
        }
        else
        {
            NotificationManager.Instance.ShowNotification("Không đủ điểm để mua súng!", 1);
        }
    }
}
