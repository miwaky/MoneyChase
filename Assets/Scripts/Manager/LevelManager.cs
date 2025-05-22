using UnityEngine;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    #region Inspector fields

    [Header("Ground prefabs")]
    [SerializeField] private GameObject[] GroundsPrefabsSafe;
    [SerializeField] private GameObject[] GroundsPrefabsLevel1;
    [SerializeField] private GameObject[] GroundsPrefabsLevel2;
    [SerializeField] private GameObject[] GroundsOnStage;

    [Header("Obstacle prefabs")]
    [SerializeField] private GameObject[] ObstaclePrefabsSafe;
    [SerializeField] private GameObject[] ObstaclePrefabsLevel1;
    [SerializeField] private GameObject[] ObstaclePrefabsLevel2;
    [SerializeField] private GameObject[] ObstacleOnStage;

    [Header("Money prefabs")]
    [SerializeField] private GameObject[] MoneyPrefabsSafe;
    [SerializeField] private GameObject[] MoneyPrefabsLevel1;
    [SerializeField] private GameObject[] MoneyPrefabsLevel2;
    [SerializeField] private GameObject[] MoneyOnStage;

    [Header("Enigme prefabs")]
    [SerializeField] private GameObject[] EnigmePrefabsLevel1;
    [SerializeField] private GameObject[] EnigmePrefabsLevel2;
    private int groundCounterSinceLastEnigme = 0;
    private int nextEnigmeAt = 0;

    [SerializeField] private int enigmeIntervalMin = 20;
    [SerializeField] private int enigmeIntervalMax = 30;

    [Header("Config")]
    [SerializeField] private int nbGroundsSafeAuDebut = 4;
    [SerializeField] public int currentLevel = 1;
    [SerializeField] private int nbrGround = 4;

    [Header("Vitesses par niveau")]
    [SerializeField] private float forwardSpeedLevel0 = 0.8f;
    [SerializeField] private float forwardSpeedLevel1 = 1f;
    [SerializeField] private float forwardSpeedLevel2 = 1.2f;

    #endregion

    #region Private state

    private GameObject player;
    private float groundSize;
    private int pendingEnigmeSequence = 0;

    #endregion

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
        AppliquerVitesseJoueur();

        nextEnigmeAt = Random.Range(enigmeIntervalMin, enigmeIntervalMax + 1);


        GroundsOnStage = new GameObject[nbrGround];
        InitGrounds();
    }

    private void Update()
    {
        GroundLevel(); 
    }

    #region Ground management

    private void InitGrounds()
    {
        for (int i = 0; i < nbrGround; i++)
        {
            int levelForThisGround = (i < nbGroundsSafeAuDebut) ? 0 : currentLevel;
            GameObject prefab = GetGroundPrefab(levelForThisGround);
            GroundsOnStage[i] = Instantiate(prefab);
        }

        groundSize = GroundsOnStage[0].transform.Find("Road").localScale.z;

        float posZ = player.transform.position.z + groundSize / 2f - 1.5f;
        foreach (var ground in GroundsOnStage)
        {
            ground.transform.position = new Vector3(0f, 0.2f, posZ);
            posZ += groundSize;
        }
    }

    private void GroundLevel()
    {
        PlayerControler controller = player.GetComponent<PlayerControler>();

        for (int i = GroundsOnStage.Length - 1; i >= 0; i--)
        {
            GameObject ground = GroundsOnStage[i];
            if (ground == null || ground.transform.position.z + groundSize / 2f < player.transform.position.z - 6f)
            {
                if (ground != null) Destroy(ground);

                //  Augmente le compteur de sol créés
                groundCounterSinceLastEnigme++;

                //  Déclenche séquence d’énigme si atteint
                if (groundCounterSinceLastEnigme >= nextEnigmeAt)
                {
                    TriggerNextEnigmeSequence();
                    groundCounterSinceLastEnigme = 0;
                    nextEnigmeAt = Random.Range(enigmeIntervalMin, enigmeIntervalMax + 1);
                    Debug.Log($"[LevelManager] Séquence énigme déclenchée ! Prochaine dans {nextEnigmeAt} sols.");
                }

                // 💡 Génère prefab selon logique
                GameObject prefab = GetGroundPrefab(currentLevel);
                GameObject newGround = Instantiate(prefab);

                float z = ground != null ? ground.transform.position.z : player.transform.position.z;
                newGround.transform.position = new Vector3(0f, 0.2f, z + groundSize * nbrGround);
                GroundsOnStage[i] = newGround;
            }
        }
    }

    #endregion

    #region PUBLIC prefab helpers

    public GameObject GetGroundPrefab(int level)
    {
        if (pendingEnigmeSequence > 0)
        {
            int sequenceIndex = 5 - pendingEnigmeSequence;
            pendingEnigmeSequence--;
            Debug.Log($"[LevelManager] Séquence Enigme - StepIndex: {3 - pendingEnigmeSequence - 1} | pending={pendingEnigmeSequence}");

            GameObject result = sequenceIndex switch
            {
                0 or 1 => GroundsPrefabsSafe.FirstOrDefault(g => g != null && g.GetComponent<GroundData>()?.EnigmePosition == GroundData.EnigmeZonePosition.Before),
                2 => GroundsPrefabsSafe.FirstOrDefault(g => g != null && g.GetComponent<GroundData>()?.EnigmePosition == GroundData.EnigmeZonePosition.Center),
                3 or 4 => GroundsPrefabsSafe.FirstOrDefault(g => g != null && g.GetComponent<GroundData>()?.EnigmePosition == GroundData.EnigmeZonePosition.After),
                _ => null
            };

            if (result == null)
            {
                Debug.LogWarning($"[LevelManager] Aucun prefab trouvé pour EnigmePosition index={sequenceIndex}");
                return GroundsPrefabsSafe[0];
            }
            Debug.Log($"[LevelManager] Enigme prefab choisi: {result?.name ?? "NULL"}");

            return result;
        }

        return level switch
        {
            0 => GroundsPrefabsSafe[Random.Range(0, GroundsPrefabsSafe.Length)],
            1 => GroundsPrefabsLevel1[Random.Range(0, GroundsPrefabsLevel1.Length)],
            2 => GroundsPrefabsLevel2[Random.Range(0, GroundsPrefabsLevel2.Length)],
            _ => GroundsPrefabsSafe[0]
        };
    }

    public GameObject GetObstaclePrefab() => GetObstaclePrefab(currentLevel);
    public GameObject GetMoneyPrefab() => GetMoneyPrefab(currentLevel);

    public GameObject GetEnigmePrefab(int level)
    {
        return level switch
        {
            1 => EnigmePrefabsLevel1[Random.Range(0, EnigmePrefabsLevel1.Length)],
            2 => EnigmePrefabsLevel2[Random.Range(0, EnigmePrefabsLevel2.Length)],
            _ => null
        };
    }
    public GameObject GetMoneyPrefab(int level)
    {
        return level switch
        {
            1 => MoneyPrefabsLevel1[Random.Range(0, MoneyPrefabsLevel1.Length)],
            2 => MoneyPrefabsLevel2[Random.Range(0, MoneyPrefabsLevel2.Length)],
            _ => null
        };
    }

    public GameObject GetObstaclePrefab(int level)
    {
        return level switch
        {
            1 => ObstaclePrefabsLevel1[Random.Range(0, ObstaclePrefabsLevel1.Length)],
            2 => ObstaclePrefabsLevel2[Random.Range(0, ObstaclePrefabsLevel2.Length)],
            _ => null
        };
    }


    public void ApplyDynamicScale(GameObject obj)
    {
        PlayerControler controller = player.GetComponent<PlayerControler>();
        float laneWidth = controller.distanceMax / 3f;

        if (laneWidth <= 0f || float.IsNaN(laneWidth) || float.IsInfinity(laneWidth))
        {
            Debug.LogWarning($"[ApplyDynamicScale] laneWidth invalide: {laneWidth}");
            return;
        }

        Vector3 scale = obj.transform.localScale;
        scale.x = laneWidth;
        obj.transform.localScale = scale;

        // recale proprement l'objet au centre de sa lane
        Vector3 pos = obj.transform.position;
        pos.x = Mathf.Clamp(pos.x, controller.MIN_X, controller.MIN_X + controller.distanceMax);
        obj.transform.position = pos;
    }

    #endregion

    #region Utility

    private void AppliquerVitesseJoueur()
    {
        PlayerControler controller = player.GetComponent<PlayerControler>();
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
            _ => forwardSpeedLevel1
        };

        controller.forwardSpeed = speed;
    }
   
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
