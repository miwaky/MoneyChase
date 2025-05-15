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

        GroundsOnStage = new GameObject[nbrGround];
        InitGrounds();
    }

    private void Update()
    {
        GroundLevel(); // uniquement recycler le sol
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

                GameObject prefab = GetGroundPrefab(currentLevel);
                GameObject newGround = Instantiate(prefab);

                float z = ground != null ? ground.transform.position.z : player.transform.position.z;
                newGround.transform.position = new Vector3(0f, 0.2f, z + groundSize * nbrGround);
                GroundsOnStage[i] = newGround;

                //controller.forwardSpeed = controller.baseSpeed * (currentLevel == 0 ? speedSafe : speedLevel1);
            }
        }
    }

    #endregion

    #region Obstacle recycling (safe level only)

    //private void ObstacleLevel()
    //{
    //    if (currentLevel != 0) return; // Sécurité critique

    //    float randomDistance = Random.Range(distanceObstacleMin, distanceObstacleMax);

    //    for (int i = ObstacleOnStage.Length - 1; i >= 0; i--)
    //    {
    //        GameObject obstacle = ObstacleOnStage[i];
    //        if (obstacle == null || obstacle.transform.position.z + 5f < player.transform.position.z)
    //        {
    //            if (obstacle != null) Destroy(obstacle);

    //            GameObject newObstacle = Instantiate(GetObstaclePrefab(0));
    //            if (newObstacle == null) continue;

    //            SpawnXLocation();
    //            ApplyDynamicScale(newObstacle);

    //            float newZ = Mathf.Max(lastObstacleZ + randomDistance, player.transform.position.z + 10f);
    //            newObstacle.transform.position = new Vector3(nextLocationSpawn, 1f, newZ);
    //            lastObstacleZ = newZ;

    //        }
    //    }
    //}


    //#endregion

    //#region Money recycling (safe level only)

    //private void MoneyLevel()
    //{
    //    if (currentLevel != 0) return;

    //    float randomDistance = Random.Range(distanceMoneyMin, distanceMoneyMax);

    //    for (int i = MoneyOnStage.Length - 1; i >= 0; i--)
    //    {
    //        GameObject money = MoneyOnStage[i];
    //        if (money == null || money.transform.position.z + 5f < player.transform.position.z)
    //        {
    //            if (money != null) Destroy(money);

    //            GameObject newMoney = Instantiate(GetMoneyPrefab(0));
    //            if (newMoney == null) continue;

    //            SpawnXLocation();
    //            ApplyDynamicScale(newMoney);

    //            float newZ = Mathf.Max(lastMoneyZ + randomDistance, player.transform.position.z + 10f);
    //            newMoney.transform.position = new Vector3(nextLocationSpawn, 2f, newZ);
    //            lastMoneyZ = newZ;

    //        }
    //    }
    //}



    #endregion

    #region PUBLIC prefab helpers

    public GameObject GetGroundPrefab(int level)
    {
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

    //private void SpawnXLocation()
    //{
    //    PlayerControler controller = player.GetComponent<PlayerControler>();
    //    float laneWidth = controller.distanceMax / 3f;
    //    int randomColumn = Random.Range(0, 3);
    //    nextLocationSpawn = controller.MIN_X + laneWidth * (randomColumn + 0.5f);
    //}
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

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    #endregion
}
