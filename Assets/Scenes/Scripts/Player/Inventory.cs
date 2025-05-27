using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;



public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [Header("Inventory")]
    [SerializeField]
    public int MoneyInInventory = 0;
    [SerializeField]
    private int billAmount = 50;
    [SerializeField]
    private int wadAmount = 100;
    [SerializeField]
    private int suitcaseAmount = 500;
    [SerializeField]
    private int DiamondAmount = 1000;

    private bool level2Unlock = false;
    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI m_Text;

    [Header("Death")]
    [SerializeField] int MoneyLoseInDeath = 500;
    [SerializeField] Company compagnies;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Évite les doublons
        }
    }
  
    // Update is called once per frame
    void Update()
    {
        if (m_Text != null)
            m_Text.text = "Money : " + MoneyInInventory;

        if (MoneyInInventory >= 2000 && !level2Unlock)
        {
            level2Unlock = true;
            LevelManager.Instance.currentLevel = 2;
        }
        if (MoneyInInventory < 0)
        {
            MoneyInInventory = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {


        DropMoney(other);

    }

    private void DropMoney(Collider other)
    {
         int amount = 0;


        if (other.name.Contains("Bill"))
        {
            amount = billAmount;
        }
        else if (other.name.Contains("Wad"))
        {
            amount = wadAmount;
        }
        else if (other.name.Contains("Suitcase"))
        {
            amount = suitcaseAmount;
        }
        else if (other.name.Contains("Diamond"))
        {
            amount = DiamondAmount;
        }
        MoneyInInventory += amount;

        //  Notifie le QuestManager pour vérifier si l'argent est lié à une quete
        QuestManager.Instance?.AddMoneyToQuests(amount);

        Destroy(other.gameObject);
        }

    internal void DeathMoney()
    {
        
        MoneyInInventory -= MoneyLoseInDeath;
        if (MoneyInInventory < 0) { MoneyInInventory=0; } 
        // Met à jour le score dans le ScriptableObject
        foreach (var company in compagnies.GetCompanies())
        {
            if (company.player)
            {
                company.score = MoneyInInventory;
                break;
            }
        }

        compagnies.SaveToFile();
    }
}




    


