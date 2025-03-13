using UnityEngine;
using TMPro;
using System.Collections;

public class FadeText : MonoBehaviour
{
    [SerializeField] private float fadeInTime = 0.5f;
    [SerializeField] private float visibleTime = 1f;
    [SerializeField] private float fadeOutTime = 0.5f;

    private TextMeshProUGUI textMesh;
    private RectTransform rectTransform;

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        StartCoroutine(AnimateText());
    }

    public IEnumerator AnimateText()
    {
        Color textColor = textMesh.color;
        textColor.a = 0;
        textMesh.color = textColor;

        float timer = 0;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            textColor.a = Mathf.Lerp(0, 1, timer / fadeInTime);
            textMesh.color = textColor;
            yield return null;
        }

        yield return new WaitForSeconds(visibleTime);

        timer = 0;
        Vector3 startScale = rectTransform.localScale;
        Vector3 targetScale = startScale * 1.5f;

        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;

            textColor.a = Mathf.Lerp(1, 0, timer / fadeOutTime);
            textMesh.color = textColor;

            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, timer / fadeOutTime);

            yield return null;
        }

        Destroy(gameObject);
    }
}
