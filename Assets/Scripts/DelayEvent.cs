using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DelayEvent : MonoBehaviour
{
    public void WaitAndInvoke(float delay)
    {
        StartCoroutine(WaitCoroutine(delay));
    }

    private IEnumerator WaitCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        onDelayComplete?.Invoke();
    }

    [Header("Sự kiện sau khi đợi xong")]
    public UnityEvent onDelayComplete;
}
