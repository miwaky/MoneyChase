using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreUi : MonoBehaviour
{
    [SerializeField] private Company companies;           
    [SerializeField] private GameObject scoreTextPrefab;    
    [SerializeField] private Transform contentContainer;
    private int classement = 1;

    void Start()
    {
        List<CompanyScore> scores = companies.GetCompanies();
        scores = scores.OrderByDescending(s => s.score).ToList();

        foreach (var company in scores)
        {
            GameObject instance = Instantiate(scoreTextPrefab, contentContainer);
            TextMeshProUGUI text = instance.GetComponent<TextMeshProUGUI>();
            text.text = $"{classement} . {company.name} : {company.score}";
            classement++;
        }
    }
}
