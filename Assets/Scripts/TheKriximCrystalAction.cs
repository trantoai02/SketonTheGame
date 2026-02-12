using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheKriximCrystalAction : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ThrowGem()
    {
        animator.SetBool("isThrow", true);
       // CameraSequenceController.Instance.FollowGem();

    }

    public void StopFollowGem()
    {
       
        CameraSequenceController.Instance.StopFollow();

    }
}
