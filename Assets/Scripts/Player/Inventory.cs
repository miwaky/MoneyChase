using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public interface IUsableItem
{
    void Activate();
    Sprite GetIcon();
    int GetPrice();
    bool IsPassive();
}

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public int MoneyInInventory = 0;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private Image activeItemIcon;
    [Header("Money")]
    [SerializeField] private int billAmount = 50;
    [SerializeField] private int wadAmount = 100;
    [SerializeField] private int suitcaseAmount = 500;
    [SerializeField] private int DiamondAmount = 1000;


    private float moneyMultiplier = 1f;
    [Header("Score")]
    [SerializeField] Company compagnies;
    private GameObject currentItem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
         
        m_Text.text = "Money : " + MoneyInInventory;

        LevelManager.Instance.CheckLevelProgression(MoneyInInventory);

    }
    private void OnTriggerEnter(Collider other)
    {
        DropMoney(other);
    }

    private void DropMoney(Collider other)
    {
        int amount = 0;

        if (other.name.Contains("Bill")) amount = billAmount;
        else if (other.name.Contains("Wad")) amount = wadAmount;
        else if (other.name.Contains("Suitcase")) amount = suitcaseAmount;
        else if (other.name.Contains("Diamond")) amount = DiamondAmount;

        Destroy(other.gameObject);
        MoneyInInventory += Mathf.RoundToInt(amount * moneyMultiplier);
    }
    public void SetCurrentItem(GameObject itemPrefab)
    {
        if (currentItem != null)
        {
            if (currentItem.name.Contains(itemPrefab.name))
            {
                Debug.Log("[Inventory] Même objet déjà possédé.");
                return;
            }
            Destroy(currentItem);
        }

        currentItem = Instantiate(itemPrefab);
        currentItem.SetActive(false);

        IUsableItem usable = currentItem.GetComponent<IUsableItem>();
        if (usable != null && activeItemIcon != null)
        {
            activeItemIcon.sprite = usable.GetIcon();
            activeItemIcon.enabled = true;
        }
    }

    public void UseCurrentItem()
    {
        if (currentItem != null)
        {
            IUsableItem usable = currentItem.GetComponent<IUsableItem>();
            if (usable != null)
            {
                if (!usable.IsPassive())
                {
                    usable.Activate();
                    Destroy(currentItem);
                    currentItem = null;
                    activeItemIcon.enabled = false;
                }
                else
                {
                    Debug.Log("[Inventory] Impossible d'activer un objet passif manuellement.");
                }
            }
        }
    }

    public bool HasItem(GameObject itemPrefab)
    {
        if (currentItem == null) return false;
        return currentItem.name.Contains(itemPrefab.name);
    }

    public bool HasPassiveItem<T>() where T : IUsableItem
    {
        if (currentItem == null) return false;
        return currentItem.GetComponent<T>() != null;
    }

    public void ConsumePassiveItem<T>() where T : IUsableItem
    {
        if (currentItem != null && currentItem.GetComponent<T>() != null)
        {
            Destroy(currentItem);
            currentItem = null;
            activeItemIcon.enabled = false;
        }
    }

    public void SetMoneyMultiplier(float multiplier, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(MoneyBoost(multiplier, duration));
    }

    private IEnumerator MoneyBoost(float multiplier, float duration)
    {
        moneyMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        moneyMultiplier = 1f;
    }

    internal void DeathMoney()
    {
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
