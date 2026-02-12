using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EgoAction : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

   

    public void EgoFindingBook()
    {
        animator.SetBool("isFinding", true);

    }

    public void EgoCatching()
    {
        animator.SetBool("isCatching", true);

    }

    public void SummonEgoDemon()
    {

    }
}
