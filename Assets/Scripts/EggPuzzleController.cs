using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
public class EggPuzzleController : MonoBehaviour
{
    public EggController[] eggs; // 3 quả trứng

    // Mỗi lượt là 1 mảng 3 phần tử (thứ tự trứng)
    public List<int[]> rounds = new List<int[]>
    {
        new int[]{0,1,2,1},
        new int[]{2,2,1,0,1,0},
        new int[]{1,0,1,2,1,0,2,2}
    };

    private int currentRound = 0;
    private List<int> playerInput = new List<int>();
    private bool canPlayerInput = false;

    public UnityEvent eventAfterEggPuzzleEnd;

    private void Start()
    {
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        canPlayerInput = false;
        playerInput.Clear();

        int[] sequence = rounds[currentRound];

        yield return new WaitForSeconds(1f);

        foreach (int id in sequence)
        {
            eggs[id].Sing();
            yield return new WaitForSeconds(0.8f);
        }

        canPlayerInput = true;
    }

    public void OnEggHit(EggController egg)
    {
        if (!canPlayerInput) return;

        int id = System.Array.IndexOf(eggs, egg);
        playerInput.Add(id);

        int[] correctSeq = rounds[currentRound];

        // Nếu sai tại bất kỳ thời điểm nào
        if (playerInput[playerInput.Count - 1] != correctSeq[playerInput.Count - 1])
        {
            StartCoroutine(WrongInput());
            return;
        }

        // Nếu đã gõ đủ 3 quả → thắng lượt
        if (playerInput.Count == correctSeq.Length)
        {
            currentRound++;

            if (currentRound >= rounds.Count)
            {
                Debug.Log("WIN TOÀN PUZZLE!");
                for (int i = 0; i < eggs.Length; i++)
                {
                    if (eggs[i] != null)
                        eggs[i].Explode();

                }
                eventAfterEggPuzzleEnd?.Invoke();
            }
            else
            {
                StartCoroutine(PlaySequence());
            }
        }
    }

    private IEnumerator WrongInput()
    {
        canPlayerInput = false;

        // Nổ từng quả trứng theo thứ tự
        for (int i = 0; i < eggs.Length; i++)
        {
            if (eggs[i] != null)
                eggs[i].Explode();

            yield return new WaitForSeconds(0.2f);
        }
        PlayerHealth.Instance.TakeDamage(999, transform);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Thua puzzle!");
    }

}
