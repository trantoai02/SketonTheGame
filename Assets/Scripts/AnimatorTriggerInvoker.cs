using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorTriggerInvoker : MonoBehaviour
{

    [Header("Delay Settings")]
    public float delayTime = 1f;
    public UnityEvent delayedEvents;

    public Animator animator;

    public void SetBoolTrue(string parameterName)
    {
        animator.SetBool(parameterName, true);
    }

    public void SetBoolFalse(string parameterName)
    {
        animator.SetBool(parameterName, false);
    }

    public void SetBool(string parameterName, bool value)
    {
        animator.SetBool(parameterName, value);
    }

    public void SetTrigger(string parameterName)
    {
        animator.SetTrigger(parameterName);
    }

    public void InvokeDelayedEvents()
    {
        StartCoroutine(InvokeAfterSeconds(delayTime));
    }
    private IEnumerator InvokeAfterSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        delayedEvents?.Invoke();
    }

}
