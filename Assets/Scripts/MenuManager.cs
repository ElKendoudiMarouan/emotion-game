using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager  : MonoBehaviour
{
    public static bool gameIsPaused;
    public GameObject pauseMenuUI;
    public GameObject conversationMenuUI;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                ShowConversationMenu();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        conversationMenuUI.SetActive(false);

        Time.timeScale = 1.0f;
        gameIsPaused = false;

    }
    void Pause()
    {
        pauseMenuUI.SetActive(true);   
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    void ShowConversationMenu()
    {
        conversationMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void loadMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
