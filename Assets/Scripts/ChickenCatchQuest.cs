using UnityEngine;

public class ChickenCatchQuest : MonoBehaviour
{
    [Header("Cấu hình nhiệm vụ")]
    public string questID = "CatchChicken";    // ID của nhiệm vụ trong QuestManager
    public int totalChickensRequired = 3;      // Tổng số gà cần bắt

    [Header("Tham chiếu đối tượng")]
    public Transform handlePoint;              // Vị trí tay cầm của player
    public Transform coopEntryPoint;           // Điểm gà chui vào chuồng
    public Collider2D coopTriggerCollider;     // Collider phía trước chuồng

    private Chicken currentChicken = null;     // Gà hiện tại đang được cầm
    private bool isPlayerNearCoop = false;
    private int currentCount = 0;

    [Header("Âm thanh nhiệm vụ")]
    public AudioClip catchingMusicClip; // Nhạc khi đang bắt gà

    private AudioClip previousMainClip;
    private bool isUsingCatchMusic = false;


    private void Start()
    {
        // Gắn trigger để phát hiện người chơi tới gần chuồng
        if (coopTriggerCollider != null)
        {
            var trigger = coopTriggerCollider.gameObject.AddComponent<CoopTrigger>();
            trigger.questRef = this;
        }
    }

    private void Update()
    {
        if (isPlayerNearCoop && currentChicken != null)
        {
            Chicken chickenToRelease = currentChicken;
            currentChicken = null;

            chickenToRelease.MoveToCoop(coopEntryPoint.position, () =>
            {
                currentCount++;
                QuestManager.Instance.UpdateChickenQuest(questID, 1);

                // 🐔 Khi gà được thả → khôi phục nhạc chính
                var audio = AudioManager.instance;
                if (audio != null && isUsingCatchMusic)
                {
                    audio.mainMusicSource.Stop();
                    audio.mainMusicSource.clip = previousMainClip;
                    audio.mainMusicSource.loop = true;
                    audio.mainMusicSource.Play();
                    isUsingCatchMusic = false;
                }

                if (currentCount >= totalChickensRequired)
                {
                    QuestManager.Instance.CompleteQuest(questID);
                }
            });
        }
    }


    /// <summary>
    /// Gọi khi player chạm vào gà
    /// </summary>
    public void TryCatchChicken(Chicken chicken)
    {
        if (currentChicken == null)
        {
            currentChicken = chicken;
            chicken.MoveToHandle(handlePoint);

            // 🔊 Tạm dừng nhạc chính và phát nhạc "bắt gà"
            var audio = AudioManager.instance;
            if (audio != null && catchingMusicClip != null && !isUsingCatchMusic)
            {
                previousMainClip = audio.mainMusicSource.clip;
                audio.mainMusicSource.Pause(); // tạm dừng nhạc hiện tại

                audio.mainMusicSource.clip = catchingMusicClip;
                audio.mainMusicSource.loop = true;
                audio.mainMusicSource.Play();

                isUsingCatchMusic = true;
            }
        }
    }


    public void SetPlayerNearCoop(bool value)
    {
        isPlayerNearCoop = value;
    }
}
