using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    [SerializeField] private GameObject[] notificationPrefabs; // Mảng chứa các prefab
    [SerializeField] private Transform notificationParent;
    [SerializeField] private float notificationDuration = 3f;
    [SerializeField] private int maxNotification = 3;
    [SerializeField] private float fadeDuration = 0.3f;
    private List<GameObject> notifications = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowNotification(string message, int prefabIndex = 0)
    {
        StartCoroutine(ShowNotificationCoroutine(message, prefabIndex));
    }

    private IEnumerator ShowNotificationCoroutine(string message, int prefabIndex)
    {
        // Kiểm tra nếu prefabIndex hợp lệ
        if (prefabIndex < 0 || prefabIndex >= notificationPrefabs.Length)
        {
            Debug.LogWarning($"⚠️ Prefab index {prefabIndex} không hợp lệ! Dùng mặc định (0).");
            prefabIndex = 0; // Nếu lỗi, dùng prefab đầu tiên
        }

        GameObject prefabToUse = notificationPrefabs[prefabIndex];

        // Tạo thông báo mới
        GameObject newNotification = Instantiate(prefabToUse, notificationParent);
        newNotification.GetComponentInChildren<Text>().text = message;
        newNotification.transform.localPosition = new Vector3(0, notifications.Count, 0);

        notifications.Add(newNotification);

        // Lấy CanvasGroup để tạo hiệu ứng
        CanvasGroup cg = newNotification.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0;
            cg.DOKill();
            cg.DOFade(1, fadeDuration).SetUpdate(true);
        }

        // Xóa thông báo cũ nếu quá giới hạn
        if (notifications.Count > maxNotification)
        {
            GameObject oldest = notifications[0];
            notifications.RemoveAt(0);

            if (oldest != null)
            {
                CanvasGroup oldCg = oldest.GetComponent<CanvasGroup>();
                if (oldCg != null)
                {
                    oldCg.DOKill();
                    oldCg.DOFade(0, fadeDuration).OnComplete(() =>
                    {
                        if (oldest != null) Destroy(oldest);
                    });
                }
                else
                {
                    Destroy(oldest);
                }
            }
        }

        yield return new WaitForSeconds(notificationDuration);

        // Kiểm tra nếu notification chưa bị xóa
        if (newNotification != null)
        {
            cg = newNotification.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.DOKill();
                cg.DOFade(0, fadeDuration).OnComplete(() =>
                {
                    if (newNotification != null)
                    {
                        notifications.Remove(newNotification);
                        Destroy(newNotification);
                    }
                });
            }
            else
            {
                notifications.Remove(newNotification);
                Destroy(newNotification);
            }
        }
    }
}
