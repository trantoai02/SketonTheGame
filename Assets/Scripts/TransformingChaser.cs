using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TransformingChaser : MonoBehaviour
{
    [Header("Các biến thể (GameObject con đã có sẵn trong scene)")]
    public GameObject[] transformations; // GameObject con đã gắn sẵn

    [Header("Các waypoint tương ứng")]
    public Transform[] waypoints;

    [Header("Tốc độ di chuyển")]
    public float moveSpeed = 3f;

    [Header("Sự kiện khi hoàn thành toàn bộ biến thể")]
    public UnityEvent onAllFormsCaught; // 👈 Thêm sự kiện ở đây

    private int currentIndex = 0;
    private GameObject currentObject;
    private bool isTransforming = false;

    void Start()
    {
        ActivateCurrentForm();
    }

    void ActivateCurrentForm()
    {
        // Tắt tất cả trước
        for (int i = 0; i < transformations.Length; i++)
        {
            transformations[i].SetActive(i == currentIndex);
        }

        currentObject = transformations[currentIndex];

        // Gắn sự kiện chết
        EnemyForm form = currentObject.GetComponent<EnemyForm>();
        form.onDeath += OnCurrentFormDeath;
    }

    void OnCurrentFormDeath()
    {
        StartCoroutine(HandleTransformation());
    }

    IEnumerator HandleTransformation()
    {
        if (isTransforming) yield break;
        isTransforming = true;

        if (currentIndex < waypoints.Length)
        {
            Vector3 target = waypoints[currentIndex].position;
            while (Vector3.Distance(currentObject.transform.position, target) > 0.1f)
            {
                currentObject.transform.GetComponent<Collider2D>().isTrigger = true;
                currentObject.transform.GetChild(0).GetComponent<Animator>().SetBool("isRun", true);

                currentObject.transform.position = Vector3.MoveTowards(currentObject.transform.position, target, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        // Tắt object hiện tại
        currentObject.SetActive(false);

        currentIndex++;

        if (currentIndex >= transformations.Length)
        {
            Debug.Log("🎉 Chúc mừng! Bạn đã bắt được đối tượng cuối cùng!");
            onAllFormsCaught?.Invoke(); // 👈 Gọi sự kiện ở đây
        }
        else
        {
            ActivateCurrentForm();
        }

        isTransforming = false;
    }
}
