using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


// Gère l'affichage de la liste des entreprises (joueurs) dans l'écran de sélection.

public class CompanyListUI : MonoBehaviour
{
    #region Inspector References

    [SerializeField] private Company companies;                   // Référence au ScriptableObject contenant toutes les entreprises
    [SerializeField] private GameObject companyEntryPrefab;       // Préfab d'une ligne d'affichage pour une entreprise
    [SerializeField] private Transform contentContainer;          // Conteneur dans lequel instancier les lignes

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        RefreshUI(); // Remplit la liste à l'ouverture de l'écran
    }

    #endregion

    #region UI Logic

    // Vide et regénère entièrement la liste affichée des entreprises.

    private void RefreshUI()
    {
        foreach (Transform child in contentContainer)
            Destroy(child.gameObject);

        List<CompanyScore> scores = companies.GetCompanies()
            .OrderByDescending(s => s.scores.LastOrDefault()) // Tri décroissant selon le dernier score
            .ToList();

        foreach (var company in scores)
        {
            GameObject instance = Instantiate(companyEntryPrefab, contentContainer);

            // Nom de l'entreprise
            instance.transform.Find("CompanyName").GetComponent<TextMeshProUGUI>().text = company.name;

            // Score affiché
            int lastScore = company.scores.LastOrDefault();
            instance.transform.Find("Score").GetComponent<TextMeshProUGUI>().text = $"{lastScore}";

            // Variation de score entre les deux derniers essais
            
            string variationText;

            if (company.scores.Count > 1)
            {
                int previous = company.scores[company.scores.Count - 2];

                if (previous == 0)
                {
                    variationText = "";
                }
                else
                {
                    float variation = company.variation;
                    string sign = variation >= 0 ? "+" : "";
                    variationText = $"({sign}{variation:F2}%)";
                }
            }
            else
            {
                variationText = "";
            }

            instance.transform.Find("Variation").GetComponent<TextMeshProUGUI>().text = variationText;

            // Boutons
            Button playButton = instance.transform.Find("PlayButton").GetComponent<Button>();
            Button deleteButton = instance.transform.Find("DeleteButton").GetComponent<Button>();

            playButton.onClick.AddListener(() => SelectCompany(company));
            deleteButton.onClick.AddListener(() => DeleteCompany(company));
        }
    }

     
    //Active une entreprise et charge la scène principale.
    private void SelectCompany(CompanyScore company)
    {
        companies.SetActiveCompany(company);
        companies.SaveToFile();
        SceneManager.LoadScene("Game");
    }

     
    // Supprime une entreprise et rafraîchit l'affichage.
    private void DeleteCompany(CompanyScore company)
    {
        companies.DeleteCompany(company);
        companies.SaveToFile();
        RefreshUI();
    }

    #endregion
}
