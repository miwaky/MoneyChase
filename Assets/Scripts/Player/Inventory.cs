using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
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

    private void Awake()
    {
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
    }

    private void OnTriggerEnter(Collider other)
    {


        DropMoney(other);

    }

    private void DropMoney(Collider other)
    {

        if (other.CompareTag("Money"))
        {
            string name = other.gameObject.name;

            if (name.Contains("Bill"))
            {
                MoneyInInventory += billAmount;
            }
            else if (name.Contains("Wad"))
            {
                MoneyInInventory += wadAmount;
            }
            else if (name.Contains("Suitcase"))
            {
                MoneyInInventory += suitcaseAmount;
            }
            else if (name.Contains("Diamond"))
            {
                MoneyInInventory += DiamondAmount;
            }

            Destroy(other.gameObject);
        }
    }
}


