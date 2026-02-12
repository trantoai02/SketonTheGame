using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class MashRandomKey : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text keyDisplayText;
    public Image progressFill;

    [Header("Progress Settings")]
    public float maxProgress = 100f;
    public float progressPerPress = 10f;
    public float decayPerSecond = 20f;
    public float penaltyWrongKey = 15f;   // ⭐ Trừ khi nhấn sai phím

    [Header("Key Change Settings")]
    public float changeKeyInterval = 3f;

    [Header("Events")]
    public UnityEvent onComplete;

    private float currentProgress = 0f;
    private float timer = 0f;

    private KeyCode currentKey;
    private bool isCompleted = false;

    private KeyCode[] availableKeys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };

    public float lockRadius = 3.5f;
    private void Start()
    {
        ChooseNewKey();
        UpdateUI();
    }

    private void Update()
    {
        float dist = Vector2.Distance(Player.instance.transform.position, transform.position);

        // 🔒 Nếu player lại gần minigame → khóa di chuyển
        if (dist <= lockRadius)
        {
            Player.instance.isMovementLocked = true;
        }
        else
        {
            Player.instance.isMovementLocked = false;
        }

        if (isCompleted) return;

        timer += Time.deltaTime;

        // Đổi phím sau mỗi interval
        if (timer >= changeKeyInterval)
        {
            ChooseNewKey();
            timer = 0f;
        }

        // ⭐ Nếu đúng phím
        if (Input.GetKeyDown(currentKey))
        {
            currentProgress += progressPerPress;
        }
        else
        {
            // ⭐ Nếu người chơi ấn bất kỳ phím nào DÙNG để mash nhưng sai
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(KeyCode.W) ||
                    Input.GetKeyDown(KeyCode.A) ||
                    Input.GetKeyDown(KeyCode.S) ||
                    Input.GetKeyDown(KeyCode.D))
                {
                    if (!Input.GetKeyDown(currentKey))
                    {
                        currentProgress -= penaltyWrongKey;
                    }
                }
            }
        }

        // ⭐ Chỉ decay khi CHƯA hoàn thành
        if (currentProgress < maxProgress)
        {
            currentProgress -= decayPerSecond * Time.deltaTime;
        }

        currentProgress = Mathf.Clamp(currentProgress, 0f, maxProgress);

        // Update UI
        progressFill.fillAmount = currentProgress / maxProgress;

        // Completed
        if (currentProgress >= maxProgress)
        {
            isCompleted = true;
            Player.instance.isMovementLocked = false;

            onComplete?.Invoke();
        }
    }

    void ChooseNewKey()
    {
        KeyCode newKey;

        do newKey = availableKeys[Random.Range(0, availableKeys.Length)];
        while (newKey == currentKey);

        currentKey = newKey;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (keyDisplayText != null)
            keyDisplayText.text = currentKey.ToString();
    }
}
