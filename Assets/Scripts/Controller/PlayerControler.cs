using System.Collections;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerControler : MonoBehaviour
{
    #region Variable
    public static PlayerControler Instance;


    [Header("StartPlayerPosition")]
    [SerializeField] private float startZ = -70;

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

    [Header("Death")]
    private bool death;
    private int reviveNumber = 0;
    private int defaultPrice = 1000;
    private int currentPriceRevive;
    private bool blink;
    [SerializeField] TextMeshProUGUI ReviveText;
    [SerializeField] private GameObject DeathPanel;
    private bool isInvincible = false;
    private bool isRolling = false;

    private float originalSpeed;
    private Rigidbody rb;
    Animator animator;
    private GameObject SkinPlayer;
    private bool autoRevive = false;


    #endregion


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
        rb.constraints = RigidbodyConstraints.FreezePositionY;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        SkinPlayer = GameObject.Find("SkinPlayer");

        //SkinnedMeshRenderer 

    }

    private void Start()
    {
        originalSpeed = forwardSpeed;

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

        rb.position = new Vector3(0f, rb.position.y, startZ);
        Debug.Log($"[DEBUG] groundWidth: {groundWidth}, laneWidth: {laneWidth}");

        Destroy(tempGround);

        currentPriceRevive = defaultPrice;
        ReviveText.text = $"Revive : {currentPriceRevive}$";
    }


    private void Update()
    {
        if (death) return;

        float input = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(input) > 0.1f)
        {
            lastDirection = (int)Mathf.Sign(input);
            timeInput += Time.deltaTime;
            timeInput = Mathf.Min(timeInput, tempsAcceleration);
        }
        else
        {
            timeInput = 0f;
            lastDirection = 0;
        }

        currentSpeed = accelerate * timeInput;

        Vector3 currentPos = rb.position;

        // Mouvement en X (latéral) basé sur input
        Vector3 horizontalMove = transform.right * lastDirection * currentSpeed * Time.deltaTime;

        // Mouvement en Z (toujours en avant)
        Vector3 forwardMove = transform.forward * forwardSpeed * Time.deltaTime;

        // Position suivante
        float nextX = Mathf.Clamp(currentPos.x + horizontalMove.x, MIN_X, MAX_X);
        Vector3 nextPos = new Vector3(nextX, currentPos.y, currentPos.z + forwardMove.z);

        // Appliquer le mouvement
        rb.MovePosition(nextPos);

        // Forcer la hauteur constante du joueur (pour éviter tout drift en Y)
        Vector3 fixedYPosition = rb.position;
        fixedYPosition.y = 1.7f;
        rb.MovePosition(fixedYPosition);

        //Utilisation de l'objet
        if (Input.GetKeyDown(KeyCode.X))
        {
            Inventory.Instance.UseCurrentItem();
        }
    }


    #region collider
    private void OnCollisionEnter(Collision collision)
    {
        // Cherche un parent avec le tag "Obstacle"
        Transform t = collision.transform;
        while (t != null)
        {
            if (t.CompareTag("Obstacle"))
            {
                if (isInvincible || death) return;

                Inventory.Instance.DeathMoney();
                animator.SetBool("Death", true);
                death = true;
                if (Inventory.Instance.HasPassiveItem<SandwichItem>())
                {
                    ReviveText.text = "Sandwich";
                }
                else
                {
                    ReviveText.text = $"Revive : {currentPriceRevive}$";
                }

                DeathPanel.gameObject.SetActive(true);
                return;
            }


            t = t.parent;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ObstacleNonMortel"))
        {
            if (!isInvincible && !death && !isRolling)
            {
                StartCoroutine(PlayRollReaction());
                Inventory.Instance.MoneyInInventory -= 100;
                if (Inventory.Instance.MoneyInInventory < 0) Inventory.Instance.MoneyInInventory = 0;   
            }
        }

        if (other.CompareTag("PauseSlowZone"))
        {
            originalSpeed = forwardSpeed; // stocke la vitesse actuelle
            forwardSpeed *= 0.3f; // ralentit
            Debug.Log("[PlayerControler] Entrée dans PauseSlowZone : vitesse réduite.");
        }

        if (other.CompareTag("PauseExitZone"))
        {
            forwardSpeed = originalSpeed; // restaure la vitesse
            Debug.Log("[PlayerControler] Sortie de PauseExitZone : vitesse restaurée.");
        }
    }

    #endregion

    public void RestoreSpeedAfterPause()
    {
        forwardSpeed = originalSpeed;
        Debug.Log("[PlayerControler] Vitesse restaurée après achat.");
    }



    #region Coroutine
    private IEnumerator ReviveInvincibility(float duration)
    {
        isInvincible = true;

        // Désactive le collider pour permettre de traverser les obstacles
        CapsuleCollider playerCollider = GetComponent<CapsuleCollider>();
        playerCollider.enabled = false;

        float timer = 0f;

        while (timer < duration)
        {
            blink = !blink;
            SkinPlayer.SetActive(blink);

            yield return new WaitForSeconds(0.3f);
            timer += 0.3f;
        }

        // Réactive le collider
        playerCollider.enabled = true;
        SkinPlayer.SetActive(true);
        isInvincible = false;
    }
    private IEnumerator PlayRollReaction()
    {
        isRolling = true;

        animator.SetBool("Roll", true);

        AnimationClip rollClip = animator.runtimeAnimatorController.animationClips
            .FirstOrDefault(c => c.name == "Roll");

        float duration = rollClip != null ? rollClip.length : 1f;
        yield return new WaitForSeconds(duration);

        animator.SetBool("Roll", false);
        isRolling = false;
    }

    #endregion

    #region Death
    public void Restart()
    {
      
        SceneManager.LoadScene("Game");
    }

    public void ActivateAutoRevive()
    {
        autoRevive = true;
    }
    public void Revive()
    {
        if (Inventory.Instance.HasPassiveItem<SandwichItem>())
        {
            Inventory.Instance.ConsumePassiveItem<SandwichItem>();
            Debug.Log("[PlayerControler] Résurrection gratuite via Sandwich !");
            death = false;
            DeathPanel.SetActive(false);
            StartCoroutine(ReviveInvincibility(5f)); 
            return;
        }

        if (Inventory.Instance.MoneyInInventory < currentPriceRevive) { return; }

        Inventory.Instance.MoneyInInventory -= currentPriceRevive;
        death = false;
        DeathPanel.SetActive(false);

        reviveNumber++;
        currentPriceRevive = currentPriceRevive * (2 * reviveNumber);
        ReviveText.text = $"Revive : {currentPriceRevive}$";

        StartCoroutine(ReviveInvincibility(5f));
    }

    #endregion
}
