using UnityEngine;

public class randomPosTv : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private float posZ;
    [SerializeField] private float posX;
    [SerializeField] private float posY;

    #endregion

    #region Unity Callbacks

    // Appelé une fois au démarrage
    void Start()
    {
        // Tire aléatoirement gauche (0) ou droite (1)
        int LeftOrRight = Random.Range(0, 2); // correction : max est exclusif
        if (LeftOrRight == 0) posX = -7.32f;
        else posX = 3.74f;

        // Recul aléatoire sur l’axe Z par rapport à la position actuelle
        float currentPosZ = transform.position.z;
        posZ = currentPosZ - Random.Range(250, 320);

        // Garde la hauteur d’origine
        posY = transform.position.y;

        // Applique la nouvelle position
        transform.position = new Vector3(posX, posY, posZ);
    }

    #endregion
}
