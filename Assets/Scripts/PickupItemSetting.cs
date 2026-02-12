using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItemSetting : MonoBehaviour
{
    public float waitingTime = 2f;
    public bool isAllowToPickup;

    private void Start()
    {
        StartCoroutine(WaitToBePickedUp());
    }


    IEnumerator WaitToBePickedUp()
    {
        yield return new WaitForSeconds(waitingTime);
        isAllowToPickup = true;
    }
}
