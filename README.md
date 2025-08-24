# Kẻ Săn Sức Mạnh (Power Hunter)

Một game hành động platformer 2D pixel art trong bối cảnh dark fantasy, kết hợp sự tự do di chuyển của **Super Smash Bros**, chiều sâu combat biểu cảm của **Devil May Cry**, và không khí trừng phạt khắc nghiệt của **Dark Souls/Hollow Knight**.

---

## 📑 Mục lục
- [Giới thiệu dự án](#giới-thiệu-dự-án)
- [Các công nghệ sử dụng](#các-công-nghệ-sử-dụng)
- [Hướng dẫn cài đặt](#hướng-dẫn-cài-đặt)
- [Lộ trình phát triển](#lộ-trình-phát-triển)
- [Quy trình đóng góp](#quy-trình-đóng-góp)
- [Đội ngũ phát triển](#đội-ngũ-phát-triển)

---

## 🎮 Giới thiệu dự án

*Kẻ Săn Sức Mạnh* là một dự án đồ án game, nơi người chơi vào vai một **thợ săn linh hồn đơn độc** trong một vương quốc đã sụp đổ.  
Nhiệm vụ của người chơi là **săn lùng những vị thần cũ đã bị tha hóa**, đánh bại họ để chiếm lấy sức mạnh bị nguyền rủa, và **tùy chỉnh bộ kỹ năng** của mình để đối đầu với những thử thách ngày càng khắc nghiệt.

### 🔑 Các trụ cột thiết kế chính
- ⚔️ **Combat Sáng Tạo & Biểu Cảm**: Tự do kết hợp các kỹ năng từ nhiều boss khác nhau để tạo ra phong cách chiến đấu của riêng bạn.  
- 🧗 **Di Chuyển Linh Hoạt**: Làm chủ không trung cũng như mặt đất với hệ thống di chuyển lấy cảm hứng từ các game đối kháng platform.  
- 🗺️ **Khám Phá Hữu Cơ**: Một thế giới không có bản đồ chỉ đường. Câu chuyện được hé lộ qua môi trường và sự tò mò của người chơi.  
- 💀 **Thử Thách Nhưng Công Bằng**: Độ khó cao, đòi hỏi sự kiên nhẫn và học hỏi, nhưng mọi thử thách đều có thể vượt qua bằng kỹ năng.  

---

## 🛠️ Các công nghệ sử dụng

Dự án này được xây dựng và phát triển bằng các công nghệ sau:

- **Game Engine:** Unity 6.3  
- **Ngôn ngữ lập trình:** C#
- **Quản lý phiên bản:** Git & GitHub  
- **Thiết kế Art:** Aseprite, Photoshop  
- **Quản lý dự án:** Zalo, Discord, Google Sheets  

---

## 💻 Hướng dẫn cài đặt

### Yêu cầu phần mềm
- Unity Hub  
- Unity **2022.3.x LTS** (hoặc phiên bản mới hơn được team thống nhất)  
- Git  
- Một IDE hỗ trợ C# (Visual Studio, JetBrains Rider, VS Code)  

### Các bước cài đặt

1. **Clone repo**
   ```bash
   git clone https://github.com/Scove-Metalreal/Power-Hunter.git
   ```

2. **Mở project trong Unity Hub**
   - Mở Unity Hub  
   - Chọn **Open → Add project from disk**  
   - Trỏ đến thư mục `Power-Hunter` bạn vừa clone  

3. **Chờ Unity import**
   - Lần đầu mở project, Unity sẽ mất một khoảng thời gian để import tất cả assets và tạo thư mục **Library**.  
   - Hãy kiên nhẫn.  

4. **Mở Scene chính**
   - Trong cửa sổ **Project** của Unity, tìm đến thư mục `_Scenes`  
   - Mở scene chính (ví dụ: `MainMenu` hoặc `Test_Level_01`) để bắt đầu  

---

## 🗓️ Lộ trình phát triển

Dự án được chia thành nhiều giai đoạn và cột mốc quan trọng.  
Để xem chi tiết, tham khảo **Tài liệu Quản lý Dự án**.

- [✅] **Giai đoạn 1:** Tiền Sản Xuất  
- [🟨] **Giai đoạn 2:** Tạo Mẫu (Prototyping)  
- [⬜️] **Giai đoạn 3:** Sản Xuất (Lát Cắt Dọc)  
- [⬜️] **Giai đoạn 4:** Sản Xuất Mở Rộng (Alpha)  
- [⬜️] **Giai đoạn 5:** Hậu Kỳ (Beta)  

---

## 🤝 Quy trình đóng góp

Để đảm bảo sự đồng bộ và tránh xung đột, tất cả thành viên phải tuân thủ **Git Workflow** sau:

1. **Luôn bắt đầu từ develop**
   ```bash
   git checkout develop
   git pull origin develop
   ```

2. **Tạo branch mới**
   - Tên branch: `feature/ten-tinh-nang` hoặc `bugfix/mo-ta-loi`  
   ```bash
   git checkout -b feature/player-movement
   ```

3. **Commit thường xuyên**
   ```bash
   git add .
   git commit -m "Feat: Implement double jump for player"
   ```

4. **Tạo Pull Request (PR)**
   - Push branch lên repo  
   - Tạo PR từ branch của bạn → branch `develop`  
   - Gán ít nhất 1 thành viên để **review**  

5. **Merge vào develop**
   - Chỉ merge sau khi PR được **Approved**  
   - 🚫 **Tuyệt đối không push trực tiếp** lên `develop` hoặc `main`  

---

## 👥 Đội ngũ phát triển

| Vai trò                        | Tên thành viên           |
|--------------------------------|---------------------------|
| Project Manager / Game Designer | Trần Nguyễn Phương , Hoàng Mạnh Huy           |
| Lead Programmer                 | Trần Nguyễn Phương , Hoàng Mạnh Huy, Nguyễn Anh Tuấn  |
| Lead Artist                     | Trần Nguyễn Phương      |
| Sound Designer / Composer       | Nguyễn Anh Tuấn |

---

📌 *Dự án được thực hiện trong khuôn khổ đồ án tại Học Viện Công Nghệ Thông Tin & Thiết Kế VTC Academy.*
