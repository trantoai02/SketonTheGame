using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.InputSystem;

public class MashRandomKey : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text keyDisplayText;
    public Image progressFill;

    [Header("Progress Settings")]
    public float maxProgress = 100f;
    public float progressPerPress = 10f;
    public float decayPerSecond = 20f;
    public float penaltyWrongKey = 15f;

    [Header("Key Change Settings")]
    public float changeKeyInterval = 3f;

    [Header("Events")]
    public UnityEvent onComplete;

    [Header("Lock Settings")]
    public float lockRadius = 3.5f;

    private float currentProgress = 0f;
    private float timer = 0f;

    private bool isCompleted = false;
    private bool canRegisterInput = true;

    private KeyCode currentKey;
    private KeyCode[] availableKeys =
        { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };

    private CustomInput input;
    public float inputThreshold = 0.7f;

    private void Awake()
    {
        input = new CustomInput();

        input.Player.Movement.performed += OnMovePerformed;
        input.Player.Movement.canceled += OnMoveCanceled;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Start()
    {
        ChooseNewKey();
        UpdateUI();
    }

    private void Update()
    {
        if (Player.instance == null) return;

        float dist = Vector2.Distance(
            Player.instance.transform.position,
            transform.position);

        // 🔒 Lock movement when near
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

        // Change key over time
        if (timer >= changeKeyInterval)
        {
            ChooseNewKey();
            timer = 0f;
        }

        // Decay progress
        if (currentProgress < maxProgress)
        {
            currentProgress -= decayPerSecond * Time.deltaTime;
        }

        currentProgress = Mathf.Clamp(currentProgress, 0f, maxProgress);

        if (progressFill != null)
            progressFill.fillAmount = currentProgress / maxProgress;

        // Completed
        if (currentProgress >= maxProgress)
        {
            isCompleted = true;
            Player.instance.isMovementLocked = false;
            onComplete?.Invoke();
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        if (isCompleted) return;

        Vector2 dir = ctx.ReadValue<Vector2>();

        if (dir.magnitude < inputThreshold) return;
        if (!canRegisterInput) return;

        KeyCode detectedDirection = GetDirectionFromVector(dir);

        if (detectedDirection == currentKey)
        {
            currentProgress += progressPerPress;
        }
        else
        {
            currentProgress -= penaltyWrongKey;
        }

        canRegisterInput = false;
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        canRegisterInput = true;
    }

    KeyCode GetDirectionFromVector(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            return dir.x > 0 ? KeyCode.D : KeyCode.A;
        }
        else
        {
            return dir.y > 0 ? KeyCode.W : KeyCode.S;
        }
    }

    void ChooseNewKey()
    {
        KeyCode newKey;

        do
        {
            newKey = availableKeys[Random.Range(0, availableKeys.Length)];
        }
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
