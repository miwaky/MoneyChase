using UnityEngine;

public class AutoDestroyBehindPlayer : MonoBehaviour
{
    [SerializeField] private float offsetZ = 10f; // Distance derri�re le joueur avant destruction

    private Transform player;

    private void Start()
    {
        if (PlayerControler.Instance != null)
            player = PlayerControler.Instance.transform;
    }

    private void Update()
    {
        if (player == null) return;

        if (transform.position.z < player.position.z - offsetZ)
        {
            Destroy(gameObject);
        }
    }
}
