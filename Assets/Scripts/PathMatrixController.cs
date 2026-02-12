using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PathMatrixController : MonoBehaviour
{
    [Header("Danh sách TẤT CẢ ô trong ma trận")]
    public List<MatrixCell> allCells = new List<MatrixCell>();

    [Header("Gán ô bắt đầu & kết thúc")]
    public MatrixCell startCell;
    public MatrixCell endCell;

    [Header("Hiệu ứng nổ")]
    public GameObject explosionPrefab;

    private bool puzzleActive = false;
    private bool startedCorrectly = false;
    private bool puzzleFailed = false;
    private bool puzzleCompleted = false;

    private MatrixCell lastSteppedCell;

    public MatrixCell CurrentCell { get; private set; } = null;


    // Gọi khi giẫm lên 1 ô
    public void PlayerSteppedOnCell(MatrixCell cell)
    {
        if (puzzleFailed || puzzleCompleted) return;

        lastSteppedCell = cell;

        // Lần đầu bước vào
        if (!puzzleActive)
        {
            puzzleActive = true;

            if (cell == startCell)
                startedCorrectly = true;
            else
            {
                FailPuzzle();
                return;
            }

            return;
        }

        // Giẫm lên endCell
        if (cell == endCell)
        {
            if (!AllCellsVisited())
            {
                FailPuzzle();
                return;
            }

            WinPuzzle();
            return;
        }
    }

    public void SetCurrentCell(MatrixCell cell)
    {
        CurrentCell = cell;
    }


    public void PlayerEnteredMatrix() { }

    public void PlayerExitedMatrix()
    {
        if (puzzleFailed || puzzleCompleted) return;

        if (!startedCorrectly)
        {
            FailPuzzle();
            return;
        }

        FailPuzzle();
    }


    private bool AllCellsVisited()
    {
        foreach (var c in allCells)
        {
            if (c != null && !c.wasVisited) return false;
        }
        return true;
    }


    public void FailPuzzle()
    {
        if (puzzleFailed) return;
        puzzleFailed = true;

        StartCoroutine(ExplodeAllCells());
    }


    private IEnumerator ExplodeAllCells()
    {
        List<MatrixCell> copy = new List<MatrixCell>(allCells);

        foreach (var c in copy)
        {
            if (c != null)
            {
                c.Explode(explosionPrefab);
                yield return new WaitForSeconds(0.05f);
            }
        }

        allCells.Clear();
        PlayerHealth.Instance.TakeDamage(999, transform);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    private void WinPuzzle()
    {
        if (puzzleCompleted) return;
        puzzleCompleted = true;

        StartCoroutine(ClearAllCellsSmooth());
    }

    private IEnumerator ClearAllCellsSmooth()
    {
        List<MatrixCell> copy = new List<MatrixCell>(allCells);

        foreach (var c in copy)
        {
            if (c != null)
            {
                Destroy(c.gameObject);
                yield return new WaitForSeconds(0.03f);
            }
        }

        allCells.Clear();
    }
}
