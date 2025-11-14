using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    void Start()
    {
        // Kiểm tra xem AudioManager có tồn tại không
        if (AudioManager.Instance != null)
        {
            // Ra lệnh cho AudioManager phát nhạc của Menu
            // (Nó sẽ tự động dừng bất kỳ nhạc nào đang phát trước đó)
            AudioManager.Instance.PlayMusic(AudioManager.Instance.menuBGM);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy AudioManager.Instance!");
        }
    }
}