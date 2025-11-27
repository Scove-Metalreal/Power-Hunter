using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab; // Prefab của Enemy để spawn.
    public GameObject spawnEffectPrefab; // Prefab của hiệu ứng (đất đá, khói...).
    public float spawnDelay = 0.5f; // Thời gian chờ giữa hiệu ứng và lúc enemy xuất hiện.
    
    [Header("Enemy Gravity")]
    [Tooltip("Hướng trọng lực cho enemy được spawn ra từ đây. (0, -1) là bình thường, (0, 1) là đảo ngược, (1, 0) là hút sang phải...")]
    public Vector2 gravityDirection = new Vector2(0, -1);

    [Header("Trigger Settings")]
    public bool spawnOnStart = false; // Spawn ngay khi bắt đầu màn chơi?
    public bool spawnOnPlayerEnter = true; // Spawn khi người chơi đi vào tầm?
    public bool respawnable = false; // Có thể spawn lại sau khi enemy bị tiêu diệt?
    public float respawnTime = 10f; // Thời gian hồi sinh.
    
    

    private bool hasSpawned = false;
    private GameObject spawnedEnemy;

    void Start()
    {
        if (spawnOnStart)
        {
            StartCoroutine(SpawnSequence());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Chỉ spawn khi người chơi đi vào và chưa từng spawn (hoặc đã sẵn sàng hồi sinh).
        if (spawnOnPlayerEnter && !hasSpawned && collision.CompareTag("Player"))
        {
            StartCoroutine(SpawnSequence());
        }
    }

    // Quy trình tạo quái
    IEnumerator SpawnSequence()
    {
        hasSpawned = true;

        // 1. Tạo hiệu ứng
        if (spawnEffectPrefab != null)
        {
            Instantiate(spawnEffectPrefab, transform.position, Quaternion.identity);
        }

        // 2. Chờ một chút
        yield return new WaitForSeconds(spawnDelay);

        // 3. Tạo Enemy
        if (enemyPrefab != null)
        {
            // Tạo đối tượng Enemy
            spawnedEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.Euler(0,180,0))  ;
            
            // Lấy script Enemy từ đối tượng vừa tạo
            Enemy enemyScript = spawnedEnemy.GetComponent<Enemy>();

            // Nếu có script, truyền thông tin trọng lực cho nó
            if (enemyScript != null)
            {
                enemyScript.Initialize(gravityDirection.normalized);
            }
        }
    }

    void Update()
    {
        // Logic để hồi sinh (nếu được bật)
        if (respawnable && hasSpawned && spawnedEnemy == null)
        {
            StartCoroutine(RespawnRoutine());
        }
    }
    
    IEnumerator RespawnRoutine()
    {
        hasSpawned = false; // Đánh dấu là đã chết để không chạy Coroutine này nhiều lần
        yield return new WaitForSeconds(respawnTime);
        StartCoroutine(SpawnSequence()); // Bắt đầu spawn lại
    }
}