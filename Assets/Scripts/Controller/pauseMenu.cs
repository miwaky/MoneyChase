using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    #region Fields

    [SerializeField] private GameObject pauseUI;  // R�f�rence � l'UI de pause
    private bool isPaused = false;                // �tat actuel du jeu

    #endregion

    #region Unity Callbacks

    void Start()
    {
        // Cache le menu de pause au d�marrage
        if (pauseUI != null)
            pauseUI.SetActive(false);
    }

    void Update()
    {
        // Touche Echap pour activer/d�sactiver la pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                Pause();
            else
                Resume();
        }
    }

    #endregion

    #region Pause Methods

    // Met le jeu en pause
    public void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;
        if (pauseUI != null)
            pauseUI.SetActive(true);
    }

    // Reprend le jeu
    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;
        if (pauseUI != null)
            pauseUI.SetActive(false);
    }

    // Quitte vers le menu principal
    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    #endregion
}
