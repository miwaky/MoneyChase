using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerControler : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float tempsAcceleration = 0.1f;
    [SerializeField] private float tempsMovementDistanceMinMax = 0.6f;
    [SerializeField] public float forwardSpeed = 1f;



    [Header("Ground Calcul")]
    [SerializeField] private GameObject groundPrefab;
    public float MIN_X { get; private set; }
    private float MAX_X;

    public float distanceMax { get; private set; }

    private float speedMax;
    private float accelerate;
    private float currentSpeed = 0f;
    private float timeInput = 0f;
    private int lastDirection = 0;

    private Rigidbody rb;

    private void Awake()
    {
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

        float groundWidth = groundCollider.bounds.size.x;
        float laneWidth = groundWidth / 3f;

        MIN_X = -groundWidth / 2f + laneWidth / 2f;
        MAX_X = groundWidth / 2f - laneWidth / 2f;
        distanceMax = MAX_X - MIN_X;

        speedMax = distanceMax / (tempsMovementDistanceMinMax - (tempsAcceleration / 2f));
        accelerate = speedMax / tempsAcceleration;

        rb.position = new Vector3(0f, rb.position.y, 0f);

        Destroy(tempGround);
    }


    private void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");

        if (input != 0f)
        {
            lastDirection = (int)Mathf.Sign(input);
            timeInput += Time.fixedDeltaTime;
            timeInput = Mathf.Min(timeInput, tempsAcceleration);
        }
        else
        {
            timeInput -= Time.fixedDeltaTime;
            timeInput = Mathf.Max(timeInput, 0f);
        }

        currentSpeed = accelerate * timeInput;

        Vector3 currentPos = rb.position;
        Vector3 horizontalMove = transform.right * lastDirection * currentSpeed * Time.fixedDeltaTime;
        Vector3 forwardMove = transform.forward * forwardSpeed * Time.fixedDeltaTime;

        float nextX = Mathf.Clamp(currentPos.x + horizontalMove.x, MIN_X, MAX_X);
        Vector3 nextPos = new Vector3(nextX, currentPos.y, currentPos.z + forwardMove.z);

        rb.MovePosition(nextPos);
    }


}
