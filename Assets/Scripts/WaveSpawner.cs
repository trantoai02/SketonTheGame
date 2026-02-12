using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Enemy;

public class WaveSpawner : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onAllWavesCompleted;     // Gọi khi đã spawn xong hết các wave (về logic)
    public UnityEvent onAfterAllEnemiesDie;    // Gọi khi toàn bộ enemy chết ở wave cuối

    [Header("References")]
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private Transform childHold; // Nơi chứa enemy spawn ra

    [Header("Settings")]
    public Wave[] waves;
    public int enemyLevel;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    private bool hasCompletedAllWaves = false;

    private void Update()
    {
        // Nếu đã spawn hết các wave và tất cả enemy đã chết
        if (!hasCompletedAllWaves && currentWaveIndex >= waves.Length && childHold.childCount == 0)
        {
            hasCompletedAllWaves = true;
            onAfterAllEnemiesDie?.Invoke();
        }
    }

    /// <summary>
    /// Gọi hàm này để bắt đầu spawn toàn bộ các wave theo thứ tự.
    /// </summary>
    public void SpawnWaves()
    {
        if (!isSpawning)
            StartCoroutine(SpawnAllWavesSequentially());
    }

    private IEnumerator SpawnAllWavesSequentially()
    {
        isSpawning = true;

        while (currentWaveIndex < waves.Length)
        {
            Wave currentWave = waves[currentWaveIndex];
            yield return StartCoroutine(SpawnWave(currentWave));

            // Nếu chưa phải wave cuối, đợi khoảng thời gian giữa 2 wave (nếu có)
            if (currentWaveIndex < waves.Length - 1)
            {
                yield return new WaitForSeconds(currentWave.timeToNextWave);
            }

            currentWaveIndex++;
        }

        isSpawning = false;
        onAllWavesCompleted?.Invoke(); // Đã spawn hết các wave, chờ enemy cuối cùng chết
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        foreach (EnemySpawnData spawnData in wave.enemies)
        {
            Enemy enemy = Instantiate(spawnData.enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
            enemy.transform.SetParent(childHold);

            enemy.GetComponent<Enemy>().chaseType = spawnData.chaseType;
            enemy.GetComponent<EnemyStats>().Init(enemyLevel);

            yield return new WaitForSeconds(spawnData.timeToNextEnemy);
        }
    }

    [System.Serializable]
    public class Wave
    {
        public List<EnemySpawnData> enemies;
        public float timeToNextWave; // Thời gian chờ giữa wave này và wave kế tiếp
    }

    [System.Serializable]
    public class EnemySpawnData
    {
        public Enemy enemyPrefab;
        public ChaseType chaseType;
        public float timeToNextEnemy;
    }
}
