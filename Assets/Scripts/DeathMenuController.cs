using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenuController : MonoBehaviour
{
    [SerializeField] private string defaultLevelName = "Level1_Layout";
    [SerializeField] private string mainMenuSceneName = "Menu";

    public void StartAgain()
    {
        Time.timeScale = 1f;

        string levelToLoad = PlayerPrefs.GetString("LastPlayedLevel", defaultLevelName);
        SceneManager.LoadScene(levelToLoad);
    }
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
