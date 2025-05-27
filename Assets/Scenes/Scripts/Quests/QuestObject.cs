using UnityEngine;

[RequireComponent(typeof(Collider))]
public class QuestObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (QuestManager.Instance == null)
        {
            Debug.LogWarning("[QuestObject] QuestManager manquant !");
            return;
        }

        bool added = QuestManager.Instance.TryAddQuest();

        if (added)
        {
            Debug.Log("[QuestObject] Quête ajoutée au joueur !");
            Destroy(gameObject); // détruire l'objet de quête après interaction
        }
        else
        {
            Debug.Log("[QuestObject] Le joueur a déjà trop de quêtes actives.");
        }
    }
}
