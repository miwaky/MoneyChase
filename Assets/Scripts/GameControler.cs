using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class LevelControler : MonoBehaviour
{

    public static LevelControler Instance;
    [SerializeField]
    private GameObject[] GroundsPrefabsSafe;
    [SerializeField]
    private GameObject[] GroundsPrefabsLevel1;
    [SerializeField]
    private GameObject[] GroundsOnStage;

    private bool safeRoad = true;
    private bool level1 = false;



    private GameObject Player;

    public float GroundSize;
    private int nbrGround = 4;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            
        }
        Instance = this;
        
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Player = GameObject.Find("Player");

        GroundsOnStage = new GameObject[nbrGround];

        for (int i = 0; i < nbrGround; i++)
        {
            int n = Random.Range(0, GroundsPrefabsSafe.Length);
            GroundsOnStage[i] = Instantiate(GroundsPrefabsSafe[n]);
        }

        GroundSize = GroundsOnStage[0].GetComponentInChildren<Transform>().Find("Road").localScale.z;

        float pos = Player.transform.position.z + GroundSize / 2 - 1.5f; 
        foreach (var ground in GroundsOnStage)
        {
            ground.transform.position = new Vector3(0,0.2f, pos);
            pos += GroundSize;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (safeRoad)
        {
            for (int i = GroundsOnStage.Length - 1; i >= 0; i--)
            {
                GameObject ground = GroundsOnStage[i];

                if (ground.transform.position.z + GroundSize / 2 < Player.transform.position.z - 6f)
                {
                    float z = ground.transform.position.z;
                    Destroy(ground);
                    int n = Random.Range(0, GroundsPrefabsSafe.Length);
                    ground = Instantiate(GroundsPrefabsSafe[n]);
                    ground.transform.position = new Vector3(0, 0.2f, z + GroundSize * nbrGround);
                    GroundsOnStage[i] = ground;
                }
            }
        }
        if (level1 && !safeRoad)
        {
            for (int i = GroundsOnStage.Length - 1; i >= 0; i--)
            {
                GameObject ground = GroundsOnStage[i];

                if (ground.transform.position.z + GroundSize / 2 < Player.transform.position.z - 6f)
                {
                    float z = ground.transform.position.z;
                    Destroy(ground);
                    int n = Random.Range(0, GroundsPrefabsLevel1.Length);
                    ground = Instantiate(GroundsPrefabsLevel1[n]);
                    ground.transform.position = new Vector3(0, 0.2f, z + GroundSize * nbrGround);
                    GroundsOnStage[i] = ground;
                }
            }
        }
      
        if (Input.GetKeyDown(KeyCode.Space))
        {
            level1 = true;
            safeRoad = false;
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
