using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedObjectInactive : MonoBehaviour
{
    public float aliveTime;
    private void OnEnable()
    {
        StartCoroutine(InactiveInSeconds(aliveTime));
    }

    IEnumerator InactiveInSeconds(float sec)
    {
        yield return new WaitForSeconds(sec);
        gameObject.SetActive(false);

    }

}
