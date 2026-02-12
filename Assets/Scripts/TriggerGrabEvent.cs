using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerGrabEvent : MonoBehaviour
{
    public UnityEvent afterTrigger;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag=="Player")
            afterTrigger?.Invoke();
    }
}
