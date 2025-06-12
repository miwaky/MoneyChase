using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewCompany : MonoBehaviour
{
    [SerializeField] private Company companyData;
    [SerializeField] private TMP_InputField companyName;

    private GameObject GameObject;

    private void Start()
    {
    }
    public void StartGame()
    {
        if (!string.IsNullOrEmpty(companyName.text) && Regex.IsMatch(companyName.text, "[a-zA-Z]")) //vérifie s'il y a au moins 1 lettre. 
        {
            CompanyScore newCompany = new CompanyScore
            {

                id = 0,
                name = companyName.text,
                score = 0,
                destroyable = true,
                player = true

            }; 

            companyData.AddCompany(newCompany);
            SceneManager.LoadScene("Game");
        }
        
    }
}