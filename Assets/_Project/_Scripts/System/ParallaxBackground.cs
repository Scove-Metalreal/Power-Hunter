using UnityEngine;

namespace _Project._Scripts.System
{
    public class ParallaxBackground : MonoBehaviour
    {
        [Header("Background Layers")]
        [Tooltip("Kéo các GameObject CHA của mỗi lớp vào đây.")]
        [SerializeField] private Transform[] layerParents;

        [Header("Parallax Effect Strength")]
        [Tooltip("Hệ số di chuyển cho mỗi lớp (chỉ áp dụng cho chiều ngang).")]
        [SerializeField] private float[] parallaxScales;

        private Transform cameraTransform;
        private Vector3 previousCameraPosition;
        private float spriteWidth;

        // --- THAY ĐỔI 1: Bỏ hàm Start() đi ---

        void LateUpdate()
        {
            // --- THAY ĐỔI 2: Khởi tạo vị trí ở frame đầu tiên ---
            // Nếu previousCameraPosition chưa được thiết lập (chỉ xảy ra 1 lần)
            if (previousCameraPosition == Vector3.zero)
            {
                // Lấy vị trí camera SAU KHI Cinemachine đã cập nhật
                cameraTransform = transform;
                previousCameraPosition = cameraTransform.position;

                // Tự động lấy chiều rộng của sprite
                if (layerParents.Length > 0 && layerParents[0].childCount > 0)
                {
                    SpriteRenderer sr = layerParents[0].GetChild(0).GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        spriteWidth = sr.bounds.size.x;
                    }
                }
                // Bỏ qua frame đầu tiên này để không tính delta movement
                return; 
            }

            // 1. Tính toán vector di chuyển của camera (từ frame thứ 2 trở đi)
            Vector3 deltaMovement = cameraTransform.position - previousCameraPosition;

            // 2. Di chuyển từng lớp background (di chuyển đối tượng CHA)
            for (int i = 0; i < layerParents.Length; i++)
            {
                float parallaxMoveX = deltaMovement.x * parallaxScales[i];
                float parallaxMoveY = deltaMovement.y; // Di chuyển 1:1 theo chiều dọc

                layerParents[i].position += new Vector3(parallaxMoveX, parallaxMoveY, 0);
            }

            // 3. Cập nhật vị trí camera cho lần tính toán ở frame tiếp theo
            previousCameraPosition = cameraTransform.position;

            // 4. LOGIC LẶP LẠI (giữ nguyên)
            foreach (Transform layer in layerParents)
            {
                float relativeDistance = cameraTransform.position.x - layer.position.x;
                if (Mathf.Abs(relativeDistance) >= spriteWidth)
                {
                    float offset = (relativeDistance > 0) ? spriteWidth : -spriteWidth;
                    layer.position = new Vector3(layer.position.x + offset, layer.position.y, layer.position.z);
                }
            }
        }
    }
}