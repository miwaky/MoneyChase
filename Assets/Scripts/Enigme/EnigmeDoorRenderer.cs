using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class EnigmeDoorRenderer  : MonoBehaviour
{
    [SerializeField] private TextMeshPro RedText;
    [SerializeField] private TextMeshPro BlueText;
    [SerializeField] private TextMeshPro EnigmeText;
    [SerializeField] private GameObject Tv;
    
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
        EnigmeText.text = ObstacleEnigme.Instance.Enigme.ToString();

        float PosTvZ =  Tv.transform.position.z;
    }

}
