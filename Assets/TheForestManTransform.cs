using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheForestManTransform : MonoBehaviour
{

    Animator animator;
    public GameObject theForestMan;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TransformToCombatBase()
    {
        StartCoroutine(StartTransformToCombat());

    }

    IEnumerator StartTransformToCombat()
    {
        animator.SetTrigger("Transform");
        yield return new WaitForSeconds(2.3f);
        theForestMan.SetActive(true);

        transform.gameObject.SetActive(false);
    }
}
