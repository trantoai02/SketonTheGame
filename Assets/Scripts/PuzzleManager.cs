using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    [Header("Danh sách tile đúng (KÉO TRONG INSPECTOR)")]
    public List<PuzzleTile> allTiles1 = new List<PuzzleTile>();
    public List<PuzzleTile> allTiles2 = new List<PuzzleTile>();
    public List<PuzzleTile> allTiles3 = new List<PuzzleTile>();

    [Header("Danh sách TẤT CẢ tile trong puzzle (MỚI)")]
    public List<PuzzleTile> allPuzzleTiles = new List<PuzzleTile>();

    public GameObject explosionPrefab;

    private void Awake()
    {
        Instance = this;
    }

    // Chỉ lấy đúng tile đã được đánh dấu đúng
    public List<PuzzleTile> GetCorrectTilesZone1()
    {
        List<PuzzleTile> list = new List<PuzzleTile>();

        foreach (var tile in allTiles1)
            if (tile.isCorrectStep)
                list.Add(tile);

        return list;
    }

    public List<PuzzleTile> GetCorrectTilesZone2()
    {
        List<PuzzleTile> list = new List<PuzzleTile>();

        foreach (var tile in allTiles2)
            if (tile.isCorrectStep)
                list.Add(tile);

        return list;
    }

    public List<PuzzleTile> GetCorrectTilesZone3()
    {
        List<PuzzleTile> list = new List<PuzzleTile>();

        foreach (var tile in allTiles3)
            if (tile.isCorrectStep)
                list.Add(tile);

        return list;
    }

    // Player bước sai
    public void PlayerSteppedWrong()
    {
        StartCoroutine(ExplodeAllPuzzleTiles());
    }

    private System.Collections.IEnumerator ExplodeAllPuzzleTiles()
    {
        // copy để tránh lỗi khi destroy trong vòng lặp
        List<PuzzleTile> copy = new List<PuzzleTile>(allPuzzleTiles);
        PlayerHealth.Instance.TakeDamage(999, transform);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        foreach (var tile in copy)
        {
            tile.Explode(explosionPrefab);
            yield return new WaitForSeconds(0.05f);
        }
     
        allPuzzleTiles.Clear();
    }
}
