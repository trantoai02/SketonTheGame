using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraffGunTransform : MonoBehaviour
{
    Animator animator;
    public DialogueTrigger dialogueTrigger;
    private void Start()
    {
            animator = GetComponent<Animator>();
    }

    public void TransformToDraffle()
    {
        animator.SetBool("isTransform", true);
    }

    public void TriggerDialogue()
    {
        if (dialogueTrigger != null)
        {
            dialogueTrigger.TriggerDialogue();

        }
    }
}
