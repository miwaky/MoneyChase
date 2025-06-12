using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    public void game()
    {
        SceneManager.LoadScene("Game");

    }

    public void menu()
    {
        SceneManager.LoadScene("MainMenu");
    }


}
