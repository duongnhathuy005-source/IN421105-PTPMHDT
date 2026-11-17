# Phân tích vi phạm SOLID trong ClubManager

| # | Vị trí | Nguyên tắc | Hậu quả |
|---|--------|-----------|---------|
| 1 | `CreateClub` (validate + lưu + log + thông báo) | SRP | Một method có 4 lý do để thay đổi; khó test riêng từng việc. |
| 2 | `File.AppendAllText(...)` ở CreateClub, AddMemberToClub, CreateEvent, PrintClubReport | DIP | Phụ thuộc trực tiếp vào hệ thống file; không thể thay logger hoặc test mà không ghi file thật. |
| 3 | Chuỗi `if/else` theo `@gmail.com`, `@yahoo.com` trong `CreateClub` | OCP | Thêm kênh thông báo (SMS, Slack...) phải sửa method cũ, dễ gây lỗi. |
| 4 | `CreateEvent` tự gửi email cho từng thành viên | SRP + OCP | Tạo sự kiện bị ràng buộc với việc gửi thông báo; thêm người nhận mới phải sửa code. |
| 5 | `PrintClubReport` in thẳng ra Console | OCP | Muốn xuất CSV phải sửa method; báo cáo trộn với nghiệp vụ. |
| 6 | `_clubs`, `_members` là List nội bộ, `GetAllClubs()` trả về chính List đó | DIP + đóng gói | Không thay được nơi lưu trữ (DB); bên ngoài sửa trực tiếp dữ liệu nội bộ. |
| 7 | Hằng số `100` và `50` nằm rải trong ClubManager | SRP | Luật nghiệp vụ nên nằm ở Domain (`Club.MaxNameLength`, `Club.MaxMembers`). |