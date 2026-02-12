
using UnityEngine;

public class TriggerDialogueExit : MonoBehaviour
{
    public DialogueTrigger dialogueTrigger;
    private bool hasTriggered = false;
    // Start is called before the first frame update
    void Start()
    {
        //dialogueTrigger = GetComponentInChildren<DialogueTrigger>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!hasTriggered && collision.tag == "Player")
        {
            hasTriggered = true;
            dialogueTrigger.TriggerDialogue();
           // transform.gameObject.SetActive(false);

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasTriggered && collision.tag == "Player")
        {
            hasTriggered = true;
            dialogueTrigger.TriggerDialogue();
          //  transform.gameObject.SetActive(false);
        }
    }
}
