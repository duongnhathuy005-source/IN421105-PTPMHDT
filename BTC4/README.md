# Ch04 — EF Core, PostgreSQL và Full API: EduHub Database (starter)

Bài tập Ch04, đánh giá theo HĐ1.3 (Bài tập chương). Đọc đề bài đầy đủ ở `../de-bai.html` và rubric ở `../rubric.html`.
CLO liên quan: CLO1, CLO2, CLO4 (và CLO5, CLO7 qua quy trình Pull Request và tự nghiên cứu).

Starter đã build được. Mọi chỗ cần bạn viết đều có `// TODO [Nhiệm vụ n]` khớp với đề bài.

## Chạy

```bash
docker compose up -d                       # PostgreSQL 16 (DB_PORT=5433 nếu cổng 5432 đã bị chiếm)
dotnet build
dotnet test                                # test mẫu của Domain chạy ngay; các test Skip là việc của bạn
dotnet ef migrations add Init --project EduHub.Infrastructure --startup-project EduHub.API
dotnet run --project EduHub.API --urls http://localhost:5000     # Swagger: http://localhost:5000/swagger
```

Cài công cụ nếu chưa có: `dotnet tool install --global dotnet-ef`.

## Danh sách TODO theo nhiệm vụ

| Nhiệm vụ | Việc cần làm | Nơi làm |
|---|---|---|
| 1. EF Core, PostgreSQL, migration, seeding (20đ) | `OnModelCreating`, `SeedAsync`, `AddDbContext`, tự động migrate khi khởi động, migration `Init` | `EduHubDbContext.cs`, `Program.cs` |
| 2. Repository và Unit of Work (15đ) | `EfUnitOfWork`, các hàm truy vấn của repository, DI | `Repositories/*.cs` |
| 3. CRUD, DTO, validation, phân trang (25đ) | `SearchAsync` (filter, order, skip/take), `EventService`, `ClubService`, `GetEventsQueryHandler`, validation trong `Requests.cs` | `EduHub.Application`, `Requests.cs` |
| 4. Registration API (15đ) | đăng ký, hủy, danh sách đăng ký, các test Moq | `RegisterForEventCommand.cs`, `RegistrationQueries.cs`, `EfRegistrationRepository.cs`, `EduHub.Tests` |
| 5. Dapper cho báo cáo (10đ) | câu SQL và `QueryAsync` | `DapperEventReportQuery.cs` |
| 6. ProblemDetails và Serilog (15đ) | `GlobalExceptionHandler`, `AddProblemDetails`, cấu hình Serilog, log có cấu trúc | `GlobalExceptionHandler.cs`, `Program.cs`, `appsettings.json` |

## Ghi chú thiết kế cần bạn trả lời trong README của repo nộp bài

1. Vì sao dự án này có `IEventRepository` nhưng không tạo `IMemberRepository` hay generic `IRepository<T>`?
2. `IUnitOfWork` có cần thiết không khi `DbContext` đã là Unit of Work? Nêu một lý do giữ và một lý do bỏ.

## Quy trình nộp bài

Làm trên nhánh `feature/ch04-<mssv>`, mỗi tính năng một commit theo Conventional Commits (ví dụ `feat(db): add EF Core mappings and Init migration`), rồi mở Pull Request về `main`, mô tả đủ what, why, how to test.

## Trả lời câu hỏi thiết kế

### 1. Vì sao có `IEventRepository` nhưng không có `IMemberRepository` hay generic `IRepository<T>`?

Repository chỉ nên có khi use case cần một truy vấn riêng, có ý nghĩa nghiệp vụ. `IEventRepository` có `SearchAsync` với lọc theo CLB, tìm theo tên và phân trang, nên đáng tách riêng.

Member không có use case nào riêng ở dự án này. Việc duy nhất cần là kiểm tra thành viên có tồn tại khi đăng ký, và đã nằm trong `IRegistrationRepository.MemberExistsAsync`. Tạo `IMemberRepository` chỉ để có một hàm kiểm tra thì thừa.

Generic `IRepository<T>` (Add, Remove, GetById, GetAll...) bị loại vì:
- Nó phơi ra những thao tác mà từng entity không cần, ví dụ `Registration` không có cập nhật hay xóa trực tiếp.
- Nó che mất các truy vấn đặc thù (đếm đăng ký còn hiệu lực, join Registration với Member), nên rốt cuộc vẫn phải thêm phương thức riêng.
- `DbSet<T>` của EF Core đã là một repository generic, nên bọc thêm lớp nữa không mang lại lợi ích.

### 2. `IUnitOfWork` có cần thiết không khi `DbContext` đã là Unit of Work?

**Lý do giữ:** `IUnitOfWork` nằm ở tầng Domain, nên tầng Application không phải tham chiếu EF Core. Handler và service chỉ biết `SaveChangesAsync`, và dễ mock bằng Moq khi viết test. Đồng thời nó thể hiện rõ quy ước: repository chỉ thêm/sửa/xóa trong bộ nhớ, handler gọi lưu đúng một lần.

**Lý do bỏ:** `DbContext` đã tự theo dõi thay đổi và lưu trong một transaction qua `SaveChangesAsync`. `EfUnitOfWork` chỉ gọi hộ một dòng, nên là một lớp bọc mỏng, thêm file và thêm đăng ký DI mà không có hành vi mới. Với dự án nhỏ chỉ dùng EF Core, có thể inject thẳng `DbContext`.

Dự án này chọn giữ, vì mục tiêu học là tách Application khỏi hạ tầng và có test không cần database.