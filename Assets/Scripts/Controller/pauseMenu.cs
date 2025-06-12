using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;
    private bool isPaused = false;

    void Start()
    {
        // Cache l'UI au début
        if (pauseUI != null)
            pauseUI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                Pause();
            else
                Resume();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;
        if (pauseUI != null)
            pauseUI.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pauseUI != null)
            pauseUI.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        SceneManager.LoadScene("MainMenu");
    }
}
