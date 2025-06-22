using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;


// G�re les �nigmes dans les zones sp�ciales du jeu (calcul ou couleur).
// Le joueur choisit une porte rouge ou bleue selon l'�nonc�.

public class ObstacleEnigme : MonoBehaviour
{
    public static ObstacleEnigme Instance;

    public enum EnigmeColor { Red, Blue }
    public enum EnigmeCalcul { Red, Blue }

    private enum TypeEnigme { Calcul, Couleur }

    #region �nigme Actuelle

    [Header("�nigme de Couleur")]
    [SerializeField] private EnigmeColor GoodColor;

    [Header("�nigme de Calcul")]
    [SerializeField] private EnigmeCalcul GoodCalcul;
    [field: SerializeField] public int ResultEnigmeCalcul { get; private set; }
    [field: SerializeField] public string Enigme { get; private set; }
    [Header("R�compenses par niveau")]
    [SerializeField] private int[] rewardPerLevel = new int[5] { 100, 200, 400, 600, 700 };

    [Header("P�nalit�s par niveau")]
    [SerializeField] private int[] penaltyPerLevel = new int[5] { 100, 150, 400, 600, 700 };
    
    
    
    public int redValue { get; private set; }
    public int blueValue { get; private set; }

    private int valueA;
    private int valueB;
    private int badValue;

    private bool enigmeActive = false;
    private TypeEnigme currentType;

    #endregion

    #region UI

    [Header("Affichage")]
    [SerializeField] private TextMeshProUGUI textAffiche;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StartEnigme(); // D�marrage automatique d'une �nigme au lancement
    }

    #endregion

    #region G�n�ration d'�nigmes

    private void StartEnigme()
    {
        if (enigmeActive) return;
        enigmeActive = true;

        int enigmeType = Random.Range(0, 2); // 0 = Calcul, 1 = Couleur
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

        // G�n�re une fausse r�ponse
        do
        {
            badValue = Random.Range(0, 20);
        } while (badValue == ResultEnigmeCalcul);

        // Position al�atoire de la bonne r�ponse
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
        GoodColor = Random.value > 0.5f ? EnigmeColor.Red : EnigmeColor.Blue;
        Enigme = GoodColor.ToString();
    }

    #endregion

    #region D�tection de Collision

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FirstEnigmeGround"))
        {
            StartEnigme(); // Permet une relance si retour en arri�re
            return;
        }

        if (other.CompareTag("RedDoor"))
        {
            EvaluerReponse(currentType == TypeEnigme.Couleur
                ? GoodColor == EnigmeColor.Red
                : GoodCalcul == EnigmeCalcul.Red);
        }

        if (other.CompareTag("BlueDoor"))
        {
            EvaluerReponse(currentType == TypeEnigme.Couleur
                ? GoodColor == EnigmeColor.Blue
                : GoodCalcul == EnigmeCalcul.Blue);
        }
    }

    #endregion

    #region R�sultat et R�initialisation

    private void EvaluerReponse(bool estCorrect)
    {
        int level = Mathf.Clamp(LevelManager.Instance.currentLevel, 1, rewardPerLevel.Length);
        int reward = rewardPerLevel[level - 1];
        int penalty = penaltyPerLevel[level - 1];

        Inventory.Instance.MoneyInInventory += estCorrect ? reward : -penalty;
        textAffiche.text = ""; // Efface l'affichage
        enigmeActive = false;
        StartEnigme(); // Relance une nouvelle �nigme automatiquement
    }

    #endregion
}
