using UnityEngine;

public class SandwichItem : MonoBehaviour, IUsableItem
{
    #region Inspector Fields

    [SerializeField] private Sprite icon;           // Icône affichée dans l'inventaire
    [SerializeField] private int price = 1000;      // Prix de l'objet en monnaie

    #endregion

    #region Interface IUsableItem

    public int GetPrice() => price;                 // Retourne le prix de l'objet

    public Sprite GetIcon() => icon;                // Retourne l'icône de l'objet

    public bool IsPassive() => true;                // Cet objet est passif (pas besoin d'activation manuelle)

    public void Activate()
    {
        // Aucun effet actif : objet purement passif
    }

    #endregion
}
