using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]

public class PlayerControler : MonoBehaviour
{
    #region Variable
    // Singleton pour accéder facilement au joueur depuis d'autres scripts
    public static PlayerControler Instance;

    // ----- CONFIGURATION INITIALE -----

    // Position de départ sur l’axe Z quand la partie commence
    [Header("Start")]
    [SerializeField] private float startZ = -70;

    // Données liées au déplacement
    [Header("Movement")]
    [SerializeField] private float tempsAcceleration = 0.1f; // Temps pour atteindre la vitesse latérale max
    [SerializeField] private float tempsMovementDistanceMinMax = 0.6f; // Temps pour aller d’un bord à l’autre
    [SerializeField] public float forwardSpeed = 1f; // Vitesse d’avancement constante

    // Dimensions du sol de jeu, utiles pour définir les limites
    [Header("Ground")]
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] public float groundWidth = 25f;
    [SerializeField] public float groundDepth = 30f;
    public float MIN_X { get; private set; } // Limite gauche du terrain
    private float MAX_X; // Limite droite du terrain
    public float distanceMax { get; private set; } // Largeur jouable

    // Calculs internes pour les vitesses de déplacement latéral
    private float speedMax;
    private float accelerate;
    private float currentSpeed = 30f;
    private float timeInput = 0f; // Depuis combien de temps on tient la direction
    private int lastDirection = 0; // Dernière direction (gauche -1 / droite +1)

    // ----- GESTION DE LA MORT ET DU REVIVE -----

    private bool death;
    private int reviveNumber = 0; // Combien de fois on s’est relevé
    private int defaultPrice = 1000; // Coût de la première résurrection
    private int currentPriceRevive;
    private bool blink;
    [SerializeField] private int[] penaltyPerLevel = new int[5] { 100, 200, 400, 500, 700 };

    [SerializeField] TextMeshProUGUI ReviveText; // Texte à afficher dans le menu de mort
    [SerializeField] private GameObject DeathPanel;
    private bool isInvincible = false;
    private bool isRolling = false;

    // ----- AUTRES VARIABLES -----
   
    private bool hasUsedDistributeur = false;
    private float originalSpeed; // Pour restaurer la vitesse après un ralentissement
    private Rigidbody rb;
    Animator animator;
    private GameObject SkinPlayer;
    private bool canMove = true;

    [SerializeField] private Company companies; // Données des "companies" (profils)
    private CompanyScore company;

    // Correction du drift (mouvement pas net sur X)
    private float logicalX;


    #endregion


    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Références des composants
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        Debug.Log("[DEBUG] isKinematic avant : " + rb.isKinematic);
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;

        SkinPlayer = GameObject.Find("SkinPlayer");

        // Récupère la "company" active (le joueur en cours)
        company = companies.GetActiveCompany();
    }
    private void Start()
    {
        originalSpeed = forwardSpeed;

        rb.isKinematic = true; // Empêche toute influence physique 

        // On instancie temporairement un sol pour récupérer sa taille réelle (collider)
        GameObject tempGround = Instantiate(groundPrefab);
        Collider groundCollider = tempGround.GetComponentInChildren<Collider>();

        if (groundCollider == null)
        {
            Debug.LogError("Le sol n’a pas de collider, on peut pas jouer.");
            Destroy(tempGround);
            return;
        }

        // Calcule les bords gauche et droite
        float laneWidth = groundWidth / 3f;
        MIN_X = -groundWidth / 2f + laneWidth / 2f;
        MAX_X = groundWidth / 2f - laneWidth / 2f;
        distanceMax = MAX_X - MIN_X;

        // Détermine la vitesse max et l'accélération latérale
        speedMax = distanceMax / (tempsMovementDistanceMinMax - (tempsAcceleration / 2f));
        accelerate = speedMax / tempsAcceleration;

        // Position de départ du joueur
        rb.position = new Vector3(0f, rb.position.y, startZ);
        logicalX = 0f; // Initialisation logique X

        Destroy(tempGround);

        currentPriceRevive = defaultPrice;
        ReviveText.text = $"Revive : {currentPriceRevive}$";
    }

    private void Update()
    {
        if (!canMove) return;

        // Gestion du déplacement horizontal
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

        // Utilisation des objets (item actif sur espace)
        if (Input.GetKeyDown(KeyCode.Space))
            Inventory.Instance.UseCurrentItem();
    }

    private void FixedUpdate()
    {
        if (death || !canMove) return;

        currentSpeed = accelerate * timeInput;
        logicalX += lastDirection * currentSpeed * Time.fixedDeltaTime;
        logicalX = Mathf.Clamp(logicalX, MIN_X, MAX_X);

        Vector3 newPos = transform.position;
        newPos.x = logicalX;
        newPos.z += forwardSpeed * Time.fixedDeltaTime;
        transform.position = newPos;
    }

    #region collider
    private void OnTriggerEnter(Collider other)
    {
        // Détection d’obstacles mortels
        Transform t = other.transform;
        while (t != null)
        {
            if (t.CompareTag("Obstacle"))
            {
                if (isInvincible || death) return;

                animator.SetBool("Death", true);
                death = true;
                canMove = false;

                ReviveText.text = Inventory.Instance.HasPassiveItem<SandwichItem>() ? "Sandwich" : $"Revive : {currentPriceRevive}$";
                DeathPanel.SetActive(true);
                return;
            }

            t = t.parent;
        }

        if (other.CompareTag("ObstacleNonMortel"))
        {
            if (!isInvincible && !death && !isRolling)
            {
                StartCoroutine(PlayRollReaction());

                int level = Mathf.Clamp(LevelManager.Instance.currentLevel, 1, penaltyPerLevel.Length);
                int penalty = penaltyPerLevel[level - 1];

                Inventory.Instance.MoneyInInventory -= penalty;
                if (Inventory.Instance.MoneyInInventory < 0)
                    Inventory.Instance.MoneyInInventory = 0;
            }
        }
        // Zones de ralentissement ou de sortie de pause
        if (other.CompareTag("PauseSlowZone"))
        {
            // Toujours appliquer le ralentissement, même si invincible
            if (other.CompareTag("PauseSlowZone"))
            {
                originalSpeed = LevelManager.Instance.GetSpeedForLevel(LevelManager.Instance.currentLevel);
                forwardSpeed = originalSpeed * 0.3f;
                Debug.Log($"[SLOW ZONE] Vitesse ralentie à {forwardSpeed}");
            }
        }

        if (other.CompareTag("PauseExitZone"))
        {
            forwardSpeed = originalSpeed;
            hasUsedDistributeur = false;

        }
    }
    #endregion

    #region salle de pause
    public bool CanUseDistributeur()
    {
        return !hasUsedDistributeur;
    }
    public void MarkDistributeurUsed()
    {
        hasUsedDistributeur = true;
    }

    #endregion
    #region Speed
    public void RestoreSpeedAfterPause()
    {
        forwardSpeed = originalSpeed;
        Debug.Log("[PlayerControler] Vitesse restaurée après achat.");
    }

    #endregion


    #region Coroutine
    // COROUTINES : gestion invincibilité et roulade

    private IEnumerator ReviveInvincibility(float duration)
    {
        isInvincible = true;
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

        playerCollider.enabled = true;
        isInvincible = false;
                SkinPlayer.SetActive(true);

    }

    private IEnumerator PlayRollReaction()
    {
        isRolling = true;
        animator.SetBool("Roll", true);

        float duration = 0.8f; // On force le temps au cas où l'anim n’est pas trouvée
        yield return new WaitForSeconds(duration);

        animator.SetBool("Roll", false);
        isRolling = false;
    }

    // ------------------------------------------------------


    #endregion

    #region Death

    // SAUVEGARDE ET GESTION DE LA MORT

    public void SaveCurrentScore()
    {
        
        int earned = Inventory.Instance.MoneyInInventory;
        company.AddScore(earned);
        companies.SaveToFile();


    }

    public void Restart()
    {
        SaveCurrentScore();
        SceneManager.LoadScene("Game");
    }

    public void ReturnMenu()
    {
        SaveCurrentScore();
        SceneManager.LoadScene("MainMenu");
    }

    public void Revive()
    {
        if (Inventory.Instance.HasPassiveItem<SandwichItem>())
        {
            Inventory.Instance.ConsumePassiveItem<SandwichItem>();
            death = false;
            canMove = true;
            DeathPanel.SetActive(false);
            StartCoroutine(ReviveInvincibility(5f));
            return;
        }

        if (Inventory.Instance.MoneyInInventory < currentPriceRevive) return;

        Inventory.Instance.MoneyInInventory -= currentPriceRevive;
        death = false;
        canMove = true;
        DeathPanel.SetActive(false);

        reviveNumber++;
        currentPriceRevive += 2000;
        ReviveText.text = $"Revive : {currentPriceRevive}$";

        StartCoroutine(ReviveInvincibility(5f));
    }

    #endregion
}