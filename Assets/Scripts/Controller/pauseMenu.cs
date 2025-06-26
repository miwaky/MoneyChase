using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    #region Fields

    [SerializeField] private GameObject pauseUI;  // Référence à l'UI de pause
    private bool isPaused = false;                // État actuel du jeu

    #endregion

    #region Unity Callbacks

    void Start()
    {
        // Cache le menu de pause au démarrage
        if (pauseUI != null)
            pauseUI.SetActive(false);
    }

    void Update()
    {
        // Touche Echap pour activer/désactiver la pause
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
