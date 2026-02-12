using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TapToCollect : MonoBehaviour
{
    CustomInput input = null;

    [Header("Player Detection")]
    public float detectRadius = 3f;
    public Transform player;

    [Header("UI Components")]
    public GameObject keyPromptImage;      // UI hướng dẫn "Nhấn E"
    public GameObject progressFrame;       // Image khung vòng tròn
    public GameObject progressFillImage;        // Image phần fill (dùng fillAmount)

    [Header("Mash Settings")]
    public float maxProgress = 100f;
    public float progressPerMash = 8f;
    public float decayPerSecond = 25f;

    [Header("Events")]
    public UnityEvent onMashComplete;

    private float currentProgress = 0f;
    private bool isPlayerNearby = false;
    private bool hasStarted = false;
    private bool hasCompleted = false;
    public GameObject swordItemPrefab;

    SpriteRenderer swordRenderer;


    private void Awake()
    {
        input = new CustomInput();

        input.Player.Accept.performed += Accept_performed;
        input.Player.Accept.canceled += Accept_canceled;
    }
    private void Start()
    {
       

        

        swordRenderer = GetComponent<SpriteRenderer>();
        progressFrame.SetActive(false);
        progressFillImage.GetComponent<Image>().fillAmount = 0f;

        player = Player.instance.transform;

       onMashComplete.AddListener(SmashComplete );
    }

    private void Accept_canceled(InputAction.CallbackContext obj)
    {
      
    }

    private void Accept_performed(InputAction.CallbackContext obj)
    {
        // Nếu lần đầu nhấn
        if (!hasStarted)
        {
            hasStarted = true;
            if (keyPromptImage != null)
                keyPromptImage.SetActive(false);
            progressFrame.SetActive(true);
        }

        currentProgress += progressPerMash;
        currentProgress = Mathf.Min(currentProgress, maxProgress);
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    public void SmashComplete()
    {
        Debug.Log("da hoaan thanh");
        SpawnObjectAtPlayer();
    }

    private void Update()
    {
        if (hasCompleted || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        isPlayerNearby = distance <= detectRadius;

        if (isPlayerNearby)
        {
            // Hiện nút E nếu chưa bắt đầu
            if (!hasStarted && keyPromptImage != null)
                keyPromptImage.SetActive(true);

            //if (Input.GetKeyDown(KeyCode.E))
            //{
            //    // Nếu lần đầu nhấn
            //    if (!hasStarted)
            //    {
            //        hasStarted = true;
            //        if (keyPromptImage != null)
            //            keyPromptImage.SetActive(false);
            //        progressFrame.SetActive(true);
            //    }

            //    currentProgress += progressPerMash;
            //    currentProgress = Mathf.Min(currentProgress, maxProgress);
            //}

            // ✅ Kiểm tra hoàn tất NGAY LẬP TỨC trước khi decay
            if (!hasCompleted && currentProgress >= maxProgress)
            {
                hasCompleted = true;
                progressFillImage.GetComponent<Image>().fillAmount = 1f;
                if (keyPromptImage != null)
                    keyPromptImage.SetActive(false);
                progressFrame.SetActive(false);
                progressFillImage.SetActive(false);
                swordRenderer.enabled = false;
                Debug.Log("✅ Đã hoàn thành!"); // <-- đảm bảo có log
                onMashComplete?.Invoke();
                return;
            }

            // Giảm tiến trình nếu đã bắt đầu mà chưa hoàn thành
            if (hasStarted && !hasCompleted)
            {
                currentProgress -= decayPerSecond * Time.deltaTime;
                currentProgress = Mathf.Max(currentProgress, 0f);
                progressFillImage.GetComponent<Image>().fillAmount = currentProgress / maxProgress;

                if (currentProgress <= 0f)
                {
                    hasStarted = false;
                    if (keyPromptImage != null)
                        keyPromptImage.SetActive(true);
                    progressFrame.SetActive(false);
                }
            }
        }
        else
        {
            currentProgress = 0f;
            hasStarted = false;
            if (keyPromptImage != null)
                keyPromptImage.SetActive(false);
            progressFrame.SetActive(false);
            progressFillImage.GetComponent<Image>().fillAmount = 0f;
        }
    }

    public void SpawnObjectAtPlayer()
    {
        Instantiate(swordItemPrefab, player.position, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
