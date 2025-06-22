using UnityEngine;
using System.Linq;
using static GroundData;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    #region Inspector Fields

    // Prefabs de sol pour chaque niveau
    [Header("Ground prefabs")]
    [SerializeField] private GameObject[] GroundsPrefabsSafe;
    [SerializeField] private GameObject[] GroundsPrefabsLevel1;
    [SerializeField] private GameObject[] GroundsPrefabsLevel2;
    [SerializeField] private GameObject[] GroundsPrefabsLevel3;
    [SerializeField] private GameObject[] GroundsPrefabsLevel4;
    [SerializeField] private GameObject[] GroundsPrefabsLevel5;
    [SerializeField] private GameObject[] GroundsOnStage; // Sols actuellement en scène

    // Prefabs d'obstacles par niveau
    [Header("Obstacle prefabs")]
    [SerializeField] private GameObject[] ObstaclePrefabsSafe;
    [SerializeField] private GameObject[] ObstaclePrefabsLevel1;
    [SerializeField] private GameObject[] ObstaclePrefabsLevel2;
    [SerializeField] private GameObject[] ObstaclePrefabsLevel3;
    [SerializeField] private GameObject[] ObstaclePrefabsLevel4;
    [SerializeField] private GameObject[] ObstaclePrefabsLevel5;
    [SerializeField] private GameObject[] ObstacleOnStage;

    // Prefabs d'argent par niveau
    [Header("Money prefabs")]
    [SerializeField] private GameObject[] MoneyPrefabsSafe;
    [SerializeField] private GameObject[] MoneyPrefabsLevel1;
    [SerializeField] private GameObject[] MoneyPrefabsLevel2;
    [SerializeField] private GameObject[] MoneyPrefabsLevel3;
    [SerializeField] private GameObject[] MoneyPrefabsLevel4;
    [SerializeField] private GameObject[] MoneyPrefabsLevel5;
    [SerializeField] private GameObject[] MoneyOnStage;

    // Prefabs d'énigmes par niveau
    [Header("Enigme prefabs")]
    [SerializeField] private GameObject[] EnigmePrefabsLevel1;
    [SerializeField] private GameObject[] EnigmePrefabsLevel2;
    [SerializeField] private GameObject[] EnigmePrefabsLevel3;
    [SerializeField] private GameObject[] EnigmePrefabsLevel4;
    [SerializeField] private GameObject[] EnigmePrefabsLevel5;
    [SerializeField] private int enigmeIntervalMin = 20;
    [SerializeField] private int enigmeIntervalMax = 30;

    // Paramètres de configuration du niveau
    [Header("Config")]
    [SerializeField] public float groundSize { get; private set; } = 10f;
    [SerializeField] private int nbGroundsSafeAuDebut = 8;
    [SerializeField] public int currentLevel = 1;
    [SerializeField] private int nbrGround = 4;

    // Vitesse du joueur selon le niveau
    [Header("Vitesses par niveau")]
    [SerializeField] private float forwardSpeedLevel0 = 4f;
    [SerializeField] private float forwardSpeedLevel1 = 5.5f;
    [SerializeField] private float forwardSpeedLevel2 = 7f;
    [SerializeField] private float forwardSpeedLevel3 = 8.5f;
    [SerializeField] private float forwardSpeedLevel4 = 10f;
    [SerializeField] private float forwardSpeedLevel5 = 11.5f;

    // Paramètres pour les salles de pause
    [Header("Salle de pause")]
    [SerializeField, Range(0f, 1f)] private float chanceToSpawnPauseRoom = 0.1f;
    [SerializeField] private int minGroundsBetweenPauseRooms = 40;
    [SerializeField] private GameObject prefabSallePause;

    // Seuils pour changer de niveau en fonction de l'argent
    [Header("Level progression")]
    [SerializeField] private int level2AtMoney = 2000;
    [SerializeField] private int level3AtMoney = 5000;
    [SerializeField] private int level4AtMoney = 8000;
    [SerializeField] private int level5AtMoney = 12000;

    #endregion

    #region Private Fields

    private int groundsSinceLastPauseRoom = 0; // Compteur de sols depuis la dernière salle de pause
    private bool eligibleForPauseSpawn = false; // Est-ce qu'on peut générer une salle de pause
    private int groundCounterSinceLastEnigme = 0; // Compteur de sols depuis la dernière énigme
    private int nextEnigmeAt = 0; // Prochaine énigme prévue après X sols
    private GameObject player;
    private int pendingEnigmeSequence = 0; // Nombre de segments d’énigmes restants à générer

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player not found – ajoute le tag 'Player' à ton objet joueur");
            return;
        }

        AppliquerVitesseJoueur(); // Appliquer la vitesse initiale selon le niveau

        GroundsOnStage = new GameObject[nbrGround];
        groundSize = PlayerControler.Instance.groundDepth; // Récupère la taille du sol
        InitGrounds(); // Initialise les premiers sols

        pendingEnigmeSequence = 0;
        groundCounterSinceLastEnigme = 0;
        nextEnigmeAt = nbGroundsSafeAuDebut + Random.Range(enigmeIntervalMin, enigmeIntervalMax + 1);
    }

    private void Update()
    {
        GroundLevel(); // Met à jour les sols au fur et à mesure que le joueur avance
    }

    #endregion

    #region Level Progression

    // Vérifie si on doit changer de niveau en fonction de l'argent actuel
    public void CheckLevelProgression(int money)
    {
        int oldLevel = currentLevel;

        if (money >= level5AtMoney) currentLevel = 5;
        else if (money >= level4AtMoney) currentLevel = 4;
        else if (money >= level3AtMoney) currentLevel = 3;
        else if (money >= level2AtMoney) currentLevel = 2;
        else currentLevel = 1;

        if (currentLevel != oldLevel)
        {
            AppliquerVitesseJoueur();
            Debug.Log($"[LevelManager] Changement de niveau : {oldLevel} → {currentLevel}");
        }
    }

    #endregion

    #region Ground Logic

    // Instancie les premiers sols au démarrage du jeu
    private void InitGrounds()
    {
        for (int i = 0; i < nbrGround; i++)
        {
            GameObject prefab = (i < nbGroundsSafeAuDebut)
                ? GroundsPrefabsSafe.FirstOrDefault(p => p != null && p.name.Contains("GroundSafe")) ?? GroundsPrefabsSafe[0]
                : GetGroundPrefab(currentLevel);

            GroundsOnStage[i] = Instantiate(prefab);
        }

        float posZ = player.transform.position.z + groundSize / 2f - 1.5f;
        foreach (var ground in GroundsOnStage)
        {
            ground.transform.position = new Vector3(0f, 0.2f, posZ);
            posZ += groundSize;
        }
    }

    // Gère la progression du terrain au fur et à mesure que le joueur avance
    private void GroundLevel()
    {
        groundsSinceLastPauseRoom++;

        for (int i = GroundsOnStage.Length - 1; i >= 0; i--)
        {
            GameObject ground = GroundsOnStage[i];
            if (ground == null || ground.transform.position.z + groundSize / 2f < player.transform.position.z - 6f)
            {
                if (ground != null) Destroy(ground);

                groundCounterSinceLastEnigme++;

                if (groundCounterSinceLastEnigme >= nextEnigmeAt)
                {
                    TriggerNextEnigmeSequence();
                    groundCounterSinceLastEnigme = 0;
                    nextEnigmeAt = Random.Range(enigmeIntervalMin, enigmeIntervalMax + 1);
                }

                float lastZ = GroundsOnStage.Max(g => g != null ? g.transform.position.z : 0f);

                if (groundsSinceLastPauseRoom >= minGroundsBetweenPauseRooms)
                    eligibleForPauseSpawn = true;

                if (eligibleForPauseSpawn && Random.value < chanceToSpawnPauseRoom)
                {
                    GameObject pauseRoom = Instantiate(prefabSallePause);
                    pauseRoom.transform.position = new Vector3(0f, 0.2f, lastZ + groundSize);
                    GroundsOnStage[i] = pauseRoom;

                    groundsSinceLastPauseRoom = 0;
                    eligibleForPauseSpawn = false;
                    return;
                }

                GameObject prefab = GetGroundPrefab(currentLevel);
                GameObject newGround = Instantiate(prefab);
                newGround.transform.position = new Vector3(0f, 0.2f, lastZ + groundSize);
                GroundsOnStage[i] = newGround;
            }
        }
    }

    #endregion

    #region Prefab Choosers

    // Retourne un prefab de sol correspondant au niveau actuel ou à une énigme
    public GameObject GetGroundPrefab(int level)
    {
        if (pendingEnigmeSequence > 0)
        {
            int sequenceIndex = 5 - pendingEnigmeSequence;
            pendingEnigmeSequence--;

            EnigmeZonePosition pos = sequenceIndex switch
            {
                0 or 1 => EnigmeZonePosition.Before,
                3 or 4 => EnigmeZonePosition.After,
                _ => EnigmeZonePosition.Center
            };

            GameObject result = GroundsPrefabsSafe.FirstOrDefault(g => g != null && g.GetComponent<GroundData>()?.EnigmePosition == pos);
            return result ?? GroundsPrefabsSafe[0];
        }

        return level switch
        {
            0 => GroundsPrefabsSafe.RandomElement(),
            1 => GroundsPrefabsLevel1.RandomElement(),
            2 => GroundsPrefabsLevel2.RandomElement(),
            3 => GroundsPrefabsLevel3.RandomElement(),
            4 => GroundsPrefabsLevel4.RandomElement(),
            5 => GroundsPrefabsLevel5.RandomElement(),
            _ => GroundsPrefabsSafe[0]
        };
    }

    public GameObject GetObstaclePrefab() => GetObstaclePrefab(currentLevel);
    public GameObject GetObstaclePrefab(int level) => level switch
    {
        1 => ObstaclePrefabsLevel1.RandomElement(),
        2 => ObstaclePrefabsLevel2.RandomElement(),
        3 => ObstaclePrefabsLevel3.RandomElement(),
        4 => ObstaclePrefabsLevel4.RandomElement(),
        5 => ObstaclePrefabsLevel5.RandomElement(),
        _ => null
    };

    public GameObject GetMoneyPrefab() => GetMoneyPrefab(currentLevel);
    public GameObject GetMoneyPrefab(int level) => level switch
    {
        1 => MoneyPrefabsLevel1.RandomElement(),
        2 => MoneyPrefabsLevel2.RandomElement(),
        3 => MoneyPrefabsLevel3.RandomElement(),
        4 => MoneyPrefabsLevel4.RandomElement(),
        5 => MoneyPrefabsLevel5.RandomElement(),
        _ => null
    };

    public GameObject GetEnigmePrefab(int level) => level switch
    {
        1 => EnigmePrefabsLevel1.RandomElement(),
        2 => EnigmePrefabsLevel2.RandomElement(),
        3 => EnigmePrefabsLevel3.RandomElement(),
        4 => EnigmePrefabsLevel4.RandomElement(),
        5 => EnigmePrefabsLevel5.RandomElement(),
        _ => null
    };

    #endregion

    #region Utility

    // Applique la vitesse du joueur en fonction du niveau courant
    public void AppliquerVitesseJoueur()
    {
        var controller = player.GetComponent<PlayerControler>();
        if (controller == null)
        {
            Debug.LogError("[LevelManager] Aucun PlayerControler trouvé !");
            return;
        }

        float speed = currentLevel switch
        {
            0 => forwardSpeedLevel0,
            1 => forwardSpeedLevel1,
            2 => forwardSpeedLevel2,
            3 => forwardSpeedLevel3,
            4 => forwardSpeedLevel4,
            5 => forwardSpeedLevel5,
            _ => forwardSpeedLevel1
        };

        controller.forwardSpeed = speed;

        Debug.Log($"[VITESSE] Appliquée depuis LevelManager → Niveau {currentLevel} → Vitesse = {speed}");
    }


    // Lance une séquence de 5 zones pour une énigme
    public void TriggerNextEnigmeSequence()
    {
        pendingEnigmeSequence = 5;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    #endregion
}

// Extension utile pour tirer un élément aléatoire d’un tableau
public static class ArrayExtensions
{
    public static T RandomElement<T>(this T[] array) =>
        array != null && array.Length > 0 ? array[Random.Range(0, array.Length)] : default;
}
