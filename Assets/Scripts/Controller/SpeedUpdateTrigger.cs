using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpeedUpdateTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerControler player = other.GetComponent<PlayerControler>();
        if (player == null)
        {
            Debug.LogWarning("[SpeedUpdateTrigger] Aucun PlayerControler trouv� sur le joueur.");
            return;
        }

        // Applique la vitesse actuelle du niveau via le LevelManager
        LevelManager.Instance.AppliquerVitesseJoueur();

        Debug.Log($"[SpeedUpdateTrigger] Trigger activ� � Vitesse mise � jour pour niveau {LevelManager.Instance.currentLevel}");
    }
}


