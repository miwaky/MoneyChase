using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#region Interface Usable Item
public interface IUsableItem
{
    void Activate();         // Ce que l'objet fait quand on l'utilise
    Sprite GetIcon();        // Pour afficher dans l'UI
    int GetPrice();          // Prix (si boutique)
    bool IsPassive();        // Passif = automatique ou Actif = déclenché manuellement
}
#endregion

public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    #endregion

    #region Fields

    public int MoneyInInventory = 0;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private Image activeItemIcon;

    [Header("Money Values")]
    [SerializeField] private int billAmount = 50;
    [SerializeField] private int wadAmount = 100;
    [SerializeField] private int suitcaseAmount = 500;
    [SerializeField] private int DiamondAmount = 1000;

    private float moneyMultiplier = 1f; // Ex: bonus x2

    [Header("Score / Entreprises")]
    [SerializeField] Company compagnies;
    private GameObject currentItem;

    #endregion

    #region Unity Callbacks

    private void Start()
    {
        activeItemIcon.enabled = false;
    }

    private void Update()
    {
        m_Text.text = " " + MoneyInInventory;

        // Met à jour la progression du joueur selon son argent
        LevelManager.Instance.CheckLevelProgression(MoneyInInventory);
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isTagValid = other.CompareTag("Money");
        bool isTrigger = other.GetComponent<Collider>()?.isTrigger == true;

        if (!isTagValid || !isTrigger)
        {
            Debug.LogWarning($"[Debug] Problème sur {other.name} => Tag: {other.tag}, IsTrigger: {isTrigger}");
        }
        else
        {
            Debug.Log($"[Debug] {other.name} OK ");
        }

        DropMoney(other);
    }

    #endregion

    #region Money Pickup

    private void DropMoney(Collider other)
    {
        int amount = 0;

        // Attribution de valeur en fonction du nom de l’objet ramassé
        if (other.name.Contains("Bill")) amount = billAmount;
        else if (other.name.Contains("Wad")) amount = wadAmount;
        else if (other.name.Contains("Suitcase")) amount = suitcaseAmount;
        else if (other.name.Contains("Diamond")) amount = DiamondAmount;

        Destroy(other.gameObject);

        // Ajoute l'argent avec éventuel bonus multiplicateur
        MoneyInInventory += Mathf.RoundToInt(amount * moneyMultiplier);
    }

    #endregion

    #region Item Management

    public void SetCurrentItem(GameObject itemPrefab)
    {
        // Empêche d’empiler deux fois le même objet
        if (currentItem != null)
        {
            if (currentItem.name.Contains(itemPrefab.name))
            {
                Debug.Log("[Inventory] Même objet déjà possédé.");
                return;
            }
            Destroy(currentItem);
        }

        // Instancie l’objet mais ne l’active pas
        currentItem = Instantiate(itemPrefab);
        currentItem.SetActive(false);

        // Affiche l’icône dans l’UI
        IUsableItem usable = currentItem.GetComponent<IUsableItem>();
        activeItemIcon.sprite = usable.GetIcon();
        activeItemIcon.enabled = true;
    
    }

    public void UseCurrentItem()
    {
        if (currentItem == null) return;

        IUsableItem usable = currentItem.GetComponent<IUsableItem>();
        if (usable == null) return;

        if (!usable.IsPassive())
        {
            usable.Activate();
            Destroy(currentItem);
            currentItem = null;
            activeItemIcon.enabled = false;
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

    #endregion

    #region Boost Management

    public void SetMoneyMultiplier(float multiplier, float duration)
    {
        StopAllCoroutines(); // Pour éviter plusieurs boosts en même temps
        StartCoroutine(MoneyBoost(multiplier, duration));
    }

    private IEnumerator MoneyBoost(float multiplier, float duration)
    {
        moneyMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        moneyMultiplier = 1f;
    }

    #endregion

   
}
