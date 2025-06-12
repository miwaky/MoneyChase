using UnityEngine;

public class MoneyBoostItem : MonoBehaviour, IUsableItem
{
    [SerializeField] private float duration = 10f;
    [SerializeField] private Sprite icon;
    [SerializeField] private int price = 1000;

    public int GetPrice() => price;

    public void Activate()
    {
        Inventory.Instance.SetMoneyMultiplier(2f, duration);
        Destroy(gameObject);
    }

    public Sprite GetIcon() => icon;

    public bool IsPassive() => false;
}