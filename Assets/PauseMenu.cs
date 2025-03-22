using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;
    [SerializeField] private GameObject canvas, noficationCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        SetCanvasActive(false);
    }

    public void SetCanvasActive(bool active)
    {
        gameObject.SetActive(active);
        canvas.SetActive(!active);
        noficationCanvas.SetActive(!active);
    }

    public void OnButtonMenu()
    {
        // Load the game scene
        SceneManager.LoadScene("Menu");
    }

    public void OnButtonPlayAgain()
    {
        // Quit the game
        SceneManager.LoadScene("Main");
    }
}
