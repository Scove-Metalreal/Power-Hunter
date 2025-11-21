using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Một lớp (class) để định nghĩa cấu trúc của một lần rớt đồ.
/// Nó không phải là một component, chỉ là một cấu trúc dữ liệu.
/// </summary>
[System.Serializable]
public class LootDrop
{
    [Tooltip("Số lượng item sẽ rớt ra cho mục này.")]
    public int amount;
    [Tooltip("Tỷ lệ/trọng số của mục này. Giá trị càng cao càng dễ rớt ra số lượng tương ứng.")]
    public float weight;
}

/// <summary>
/// Gắn script này vào một object có thể bị phá hủy (rương, thùng,...).
/// Object này cần có một Collider2D được set là "Is Trigger".
/// </summary>
public class DestructibleChest : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("Số lần chịu đòn trước khi bị phá hủy.")]
    public int health = 1;

    [Header("Loot Settings")]
    [Tooltip("Prefab của item sẽ được thả ra khi object này bị phá hủy. Prefab này NÊN có script ThrowableObject.")]
    public GameObject itemPrefab;
    [Tooltip("Bảng tỷ lệ rớt item. Tổng weight không nhất thiết phải là 100.")]
    public List<LootDrop> lootTable;

    [Header("Splinter Effect")]
    [Tooltip("Lực văng ra của các item khi rương bị phá hủy.")]
    public float dropForce = 5f;

    [Header("Effects")]
    [Tooltip("Prefab hiệu ứng (ví dụ: khói, mảnh vỡ) sẽ xuất hiện khi object bị phá hủy.")]
    public GameObject breakEffectPrefab;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = health;
    }

    // Script này giả định rằng vũ khí của người chơi có tag là "PlayerWeapon".
    // Khi vũ khí chạm vào trigger của rương, rương sẽ nhận sát thương.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerWeapon"))
        {
            TakeDamage(1); // Giả sử mỗi cú đánh gây 1 sát thương.
        }
    }

    /// <summary>
    /// Hàm nhận sát thương.
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        if (currentHealth <= 0) return; // Đã bị phá hủy, không xử lý nữa.

        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Break();
        }
    }

    /// <summary>
    /// Xử lý logic khi rương bị phá hủy.
    /// </summary>
    private void Break()
    {
        // 1. Tạo hiệu ứng vỡ (nếu có)
        if (breakEffectPrefab != null)
        {
            Instantiate(breakEffectPrefab, transform.position, Quaternion.identity);
        }

        // 2. Xác định số lượng item sẽ rớt ra dựa trên bảng tỷ lệ
        int amountToDrop = GetRandomLootAmount();

        // 3. Thả item ra với hiệu ứng văng ra (splinter effect)
        if (itemPrefab != null && amountToDrop > 0)
        {
            for (int i = 0; i < amountToDrop; i++)
            {
                // Tạo item tại vị trí của rương
                GameObject droppedItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
                
                // Lấy Rigidbody2D của item và áp dụng lực văng
                Rigidbody2D rb = droppedItem.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // Tạo một hướng văng ngẫu nhiên
                    Vector2 randomDirection = Random.insideUnitCircle.normalized;
                    rb.AddForce(randomDirection * dropForce, ForceMode2D.Impulse);
                }
            }
        }

        // 4. Hủy object rương
        Destroy(gameObject);
    }

    /// <summary>
    /// Quay số ngẫu nhiên dựa trên bảng tỷ lệ để quyết định số lượng item.
    /// </summary>
    private int GetRandomLootAmount()
    {
        if (lootTable == null || lootTable.Count == 0)
        {
            return 0;
        }

        float totalWeight = 0;
        foreach (var drop in lootTable)
        {
            totalWeight += drop.weight;
        }

        float randomValue = Random.Range(0, totalWeight);
        float currentWeight = 0;

        foreach (var drop in lootTable)
        {
            currentWeight += drop.weight;
            if (randomValue <= currentWeight)
            {
                return drop.amount;
            }
        }

        // Fallback, không bao giờ nên xảy ra nếu bảng tỷ lệ được thiết lập đúng
        return 0;
    }
}


