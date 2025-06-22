using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    #region Scene Management

    // Quitte compl�tement le jeu (ne fonctionne que dans une build, pas dans l'�diteur)
    public void exitGame()
    {
        Application.Quit();
    }

    // Charge la sc�ne du menu principal
    public void SwitchMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Charge la sc�ne de cr�ation de soci�t�
    public void SwitchNewCompany()
    {
        SceneManager.LoadScene("NewCompanyScene");
    }

    #endregion
}
