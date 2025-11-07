using System.IO;
using UnityEditor;
using UnityEngine;

namespace _Project._Scripts.System
{
    public class TextureExtractor
    {
        [MenuItem("Assets/Extract Selected Texture")]
        private static void ExtractTexture()
        {
            // Lấy texture đang được chọn trong cửa sổ Project
            Texture2D sourceTexture = Selection.activeObject as Texture2D;

            if (sourceTexture == null)
            {
                EditorUtility.DisplayDialog("Lỗi", "Vui lòng chọn một file Texture 2D trong cửa sổ Project trước.", "OK");
                return;
            }

            // Kiểm tra xem texture có được bật Read/Write không
            if (!sourceTexture.isReadable)
            {
                EditorUtility.DisplayDialog("Lỗi", "Texture cần phải bật 'Read/Write Enabled' trong Import Settings.", "OK");
                return;
            }

            // Tạo một texture mới với cùng kích thước và định dạng
            Texture2D newTexture = new Texture2D(sourceTexture.width, sourceTexture.height, sourceTexture.format, false);

            // Sao chép toàn bộ dữ liệu pixel
            newTexture.SetPixels(sourceTexture.GetPixels());
            newTexture.Apply();

            // Chuyển dữ liệu thành file PNG
            byte[] bytes = newTexture.EncodeToPNG();
            Object.DestroyImmediate(newTexture); // Dọn dẹp texture trong bộ nhớ

            // Lấy đường dẫn và lưu file
            string path = AssetDatabase.GetAssetPath(sourceTexture);
            string newPath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "_extracted.png";
        
            File.WriteAllBytes(newPath, bytes);
            AssetDatabase.Refresh(); // Báo cho Unity biết có file mới

            Debug.Log("Đã trích xuất texture thành công tại: " + newPath);
        }
    }
}