using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    #region Scene Management

    // Quitte complètement le jeu (ne fonctionne que dans une build, pas dans l'éditeur)
    public void exitGame()
    {
        Application.Quit();
    }

    // Charge la scène du menu principal
    public void SwitchMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Charge la scène de création de société
    public void SwitchNewCompany()
    {
        SceneManager.LoadScene("NewCompanyScene");
    }

    #endregion
}
