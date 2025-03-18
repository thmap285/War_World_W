using TMPro;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance;
    [SerializeField] private int point = 0;
    [SerializeField] private TextMeshProUGUI pointText;
    [SerializeField] private GameObject fadeGreen, fadeRed;
    [SerializeField] private Transform tfFadePoint;

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

    private void Start()
    {
        pointText.text = point.ToString() + "p";
    }

    public void AddPoints(int amount)
    {
        point += amount;
        pointText.text = point.ToString() + "p";
        
        GameObject textObj = Instantiate(fadeGreen, tfFadePoint.position, Quaternion.identity);
        textObj.transform.SetParent (GameObject.FindGameObjectWithTag("Canvas").transform, false);
        textObj.transform.position = tfFadePoint.position;
        textObj.GetComponent<TMP_Text>().text = "+ " + amount;
    }

    public void MinusPoints(int amount)
    {
        point -= amount;
        pointText.text = point.ToString() + "p";
        
        GameObject textObj = Instantiate(fadeRed, tfFadePoint.position, Quaternion.identity);
        textObj.transform.SetParent (GameObject.FindGameObjectWithTag("Canvas").transform, false);
        textObj.transform.position = tfFadePoint.position;
        textObj.GetComponent<TMP_Text>().text = "- " + amount;
    }

    public bool HasEnoughPoints(int requiredAmount)
    {
        return point >= requiredAmount;
    }
}
