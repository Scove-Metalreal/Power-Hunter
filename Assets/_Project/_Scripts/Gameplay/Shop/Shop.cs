using UnityEngine;

public class Shop : MonoBehaviour
{
    [Header("Shop UI Settings")]
    public GameObject pressEUI; // UI chữ "Nhấn E để mở shop" (gán trong Inspector)

    private void Start()
    {
        if (pressEUI != null)
            pressEUI.SetActive(false); // Ẩn khi bắt đầu
    }

    // Khi player vào vùng collider của shop
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (pressEUI != null)
                pressEUI.SetActive(true);
        }
    }

    // Khi player rời khỏi vùng collider của shop
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (pressEUI != null)
                pressEUI.SetActive(false);
        }
    }
}
