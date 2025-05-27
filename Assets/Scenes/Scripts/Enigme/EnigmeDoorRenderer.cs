using TMPro;
using UnityEngine;

public class EnigmeDoorRenderer  : MonoBehaviour
{
    [SerializeField] private TextMeshPro RedText;
    [SerializeField] private TextMeshPro BlueText;
    [SerializeField] private TextMeshPro EnigmeText;

    private void Start()
    {
        Transform Redchild = transform.Find("RedText");
        Transform Bluechild = transform.Find("BlueText");
        Transform EnigmeChild = transform.Find("EnigmeText");

        RedText = Redchild.GetComponent<TextMeshPro>();
        BlueText = Bluechild.GetComponent<TextMeshPro>();
        EnigmeText = EnigmeChild.GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        if (ObstacleEnigme.Instance == null) return;

        RedText.text = ObstacleEnigme.Instance.redValue.ToString();
        BlueText.text = ObstacleEnigme.Instance.blueValue.ToString();
        EnigmeText.text = ObstacleEnigme.Instance.Enigme.ToString();
    }

}
