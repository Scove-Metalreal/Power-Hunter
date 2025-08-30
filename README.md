# Kẻ Săn Sức Mạnh (Power Hunter)

Một game hành động platformer 2D pixel art trong bối cảnh dark fantasy, kết hợp sự tự do di chuyển của **Super Smash Bros**, chiều sâu combat biểu cảm của **Devil May Cry**, và không khí trừng phạt khắc nghiệt của **Dark Souls/Hollow Knight**.

---

## 📑 Mục lục

- [Giới thiệu dự án](#giới-thiệu-dự-án)
- [Các công nghệ sử dụng](#các-công-nghệ-sử-dụng)
- [Hướng dẫn cài đặt](#hướng-dẫn-cài-đặt)
- [Lộ trình phát triển](#lộ-trình-phát-triển)
- [Quy trình đóng góp](#quy-trình-đóng-góp)
  - [Git bằng Terminal](#git-bằng-terminal)
  - [Git bằng GitHub Desktop](#git-bằng-github-desktop)
- [Quy ước đặt tên](#quy-ước-đặt-tên)
- [Workflow làm việc](#workflow-làm-việc)
- [Code communication](#code-communication)
- [Đội ngũ phát triển](#đội-ngũ-phát-triển)

---

## 🎮 Giới thiệu dự án

*Kẻ Săn Sức Mạnh* là một dự án đồ án game, nơi người chơi vào vai một **thợ săn linh hồn đơn độc** trong một vương quốc đã sụp đổ.\
Nhiệm vụ của người chơi là **săn lùng những vị thần cũ đã bị tha hóa**, đánh bại họ để chiếm lấy sức mạnh bị nguyền rủa, và **tùy chỉnh bộ kỹ năng** của mình để đối đầu với những thử thách ngày càng khắc nghiệt.

### 🔑 Các trụ cột thiết kế chính

- ⚔️ **Combat Sáng Tạo & Biểu Cảm**: Tự do kết hợp các kỹ năng từ nhiều boss khác nhau để tạo ra phong cách chiến đấu của riêng bạn.
- 🧗 **Di Chuyển Linh Hoạt**: Làm chủ không trung cũng như mặt đất với hệ thống di chuyển lấy cảm hứng từ các game đối kháng platform.
- 🗺️ **Khám Phá Hữu Cơ**: Một thế giới không có bản đồ chỉ đường. Câu chuyện được hé lộ qua môi trường và sự tò mò của người chơi.
- 💀 **Thử Thách Nhưng Công Bằng**: Độ khó cao, đòi hỏi sự kiên nhẫn và học hỏi, nhưng mọi thử thách đều có thể vượt qua bằng kỹ năng.

---

## 🛠️ Các công nghệ sử dụng

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
- IDE hỗ trợ C# (Visual Studio, JetBrains Rider, VS Code)

### Các bước cài đặt

1. **Clone repo**
   ```bash
   git clone https://github.com/Scove-Metalreal/Power-Hunter.git
   ```
2. **Mở project trong Unity Hub**
3. **Chờ Unity import** (tự động tạo Library, mất vài phút).
4. **Mở Scene chính** trong `_Scenes`.

---

## 🗓️ Lộ trình phát triển

- [✅] **Giai đoạn 1:** Tiền Sản Xuất
- [🟨] **Giai đoạn 2:** Tạo Mẫu (Prototyping)
- [⬜️] **Giai đoạn 3:** Sản Xuất (Lát Cắt Dọc)
- [⬜️] **Giai đoạn 4:** Sản Xuất Mở Rộng (Alpha)
- [⬜️] **Giai đoạn 5:** Hậu Kỳ (Beta)

---

## 🤝 Quy trình đóng góp

### Git bằng Terminal

1. **Đồng bộ project mới nhất**

   ```bash
   git checkout develop
   git pull origin develop
   ```

   👉 Đảm bảo bạn đang làm trên bản mới nhất, tránh conflict.

2. **Tạo branch mới**

   ```bash
   git checkout -b feature/player-movement
   ```

   👉 `feature/` cho chức năng, `bugfix/` cho sửa lỗi.

3. **Commit thường xuyên**

   ```bash
   git add .
   git commit -m "Feat: Implement double jump for player"
   ```

   👉 Commit nhỏ, rõ ràng, giúp dễ review.

4. **Push lên GitHub**

   ```bash
   git push origin feature/player-movement
   ```

5. **Tạo Pull Request** trên GitHub, merge vào `develop` sau khi được review.

---

### Git bằng GitHub Desktop

1. Mở GitHub Desktop → chọn repo.
2. Chuyển sang branch `develop`, chọn **Fetch origin** để cập nhật.
3. Tạo branch mới bằng **Branch → New branch**.
4. Làm việc trong Unity, thay đổi sẽ hiển thị trong tab **Changes**.
5. Viết message commit, nhấn **Commit to ...**.
6. Nhấn **Push origin** để đưa code lên GitHub.
7. Truy cập GitHub web → tạo Pull Request.

👉 GitHub Desktop phù hợp cho newbie chưa quen Terminal.

---

## 📂 Quy ước đặt tên

### Scripts

- PascalCase: `PlayerController.cs`, `BossHealth.cs`.
- Interface: tiền tố `I` → `IDamageable.cs`.
- Abstract class: hậu tố `Base` → `EnemyBase.cs`.

### Prefabs

- `TênĐốiTượng_Prefab`: `EnemyGoblin_Prefab`.
- UI prefab: `UI_ElementName`.

### Sprites & Textures

- Nhân vật: `player_idle_01.png`.
- Vũ khí: `sword_slash_vfx.png`.

### Animation Clips & Controllers

- Clip: `Player_Run`, `Boss_Attack`.
- Controller: `Player_AC`, `EnemyGoblin_AC`.

### Scenes

- `Level_01`, `MainMenu`, `BossRoom`.

### Branch

- `feature/...`, `bugfix/...`, `hotfix/...`.

👉 Quy ước này giúp tìm kiếm nhanh, dễ quản lý trong project lớn.

---

## ⚙️ Workflow làm việc

1. **Bắt đầu ngày làm việc**

   - Pull code mới từ `develop`.
   - Tạo branch theo task.

2. **Trong quá trình làm**

   - Commit thường xuyên, tránh commit 1 cục.
   - Nếu gặp lỗi, dùng comment `// FIXME:` để đồng đội thấy.
   - Nếu cần bổ sung sau, dùng `// TODO:`.

3. **Hoàn thành task**

   - Test cẩn thận trên local.
   - Push branch lên.
   - Tạo Pull Request, mô tả rõ thay đổi.
   - Ping đồng đội review.

4. **Nếu có xung đột (conflict)**

   - Trao đổi trong nhóm trước khi resolve.
   - Đảm bảo không xóa nhầm code người khác.

---

## 💬 Code communication

Ví dụ:

```csharp
// GOOD: giải thích logic
// Check if player is on ground before allowing jump
tempBool = Physics2D.Raycast(...);

// TODO: Refactor jump system to support double jump later

// FIXME: Current health calculation ignores armor
```

Quy tắc:

- Comment ngắn gọn, tránh spam.
- Dùng TODO/FIXME để người khác nắm được ý định.
- Viết hàm rõ nghĩa thay vì giải thích dài dòng trong comment.
- Pull Request phải có **mô tả + tại sao thay đổi** (không chỉ "update code").

---

## 👥 Đội ngũ phát triển

| Vai trò                         | Tên thành viên                                       |
| ------------------------------- | ---------------------------------------------------- |
| Project Manager / Game Designer | Trần Nguyễn Phương , Hoàng Mạnh Huy                  |
| Lead Programmer                 | Trần Nguyễn Phương , Hoàng Mạnh Huy, Nguyễn Anh Tuấn |
| Lead Artist                     | Trần Nguyễn Phương                                   |
| Sound Designer / Composer       | Nguyễn Anh Tuấn                                      |

---

📌 *Dự án được thực hiện trong khuôn khổ đồ án tại Học Viện Công Nghệ Thông Tin & Thiết Kế VTC Academy.*


