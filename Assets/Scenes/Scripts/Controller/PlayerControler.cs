using System.Drawing;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerControler : MonoBehaviour
{
    public static PlayerControler Instance;


   [Header("Movement")]
    [SerializeField] private float tempsAcceleration = 0.1f;
    [SerializeField] private float tempsMovementDistanceMinMax = 0.6f;
    [SerializeField] public float forwardSpeed = 1f;



    [Header("Ground Calcul")]
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] public float groundWidth = 25f;
    [SerializeField] public float groundDepth = 30f;
    public float MIN_X { get; private set; }
    private float MAX_X;
 
    public float distanceMax { get; private set; }

    private float speedMax;
    private float accelerate;
    private float currentSpeed = 30f;
    private float timeInput = 0f;
    private int lastDirection = 0;

    private Rigidbody rb;
    Animator animator;
    private bool death;

    [Header("Retry button UI")]
    [SerializeField] private Button ButtonRetry;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Start()
    {
        
        GameObject tempGround = Instantiate(groundPrefab);
        Collider groundCollider = tempGround.GetComponentInChildren<Collider>();

        if (groundCollider == null)
        {
            Debug.LogError("Le groundPrefab instancié n'a pas de collider !");
            Destroy(tempGround);
            return;
        }


        float laneWidth = groundWidth / 3f;

        MIN_X = -groundWidth / 2f + laneWidth / 2f;
        MAX_X = groundWidth / 2f - laneWidth / 2f;
        distanceMax = MAX_X - MIN_X;

        speedMax = distanceMax / (tempsMovementDistanceMinMax - (tempsAcceleration / 2f));
        accelerate = speedMax / tempsAcceleration;

        rb.position = new Vector3(0f, rb.position.y, 0f);
        Debug.Log($"[DEBUG] groundWidth: {groundWidth}, laneWidth: {laneWidth}");

        Destroy(tempGround);
    }


    private void Update()
    {
        if (death) return;

        float input = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(input) > 0.01f)
        {
            lastDirection = (int)Mathf.Sign(input);
            timeInput += Time.deltaTime;
            timeInput = Mathf.Min(timeInput, tempsAcceleration);
        }
        else
        {
            timeInput -= Time.deltaTime * 2f; 
            timeInput = Mathf.Max(timeInput, 0f);
        }

        currentSpeed = accelerate * timeInput;

        Vector3 currentPos = rb.position;
        Vector3 horizontalMove = transform.right * lastDirection * currentSpeed * Time.deltaTime;
        Vector3 forwardMove = transform.forward * forwardSpeed * Time.deltaTime;

        float nextX = Mathf.Clamp(currentPos.x + horizontalMove.x, MIN_X, MAX_X);
        Vector3 nextPos = new Vector3(nextX, currentPos.y, currentPos.z + forwardMove.z);

        rb.MovePosition(nextPos);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Inventory.Instance.DeathMoney();
            animator.SetBool("Death", true);
            death = true;
            ButtonRetry.gameObject.SetActive(true);
            

        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("Game");
    }
}
