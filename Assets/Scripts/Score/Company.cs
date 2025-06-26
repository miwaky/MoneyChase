using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#region Score Class - Données de performance d'une entreprise

[System.Serializable]
public class CompanyScore
{
    public int id;
    public string name;
    public List<int> scores = new List<int>(); // Historique des scores obtenus
    public float variation; // Variation de performance par rapport au score précédent
    public bool destroyable; // Peut-elle être supprimée ?
    public bool player; // Est-ce l'entreprise active (le joueur) ?
    public int totalMoney = 0; // Argent total accumulé
    public int nbrTry = 0; // Nombre de tentatives
    public bool level2 = false;
    public bool level3 = false;
    public bool level4 = false;
    public bool level5 = false;
    public int nbrRessurect = 0;

    // Ajoute un nouveau score et calcule la variation
    public void AddScore(int newScore)
    {
        scores.Add(newScore);

        if (scores.Count >= 2)
        {
            int previous = scores[scores.Count - 2];
            if (previous == 0)
                variation = (newScore == 0) ? 0f : 100f;
            else
                variation = ((float)(newScore - previous) / Mathf.Abs(previous)) * 100f;
        }
        else
        {
            variation = 0f;
        }
    }

    // Récupère le dernier score connu
    public int GetLatestScore()
    {
        return scores.Count > 0 ? scores[scores.Count - 1] : 0;
    }
}

#endregion

#region Company Class - Gestion des entreprises et sauvegarde

[CreateAssetMenu(fileName = "Company", menuName = "Scriptable Objects/Company")]
public class Company : ScriptableObject
{
    [SerializeField] private List<CompanyScore> companies = new List<CompanyScore>();

    [System.Serializable]
    private class CompanyDataWrapper { public List<CompanyScore> companies; }

    // Retourne la liste des entreprises
    public List<CompanyScore> GetCompanies() => companies;

    // Ajoute une entreprise
    public void AddCompany(CompanyScore company) => companies.Add(company);

    // Supprime une entreprise
    public void DeleteCompany(CompanyScore companyToDelete) => companies.Remove(companyToDelete);

    // Retourne l’entreprise actuellement contrôlée par le joueur
    public CompanyScore GetActiveCompany() => companies.FirstOrDefault(c => c.player);

    // Définit l’entreprise jouée comme active, les autres deviennent inactives
    public void SetActiveCompany(CompanyScore selectedCompany)
    {
        foreach (var c in companies)
            c.player = false;

        selectedCompany.player = true;
    }

    // Vérifie si une entreprise avec le même nom existe déjà (insensible à la casse)
    public bool CompanyNameExists(string name)
    {
        return companies.Any(c => c.name.ToLower() == name.ToLower());
    }

    // Sauvegarde la liste des entreprises dans un fichier JSON local
    public void SaveToFile()
    {
        // Debug pour confirmer le chemin de sauvegarde
        string path = Application.persistentDataPath + "/companies.json";
        Debug.Log($"[SAVE] Tentative de sauvegarde dans : {path}");

        if (companies == null)
        {
            Debug.LogError("[SAVE] La liste des entreprises est NULL – aucune donnée à sauvegarder.");
            return;
        }

        Debug.Log($"[SAVE] Nombre d'entreprises à sauvegarder : {companies.Count}");

        try
        {
            CompanyDataWrapper wrapper = new CompanyDataWrapper { companies = companies };
            string json = JsonUtility.ToJson(wrapper, true);

            File.WriteAllText(path, json);
            Debug.Log("[SAVE] Sauvegarde réussie.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SAVE] Erreur lors de la sauvegarde : {e.Message}");
        }
    }


    // Charge la liste des entreprises depuis le fichier JSON
    public void LoadFromFile()
    {
        string path = Application.persistentDataPath + "/companies.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            CompanyDataWrapper wrapper = JsonUtility.FromJson<CompanyDataWrapper>(json);
            companies = wrapper.companies;

            foreach (var c in companies)
            {
                if (c.scores == null)
                    c.scores = new List<int>();

                // Recalcule la variation après chargement
                if (c.scores.Count >= 2)
                {
                    int previous = c.scores[c.scores.Count - 2];
                    int latest = c.scores[c.scores.Count - 1];
                    c.variation = (previous == 0) ? (latest == 0 ? 0f : 100f) : ((float)(latest - previous) / Mathf.Abs(previous)) * 100f;
                }
                else
                {
                    c.variation = 0f;
                }
            }

            Debug.Log("Scores loaded from file.");
        }
        else
        {
            Debug.LogWarning("Save file not found at: " + path);
        }
    }
}

#endregion
