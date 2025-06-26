using UnityEngine;

public class rotate : MonoBehaviour
{
    #region Rotation Settings

    [SerializeField] private Vector3 AxeRotation = new Vector3(); // Axe de rotation (X, Y, Z)
    [SerializeField] private float SpeedRotation = 50f;           // Vitesse de rotation en degr�s/seconde

    #endregion

    #region Unity Callbacks

    // Appel� � chaque frame pour appliquer la rotation
    private void Update()
    {
        transform.Rotate(AxeRotation * SpeedRotation * Time.deltaTime);
    }

    #endregion
}
