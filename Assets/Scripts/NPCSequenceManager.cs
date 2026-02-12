using UnityEngine;
using UnityEngine.Events;

public class NPCSequenceManager : MonoBehaviour
{
    public NPCMoveToPoint npc1;
    public NPCMoveToPoint npc2;
    public Transform waypoint1;
    public Transform waypoint2;
    public DialogueTrigger dialogueTrigger; // Gắn GameObject có script dialogue vào đây

    private int npcsArrived = 0;

    bool isTriggered = false;

    public UnityEvent onSequenceEnd;

    public void StartSequence()
    {
        
            Debug.Log("start sequence");
            npcsArrived = 0;

            npc1.MoveTo(waypoint1);
            npc2.MoveTo(waypoint2);

            npc1.OnReachedTarget += OnNPCReached;
            //npc2.OnReachedTarget += OnNPCReached;
       
        onSequenceEnd?.Invoke();
        
    }

    void OnNPCReached()
    {
        npcsArrived++;
        if (npcsArrived >= 2)
        {
            if (!isTriggered)
            {
                isTriggered = true;
                //TriggerDialogue();
            }
        }
    }

    void TriggerDialogue()
    {
        if (dialogueTrigger != null)
        {
            dialogueTrigger.TriggerDialogue();
        }
    }
}
