using UnityEngine;
using System.Collections;

public class BookSummon : MonoBehaviour
{
    [Header("Tham chiếu trong scene")]
    public Transform spawnPoint;           // Điểm xuất hiện ban đầu của sách
    public GameObject bookPrefab;          // Prefab quyển sách
   // public GameObject egoDemonPrefab;      // Prefab quái EgoDemon

    //private GameObject currentBook;

    private void Start()
    {
        //StartCoroutine(SummonSequence());
    }

    // ==========================
    // Giai đoạn 1: Spawn sách + chờ 1 giây rồi ẩn spawnPoint
    // ==========================

   
    IEnumerator SummonSequence()
    {
        // Spawn quyển sách
       // Instantiate(bookPrefab, spawnPoint.position, Quaternion.identity);
       if(bookPrefab != null)
        {
            bookPrefab.transform.position = spawnPoint.position;
            bookPrefab.SetActive(true);

            CameraSequenceController.Instance.FollowBook();
        }

        // Chờ 1 giây trước khi ẩn
        yield return new WaitForSeconds(1f);

        transform.gameObject.SetActive(false);
        // Tắt sprite và animator của spawnPoint
        //SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        //Animator anim = GetComponent<Animator>();

        //if (sprite) sprite.enabled = false;
        //if (anim) anim.enabled = false;
    }

    // ==========================
    // Giai đoạn 2: Gọi từ animation event khi sách kết thúc bay
    // ==========================
  
}
