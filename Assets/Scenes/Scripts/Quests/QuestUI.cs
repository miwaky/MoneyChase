using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private GameObject questTextPrefab;
    [SerializeField] private float updateInterval = 1f;

    private readonly List<TextMeshProUGUI> activeTexts = new();

    private void Start()
    {
        InvokeRepeating(nameof(UpdateQuestDisplay), 0f, updateInterval);
    }

    void UpdateQuestDisplay()
    {
        // Nettoie les anciens textes
        foreach (var text in activeTexts)
        {
            if (text != null) Destroy(text.gameObject);
        }
        activeTexts.Clear();

        List<ActiveQuest> quests = QuestManager.Instance.GetActiveQuests();
        foreach (var quest in quests)
        {
            GameObject go = Instantiate(questTextPrefab, transform);
            TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();

            string progress = "";
            if (quest.Model.Type == QuestCategory.Money)
            {
                progress = $"{quest.ProgressMoney}/{quest.Model.Reward}";
            }
            else if (quest.Model.Type == QuestCategory.Timer)
            {
                progress = $"{Mathf.CeilToInt(quest.TimeLeft)}s restantes";
            }

            text.text = $"<b>{quest.Model.QuestName}</b> ({quest.Model.Type})\n→ {progress}";
            activeTexts.Add(text);
        }
    }
}
