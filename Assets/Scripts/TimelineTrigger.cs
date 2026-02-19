using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class TimelineTrigger : MonoBehaviour
{
    [Header("Object chứa Timeline")]
    public GameObject timelineObject;

    private PlayableDirector director;
    private bool hasPlayed = false;

    public UnityEvent eventAfterTimelineEnd;

    private void Start()
    {
        if (timelineObject != null)
        {
            director = timelineObject.GetComponent<PlayableDirector>();
            if (director != null)
            {
                // Đảm bảo timeline ban đầu chưa bật nếu muốn
                timelineObject.SetActive(false);

                // Lắng nghe sự kiện kết thúc Timeline
                director.stopped += OnTimelineFinished;
            }
        }
    }

    public GameObject pcCanvas;
    public GameObject questUI;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasPlayed && collision.CompareTag("Player"))
        {
            TriggerTimeline();
        }
    }

    public void TriggerTimeline()
    {
        hasPlayed = true;
        pcCanvas.SetActive(false);
        questUI.SetActive(false);
        // Bật timeline object và phát
        timelineObject.SetActive(true);

        if (director != null)
        {
            director.Play();
        }
        else
        {
            Debug.LogWarning("PlayableDirector chưa được gán!");
        }
    }
    //public GameManager gameManager;
    //public EndTrigger endTrigger;
    private void OnTimelineFinished(PlayableDirector dir)
    {
        //Debug.Log("EndLevel");
        //// Tại đây bạn có thể gọi hàm kết thúc level thực sự nếu muốn
        //// ví dụ: GameManager.Instance.EndLevel();
        //// endTrigger.GoToPlayerPos();
        //gameManager.CompleteLevel();
        pcCanvas.SetActive(true);
        questUI.SetActive(true);
        eventAfterTimelineEnd?.Invoke();

    }



    private void OnDestroy()
    {
        if (director != null)
            director.stopped -= OnTimelineFinished;
    }
}
