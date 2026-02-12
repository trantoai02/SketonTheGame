using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Danh sách quest cho level này")]
    public List<Quest> quests = new List<Quest>();

    [Header("Prefab Quest Item (cha, có con là TMP_Text)")]
    public GameObject questItemPrefab;

    [Header("Parent chứa các quest trong UI")]
    public Transform questListParent;

    private Dictionary<string, TMP_Text> questTexts = new Dictionary<string, TMP_Text>();

    public UnityEvent afterKillQuestCompleted;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        LoadQuestStates();   // load trạng thái từ PlayerPrefs
        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (Transform child in questListParent)
            Destroy(child.gameObject);

        questTexts.Clear();

        foreach (Quest quest in quests)
        {
            GameObject newQuestObj = Instantiate(questItemPrefab, questListParent);
            TMP_Text questText = newQuestObj.GetComponentInChildren<TMP_Text>();

            if (questText != null)
            {
                string displayText = quest.description;

                if (quest.isKillCountQuest)
                {
                    displayText += $" ({quest.currentKill}/{quest.goal})";
                }

                questText.text = displayText;

                if (quest.isCompleted)
                {
                    questText.color = Color.green;
                    questText.fontStyle = FontStyles.Bold;
                }

                questTexts[quest.questID] = questText;
            }
        }
    }

    public void UpdateKillQuest(string questID, int amount = 1)
    {
        Quest quest = quests.Find(q => q.questID == questID);
        if (quest != null && quest.isKillCountQuest && !(quest.currentKill >= quest.goal))
        {
            quest.currentKill += amount;
            if (quest.currentKill >= quest.goal)
            {
                quest.currentKill = quest.goal;
                CompleteQuest(questID);
                afterKillQuestCompleted?.Invoke();
            }

            if (questTexts.TryGetValue(questID, out TMP_Text questText))
            {
                questText.text = $"{quest.description} ({quest.currentKill}/{quest.goal})";
            }
            else
            {
                RefreshUI();
            }
        }
    }

    public void OnEnemyKilled(GameObject enemyObj)
    {
        foreach (Quest quest in quests)
        {
            if (quest.isKillCountQuest && !(quest.currentKill >= quest.goal) && enemyObj.CompareTag("Enemy"))
            {
                UpdateKillQuest(quest.questID);
            }
        }
    }

    public void UpdateChickenQuest(string questID, int amount = 1)
    {
        Quest quest = quests.Find(q => q.questID == questID);
        if (quest != null && quest.isChickenQuest && !quest.isCompleted)
        {
            quest.currentChicken += amount;
            if (quest.currentChicken >= quest.goal)
            {
                quest.currentChicken = quest.goal;
                CompleteQuest(questID);
            }

            if (questTexts.TryGetValue(questID, out TMP_Text questText))
                questText.text = $"{quest.description} ({quest.currentChicken}/{quest.goal})";
            else
                RefreshUI();
        }
    }

    public void OnChickenCaught(GameObject chickenObj)
    {
        foreach (Quest quest in quests)
        {
            if (quest.isChickenQuest && !quest.isCompleted && chickenObj.CompareTag("Chicken"))
            {
                UpdateChickenQuest(quest.questID);
            }
        }
    }

    /// <summary>
    /// Đánh dấu quest hoàn thành
    /// </summary>
    public void CompleteQuest(string questID)
    {
        Quest quest = quests.Find(q => q.questID == questID);
        if (quest == null) return;

        // ✅ NEW: Nếu chưa hoàn thành thì đánh dấu, nếu rồi thì vẫn spawn star nếu cần
        if (!quest.isCompleted)
        {
            quest.isCompleted = true;
        }

        // ✅ NEW: Spawn star nếu chưa có hoặc là prefab
        if (quest.starObject != null)
        {
            GameObject starInstance;

            if (!quest.starObject.scene.IsValid())
            {
                // Là prefab → tạo instance mới
                starInstance = Instantiate(quest.starObject, Player.instance.transform.position, Quaternion.identity);
                quest.starObject = starInstance;
            }
            else
            {
                // Đã nằm trong scene
                starInstance = quest.starObject;

                // Nếu có Player instance thì đặt vị trí tại Player
                if (Player.instance != null)
                    starInstance.transform.position = Player.instance.transform.position;

                starInstance.SetActive(true);
            }

            
        }

        if (questTexts.TryGetValue(questID, out TMP_Text questText))
        {
            questText.color = Color.green;
            questText.fontStyle = FontStyles.Bold;
        }
        else
        {
            RefreshUI();
        }
    }

    /// <summary>
    /// Lưu trạng thái tất cả quest
    /// </summary>
    public void SaveAllQuests()
    {
        foreach (Quest quest in quests)
        {
            PlayerPrefs.SetInt($"Quest_{quest.questID}", quest.isCompleted ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    private void SaveQuestState(string questID, bool isCompleted)
    {
        PlayerPrefs.SetInt($"Quest_{questID}", isCompleted ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Load trạng thái quest từ PlayerPrefs
    /// </summary>
    private void LoadQuestStates()
    {
        foreach (Quest quest in quests)
        {
            if (PlayerPrefs.HasKey($"Quest_{quest.questID}"))
            {
                int state = PlayerPrefs.GetInt($"Quest_{quest.questID}");
                quest.isCompleted = state == 1;

                // ✅ NEW: Nếu đã hoàn thành nhưng star chưa có trong scene → spawn lại
                if (quest.isCompleted && quest.starObject != null && !quest.starObject.scene.IsValid())
                {
                    GameObject starInstance = Instantiate(quest.starObject);

                    if (Player.instance != null)
                        starInstance.transform.position = Player.instance.transform.position;

                    starInstance.SetActive(true);
                    quest.starObject = starInstance;
                }
            }
        }
    }

    /// <summary>
    /// Reset toàn bộ quest (debug hoặc khi chơi lại level)
    /// </summary>
    public void ResetAllQuests()
    {
        foreach (Quest quest in quests)
        {
            quest.isCompleted = false;
            PlayerPrefs.DeleteKey($"Quest_{quest.questID}");
        }
        RefreshUI();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var ui = FindObjectOfType<QuestUIConnector>();
        if (ui != null)
        {
            questListParent = ui.questListParent;
            questItemPrefab = ui.questItemPrefab;
            RefreshUI();
        }
    }
}
