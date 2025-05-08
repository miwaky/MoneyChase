using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [SerializeField] private float MIN_X = -27f;
    [SerializeField] private float MAX_X = 27f;

    [SerializeField] private float tempsAcceleration = 0.1f;
    [SerializeField] private float tempsMovementDistanceMinMax = 0.6f;

    [SerializeField] private float forwardSpeed = 1f;

    private float distanceMax;
    private float speedMax;
    private float accelerate;
    private float currentSpeed = 0f;
    private float timeInput = 0f;
    private int lastDirection = 0;
    void Start()
    {
        transform.position = new Vector3(0f, 0.5f, 0f);

      
        distanceMax = MAX_X - MIN_X;
        speedMax = distanceMax / (tempsMovementDistanceMinMax - (tempsAcceleration / 2f));
        accelerate = speedMax / tempsAcceleration;
    }

    void Update()
    {
        float input = Input.GetAxisRaw("Horizontal"); 

        if (input != 0f)
        {
            lastDirection = (int)Mathf.Sign(input);
            timeInput += Time.deltaTime;
            timeInput = Mathf.Min(timeInput, tempsAcceleration);
        }
        else
        {
            timeInput -= Time.deltaTime;
            timeInput = Mathf.Max(timeInput, 0f);
        }

        // Calcul de la vitesse courante
        currentSpeed = accelerate * timeInput;

        // Utilise lastDirection ici
        Vector3 horizontalMove = transform.right * lastDirection * currentSpeed * Time.deltaTime;
        transform.position += horizontalMove;


        // Clamp entre MIN_X et MAX_X
        float clampedX = Mathf.Clamp(transform.position.x, MIN_X, MAX_X);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);

        // Mouvement avant constant
        transform.position += transform.forward * forwardSpeed * Time.deltaTime;

    }

    private void OnTriggerEnter(Collider other)
    {
        {
            Debug.Log("touché");       
        }
    }
}
