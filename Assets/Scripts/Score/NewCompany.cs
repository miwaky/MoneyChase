using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewCompany : MonoBehaviour
{
    #region Inspector References

    [SerializeField] private Company companyData;         // Référence au scriptable object contenant la liste des entreprises
    [SerializeField] private TMP_InputField companyName;  // Champ de saisie pour le nom de la nouvelle entreprise

    #endregion

    #region Public Methods

    // Méthode appelée pour valider la création et lancer le jeu
    public void StartGame()
    {
        // 🔁 Ajout : Charger les données depuis le fichier en amont
        companyData.LoadFromFile();

        if (!string.IsNullOrEmpty(companyName.text) && Regex.IsMatch(companyName.text, "[a-zA-Z]"))
        {
            if (companyData.CompanyNameExists(companyName.text))
            {
                Debug.Log("Company already exists.");
                return;
            }

            CompanyScore newCompany = new CompanyScore
            {
                id = 0,
                name = companyName.text,
                scores = new List<int> { 0 },
                destroyable = true,
                player = true
            };

            companyData.SetActiveCompany(newCompany);
            companyData.AddCompany(newCompany);
            companyData.SaveToFile();

            SceneManager.LoadScene("Game");
        }
    }


    #endregion
}
