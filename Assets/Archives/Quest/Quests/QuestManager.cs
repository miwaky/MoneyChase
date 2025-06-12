//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public enum QuestCategory
//{
//    Money,
//    Timer
//}

//[System.Serializable]
//public class Quest
//{
//    public string QuestName;
//    public QuestCategory Type;
//    public int Reward;
//}

//[System.Serializable]
//public class ActiveQuest
//{
//    public Quest Model;
//    public bool IsActive;

//    public int ProgressMoney;
//    public float TimeLeft;

    
//    public int TargetMoney;
//    public float TargetTime;

//    public void ResetProgress()
//    {
//        ProgressMoney = 0;
//        TimeLeft = 0;
//        TargetMoney = 0;
//        TargetTime = 0;
//        IsActive = false;
//    }
//}


//public class QuestManager : MonoBehaviour
//{
//    public static QuestManager Instance;

//    [Header("Quest Money parameter")]
//    [SerializeField] private int level1MoneyNeededMin = 200;
//    [SerializeField] private int level1MoneyNeededMax = 500;

//    [SerializeField] private int level2MoneyNeededMin = 400;
//    [SerializeField] private int level2MoneyNeededMax = 700;

//    [SerializeField] private int level3MoneyNeededMin = 500;
//    [SerializeField] private int level3MoneyNeededMax = 1000;

//    [Header("Quest Timer parameter")]
//    [SerializeField] private int level1TimerNeededMin = 15;
//    [SerializeField] private int level1TimerNeededMax = 30;

//    [SerializeField] private int level2TimerNeededMin = 30;
//    [SerializeField] private int level2TimerNeededMax = 60;

//    [SerializeField] private int level3TimerNeededMin = 60;
//    [SerializeField] private int level3TimerNeededMax = 120;

//    [Header("Quest Settings")]
//    [SerializeField] private int maxActiveQuests = 3;
//    [SerializeField] private List<ActiveQuest> activeQuests = new List<ActiveQuest>();

//    private int currentLevel;
//    private void Awake()
//    {
//        if (Instance != null)
//        {
//            Destroy(gameObject);
//            return;
//        }
//        Instance = this;
//    }

//    private void Start()
//    {
//        currentLevel = LevelManager.Instance.currentLevel;
//        for (int i = 0; i < maxActiveQuests; i++)
//        {
//            activeQuests.Add(null);
//        }
//    }
//    private void Update()
//    {
//        currentLevel = LevelManager.Instance.currentLevel;
//    }
//    public bool TryAddQuest()
//    {
//        int slot = GetFirstFreeSlot();
//        if (slot == -1) return false;

//        SpawnSingleQuest(slot);
//        return true;
//    }

//    public void SpawnSingleQuest(int slotIndex)
//    {
//        if (slotIndex < 0 || slotIndex >= maxActiveQuests) return;
//        if (activeQuests[slotIndex] != null && activeQuests[slotIndex].IsActive) return;

//        QuestCategory type = (QuestCategory)Random.Range(0, System.Enum.GetValues(typeof(QuestCategory)).Length);
//        Quest model = new Quest();
//        ActiveQuest newQuest = new ActiveQuest();

//        switch (type)
//        {
//            case QuestCategory.Money:
//                int moneyTarget = Random.Range(GetMinMoneyByLevel(), GetMaxMoneyByLevel() + 1);
//                int moneyReward = Mathf.RoundToInt(moneyTarget * 0.25f + currentLevel * 25);

//                model.QuestName = $"Collect {moneyTarget} coins";
//                model.Type = QuestCategory.Money;
//                model.Reward = moneyReward;

//                newQuest.Model = model;
//                newQuest.IsActive = true;
//                newQuest.ProgressMoney = 0;
//                newQuest.TargetMoney = moneyTarget;
//                break;

//            case QuestCategory.Timer:
//                int durationSec = Random.Range(GetMinTimeByLevel(), GetMaxTimeByLevel() + 1);
//                int timeReward = Mathf.RoundToInt(durationSec * 0.5f + currentLevel * 15);

//                model.QuestName = $"Survive {durationSec} seconds";
//                model.Type = QuestCategory.Timer;
//                model.Reward = timeReward;

//                newQuest.Model = model;
//                newQuest.IsActive = true;
//                newQuest.TimeLeft = durationSec;
//                newQuest.TargetTime = durationSec; // ✅ objectif de temps

//                StartCoroutine(UpdateTimer(slotIndex, durationSec));
//                break;
//        }

//        activeQuests[slotIndex] = newQuest;
//        Debug.Log($"[QuestManager] Nouvelle quête : {model.QuestName} (slot {slotIndex})");
//    }



//    public void AddMoneyToQuests(int amount)
//    {
//        for (int i = 0; i < activeQuests.Count; i++)
//        {
//            ActiveQuest quest = activeQuests[i];
//            if (quest == null || !quest.IsActive) continue;
//            if (quest.Model.Type != QuestCategory.Money) continue;

//            quest.ProgressMoney += amount;
//            Debug.Log($"[QuestManager] Progression : {quest.ProgressMoney}/{quest.Model.Reward}");

//            if (quest.ProgressMoney >= quest.TargetMoney)
//            {
//                CompleteQuest(i);
//            }
//        }
//    }

//    private IEnumerator UpdateTimer(int slotIndex, float duration)
//    {
//        float remaining = duration;

//        while (remaining > 0f)
//        {
//            yield return new WaitForSeconds(1f);
//            remaining--;

//            ActiveQuest quest = activeQuests[slotIndex];
//            if (quest == null || !quest.IsActive || quest.Model.Type != QuestCategory.Timer) yield break;

//            quest.TimeLeft = remaining;
//        }

//        CompleteQuest(slotIndex);
//    }

//    public void CompleteQuest(int slotIndex)
//    {
//        if (slotIndex < 0 || slotIndex >= activeQuests.Count) return;

//        ActiveQuest quest = activeQuests[slotIndex];
//        if (quest == null || !quest.IsActive) return;

//        Inventory.Instance.MoneyInInventory += quest.Model.Reward;
//        Debug.Log($"[QuestManager] Quête terminée : {quest.Model.QuestName} | Récompense : {quest.Model.Reward}");

//        quest.ResetProgress();
//        activeQuests[slotIndex] = null;
//    }

//    public bool HasFreeSlot()
//    {
//        return activeQuests.Exists(q => q == null || !q.IsActive);
//    }

//    public int GetFirstFreeSlot()
//    {
//        for (int i = 0; i < activeQuests.Count; i++)
//        {
//            if (activeQuests[i] == null || !activeQuests[i].IsActive)
//                return i;
//        }
//        return -1;
//    }

//    public List<ActiveQuest> GetActiveQuests()
//    {
//        return activeQuests.FindAll(q => q != null && q.IsActive);
//    }

//    private int GetMinMoneyByLevel()
//    {
//        return currentLevel switch
//        {
//            1 => level1MoneyNeededMin,
//            2 => level2MoneyNeededMin,
//            3 => level3MoneyNeededMin,
//            _ => 200
//        };
//    }

//    private int GetMaxMoneyByLevel()
//    {
//        return currentLevel switch
//        {
//            1 => level1MoneyNeededMax,
//            2 => level2MoneyNeededMax,
//            3 => level3MoneyNeededMax,
//            _ => 500
//        };
//    }

//    private int GetMinTimeByLevel()
//    {
//        return currentLevel switch
//        {
//            1 => level1TimerNeededMin,
//            2 => level2TimerNeededMin,
//            3 => level3TimerNeededMin,
//            _ => 2
//        };
//    }

//    private int GetMaxTimeByLevel()
//    {
//        return currentLevel switch
//        {
//            1 => level1TimerNeededMax,
//            2 => level2TimerNeededMax,
//            3 => level3TimerNeededMax,
//            _ => 5
//        };
//    }
//}
