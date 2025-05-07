using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public enum EnemyType
{
    Normal,
    Follow,
    Flee,
    Elite_A,
    Elite_B,
    Count
}

[System.Serializable]
public class EnemyPoolItem
{
    public EnemyType Type;        // 풀을 식별할 타입
    public GameObject Prefab;     // 생성할 적 프리팹
    public int InitialSize = 10;  // 시작 시 생성할 개수
}

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [SerializeField]
    private List<EnemyPoolItem> poolItems;

    private Dictionary<EnemyType, Queue<GameObject>> pools;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePools();
    }

    private void InitializePools()
    {
        pools = new Dictionary<EnemyType, Queue<GameObject>>();

        foreach (var item in poolItems)
        {
            var queue = new Queue<GameObject>();
            for (int i = 0; i < item.InitialSize; i++)
            {
                Vector3 spawnPos = GetSafeSpawnPosition();
                GameObject go = Instantiate(item.Prefab, spawnPos, Quaternion.identity, transform);
                go.SetActive(false);
                queue.Enqueue(go);
            }
            pools[item.Type] = queue;
        }
    }
    private Vector3 GetSafeSpawnPosition()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(Vector3.zero, out hit, 100f, NavMesh.AllAreas))
        {
            return hit.position + Vector3.up * 0.1f; // 약간 띄워서
        }
        return Vector3.zero;
    }
    public GameObject GetEnemy(EnemyType type)
    {
        if (!pools.ContainsKey(type))
        {
            Debug.LogWarning($"[EnemyPool] 풀에 타입이 없습니다: {type}");
            return null;
        }

        var queue = pools[type];
        GameObject go;
        if (queue.Count > 0)
        {
            go = queue.Dequeue();
        }
        else
        {
            var item = poolItems.Find(x => x.Type == type);
            go = Instantiate(item.Prefab, transform);
        }

        go.SetActive(true);
        return go;
    }

    public void ReturnEnemy(EnemyType type, GameObject enemy)
    {
        enemy.SetActive(false);
        if (pools.ContainsKey(type))
        {
            pools[type].Enqueue(enemy);
        }
        else
        {
            Destroy(enemy);
        }
    }
}
