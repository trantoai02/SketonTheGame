using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    CustomInput input = null;

    [Header("Event sau khi kết thúc toàn bộ dialogue")]
    public UnityEvent eventBeforeTriggerDialogue;

    public UnityEvent onDialogueEnd;

    DialogueManager dialogueManager;

    public DialogueList dialogueList;

    public GameObject talkButton;

    bool isPlayerInRange = false;

    public float dialogueDelay = 0f; // 
    private void Awake()
    {
        input = new CustomInput();

        dialogueManager = FindObjectOfType<DialogueManager>();

        input.Player.Accept.performed += Accept_performed;
        input.Player.Accept.canceled += Accept_canceled;
    }

    private void Accept_canceled(InputAction.CallbackContext obj)
    {
      
    }

    private void Accept_performed(InputAction.CallbackContext obj)
    {
        if (isPlayerInRange)
        {
            TriggerDialogue();

        }
    }

 

    private void Start()
    {
        if(talkButton != null)
        {
            talkButton.SetActive(false);

        }
        else
        {
            return;
        }
    }
    private void Update()
    {
        if (dialogueManager.dialogueList == this.dialogueList && dialogueList != null && dialogueList.dialogues.Length > 0)
        {
            dialogueManager.CheckInput(dialogueList.dialogues[dialogueManager.currentDialogueIndex]);
        }

        if(isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TriggerDialogue();

        }
    }
    public void TriggerDialogue()
    {
        eventBeforeTriggerDialogue?.Invoke();

        StartCoroutine(DelayedTriggerDialogue());

        //if (dialogueList != null && dialogueList.dialogues.Length > 0)
        //{
        //    dialogueManager.StartDialogue(dialogueList, 0); 
        //}
    }

    private IEnumerator DelayedTriggerDialogue()
    {
        if (dialogueDelay > 0f)
        {
            yield return new WaitForSeconds(dialogueDelay);
        }

        if (dialogueList != null && dialogueList.dialogues.Length > 0)
        {
            
            dialogueManager.StartDialogue(dialogueList, 0);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(collision.tag=="Player")
        {
            isPlayerInRange = true;
            if (talkButton != null)
            {
                talkButton.SetActive(true);
            }
            else
            {
                return;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isPlayerInRange = false;
            if (talkButton != null)
            {
                talkButton.SetActive(false);
            }
            else
            {
                return;
            }
        }
    }


    private void OnEnable()
    {
        input.Enable();
        if (dialogueManager != null)
        {
            // Đăng ký lắng nghe sự kiện khi hoàn tất tất cả các dialogue
            dialogueManager.onDialogueListFinishedGlobal.AddListener(OnDialogueFinished);
        }
    }

    private void OnDisable()
    {
        input.Disable();
        if (dialogueManager != null)
        {

            // Hủy đăng ký sự kiện khi không còn cần lắng nghe
            dialogueManager.onDialogueListFinishedGlobal.RemoveListener(OnDialogueFinished);
        }
    }

    private void OnDialogueFinished()
    {
        // Kiểm tra xem đoạn hội thoại hiện tại có phải là của đối tượng này không
        if (dialogueManager.dialogueList == this.dialogueList)
        {

            onDialogueEnd?.Invoke();  // Gọi UnityEvent
        }
    }


    public GameObject itemPrefab;
    public void SpawnObjectAtPlayer()
    {
        if(itemPrefab != null)
        Instantiate(itemPrefab, Player.instance.transform.position, Quaternion.identity);


    }
    public GameObject objToMoveToPlayer;
    public void MoveObjectToPlayerPos()
    {
        if (objToMoveToPlayer != null)
        objToMoveToPlayer.transform.position = Player.instance.transform.position;
    }

    public Transform positionSetterForPlayer;
    public void MovePlayerToPos()
    {
        Player.instance.transform.position = positionSetterForPlayer.transform.position;
    }

    public void LockPlayerMovement()
    {
        Player.instance.isMovementLocked = true;
    }
    public void UnlockPlayerMovement()
    {
        Player.instance.isMovementLocked = false;
    }

    public void ActiveDialogueAvatar()
    {
        dialogueManager.ActiveAvatar();
    }
}
