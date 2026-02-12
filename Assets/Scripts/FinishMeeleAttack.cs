using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishMeeleAttack : MonoBehaviour
{
   public void DoneMeeleAttack()
    {
        Stick.Instance.DoneAttackingAnimationEvent();
    } 
}
