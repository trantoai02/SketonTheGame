using UnityEngine;
using UnityEngine.Events;

public class TriggerWelcomeZone : MonoBehaviour
{
    public NPCSequenceManager npcSequenceManager;

    private bool hasTriggered = false;

    public UnityEvent onTriggerWelcome;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;

            onTriggerWelcome?.Invoke();
           
        }
    }

    public void StartNpcSequence()
    {
        if (npcSequenceManager != null)
        {

             npcSequenceManager.StartSequence();

        }
    }
}
