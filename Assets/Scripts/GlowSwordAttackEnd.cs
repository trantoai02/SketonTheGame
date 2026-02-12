using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowSwordAttackEnd : MonoBehaviour
{
    public void DoneMeeleAttack()
    {
        GlowSword.Instance.DoneAttackingAnimationEvent();
    }
}
