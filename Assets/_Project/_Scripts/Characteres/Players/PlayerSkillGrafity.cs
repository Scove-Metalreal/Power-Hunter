using UnityEngine;

public class PlayerSkillGrafity : MonoBehaviour
{
    private PlayerController playerController;
    
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {
        // Kiểm tra nếu người chơi đang không trong quá trình đổi trọng lực thì mới cho đổi tiếp
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.S)) ChangeGravity(Vector2.down, 0, true, false, false, false);
            if (Input.GetKeyDown(KeyCode.W)) ChangeGravity(Vector2.up, 180, false, true, false, false);
            
            // --- SỬA LỖI Ở ĐÂY: Đảo ngược góc xoay cho A và D ---
            if (Input.GetKeyDown(KeyCode.A)) ChangeGravity(Vector2.left, -90, false, false, true, false); // Đổi thành -90
            if (Input.GetKeyDown(KeyCode.D)) ChangeGravity(Vector2.right, 90, false, false, false, true);  // Đổi thành 90
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