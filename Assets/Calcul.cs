using TMPro;
using UnityEngine;

public class Calcul : MonoBehaviour
{
    [SerializeField] private TextMeshPro RedText;
    [SerializeField] private TextMeshPro BlueText;

    private void Start()
    {
        Transform Redchild = transform.Find("RedText");
        Transform Bluechild = transform.Find("BlueText");

        RedText = Redchild.GetComponent<TextMeshPro>();
        BlueText = Bluechild.GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        if (ObstacleEnigme.Instance == null) return;

        RedText.text = ObstacleEnigme.Instance.redValue.ToString();
        BlueText.text = ObstacleEnigme.Instance.blueValue.ToString();
    }

}
