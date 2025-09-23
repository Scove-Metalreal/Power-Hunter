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

        void Start()
        {
            cameraTransform = transform;
            previousCameraPosition = cameraTransform.position;

            // Tự động lấy chiều rộng của sprite từ con của lớp background đầu tiên
            // Giả định tất cả các sprite con trong một layer đều có cùng kích thước
            if (layerParents.Length > 0 && layerParents[0].childCount > 0)
            {
                SpriteRenderer sr = layerParents[0].GetChild(0).GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    spriteWidth = sr.bounds.size.x;
                }
            }
        }

        void LateUpdate()
        {
            // 1. Tính toán vector di chuyển của camera
            Vector3 deltaMovement = cameraTransform.position - previousCameraPosition;

            // 2. Di chuyển từng lớp background (di chuyển đối tượng CHA)
            for (int i = 0; i < layerParents.Length; i++)
            {
                float parallaxMoveX = deltaMovement.x * parallaxScales[i];
                float parallaxMoveY = deltaMovement.y; // Di chuyển 1:1 theo chiều dọc

                layerParents[i].position += new Vector3(parallaxMoveX, parallaxMoveY, 0);
            }

            // 3. Cập nhật vị trí camera
            previousCameraPosition = cameraTransform.position;

            // 4. LOGIC LẶP LẠI MỚI - Chống lỗi sai số
            foreach (Transform layer in layerParents)
            {
                // Khoảng cách tương đối giữa camera và tâm của layer
                float relativeDistance = cameraTransform.position.x - layer.position.x;

                // Nếu camera đã di chuyển qua một nửa chiều rộng của sprite
                // (tức là camera đang nhìn vào "đường nối" giữa 2 sprite)
                if (Mathf.Abs(relativeDistance) >= spriteWidth)
                {
                    // Dịch chuyển layer một khoảng bằng đúng chiều rộng sprite
                    // để đưa đường nối ra khỏi tầm nhìn của camera
                    float offset = (relativeDistance > 0) ? spriteWidth : -spriteWidth;
                    layer.position = new Vector3(layer.position.x + offset, layer.position.y, layer.position.z);
                }
            }
        }
    }
}
