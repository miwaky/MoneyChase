using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

[System.Serializable]
public class CompanyScore
{
    public int id;
    public string name;
    public int score;
    public bool destroyable;
    public bool player;
}


[CreateAssetMenu(fileName = "Company", menuName = "Scriptable Objects/Company")]
public class Company : ScriptableObject
{
    [SerializeField] private List<CompanyScore> companies = new List<CompanyScore>();

    public List<CompanyScore> GetCompanies() => companies;

    public void AddCompany(CompanyScore company)
    {
        companies.Add(company);
    }

    [System.Serializable]
    private class CompanyDataWrapper
    {
        public List<CompanyScore> companies;
    }

    public void SaveToFile()
    {
        CompanyDataWrapper wrapper = new CompanyDataWrapper { companies = companies };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(Application.persistentDataPath + "/companies.json", json);
        Debug.Log("Scores saved to: " + Application.persistentDataPath);
    }

    public void LoadFromFile()
    {
        string path = Application.persistentDataPath + "/companies.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            CompanyDataWrapper wrapper = JsonUtility.FromJson<CompanyDataWrapper>(json);
            companies = wrapper.companies;
            Debug.Log("Scores loaded from file.");
        }
        else
        {
            Debug.LogWarning("Save file not found at: " + path);
        }
    }

   // Question, vous savez comment fonctionne la sauvegarde dans le json?
}