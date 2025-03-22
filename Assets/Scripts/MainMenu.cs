using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnButtonStart()
    {
        // Load the game scene
        SceneManager.LoadScene("Main");
    }

    public void OnButtonQuit()
    {
        // Quit the game
        Application.Quit();
    }
}
