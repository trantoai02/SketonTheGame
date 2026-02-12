using UnityEngine;

public class QuestItem : MonoBehaviour
{
    public GameObject starObject;

    [Tooltip("ID của quest sẽ hoàn thành khi nhặt vật phẩm này")]
    public string questIDToComplete;

    [Tooltip("Tên hiển thị (nếu muốn)")]
    public string itemName;

    [Tooltip("Có tự hủy sau khi nhặt không")]
    public bool destroyOnPickup = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        //.....
        //if (starObject != null)
        //{
        //    GameObject starInstance;

        //    if (!starObject.scene.IsValid())
        //    {
        //        // Là prefab → tạo instance mới
        //        starInstance = Instantiate(starObject);
        //        starObject = starInstance;
        //    }
        //    else
        //    {
        //        // Đã nằm trong scene
        //        starInstance = starObject;
        //    }

        //    // Nếu có Player instance thì đặt vị trí tại Player
        //    if (Player.instance != null)
        //        starInstance.transform.position = Player.instance.transform.position;

        //    starInstance.SetActive(true);
        //}

       
        QuestManager.Instance?.CompleteQuest(questIDToComplete);

        if (destroyOnPickup)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.CompareTag("Player")) return;

        //if (starObject != null)
        //{
        //    if (Player.instance != null)
        //    {
        //        starObject.transform.position = Player.instance.transform.position;
        //    }
        //    starObject.SetActive(true);
        //}

        QuestManager.Instance?.CompleteQuest(questIDToComplete);

        if (destroyOnPickup)
            Destroy(gameObject);
    }

    public void CallToCompleteQuest()
    {

        if (starObject != null)
        {
            if (Player.instance != null)
            {
                starObject.transform.position = Player.instance.transform.position;
            }
            starObject.SetActive(true);
        }

        QuestManager.Instance?.CompleteQuest(questIDToComplete);

        if (destroyOnPickup)
            Destroy(gameObject);
    }
}
