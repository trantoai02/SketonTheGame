using UnityEngine;

public class SkeletonScare : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource scaryAudio;

    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string scareTriggerName = "Scare";
    [SerializeField] private bool onlyTriggerOnce = true;

    private bool hasScared = false;

    private void Reset()
    {
        animator = GetComponent<Animator>();
        scaryAudio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasScared && onlyTriggerOnce) return;

        if (collision.collider.CompareTag(playerTag))
        {
            TriggerScare();
        }
    }

    void TriggerScare()
    {
        hasScared = true;

        // Chạy animation scare
        if (animator != null)
        {
            animator.SetTrigger(scareTriggerName);
        }

        // Phát nhạc rùng rợn
        if (scaryAudio != null)
        {
            scaryAudio.Play();
        }
    }
}
