using EduHub.Ch02;

// ═══════════════════════════════════════════════════════════════
//  EduHub Ch02 — Refactoring với SOLID và Design Patterns
//  Composition root: nơi duy nhất khởi tạo các lớp cụ thể và nối phụ thuộc.
// ═══════════════════════════════════════════════════════════════

// 1. Tạo các phụ thuộc (DIP: các service chỉ biết interface)
IAppLogger logger = new FileAppLogger();
IClubRepository clubRepo = new LoggingClubRepository(new InMemoryClubRepository(), logger); // Decorator
IMemberRepository memberRepo = new InMemoryMemberRepository();
INotificationServiceFactory notificationFactory = new NotificationServiceFactory();          // Factory

// 2. Tạo các service
var clubService = new ClubService(clubRepo, memberRepo, logger, notificationFactory);
var eventService = new EventService(clubRepo);
var reportService = new ReportService(clubRepo, memberRepo, logger);

// 3. Đăng ký observer cho sự kiện (Observer)
eventService.Subscribe(new EmailNotifier(memberRepo, notificationFactory));
eventService.Subscribe(new LogNotifier(logger));

// 4. Chạy lại kịch bản demo
var club = clubService.CreateClub("Tech Club", "Câu lạc bộ công nghệ", "president@gmail.com");
clubService.AddMemberToClub(club.Id, "Nguyễn Văn A", "a@student.dthu.edu.vn");
eventService.CreateEvent(club.Id, "Workshop SOLID", new DateTime(2026, 11, 20));

// 5. In báo cáo (Strategy)
reportService.Generate(new ConsoleReportStrategy());
reportService.Generate(new CsvReportStrategy());