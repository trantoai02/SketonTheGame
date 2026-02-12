using System.Collections;
using UnityEngine;

public class PlayerFlatten : MonoBehaviour
{
    [SerializeField] private float flatDuration = 5f;

    private Player player;
    private Animator animator;
    private Coroutine flatRoutine;

    void Awake()
    {
        player = GetComponent<Player>();
        animator = GetComponentInChildren<Animator>();
    }

    public void Flatten()
    {
        if (flatRoutine != null)
            StopCoroutine(flatRoutine);

        flatRoutine = StartCoroutine(FlatRoutine());
    }

    IEnumerator FlatRoutine()
    {
        // 👉 vào trạng thái Flat
        player.EnterFlatState();
        animator.SetBool("isFlat", true);

        yield return new WaitForSeconds(flatDuration);

        // 👉 thoát Flat
        animator.SetBool("isFlat", false);
        player.ExitFlatState();

        flatRoutine = null;
    }
}
