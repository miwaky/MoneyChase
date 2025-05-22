using UnityEngine;

public class rotate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Vector3 AxeRotation = new Vector3();

    [SerializeField] float SpeedRotation = 50f;
    private void Update()
    {
        transform.Rotate(AxeRotation * SpeedRotation * Time.deltaTime);
    }
}
