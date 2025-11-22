using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSkillGrafity : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerStat playerStat;
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerStat = GetComponent<PlayerStat>();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "Level 3")
        {
            // Kiểm tra nếu người chơi đang không trong quá trình đổi trọng lực thì mới cho đổi tiếp
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    if (playerStat.StaminaPlayer >= 20)
                    {
                        ChangeGravity(Vector2.down, 0, true, false, false, false);
                        playerStat.UseStamina(20);
                    }
                }

                if (Input.GetKeyDown(KeyCode.W))
                {
                    if (playerStat.StaminaPlayer >= 20)
                    {
                        ChangeGravity(Vector2.up, 180, false, true, false, false);
                        playerStat.UseStamina(20);
                    }
                }
                
                // --- SỬA LỖI Ở ĐÂY: Đảo ngược góc xoay cho A và D ---
                if (Input.GetKeyDown(KeyCode.A))
                {
                    if (playerStat.StaminaPlayer >= 20)
                    {
                        ChangeGravity(Vector2.left, -90, false, false, true, false); // Đổi thành -90
                        playerStat.UseStamina(20);
                    }
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    if (playerStat.StaminaPlayer >= 20)
                    {
                        ChangeGravity(Vector2.right, 90, false, false, false, true); // Đổi thành 90
                        playerStat.UseStamina(20);
                    }
                }
            }
        }
    }

    void ChangeGravity(Vector2 gravityDir, float angle, bool down, bool up, bool left, bool right)
    {
        Physics2D.gravity = gravityDir * 9.81f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        // Cập nhật trạng thái hướng trọng lực
        playerController.GrafityDown = down;
        playerController.GrafityUp = up;
        playerController.GrafityLeft = left;
        playerController.GrafityRight = right;

        // Báo cho PlayerController biết để bắt đầu quá trình chuyển đổi
        playerController.StartGravityChange();
    }
}