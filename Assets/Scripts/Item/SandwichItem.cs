using UnityEngine;

public class SandwichItem : MonoBehaviour, IUsableItem
{
    [SerializeField] private Sprite icon;
    [SerializeField] private int price = 1000;

    public int GetPrice() => price;

    public void Activate()
    {
        
    }

    public Sprite GetIcon() => icon;

    public bool IsPassive() => true; // c'est un objet passif
}
