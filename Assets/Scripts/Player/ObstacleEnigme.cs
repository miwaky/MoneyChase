using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleEnigme : MonoBehaviour
{
    public static ObstacleEnigme Instance;

    public enum EnigmeColor { Red, Blue }
    public enum EnigmeCalcul { Red, Blue }

    // === Configuration ===
    [Header("Énigme de Couleur")]
    [SerializeField] private EnigmeColor GoodColor;

    [Header("Énigme de Calcul")]
    [SerializeField] private EnigmeCalcul GoodCalcul;
    [SerializeField] public int ResultEnigmeCalcul { get; private set; }
    public int redValue { get; private set; }
    public int blueValue { get; private set; }

    private int valueA;
    private int valueB;
    private int badValue;

    [Header("Affichage")]
    [SerializeField] private TextMeshProUGUI textAffiche;

    // === Init Singleton ===
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void StartEnigme()
    {
        int enigmeType = Random.Range(0, 2); // 0 ou 1

        if (enigmeType == 0)
            StartEnigmeCalcul();
        else
            StartEnigmeColor();
    }

    private void StartEnigmeCalcul()
    {
        valueA = Random.Range(0, 10);
        valueB = Random.Range(0, 10);
        ResultEnigmeCalcul = valueA + valueB;

        // Génère une mauvaise valeur différente
        do
        {
            badValue = Random.Range(0, 20);
        } while (badValue == ResultEnigmeCalcul);

        // Tire au hasard la bonne réponse sur Red ou Blue
        bool goodOnRed = Random.value > 0.5f;

        if (goodOnRed)
        {
            redValue = ResultEnigmeCalcul;
            blueValue = badValue;
            GoodCalcul = EnigmeCalcul.Red;
        }
        else
        {
            redValue = badValue;
            blueValue = ResultEnigmeCalcul;
            GoodCalcul = EnigmeCalcul.Blue;
        }

        textAffiche.text = $"{valueA} + {valueB} = ?";
    }

    public void StartEnigmeColor()
    {
        GoodColor = (Random.value > 0.5f) ? EnigmeColor.Red : EnigmeColor.Blue;
        textAffiche.text = $"Va vers le {GoodColor}";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FirstEnigmeGround"))
        {
            StartEnigme(); // Redémarre une énigme si on retraverse le trigger
            return;
        }

        if (other.CompareTag("RedDoor"))
        {
            EvaluerReponse(GoodColor == EnigmeColor.Red || GoodCalcul == EnigmeCalcul.Red);
        }

        if (other.CompareTag("BlueDoor"))
        {
            EvaluerReponse(GoodColor == EnigmeColor.Blue || GoodCalcul == EnigmeCalcul.Blue);
        }
    }

    private void EvaluerReponse(bool estCorrect)
    {
        Inventory.Instance.MoneyInInventory += estCorrect ? 100 : -100;
        textAffiche.text = "";
    }
}
