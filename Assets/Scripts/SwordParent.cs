using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordParent : MonoBehaviour
{
    [SerializeField]
    private Transform weaponCollider;

    public Vector2 pointerPosition { get; set; }


    public Animator animator;
    public float delay = 0.3f;
    private bool attackBlocked;

    private Vector2 mouseWorldPos;
 
    public void Attack()
    {
        weaponCollider.gameObject.SetActive(true);
        if (attackBlocked)
        {
            return;
        }
        animator.SetTrigger("Attack");
        attackBlocked = true;
        StartCoroutine(DelayAttack());
        
    }

    public void DoneAttackingAnimationEvent()
    {
        weaponCollider.gameObject.SetActive(false);
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(delay);
        attackBlocked=false;
        DoneAttackingAnimationEvent();
    }
}
