using UnityEngine;

namespace _Project._Scripts.System
{
    public class SimpleMiniMap : MonoBehaviour
    {
        [Header("Đối tượng cần gán")]
        [SerializeField] private Transform playerTransform; // Kéo Player vào đây
        [SerializeField] private RectTransform mapImage;    // Kéo MiniMap_Image vào đây
        [SerializeField] private RectTransform playerIcon;  // Kéo Player_Icon vào đây

        [Header("Thông số bản đồ")]
        [Tooltip("Kích thước thực tế của thế giới game (chiều rộng, chiều cao)")]
        [SerializeField] private Vector2 worldSize;

        void LateUpdate()
        {
            // Lấy vị trí hiện tại của người chơi trong thế giới
            Vector2 playerPos = new Vector2(playerTransform.position.x, playerTransform.position.y);
        
            // Tính toán vị trí tương ứng trên ảnh minimap
            // Chuyển đổi tọa độ thế giới thành % (từ 0 đến 1)
            // Lưu ý: Đoạn code này giả định góc dưới bên trái của thế giới là (0,0)
            float percentX = playerPos.x / worldSize.x;
            float percentY = playerPos.y / worldSize.y;

            // Lấy kích thước của ảnh minimap trên UI
            Vector2 mapSize = mapImage.rect.size;

            // Tính tọa độ cuối cùng của icon trên minimap
            float iconX = percentX * mapSize.x;
            float iconY = percentY * mapSize.y;

            // Cập nhật vị trí của icon
            playerIcon.anchoredPosition = new Vector2(iconX, iconY);
        }
    }
}
