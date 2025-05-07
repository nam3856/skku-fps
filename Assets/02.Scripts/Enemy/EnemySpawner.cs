using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("플레이어 참조")]
    [SerializeField] private Transform player;

    [Header("스폰 거리 설정")]
    [SerializeField] private float minSpawnDistance = 5f;
    [SerializeField] private float maxSpawnDistance = 15f;

    [Header("스폰 높이")]
    [SerializeField] private float spawnHeight = 50f;

    [Header("스폰 주기")]
    [SerializeField] private float spawnInterval = 3f;

    [Header("최대 동시 생성 수")]
    [SerializeField] private int maxSpawnCount = 20;

    [Header("스폰할 적 타입")]
    [SerializeField] private EnemyType enemyType;

    private float spawnTimer;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Update()
    {
        activeEnemies.RemoveAll(e => !e.activeInHierarchy);
        if (activeEnemies.Count >= maxSpawnCount)
            return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer < spawnInterval)
            return;

        //enemyType = (EnemyType)(Random.Range(0, (int)EnemyType.Count));
        TrySpawn();
        spawnTimer = 0f;
    }

    private void TrySpawn()
    {
        Vector3 spawnPos = GetRandomSpawnPosition();
        if (spawnPos == Vector3.zero)
            return;
        spawnPos.y = spawnHeight;
        GameObject enemy = EnemyPool.Instance.GetEnemy(enemyType);
        enemy.transform.position = spawnPos;
        activeEnemies.Add(enemy);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 randCircle = Random.insideUnitCircle.normalized
                                 * Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 candidate = player.position + new Vector3(randCircle.x, 0f, randCircle.y);
            

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        Debug.LogWarning("적절한 스폰지점을 찾지 못했습니다.");
        return Vector3.zero;
    }
}
