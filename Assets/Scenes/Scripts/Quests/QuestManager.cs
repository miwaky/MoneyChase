using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum QuestCategory
{
    Money,
    Timer
}

[System.Serializable]
public class Quest
{
    public string QuestName;
    public QuestCategory Type;
    public int Reward;
}

[System.Serializable]
public class ActiveQuest
{
    public Quest Model;
    public bool IsActive;

    public int ProgressMoney;
    public float TimeLeft;

    public void ResetProgress()
    {
        ProgressMoney = 0;
        TimeLeft = 0;
        IsActive = false;
    }
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Quest Settings")]
    [SerializeField] private List<Quest> questTemplates = new List<Quest>();
    [SerializeField] private int maxActiveQuests = 3;
    [SerializeField] private List<ActiveQuest> activeQuests = new List<ActiveQuest>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < maxActiveQuests; i++)
        {
            activeQuests.Add(null);
        }
    }

    public bool TryAddQuest()
    {
        int slot = GetFirstFreeSlot();
        if (slot == -1) return false;

        SpawnSingleQuest(slot);
        return true;
    }

    public void SpawnSingleQuest(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= maxActiveQuests) return;
        if (activeQuests[slotIndex] != null && activeQuests[slotIndex].IsActive) return;
        if (questTemplates.Count == 0)
        {
            Debug.LogWarning("[QuestManager] Aucun modèle de quête défini !");
            return;
        }

        int questModelId = Random.Range(0, questTemplates.Count);
        Quest model = questTemplates[questModelId];

        ActiveQuest newQuest = new ActiveQuest
        {
            Model = model,
            IsActive = true
        };

        switch (model.Type)
        {
            case QuestCategory.Money:
                newQuest.ProgressMoney = 0;
                break;
            case QuestCategory.Timer:
                newQuest.TimeLeft = Random.Range(5f, 10f);
                StartCoroutine(UpdateTimer(slotIndex, newQuest.TimeLeft));
                break;
        }

        activeQuests[slotIndex] = newQuest;
        Debug.Log($"[QuestManager] Nouvelle quête : {model.QuestName} (slot {slotIndex})");
    }

    public void AddMoneyToQuests(int amount)
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            ActiveQuest quest = activeQuests[i];
            if (quest == null || !quest.IsActive) continue;
            if (quest.Model.Type != QuestCategory.Money) continue;

            quest.ProgressMoney += amount;
            Debug.Log($"[QuestManager] Progression : {quest.ProgressMoney}/{quest.Model.Reward}");

            if (quest.ProgressMoney >= quest.Model.Reward)
            {
                CompleteQuest(i);
            }
        }
    }

    private IEnumerator UpdateTimer(int slotIndex, float duration)
    {
        float remaining = duration;

        while (remaining > 0f)
        {
            yield return new WaitForSeconds(1f);
            remaining--;

            ActiveQuest quest = activeQuests[slotIndex];
            if (quest == null || !quest.IsActive || quest.Model.Type != QuestCategory.Timer) yield break;

            quest.TimeLeft = remaining;
        }

        CompleteQuest(slotIndex);
    }

    public void CompleteQuest(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= activeQuests.Count) return;

        ActiveQuest quest = activeQuests[slotIndex];
        if (quest == null || !quest.IsActive) return;

        Inventory.Instance.MoneyInInventory += quest.Model.Reward;
        Debug.Log($"[QuestManager] Quête terminée : {quest.Model.QuestName} | Récompense : {quest.Model.Reward}");

        quest.ResetProgress();
        activeQuests[slotIndex] = null;
    }

    public bool HasFreeSlot()
    {
        return activeQuests.Exists(q => q == null || !q.IsActive);
    }

    public int GetFirstFreeSlot()
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            if (activeQuests[i] == null || !activeQuests[i].IsActive)
                return i;
        }
        return -1;
    }

    public List<ActiveQuest> GetActiveQuests()
    {
        return activeQuests.FindAll(q => q != null && q.IsActive);
    }
}
