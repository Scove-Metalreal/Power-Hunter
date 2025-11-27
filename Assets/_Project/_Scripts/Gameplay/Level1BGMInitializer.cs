using UnityEngine;

public class Level1BGMInitializer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Ví dụ: Level1BGMInitializer.cs (trong Scene "Level 1")

    void Start()
    {
        if (AudioManager.Instance != null && AudioManager.Instance.level1BGM != null)
        {
            // Hàm PlayMusic sẽ tự động gọi StopMusic() trước khi phát
            AudioManager.Instance.PlayMusic(AudioManager.Instance.level1BGM);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
