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
    [SerializeField] public string Enigme { get; private set; }

    public int redValue { get; private set; }
    public int blueValue { get; private set; }

    private int valueA;
    private int valueB;
    private int badValue;
    private bool enigmeActive = false;

    private enum TypeEnigme { Calcul, Couleur }
    private TypeEnigme currentType;

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
    private void Start()
    {
        StartEnigme();
    }
    private void StartEnigme()
    {
        if (enigmeActive) return;
        enigmeActive = true;

        int enigmeType = Random.Range(0, 2); // 0 ou 1

        if (enigmeType == 0)
            StartEnigmeCalcul();
        else
            StartEnigmeColor();
    }

    private void StartEnigmeCalcul()
    {
        currentType = TypeEnigme.Calcul;
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

        Enigme = $"{valueA} + {valueB} = ?";
    }

    public void StartEnigmeColor()
    {
        currentType = TypeEnigme.Couleur;
        GoodColor = (Random.value > 0.5f) ? EnigmeColor.Red : EnigmeColor.Blue;
        Enigme = GoodColor.ToString();
       
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
            if (currentType == TypeEnigme.Couleur)
                EvaluerReponse(GoodColor == EnigmeColor.Red);
            else
                EvaluerReponse(GoodCalcul == EnigmeCalcul.Red);
        }

        if (other.CompareTag("BlueDoor"))
        {
            if (currentType == TypeEnigme.Couleur)
                EvaluerReponse(GoodColor == EnigmeColor.Blue) ;
            else
                EvaluerReponse(GoodCalcul == EnigmeCalcul.Blue);
        }
    }

    private void EvaluerReponse(bool estCorrect)
    {
        Inventory.Instance.MoneyInInventory += estCorrect ? 100 : -100;
        textAffiche.text = "";
        enigmeActive = false;
        StartEnigme();
    }
}
