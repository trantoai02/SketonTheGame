using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using static Enemy;

public class EnemySpawner : MonoBehaviour
{
    public UnityEvent onDie;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private List<EnemySpawnData> spawnEnemies;

    [Header("Enemy Level Range")]
    [SerializeField] private int minEnemyLevel = 1;
    [SerializeField] private int maxEnemyLevel = 5;

    [SerializeField] private float spawnInterval = 2f;

    public GameObject childHolder;

    private bool isSpawning = true;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            if (this == null || gameObject == null)
                yield break;

            foreach (var spawnData in spawnEnemies)
            {
                if (this == null || gameObject == null)
                    yield break;

                Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                Enemy enemy = Instantiate(spawnData.enemyPrefab, randomPoint.position, Quaternion.identity);
                enemy.transform.SetParent(transform);
                enemy.GetComponent<Enemy>().chaseType = spawnData.chaseType;

                // Random cấp độ trong khoảng cho mỗi enemy
                int randomLevel = Random.Range(minEnemyLevel, maxEnemyLevel + 1); // +1 vì Random.Range với int là exclusive
                enemy.GetComponent<EnemyStats>().Init(randomLevel);

                yield return new WaitForSeconds(spawnData.timeToNextEnemy);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    [System.Serializable]
    public class EnemySpawnData
    {
        public Enemy enemyPrefab;
        public ChaseType chaseType;
        public float timeToNextEnemy = 1f;
    }

    private void OnDestroy()
    {
        isSpawning = false;
        onDie?.Invoke();
    }
}
