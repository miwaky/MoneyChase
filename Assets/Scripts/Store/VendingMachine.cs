using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendingMachine : MonoBehaviour
{
    [SerializeField] private int cost = 500;
    [SerializeField] private List<GameObject> itemPrefabs;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI priceText;

    private bool used = false;
    private GameObject selectedItem;

    private void Start()
    {
        selectedItem = itemPrefabs[Random.Range(0, itemPrefabs.Count)];

        IUsableItem usable = selectedItem.GetComponent<IUsableItem>();

        if (usable != null)
        {
            if (iconImage != null)
                iconImage.sprite = usable.GetIcon();

            cost = usable.GetPrice();  // <<< ici tu récupères le prix directement depuis l’objet
        }

        if (priceText != null)
            priceText.text = cost + "$";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (used || other.tag != "Player") return;

        if (Inventory.Instance.HasItem(selectedItem))
        {
            Debug.Log("[VendingMachine] Objet déjà possédé. Aucun paiement.");
            used = true;
            PlayerControler player = other.GetComponent<PlayerControler>();
            if (player != null) player.RestoreSpeedAfterPause();
            return;
        }

        if (Inventory.Instance.MoneyInInventory >= cost)
        {
            Inventory.Instance.MoneyInInventory -= cost;
            Inventory.Instance.SetCurrentItem(selectedItem);
            used = true;
            PlayerControler player = other.GetComponent<PlayerControler>();
            if (player != null) player.RestoreSpeedAfterPause();
        }
        else
        {
            Debug.Log("[VendingMachine] Pas assez d'argent !");
        }
    }
    private void DisableAllVendingMachinesInParent()
    {
        Transform root = transform.parent;
        if (root == null) return;

        var vendingMachines = root.GetComponentsInChildren<VendingMachine>();
        foreach (var vm in vendingMachines)
        {
            vm.GetComponent<Collider>().enabled = false;
            vm.used = true;
        }
    }
}