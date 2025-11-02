using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public bool isActive = false; // Checkpoint này có đang được kích hoạt không
    private static CheckPoint currentCheckPoint; // Checkpoint đang được lưu hiện tại

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && gameObject.CompareTag("CheckPoint"))
        {
            Debug.Log("Player đã vào vùng checkpoint (2D): " + name);

            // Nếu có checkpoint cũ, hủy kích hoạt nó
            if (currentCheckPoint != null && currentCheckPoint != this)
            {
                Debug.Log("Hủy kích hoạt checkpoint cũ: " + currentCheckPoint.name);
                currentCheckPoint.isActive = false;
            }

            // Kích hoạt checkpoint mới
            currentCheckPoint = this;
            isActive = true;

            Debug.Log(">>> Checkpoint mới được kích hoạt (2D): " + name +
                      " | Vị trí: " + transform.position +
                      " | Thời gian: " + Time.time.ToString("F2") + "s");
            AudioManager.Instance.PlayPotion();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isActive)
        {
            Debug.Log("Player đã rời khỏi checkpoint (2D): " + name);
        }
    }

    public static Vector3 GetRespawnPosition()
    {
        if (currentCheckPoint != null)
        {
            Debug.Log("Lấy vị trí checkpoint hiện tại (2D): " + currentCheckPoint.name +
                      " | Pos: " + currentCheckPoint.transform.position);
            return currentCheckPoint.transform.position;
        }
        else
        {
            Debug.LogWarning("Chưa có checkpoint nào được kích hoạt!");
            return Vector3.zero;
        }
    }
}
