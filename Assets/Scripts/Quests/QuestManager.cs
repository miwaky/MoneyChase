using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum QuestCategory
{
    Money,
    Timer,
    Item,
    Enigme 
}


[System.Serializable]
public class Quest
{
    public string QuestName;
    public QuestCategory Type;
    public int Reward;
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [SerializeField] private List<Quest> quests = new List<Quest>();

    [Header("Quest System")]
    [SerializeField] private int NumberOfQuest = 2;
    [SerializeField] private bool[] QuestActivated;
    [SerializeField] private QuestCategory[] QuestType;
    [SerializeField] private int[] idQuest;

    [Header("Quest Money System")]
    [SerializeField] private int[] objectiveMoney;
    [SerializeField] private int[] questAmount;

    [Header("Quest Timer System")]
    [SerializeField] private int[] objectiveTimer;
    [SerializeField] private float[] timeLeft;
    private Coroutine[] timerCoroutines;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
        timerCoroutines = new Coroutine[NumberOfQuest];
        QuestActivated = new bool[NumberOfQuest];
        QuestType = new QuestCategory[NumberOfQuest];
        objectiveMoney = new int[NumberOfQuest];
        objectiveTimer = new int[NumberOfQuest];
        questAmount = new int[NumberOfQuest];
        idQuest = new int[NumberOfQuest];
        timeLeft = new float[NumberOfQuest];

        SpawnQuest();
    }

    public void SpawnQuest()
    {
        for (int i = 0; i < NumberOfQuest; i++)
        {
            if (!QuestActivated[i])
            {
                AssignQuest(i);
            }
        }
    }

    private void AssignQuest(int slotIndex)
    {
        int questModelId = Random.Range(0, quests.Count);
        idQuest[slotIndex] = questModelId;

        Quest model = quests[questModelId];
        QuestType[slotIndex] = model.Type;

        // ✅ Active la quête AVANT d'appeler Setup
        QuestActivated[slotIndex] = true;

        switch (model.Type)
        {
            case QuestCategory.Money:
                SetupMoneyQuest(slotIndex, model);
                break;

            case QuestCategory.Timer:
                SetupTimerQuest(slotIndex, model);
                break;

            case QuestCategory.Enigme:
                SetupEnigmeQuest(slotIndex, model);
                break;

                //case QuestCategory.Item:
                //    SetupItemQuest(slotIndex, model);
                //    break;
        }
    }

    private void SetupMoneyQuest(int slotIndex, Quest model)
    {
        objectiveMoney[slotIndex] = Random.Range(500, 1000);
        questAmount[slotIndex] = 0;

        Debug.Log($"[QuestManager] New MoneyQuest in slot {slotIndex}: Need {objectiveMoney[slotIndex]}");
    }

    private void SetupTimerQuest(int slotIndex, Quest model)
    {
        objectiveTimer[slotIndex] = Random.Range(5, 11); // entre 5 et 10 secondes
        timeLeft[slotIndex] = objectiveTimer[slotIndex];

        Debug.Log($"[QuestManager] Setup TimerQuest for slot {slotIndex}, time: {objectiveTimer[slotIndex]}");

        if (timerCoroutines[slotIndex] != null)
        {
            StopCoroutine(timerCoroutines[slotIndex]);
        }

        timerCoroutines[slotIndex] = StartCoroutine(UpdateTimer(slotIndex));
    }
    private void SetupEnigmeQuest(int slotIndex, Quest model)
    {
        Debug.Log($"[QuestManager] Enigme quest setup for slot {slotIndex}");
    }
    private void SetupItemQuest(int slotIndex, Quest model)
    {
        Debug.Log($"[QuestManager] Item quest setup not implemented for slot {slotIndex}");
    }

    public void AddMoneyToQuests(int amount)
    {
        for (int i = 0; i < NumberOfQuest; i++)
        {
            if (QuestActivated[i] && QuestType[i] == QuestCategory.Money)
            {
                questAmount[i] += amount;
                Debug.Log($"[QuestManager] Money added to quest {i}: {questAmount[i]}/{objectiveMoney[i]}");

                if (questAmount[i] >= objectiveMoney[i])
                {
                    CompleteQuest(i);
                }
            }
        }
    }

    private IEnumerator UpdateTimer(int slotIndex)
    {
        float remaining = objectiveTimer[slotIndex];
        timeLeft[slotIndex] = remaining;

        Debug.Log($"[QuestManager] Timer started for slot {slotIndex} ({remaining}s)");

        while (remaining > 0 && QuestActivated[slotIndex] && QuestType[slotIndex] == QuestCategory.Timer)
        {
            yield return new WaitForSeconds(1f);
            remaining--;
            timeLeft[slotIndex] = remaining;

            Debug.Log($"[QuestManager] Timer for quest {slotIndex}: {remaining}s remaining");
        }

        timerCoroutines[slotIndex] = null;

        if (QuestActivated[slotIndex] && QuestType[slotIndex] == QuestCategory.Timer)
        {
            Debug.Log($"[QuestManager] Timer expired, completing quest {slotIndex}");
            CompleteQuest(slotIndex);
        }
    }
    public void ValidateEnigme()
    {
        for (int i = 0; i < NumberOfQuest; i++)
        {
            if (QuestActivated[i] && QuestType[i] == QuestCategory.Enigme)
            {
                CompleteQuest(i);
                break;
            }
        }
    }
    private void CompleteQuest(int i)
    {
        if (timerCoroutines[i] != null)
        {
            StopCoroutine(timerCoroutines[i]);
            timerCoroutines[i] = null;
        }

        int modelIndex = idQuest[i];

        if (modelIndex >= 0 && modelIndex < quests.Count && quests[modelIndex] != null)
        {
            Inventory.Instance.MoneyInInventory += quests[modelIndex].Reward;
            Debug.Log($"[QuestManager] Quest completed: {quests[modelIndex].QuestName} | Reward: {quests[modelIndex].Reward}");
        }
        else
        {
            Debug.LogWarning($"[QuestManager] Invalid quest model index: {modelIndex}");
        }

        // Reset quest slot
        questAmount[i] = 0;
        timeLeft[i] = 0;
        objectiveTimer[i] = 0;
        QuestActivated[i] = false;
    }
}
