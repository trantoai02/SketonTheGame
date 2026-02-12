using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public UnityEvent onDialogueListFinishedGlobal;

    Queue<string> sentences;
    string sentence;
    public TextMeshProUGUI dialogueName;
    public TextMeshProUGUI textComponent;
    public GameObject dialogueBox;
    public GameObject avatar;
    public float textSpeed;

    public Animator animator;

    public int currentDialogueIndex = 0;
    public DialogueList dialogueList;

    public bool isDialogueActive = false;

    [Header("Portrait")]
    public PortraitDatabase portraitDatabase;
    public UnityEngine.UI.Image portraitImage;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        sentences = new Queue<string>();
        
    }

    void UpdatePortrait(string speakerName)
    {
        if (portraitDatabase == null || portraitImage == null)
            return;

        Sprite portrait = portraitDatabase.GetPortrait(speakerName);

        if (portrait != null)
        {
            portraitImage.sprite = portrait;
            portraitImage.gameObject.SetActive(true);
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }
    }

    public void CheckInput(Dialogue dialogue)
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == sentence)
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = sentence;
            }
        }
    }
    public void StartDialogue(DialogueList dialogueList, int index)
    {
        //khóa di chuyển người chơi
        isDialogueActive = true;


        this.dialogueList = dialogueList; // Gán dialogueList cho đối tượng hiện tại
        currentDialogueIndex = index;

        dialogueBox.gameObject.SetActive(true);
        //avatar.gameObject.SetActive(true);
        animator.SetBool("IsOpen", true);

        string speakerName = dialogueList.dialogues[index].name;
        dialogueName.text = speakerName;
        UpdatePortrait(speakerName);

        textComponent.text = string.Empty;
        sentences.Clear();
        StopAllCoroutines();

        foreach (string s in dialogueList.dialogues[index].lines)
        {
            string processedSentence = s.Replace("{playerName}", GameManager.instance.playerName); // Thay thế {playerName} bằng tên người chơi
            sentences.Enqueue(processedSentence); // Thêm câu đã thay thế vào hàng đợi
        }
        if (sentences.Count > 0)
        {
            sentence = sentences.Dequeue();
            StartCoroutine(TypeLine(sentence));
        }
        else
        {
            Debug.LogWarning("Dialogue is empty!");
            TriggerNextCharacter(); // Chuyển ngay tới nhân vật tiếp theo nếu đoạn hội thoại rỗng
        }
    }

    IEnumerator TypeLine(string sentence)
    {
        
        foreach (char c in sentence.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {

        if (sentences.Count >0)
        {
            sentence = sentences.Dequeue();
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine(sentence));
        }
        else
        {
           animator.SetBool("IsOpen", false);
            Debug.Log(currentDialogueIndex);

            dialogueBox.gameObject.SetActive(false);
            avatar.gameObject.SetActive(false);

            TriggerNextCharacter();

        }
    }

    public void ActiveAvatar()
    {
        avatar.gameObject.SetActive(true);

    }

    public void TriggerNextCharacter()
    {
        currentDialogueIndex++;
        Debug.Log(currentDialogueIndex);

        if (currentDialogueIndex < dialogueList.dialogues.Length)
        {
            // Bắt đầu đoạn hội thoại tiếp theo
            StartDialogue(dialogueList,currentDialogueIndex);
        }
        else
        {
            //mở khóa di chuyển người chơi
            

            // Hết tất cả các đoạn hội thoại trong danh sách
            Debug.Log("Đã hoàn thành tất cả các đoạn hội thoại.");
            currentDialogueIndex = 0; // Reset để bắt đầu lại nếu cần

            // Gọi UnityEvent khi hoàn tất
            if (onDialogueListFinishedGlobal != null)
                onDialogueListFinishedGlobal.Invoke();  // Kích hoạt sự kiện
        }
    }
}
